using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.infrastructure;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Infrastructure.Stores;

public class EventStore : IEventStore
{
    
    private readonly IEventStoreRepository _eventStoreRepository;
    
    public EventStore(IEventStoreRepository eventStoreRepository)
    {
        _eventStoreRepository = eventStoreRepository;
    }
    
    public async Task<List<BaseEvent>> GetEventsAsync(Guid aggregateId)
    {
        var eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId);

        if (eventStream == null || !eventStream.Any())
        {
            throw new AggregateNotFoundException("Incorrect post ID provided");
        }

        return eventStream.OrderBy(x => x.Version).Select(x => x.EventData).ToList();
    }
    
    public async Task SaveEventAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion)
    {
        var eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId);

        // Optimistic concurrency check. If 2 mutations happen for the same post.
        if (expectedVersion != -1 && eventStream[^1].Version != expectedVersion)
        {
            throw new ConcurrencyException();
        }

        var version = expectedVersion;
        // If we produce multiple events with the Aggregate
        foreach (var @event in events)
        {
            version++;
            @event.Version = version;
            var eventType = @event.GetType().Name;
            var eventModel = new EventModel
            {
                TimeStamp = DateTime.Now,
                AggregateIdentifier = aggregateId,
                AggregateType = nameof(PostAggregate),
                Version = version,
                EventType = eventType,
                EventData = @event
            };

            await _eventStoreRepository.SaveAsync(eventModel);
        }
    }

}