﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using k8s;
using k8s.Models;
using Moq;
using K8sJanitor.WebApi.Repositories.Kubernetes;
using K8sJanitor.WebApi.Tests.TestDoubles;
using K8sJanitor.WebApi.Wrappers;
using Microsoft.Rest;
using Xunit;

namespace K8sJanitor.WebApi.Tests.Repositories.Kubernetes
{
    public class RoleRepositoryFacts
    {

        [Fact]
        public async Task CreateNamespaceFullAccessRole_IncludesCorrectPolicies()
        {
            var k8s = Dummy.Of<IKubernetesWrapper>();
            var sut = new RoleRepository(k8s);

            Mock.Get(k8s).Setup(k => k.CreateNamespacedRoleAsync(It.IsAny<V1Role>(),
                    It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .Returns(() => Task.FromResult(new V1Role()));

            var @namespace = "fancyNamespace";

            var result = await sut.CreateNamespaceFullAccessRole(@namespace);

            Moq.Mock.Get(k8s).Verify(s => s.CreateNamespacedRoleAsync(It.Is<V1Role>(r =>
                    r.Metadata.NamespaceProperty == @namespace &&
                    r.Rules.Count > 0 &&
                    r.Rules.Count(rule => rule.Resources.Any(res => res.Contains("namespace"))) == 1),
                It.Is<string>(n => n == @namespace), null, It.IsAny<CancellationToken>()));
        }


        [Fact]
        public async Task CreateExistingRoleThrowsSayingRole_WhenConflictStatusCodeIsReturned()
        {

            var k8s = Dummy.Of<IKubernetesWrapper>();
            var sut = new RoleRepository(k8s);

            Mock.Get(k8s).Setup(k => k.CreateNamespacedRoleAsync(It.IsAny<V1Role>(),
                    It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
                .Throws(new HttpOperationException
                {
                    Response =
                    new HttpResponseMessageWrapper(new HttpResponseMessage { StatusCode = HttpStatusCode.Conflict }, "")
                });

            var @namespace = "fancyNamespace";
            await Assert.ThrowsAsync<RoleAlreadyExistException>(() => sut.CreateNamespaceFullAccessRole(@namespace));
        }


        [Fact]
        public async Task EnsureGenericExceptionIsThrownWhenErrorIsUnknown()
        {

            var k8s = Dummy.Of<IKubernetesWrapper>();
            var sut = new RoleRepository(k8s);

            Mock.Get(k8s).Setup(k => k.CreateNamespacedRoleAsync(It.IsAny<V1Role>(),
                    It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
                .Throws(new HttpOperationException
                {
                    Response =
                    new HttpResponseMessageWrapper(new HttpResponseMessage { StatusCode = HttpStatusCode.BadGateway }, "Unable to communicate")
                });

            var @namespace = "fancyNamespace";

            await Assert.ThrowsAsync<Exception>(() => sut.CreateNamespaceFullAccessRole(@namespace));

        }

    }


}