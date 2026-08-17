using System.Text.Json;
using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Events;
using Microsoft.Extensions.Options;
using Post.Query.Infrastructure.Converters;
using Post.Query.Infrastructure.Handlers;

namespace Post.Query.Infrastructure.Consumers;

public class EventConsumer : IEventConsumer
{
    private readonly ConsumerConfig _config;
    private readonly IEventHandler _eventHandler;

    public EventConsumer(IOptions<ConsumerConfig> config, IEventHandler eventHandler)
    {
        _config = config.Value;
        _eventHandler = eventHandler;
    }

    public void Consume(string topic)
    {
        using var consumer = new ConsumerBuilder<string, string>(_config)
            .SetKeyDeserializer(Deserializers.Utf8) // Same value Utf8 as Event
            .SetValueDeserializer(Deserializers.Utf8)
            .Build();

        consumer.Subscribe(topic);

        // Keep polling Kafka for new messages
        while (true)
        {
            var consumeResult = consumer.Consume();

            if (consumeResult?.Message == null)
            {
                continue;
            }

            // Use this Deserializer to Parse JSON
            var options = new JsonSerializerOptions
            {
                Converters = { new EventJsonConverter() }
            };
            
            var @event = JsonSerializer.Deserialize<BaseEvent>(consumeResult.Message.Value, options);
            // All of our event handlers are called 'On()'. '@event.GetType()' gets the Type on the event
            // if PostCreatedEvent - use On(PostCreatedEvent @event);
            var handlerMethod = _eventHandler.GetType().GetMethod("On", new Type[] { @event.GetType() });

            if (handlerMethod == null)
            {
                throw new ArgumentNullException(nameof(handlerMethod), "Could not find event handler method");
            }

            handlerMethod.Invoke(_eventHandler, new object[] { @event });
            // Tell Kafka we consumed and handled Event, increment commit log offset
            consumer.Commit(consumeResult);
        }
    }
}