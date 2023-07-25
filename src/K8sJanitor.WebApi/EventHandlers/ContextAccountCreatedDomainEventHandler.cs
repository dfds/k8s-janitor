using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using K8sJanitor.WebApi.Application;
using K8sJanitor.WebApi.Domain.Events;
using K8sJanitor.WebApi.Infrastructure.AWS;
using K8sJanitor.WebApi.Models;
using K8sJanitor.WebApi.Repositories.Kubernetes;
using K8sJanitor.WebApi.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace K8sJanitor.WebApi.EventHandlers
{
    public class ContextAccountCreatedDomainEventHandler  : IEventHandler<ContextAccountCreatedDomainEvent>
    {
        private readonly IConfigMapService _configMapService;
        private readonly INamespaceRepository _namespaceRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IRoleBindingRepository _roleBindingRepository;
        private readonly IK8sApplicationService _k8sApplicationService;
        private readonly ILogger<ContextAccountCreatedDomainEventHandler> _logger;

        public ContextAccountCreatedDomainEventHandler(
            IConfigMapService configMapService,
            INamespaceRepository namespaceRepository,
            IRoleRepository roleRepository,
            IRoleBindingRepository roleBindingRepository,
            IK8sApplicationService k8sApplicationService,
            ILogger<ContextAccountCreatedDomainEventHandler> logger
        )
        {
            _configMapService = configMapService;
            _namespaceRepository = namespaceRepository;
            _roleRepository = roleRepository;
            _roleBindingRepository = roleBindingRepository;
            _k8sApplicationService = k8sApplicationService;
            _logger = logger;
        }


        public async Task HandleAsync(ContextAccountCreatedDomainEvent domainEvent)
        {
            var namespaceName = NamespaceName.Create(domainEvent.Payload.CapabilityRootId);

            await CreateNameSpace(namespaceName, domainEvent);

            var namespaceRoleName = await CreateRoleForNamespace(namespaceName);

            await BindNamespacedRoleToGroup(namespaceName, namespaceRoleName);

            await _k8sApplicationService.FireEventK8sNamespaceCreatedAndAwsArnConnected(namespaceName, domainEvent.Payload.ContextId, domainEvent.Payload.CapabilityId); // Emit Kafka event "k8s_namespace_created_and_aws_arn_connected"
        }

        private async Task BindNamespacedRoleToGroup(NamespaceName namespaceName, string namespaceRoleName)
        {
            try
            {
                await _roleBindingRepository.BindNamespaceRoleToGroup(
                    namespaceName: namespaceName,
                    role: namespaceRoleName,
                    @group: namespaceName
                );
            }
            catch (RoleBindingAlreadyExistInNamespaceException e)
            {
                _logger.LogInformation($"Not creating rolebinding {e.RoleBinding} as it already exist in kubernetes");
            }
        }

        private async Task<string> CreateRoleForNamespace(NamespaceName namespaceName)
        {
            try
            {
                var namespaceRoleName = await _roleRepository
                    .CreateNamespaceFullAccessRole(namespaceName);
                return namespaceRoleName;
            }
            catch (RoleAlreadyExistException e)
            {
                _logger.LogInformation($"Not creating role {e.RoleName} as it already exist in kubernetes");
                return e.RoleName;
            }
        }

        private async Task CreateNameSpace(
            NamespaceName namespaceName,
            ContextAccountCreatedDomainEvent domainEvent
        )
        {
            var labels = new List<Label>
            {
                Label.CreateSafely("capability-id", domainEvent.Payload.CapabilityId.ToString()),
                Label.CreateSafely("capability-name", domainEvent.Payload.CapabilityName),
                Label.CreateSafely("context-id", domainEvent.Payload.ContextId.ToString()),
                Label.CreateSafely("context-name", domainEvent.Payload.ContextName)
            };

            try
            {
                await _namespaceRepository.CreateNamespaceAsync(namespaceName, labels);
            }
            catch (NamespaceAlreadyExistException)
            {
                // TODO Should we assert labels exist?
                _logger.LogInformation($"Not creating namespace {namespaceName} as it already exist in kubernetes");
            }
            await _namespaceRepository.AddAnnotations(namespaceName, new Dictionary<string, string>
            {
                {
                    "iam.amazonaws.com/permitted",
                    IAM.ConstructRoleArn(domainEvent.Payload.AccountId, ".*")
                },
                {
                    "dfds-aws-account-id",
                    domainEvent.Payload.AccountId
                },
                {
                    "pod-security.kubernetes.io/audit",
                    "baseline"
                },
                {
                    "pod-security.kubernetes.io/warn",
                    "baseline"
                }
            });
        }
    }
}
