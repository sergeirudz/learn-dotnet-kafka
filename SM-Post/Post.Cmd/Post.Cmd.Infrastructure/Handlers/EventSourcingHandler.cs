using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using CQRS.Core.infrastructure;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Infrastructure.Handlers;

public class EventSourcingHandler : IEventSourcingHandler<PostAggregate>
{
    
    private readonly IEventStore _eventStore;


    public EventSourcingHandler(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public async Task<PostAggregate> GetByIdAsync(Guid id)
    {
        var aggregate = new PostAggregate();
        // DO we have events?
        var events = await _eventStore.GetEventsAsync(id);

        if (events == null || !events.Any())
        {
            // return new instance of aggregate with no events associated with it
            return aggregate;
        }
        aggregate.ReplayEvents(events);
        var latestVersion = events.Select(x=> x.Version).Max(); // return all the events linked to the aggregate.
        aggregate.Version = latestVersion;
        return aggregate;
    }
    
    public async Task SaveAsync(AggregateRoot aggregate)
    {
        // Save events to the store
        await _eventStore.SaveEventAsync(aggregate.Id, aggregate.GetUncommittedChanges(), aggregate.Version);
        aggregate.MarkChangesAsCommitted();
    }

}