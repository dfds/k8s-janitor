using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using k8s.Models;
using Microsoft.Rest;
using K8sJanitor.WebApi.Wrappers;

namespace K8sJanitor.WebApi.Repositories.Kubernetes
{
    public class RoleRepository : IRoleRepository
    {
        private readonly IKubernetesWrapper _client;

        public RoleRepository(IKubernetesWrapper client)
        {
            _client = client;
        }

        public async Task<string> CreateNamespaceFullAccessRole(string namespaceName)
        {
            var roleName = $"{namespaceName}-fullaccess";
            var role = new V1Role
            {
                Metadata = new V1ObjectMeta
                {
                    Name = roleName,
                    NamespaceProperty = namespaceName
                },
                Rules = new List<V1PolicyRule>
                {
                    new V1PolicyRule
                    {
                        ApiGroups = new List<string>
                        {
                            "extensions",
                            "networking.k8s.io"
                        },
                        Resources = new List<string>
                        {
                            "*"
                        },
                        Verbs = new List<string>
                        {
                            "*"
                        }
                    },
                    new V1PolicyRule
                    {
                        ApiGroups = new List<string>
                        {
                            "apps",
                        },
                        Resources = new List<string>
                        {
                            "controllerrevisions",
                            "deployments",
                            "deployments/scale",
                            "replicasets",
                            "statefulsets",
                            "statefulsets/scale",
                        },
                        Verbs = new List<string>
                        {
                            "*"
                        }
                    },
                    new V1PolicyRule
                    {
                        ApiGroups = new List<string>
                        {
                            "apps",
                        },
                        Resources = new List<string>
                        {
                            "daemonsets"
                        },
                        Verbs = new List<string>
                        {
                            "get",
                            "list",
                            "watch"
                        }
                    },
                    new V1PolicyRule
                    {
                        ApiGroups = new List<string>
                        {
                            "batch"
                        },
                        Resources = new List<string>
                        {
                            "jobs",
                            "cronjobs"
                        },
                        Verbs = new List<string>
                        {
                            "*"
                        }
                    },
                    new V1PolicyRule
                    {
                        ApiGroups = new List<string>
                        {
                            "rbac.authorization.k8s.io"
                        },
                        Resources = new List<string>
                        {
                            "rolebindings",
                            "roles"
                        },
                        Verbs = new List<string>
                        {
                            "*"
                        }
                    },
                    new V1PolicyRule
                    {
                        ApiGroups = new List<string>
                        {
                            ""
                        },
                        Resources = new List<string>
                        {
                            "bindings",
                            "componentstatuses",
                            "configmaps",
                            "endpoints",
                            "events",
                            "limitranges",
                            "nodes",
                            "nodes/proxy",
                            "nodes/status",
                            "persistentvolumeclaims",
                            "persistentvolumeclaims/status",
                            "persistentvolumes",
                            "persistentvolumes/status",
                            "pods",
                            "pods/attach",
                            "pods/binding",
                            "pods/eviction",
                            "pods/exec",
                            "pods/log",
                            "pods/portforward",
                            "pods/proxy",
                            "pods/status",
                            "podtemplates",
                            "replicationcontrollers",
                            "replicationcontrollers/scale",
                            "replicationcontrollers/status",
                            "resourcequotas",
                            "resourcequotas/status",
                            "secrets",
                            "serviceaccounts",
                            "services",
                            "services/proxy",
                            "services/status"
                        },
                        Verbs = new List<string>
                        {
                            "*"
                        }
                    },
                    new V1PolicyRule
                    {
                        ApiGroups = new List<string>
                        {
                            ""
                        },
                        Resources = new List<string>
                        {
                            "namespaces",
                            "namespaces/finalize",
                            "namespaces/status"
                        },
                        Verbs = new List<string>
                        {
                            "get",
                            "list",
                            "watch"
                        }
                    },
                    new V1PolicyRule
                    {
                        ApiGroups = new List<string>
                        {
                            "metrics.k8s.io"
                        },
                        Resources = new List<string>
                        {
                            "pods"
                        },
                        Verbs = new List<string>
                        {
                            "get",
                            "list",
                            "watch"
                        }
                    },
                    new V1PolicyRule
                    {
                        ApiGroups = new List<string>
                        {
                            "traefik.io"
                        },
                        Resources = new List<string>
                        {
                            "*"
                        },
                        Verbs = new List<string>
                        {
                            "*"
                        }
                    },
                    new V1PolicyRule
                    {
                        ApiGroups = new List<string>
                        {
                            "external-secrets.io"
                        },
                        Resources = new List<string>
                        {
                            "ecrauthorizationtokens",
                            "externalsecrets",
                            "fakes",
                            "passwords",
                            "secretstores",
                            "webhooks"
                        },
                        Verbs = new List<string>
                        {
                            "*"
                        }
                    },
                    new V1PolicyRule
                    {
                        ApiGroups = new List<string>
                        {
                            "monitoring.coreos.com"
                        },
                        Resources = new List<string>
                        {
                            "servicemonitors"
                        },
                        Verbs = new List<string>
                        {
                            "*"
                        }
                    },
                    new V1PolicyRule
                    {
                        ApiGroups = new List<string>
                        {
                            "druid.apache.org"
                        },
                        Resources = new List<string>
                        {
                            "druids/status"
                        },
                        Verbs = new List<string>
                        {
                            "get",
                            "patch",
                            "update"
                        }
                    },
                    new V1PolicyRule
                    {
                        ApiGroups = new List<string>
                        {
                            "druid.apache.org"
                        },
                        Resources = new List<string>
                        {
                            "druids",
                            "druidingestions"
                        },
                        Verbs = new List<string>
                        {
                            "*"
                        }
                    },
                    new V1PolicyRule
                    {
                        ApiGroups = new List<string>
                        {
                            "source.toolkit.fluxcd.io"
                        },
                        Resources = new List<string>
                        {
                            "gitrepositories",
                            "helmcharts",
                            "helmrepositories",
                            "ocirepositories"
                        },
                        Verbs = new List<string>
                        {
                            "*"
                        }
                    },
                    new V1PolicyRule
                    {
                        ApiGroups = new List<string>
                        {
                            "helm.toolkit.fluxcd.io"
                        },
                        Resources = new List<string>
                        {
                            "helmreleases"
                        },
                        Verbs = new List<string>
                        {
                            "*"
                        }
                    },
                    new V1PolicyRule
                    {
                        ApiGroups = new List<string>
                        {
                            "kustomize.toolkit.fluxcd.io"
                        },
                        Resources = new List<string>
                        {
                            "kustomizations"
                        },
                        Verbs = new List<string>
                        {
                            "*"
                        }
                    },
                }
            };
            try
            {
                var result = await _client.CreateNamespacedRoleAsync(role, namespaceName);

                return result?.Metadata?.Name;
            }
            catch (HttpOperationException e) when (e.Response.StatusCode == HttpStatusCode.Conflict)
            {
                throw new RoleAlreadyExistException($"Role with name {roleName} already exist in kubernetes. Not creating.", roleName);
            }
            catch (HttpOperationException e) when (e.Response.Content.Length != 0)
            {
                throw new Exception(
                    "Error occured while communicating with k8s:" + Environment.NewLine +
                    e.Response.Content
                    , e
                );
            }
        }
    }
}
