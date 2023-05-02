using System;
using System.Threading.Tasks;
using K8sJanitor.WebApi.Application;

namespace K8sJanitor.WebApi.Tests.TestDoubles
{
    public class K8sApplicationServiceSpy : IK8sApplicationService
    {
        public string Payload_Namespace { get; private set; }
        public string Payload_ContextId { get; private set; }
        public string Payload_CapabilityId { get; private set; }
        public Task FireEventK8sNamespaceCreatedAndAwsArnConnected(string namespaceName, string contextId, string capabilityId)
        {
            Payload_Namespace = namespaceName;
            Payload_ContextId = contextId;
            Payload_CapabilityId = capabilityId;
            return Task.CompletedTask;
        }
        
        
    }
}