using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus
{
    public interface IEventBusSubscriptionsHandler
    {
        bool IsEmpty { get; }

        event EventHandler<string> OnEventRemoved;

        void Clear();

        void AddSubscription<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;

        void RemoveSubscription<T, TH>() where TH : IIntegrationEventHandler<T> where T : IntegrationEvent;

        bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent;

        bool HasSubscriptionsForEvent(string eventName);

        Type GetEventTypeByName(string eventName);

        IEnumerable<Type> GetHandlersForEvent<T>() where T : IntegrationEvent;

        IEnumerable<Type> GetHandlersForEvent(string eventName);

        string GetEventKey<T>();
    }
}