using System;
using Newtonsoft.Json.Linq;

namespace K8sJanitor.WebApi.Domain.Events
{
    public class ContextAccountCreatedDomainEvent : IDomainEvent<ContextAccountCreatedDomainEventData>
    {
       
        public ContextAccountCreatedDomainEvent(GeneralDomainEvent domainEvent)
        {
            Version = domainEvent.Version;
            EventName = domainEvent.EventName;
            XCorrelationId = domainEvent.XCorrelationId;
            XSender = domainEvent.XSender;
            Payload = (domainEvent.Payload as JObject)?.ToObject<ContextAccountCreatedDomainEventData>();
        }

        public string Version { get; }
        public string EventName { get; }
        public string XCorrelationId { get; }
        public string XSender { get; }
        public ContextAccountCreatedDomainEventData Payload { get; }
        
    }

    public class ContextAccountCreatedDomainEventData
    {
        public string CapabilityId { get; private set; }
        public string CapabilityName { get; private set; }
        public string CapabilityRootId { get; private set; }
        public string ContextId { get; private set; }
        public string ContextName { get; private set; }
        public string AccountId { get; private set; }
        public string RoleArn { get; private set; }
        public string RoleEmail { get; private set; }

        public ContextAccountCreatedDomainEventData(
            string capabilityId, 
            string capabilityName, 
            string capabilityRootId,
            string contextId, 
            string contextName, 
            string accountId, 
            string roleArn, 
            string roleEmail
        )
        {
            CapabilityId = capabilityId;
            CapabilityName = capabilityName;
            CapabilityRootId = capabilityRootId;
            ContextId = contextId;
            ContextName = contextName;
            AccountId = accountId;
            RoleArn = roleArn;
            RoleEmail = roleEmail;
        }
    }
}