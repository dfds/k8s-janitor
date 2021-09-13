using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using k8s;
using k8s.Models;
using Microsoft.Rest;

namespace K8sJanitor.WebApi.Wrappers
{
    public class KubernetesWrapper : IKubernetesWrapper
    {
        private readonly IKubernetes _kubernetes;

        public KubernetesWrapper(IKubernetes kubernetes)
        {
            _kubernetes = kubernetes;
        }


        public Task<V1Role> CreateNamespacedRoleAsync(V1Role body, string namespaceParameter, bool? pretty = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return _kubernetes.CreateNamespacedRoleAsync(body, namespaceParameter, pretty: pretty, cancellationToken: cancellationToken);
        }

        public Task CreateNamespacedConfigMapAsync(V1ConfigMap body, string namespaceParameter)
        {
            return _kubernetes.CreateNamespacedConfigMapAsync(body, namespaceParameter);
        }

        public Task ReplaceNamespacedConfigMapAsync(V1ConfigMap body, string name, string namespaceParameter)
        {
            return _kubernetes.ReplaceNamespacedConfigMapAsync(body, name, namespaceParameter);
        }

        public async Task<V1ConfigMap> ReadNamespacedConfigMapAsync(
            string name,
            string namespaceParameter,
            bool? pretty = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _kubernetes.ReadNamespacedConfigMapAsync(
                name,
                namespaceParameter,
                pretty,
                cancellationToken
            );
        }

        public async Task<V1Namespace> ReadNamespaceAsync(
            string name,
            bool? pretty = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _kubernetes.ReadNamespaceAsync(
                name,
                pretty,
                cancellationToken
            );
        }


        public async Task<HttpOperationResponse<V1Namespace>> PatchNamespaceWithHttpMessagesAsync(
            V1Patch body,
            string name,
            bool? pretty = null,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _kubernetes.PatchNamespaceWithHttpMessagesAsync(
                body,
                name,
                pretty: pretty,
                customHeaders: customHeaders,
                cancellationToken: cancellationToken
            );
        }

        public Task CreateNamespaceAsync(
            V1Namespace body, 
            bool? pretty, 
            CancellationToken cancellationToken = default(CancellationToken)
        )
        {
            return _kubernetes.CreateNamespaceAsync(
                body, 
                pretty: pretty, 
                cancellationToken: cancellationToken
            );
        }

        public Task<V1RoleBinding> CreateNamespacedRoleBindingAsync(
            V1RoleBinding body, 
            string namespaceParameter, 
            bool? pretty = null,
            CancellationToken cancellationToken = default(CancellationToken)
        )
        {
            return _kubernetes.CreateNamespacedRoleBindingAsync(
                body,
                namespaceParameter,
                pretty: pretty,
                cancellationToken: cancellationToken
            );
        }
    }
}