using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using k8s;
using k8s.Models;
using Microsoft.Rest;
using K8sJanitor.WebApi.Extensions;
using K8sJanitor.WebApi.Wrappers;

namespace K8sJanitor.WebApi.Repositories
{
    public class AwsAuthConfigMapRepository : IAwsAuthConfigMapRepository
    {
        private readonly IKubernetesWrapper _client;
        private static readonly string ConfigMapName = "aws-auth";
        private static readonly string ConfigMapNamespace = "kube-system";

        public AwsAuthConfigMapRepository(IKubernetesWrapper client)
        {
            _client = client;
        }

         public async Task<V1ConfigMap> GetConfigMap()
        {
            var awsAuthConfigMap = await ReadConfigMap();

            awsAuthConfigMap = awsAuthConfigMap ?? new V1ConfigMap
            {
                Metadata = new V1ObjectMeta
                {
                    Name = ConfigMapName,
                    NamespaceProperty = "kube-system"
                }
            };
            
            return awsAuthConfigMap;
        }

        public async Task WriteConfigMap(V1ConfigMap configMapRoleMap)
        {
            var awsAuthConfigMap = await ReadConfigMap();
            if (awsAuthConfigMap == null)
            {
                await _client.CreateNamespacedConfigMapAsync(
                    body: configMapRoleMap,
                    namespaceParameter: ConfigMapNamespace
                );

                return;
            }
            
            await _client.ReplaceNamespacedConfigMapAsync(
                body: configMapRoleMap,
                name: ConfigMapName, 
                namespaceParameter: ConfigMapNamespace
            );
        }


        private async Task<V1ConfigMap> ReadConfigMap()
        {
            try
            {
                return await _client.ReadNamespacedConfigMapAsync(
                    name: ConfigMapName,
                    namespaceParameter: ConfigMapNamespace
                );
            }
            catch (HttpOperationException httpOperationException) when (httpOperationException.Response.StatusCode ==
                                                                        HttpStatusCode.NotFound)
            {
                return null;
            }
        }
    }
}