using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace K8sJanitor.WebApi.Tests
{
    public class ConfigMapEditorTest
    {
        private readonly string mapRolesInput =
            @"- roleARN: arn:aws:iam::228426479489:role/KubernetesAdmin
        username: kubernetes-admin:{{SessionName}}
        groups:
        - system:masters
        - roleARN: arn:aws:iam::228426479489:role/KubernetesView
        username: kubernetes-view:{{SessionName}}
        groups:
        - kub-view
        ";

        [Fact]
        public void AddRoleMapping_GivenValidInput_ReturnsValidOutputWithGroupAdded()
        {
            // Arrange
            var roleARN = "arn:aws:iam::228426479489:role/KubernetesTest";
            var username = "kubernetes-test";
            var groups = new List<string>
            {
                "kub-test"
            };

            // Act
            var result = ConfigMapEditor.AddRoleMapping(mapRolesInput, roleARN, username, groups);

            // Assert
            Assert.NotNull(result);
            Assert.Contains(groups.First(), result);
        }

        [Fact]
        public void AddRoleMapping_GivenValidInput_ReturnsUsernameWithAddedSessionName()
        {
            // Arrange
            var roleARN = "arn:aws:iam::228426479489:role/KubernetesTest";
            var username = "kubernetes-test";
            var groups = new List<string>
            {
                "kub-test"
            };

            // Act
            var result = ConfigMapEditor.AddRoleMapping(mapRolesInput, roleARN, username, groups);

            // Assert
            Assert.NotNull(result);
            Assert.Contains($"{username}:{{{{SessionName}}}}", result);
        }

        [Fact]
        public void AddRoleMapping_MultipleMappings_ReturnsMultipleUsernamesAdded()
        {
            // Arrange
            var roleARN1 = "arn:aws:iam::228426479489:role/KubernetesTest";
            var username1 = "kubernetes-test";
            var roleARN2 = "arn:aws:iam::228426479489:role/KubernetesTest2";
            var username2 = "kubernetes-test2";
            var groups = new List<string>
            {
                "kub-test"
            };

            // Act
            var result1 = ConfigMapEditor.AddRoleMapping(mapRolesInput, roleARN1, username1, groups);
            var result2 = ConfigMapEditor.AddRoleMapping(result1, roleARN2, username2, groups);

            // Assert
            Assert.NotNull(result2);
            Assert.Contains(username1, result2);
            Assert.Contains(username2, result2);
        }

        [Fact]
        public void Will_Place_RoleMapping_In_Correct_Place()
        {
            // Arrange
            var initialMap =
                "apiVersion: v1" + System.Environment.NewLine + "data:" + System.Environment.NewLine + "  mapRoles: >" + System.Environment.NewLine + "    - rolearn: arn:aws:iam::123456789012:role/Awesome" + System.Environment.NewLine + "      username: Awesome:{{SessionName}}" + System.Environment.NewLine + "      groups:" + System.Environment.NewLine + "      - DFDS-ReadOnly" + System.Environment.NewLine + "kind: ConfigMap" + System.Environment.NewLine + "metadata:" + System.Environment.NewLine + "  name: aws-auth" + System.Environment.NewLine + "  namespace: kube-system";

            
            // Act
            var resultMap = ConfigMapEditor.AddRoleMapping(
                initialMap,
                roleArn: "roleArn",
                userName: "userName",
                groups: new[] {"group1", "group2"}
            );
            
            // Assert
            var expected =
                "apiVersion: v1" + System.Environment.NewLine + "data:" + System.Environment.NewLine + "  mapRoles: >" + System.Environment.NewLine + "    - rolearn: arn:aws:iam::123456789012:role/Awesome" + System.Environment.NewLine + "      username: Awesome:{{SessionName}}" + System.Environment.NewLine + "      groups:" + System.Environment.NewLine + "      - DFDS-ReadOnly" + System.Environment.NewLine + "kind: ConfigMap" + System.Environment.NewLine + "metadata:" + System.Environment.NewLine + "  name: aws-auth" + System.Environment.NewLine + "  namespace: kube-system" + System.Environment.NewLine + "- rolearn: roleArn" + System.Environment.NewLine + "  username: userName:{{SessionName}}" + System.Environment.NewLine + "  groups:" + System.Environment.NewLine + "    - group1" + System.Environment.NewLine + "    - group2" + System.Environment.NewLine + "";
            Assert.Equal(expected,resultMap);
        }

        
        [Fact]
        public void CreateRoleArnObjectText_Will_Create_A_Valid_Object()
        {
            // Arrange / Act
            var result = ConfigMapEditor.CreateRoleArnObjectText(
                "roleArn",
                "userName",
                new[] {"group1", "group2"}
            );


            // Assert
            var expected = "- rolearn: roleArn" + System.Environment.NewLine + "  username: userName:{{SessionName}}" + System.Environment.NewLine + "  groups:" + System.Environment.NewLine + "    - group1" + System.Environment.NewLine + "    - group2" + System.Environment.NewLine + "";
            Assert.Equal(expected, result);
        }
    }
}