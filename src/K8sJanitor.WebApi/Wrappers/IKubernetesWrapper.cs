using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using k8s;
using k8s.Models;
using K8sJanitor.WebApi.Models;
using Microsoft.Rest;

namespace K8sJanitor.WebApi.Wrappers
{
    public interface IKubernetesWrapper
    {
        Task<V1Role> CreateNamespacedRoleAsync(V1Role body, string namespaceParameter, bool? pretty = null, CancellationToken cancellationToken = default(CancellationToken));
        Task CreateNamespacedConfigMapAsync(V1ConfigMap body, string namespaceParameter);
        Task ReplaceNamespacedConfigMapAsync(V1ConfigMap body, string name, string namespaceParameter);
        Task<V1ConfigMap> ReadNamespacedConfigMapAsync(
            string name,
            string namespaceParameter,
            bool? pretty = null,
            CancellationToken cancellationToken= default(CancellationToken)
        );
        
        Task<V1Namespace> ReadNamespaceAsync(
            string name,
            bool? pretty = null,
            CancellationToken cancellationToken= default(CancellationToken)
        );

        Task<HttpOperationResponse<V1Namespace>> PatchNamespaceWithHttpMessagesAsync(
            V1Patch body,
            string name,
            bool? pretty = null,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken)
        );

        Task CreateNamespaceAsync(
            V1Namespace body,
            bool? pretty = null,
            CancellationToken cancellationToken = default(CancellationToken)
        );

        Task<V1RoleBinding> CreateNamespacedRoleBindingAsync(
            V1RoleBinding body,
            string namespaceParameter,
            bool? pretty = null,
            CancellationToken cancellationToken = default(CancellationToken)
        );
    }
}