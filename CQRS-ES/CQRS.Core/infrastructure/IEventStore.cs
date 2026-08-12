using CQRS.Core.Events;

namespace CQRS.Core.infrastructure;

public interface IEventStore
{
    Task SaveEventAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion);
    Task<List<BaseEvent>> GetEventsAsync(Guid aggregateId);
}