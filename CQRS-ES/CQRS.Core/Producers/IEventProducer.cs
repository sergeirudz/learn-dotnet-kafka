using CQRS.Core.Events;

namespace CQRS.Core.Producers;

public interface IEventProducer
{
    Task ProduceAsync<T>(string topic, T message) where T : BaseEvent;
}