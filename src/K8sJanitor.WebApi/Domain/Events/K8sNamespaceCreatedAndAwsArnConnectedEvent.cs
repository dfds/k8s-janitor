using System;
using Newtonsoft.Json.Linq;

namespace K8sJanitor.WebApi.Domain.Events
{
    public class K8sNamespaceCreatedAndAwsArnConnectedEvent : IEvent
    {
        
        public string NamespaceName { get; }
        public string ContextId { get;  }
        public string CapabilityId { get;  }

        public K8sNamespaceCreatedAndAwsArnConnectedEvent(string namespaceName, string contextId, string capabilityId)
        {
            NamespaceName = namespaceName;
            ContextId = contextId;
            CapabilityId = capabilityId;
        }

        public K8sNamespaceCreatedAndAwsArnConnectedEvent()
        {
            
        }

        public K8sNamespaceCreatedAndAwsArnConnectedEvent(GeneralDomainEvent domainEvent)
        {
            
        }
    }
}