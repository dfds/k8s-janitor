using System;
using System.Linq;
using System.Threading.Tasks;
using k8s;
using k8s.Models;
using K8sJanitor.WebApi.Repositories.Kubernetes;
using K8sJanitor.WebApi.Wrappers;
using Xunit;

namespace K8sJanitor.WebApi.IntegrationTests.Repositories.Kubernetes.RoleRepository
{
    // Reads as RoleRepository.CreateNamespaceFullAccessRole can
    public class CreateNamespaceFullAccessRoleFacts
    {
        // This test breaks if it is used in a cluster where roles are created automatically in a namespace(e.g. having Crossplane running)
        [FactRunsOnK8s]
        public async Task Create_A_Role_For_A_Existing_Namespace()
        {
            // Arrange
            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();

            var client = new k8s.Kubernetes(config);
            var wrapper = new KubernetesWrapper(client);

            var namespaceRepository = new NamespaceRepository(wrapper);
            var subjectNameSpace = "namespace-with-role-test-" + Guid.NewGuid().ToString().Substring(0,5);
            var awsRoleName = "notUSed";

            var sut = new WebApi.Repositories.Kubernetes.RoleRepository(wrapper);
            try
            {
                // Act
                await namespaceRepository.CreateNamespaceAsync(subjectNameSpace, awsRoleName);
                await sut.CreateNamespaceFullAccessRole(subjectNameSpace);

                // Assert
                client.ListNamespacedRole(subjectNameSpace).Items.Single();
            }
            finally
            {
                client.DeleteNamespace(
                    body: new V1DeleteOptions(),
                    name: subjectNameSpace
                );
            }
        }
        
        [FactRunsOnK8s]
        public async Task Create_already_existing_role_should_result_in_typed_exception()
        {
            // Arrange
            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();

            var client = new k8s.Kubernetes(config);
            var wrapper = new KubernetesWrapper(client);

            var namespaceRepository = new NamespaceRepository(wrapper);
            var subjectNameSpace = "namespace-with-role-test-" + Guid.NewGuid().ToString().Substring(0,5);
            var awsRoleName = "notUSed";

            var sut = new WebApi.Repositories.Kubernetes.RoleRepository(wrapper);
            try
            {
                // Act
                await namespaceRepository.CreateNamespaceAsync(subjectNameSpace, awsRoleName);
                await sut.CreateNamespaceFullAccessRole(subjectNameSpace);
                
                // Assert
                await Assert.ThrowsAsync<RoleAlreadyExistException>(() => sut.CreateNamespaceFullAccessRole(subjectNameSpace));

            }
            finally
            {
                client.DeleteNamespace(
                    body: new V1DeleteOptions(),
                    name: subjectNameSpace
                );
            }
        }


    }
}