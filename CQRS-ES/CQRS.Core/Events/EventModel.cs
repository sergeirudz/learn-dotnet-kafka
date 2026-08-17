using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CQRS.Core.Events;

public class EventModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public DateTime TimeStamp { get; set; }
    public Guid AggregateIdentifier { get; set; }
    public string AggregateType { get; set; }
    public int Version { get; set; } // Version of the Aggregate at that time
    public string EventType { get; set; }
    public BaseEvent EventData { get; set; } // We have all of the event information, for replaying the event store
}