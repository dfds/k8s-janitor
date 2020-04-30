using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using k8s;
using k8s.Models;
using K8sJanitor.WebApi.Models;
using K8sJanitor.WebApi.Repositories.Kubernetes;
using K8sJanitor.WebApi.Wrappers;
using Xunit;

namespace K8sJanitor.WebApi.IntegrationTests.Repositories.Kubernetes
{
    public class NamespaceRepositoryFacts
    {
        [FactRunsOnK8s]
        public async Task AddAnnotations_can_add_annotations_when_none_exits()
        {
            var namespaceName =
                NamespaceName.Create("namespace-from-test-" + Guid.NewGuid().ToString().Substring(0, 5));


            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            var client = new k8s.Kubernetes(config);

            var sut = new NamespaceRepository(new KubernetesWrapper(client));

            try
            {
                await sut.CreateNamespaceAsync(namespaceName);

                var annotations = new Dictionary<string, string>
                {
                    {"type-a", "ab"},
                    {"type-b", "bc"}
                };

                // Act
                await sut.AddAnnotations(namespaceName, annotations);
                
                
                // Assert 
                var resultNamespace = client.ReadNamespace(namespaceName);

                var resultAnnotations = resultNamespace.Metadata.Annotations;
                
                Assert.Equal(annotations, resultAnnotations);
            }
            finally
            {
                client.DeleteNamespace(
                    body: new V1DeleteOptions(),
                    name: namespaceName
                );
            }
        }
        
        
        [FactRunsOnK8s]
        public async Task AddAnnotations_will_allow_the_same_annotation_twice()
        {
            // Arrange
            var namespaceName =
                NamespaceName.Create("namespace-from-test-" + Guid.NewGuid().ToString().Substring(0, 5));


            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            var client = new k8s.Kubernetes(config);

            var sut = new NamespaceRepository(new KubernetesWrapper(client));

            try
            {
                await sut.CreateNamespaceAsync(namespaceName);

                var annotationsRoundOne = new Dictionary<string, string>
                {
                    {"type-a", "ab"},
                    {"type-b", "bc"}
                };

                await sut.AddAnnotations(namespaceName, annotationsRoundOne);
                
                var annotationsRoundTwo = new Dictionary<string, string>
                {
                    {"type-a", "ab"}
                };

                // Act / Assert
                await sut.AddAnnotations(namespaceName, annotationsRoundOne);
            }
            finally
            {
                client.DeleteNamespace(
                    body: new V1DeleteOptions(),
                    name: namespaceName
                );
            }
        }
        
        
        [FactRunsOnK8s]
        public async Task AddAnnotations_will_not_override_annotations()
        {
            // Arrange
            var namespaceName =
                NamespaceName.Create("namespace-from-test-" + Guid.NewGuid().ToString().Substring(0, 5));


            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            var client = new k8s.Kubernetes(config);

            var sut = new NamespaceRepository(new KubernetesWrapper(client));

            try
            {
                await sut.CreateNamespaceAsync(namespaceName);

                var annotationsRoundOne = new Dictionary<string, string>
                {
                    {"type-a", "ab"},
                    {"type-b", "bc"}
                };

                await sut.AddAnnotations(namespaceName, annotationsRoundOne);
                
                var annotationsRoundTwo = new Dictionary<string, string>
                {
                    {"type-a", "not ab"}
                };

                // Act / Assert
                await Assert.ThrowsAsync<Exception>(async () => await sut.AddAnnotations(namespaceName, annotationsRoundTwo));
            }
            finally
            {
                client.DeleteNamespace(
                    body: new V1DeleteOptions(),
                    name: namespaceName
                );
            }
        }
      
        [FactRunsOnK8s]
        public async Task A_Created_Namespace_Will_Have_the_KIAM_Annotation()
        {
            // Arrange
            var namespaceName = "namespace-from-test";
            var accountId = "528563840976";
            var kiamRole = "arn:aws:iam::528563840976:role/.*";
            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            var client = new k8s.Kubernetes(config);

            var sut = new NamespaceRepository(new KubernetesWrapper(client));

            try
            {
                // act
                await sut.CreateNamespaceAsync(namespaceName, accountId);


                // Assert
                var namespaces = await client.ListNamespaceAsync();
                var @namespace = namespaces.Items.Single(n => n.Metadata.Name == namespaceName);


                var annotationValue = @namespace.Metadata.Annotations["iam.amazonaws.com/permitted"];

                Assert.Equal(kiamRole, annotationValue);
            }
            finally
            {
                client.DeleteNamespace(
                    body: new V1DeleteOptions(),
                    name: namespaceName
                );
            }
        }

        [FactRunsOnK8s]
        public async Task Two_Namespace_Will_Create_A_Error()
        {
            // Arrange
            var namespaceName = "namespace-from-test-" + Guid.NewGuid().ToString().Substring(0,5);
            var awsRoleName = "awsRoleName";
            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            var client = new k8s.Kubernetes(config);

            var sut = new NamespaceRepository(new KubernetesWrapper(client));

            try
            {
                await sut.CreateNamespaceAsync(namespaceName, awsRoleName);


                // Act / Assert
                await Assert.ThrowsAsync<NamespaceAlreadyExistException>(async () =>
                    await sut.CreateNamespaceAsync(namespaceName, awsRoleName));
            }
            finally
            {
                client.DeleteNamespace(
                    body: new V1DeleteOptions(),
                    name: namespaceName
                );
            }
        }
    }
}