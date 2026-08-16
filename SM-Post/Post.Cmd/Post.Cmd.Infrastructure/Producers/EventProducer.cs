using System.Text.Json;
using Confluent.Kafka;
using CQRS.Core.Events;
using CQRS.Core.Producers;
using Microsoft.Extensions.Options;

namespace Post.Cmd.Infrastructure.Producers;

public class EventProducer : IEventProducer
{
    private readonly ProducerConfig _config;

    public EventProducer(IOptions<ProducerConfig> config)
    {
        _config = config.Value; // get the appSettings URL address for Kafka broker 
    }

    // T any event but only Events inherited from BaseEvent
    public async Task ProduceAsync<T>(string topic, T @event) where T : BaseEvent 
    {
        using var producer = new ProducerBuilder<string, string>(_config)
            .SetKeySerializer(Serializers.Utf8)
            .SetValueSerializer(Serializers.Utf8)
            .Build();

        // Create new event message
        var eventMessage = new Message<string, string>
        {
            Key = Guid.NewGuid().ToString(), // Unique key every time, random partition is used with random key
            Value = JsonSerializer.Serialize(@event, @event.GetType())
        };

        // Send the Kafka event
        var deliverResult = await producer.ProduceAsync(topic, eventMessage);

        // did we produce our event successfully to Kafka
        if (deliverResult.Status == PersistenceStatus.NotPersisted)
        {
            throw new Exception(
                $"Could not produce {@event.GetType().Name} message to topic {topic} due to following reason {deliverResult.Message}");
        }
    }
}