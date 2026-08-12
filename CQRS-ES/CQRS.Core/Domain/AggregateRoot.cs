using CQRS.Core.Events;

namespace CQRS.Core.Domain;

public abstract class AggregateRoot
{
    protected Guid _id;
    private readonly List<BaseEvent> _changes = new();

    public Guid Id
    {
        get { return _id; }
    }

    public int Version { get; set; } = -1; // No version given

    public IEnumerable<BaseEvent> GetUncommittedChanges()
    {
        return _changes;
    }

    public void MarkChangesAsCommitted()
    {
        _changes.Clear(); // clear the list of events. After uncommited changes have altered the state of the aggregate
    }

    private void ApplyChanges(BaseEvent @event, bool isNew) // isNew - if it is a new event from event store
    {
        // Since you cant get .this from abstract class. It will give a GetType of the concrete aggregate
        // We will call overload methods "Apply". Parameter of the applied event. 
        var method = this.GetType().GetMethod("Apply", new Type[] { @event.GetType() });

        if (method == null)
        {
            throw new ArgumentNullException(nameof(method),
                $"The Apply method was not found in the aggregate for {@event.GetType().Name}");
        }

        // if we found the method

        method.Invoke(this, new object[] { @event });


        if (isNew)
        {
            // We dont want to add uncommited changes, if they come from event store. They have been already commited before.
            _changes.Add(@event);
        }
    }

    protected void RaiseEvent(BaseEvent @event)
    {
        ApplyChanges(@event, true);
    }


    public void
        ReplayEvents(IEnumerable<BaseEvent> events) // events are what we retrieve from the base store previously
    {
        // recreate the latest state of the aggregate before new uncommited changes have been applied
        foreach (var @event in events)
        {
            ApplyChanges(@event, false);
        }
    }
}