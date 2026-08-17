using System.Text.Json;
using System.Text.Json.Serialization;
using CQRS.Core.Events;
using Post.Common.Events;

namespace Post.Query.Infrastructure.Converters;

public class EventJsonConverter : JsonConverter<BaseEvent>
{
    public override bool CanConvert(Type typeToConvert)
    {
        // When JSON String is received, check if Type:"of BaseEvent"
        return typeToConvert.IsAssignableFrom(typeof(BaseEvent));
    }

    public override BaseEvent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // If value cant be passed/malformed to Json Document, else store in 'document'
        if (!JsonDocument.TryParseValue(ref reader, out var document))
        {
            throw new JsonException($"Failed to parse {nameof(JsonDocument)}");
        }

        // type - identifies to concrete event type, store value in 'type'
        if (!document.RootElement.TryGetProperty("Type", out var type))
            // Otherwise we won't know what concrete BaseEvent Type we need to deserialize our Json.
        {
            // Did not find the 'Type' in the String
            throw new JsonException($"Could not detect the Type discriminator property");
        }

        var typeDiscriminator = type.GetString();
        // get the Json payload as raw string
        var json = document.RootElement.GetRawText();

        // To what concrete event type we need to deserialize our Json
        return typeDiscriminator switch
        {
            // options = JsonSerializer options
            nameof(PostCreatedEvent) => JsonSerializer.Deserialize<PostCreatedEvent>(json, options),
            nameof(MessageUpdatedEvent) => JsonSerializer.Deserialize<MessageUpdatedEvent>(json, options),
            nameof(PostLikedEvent) => JsonSerializer.Deserialize<PostLikedEvent>(json, options),
            nameof(CommentAddedEvent) => JsonSerializer.Deserialize<CommentAddedEvent>(json, options),
            nameof(CommentUpdatedEvent) => JsonSerializer.Deserialize<CommentUpdatedEvent>(json, options),
            nameof(CommentRemovedEvent) => JsonSerializer.Deserialize<CommentRemovedEvent>(json, options),
            nameof(PostRemovedEvent) => JsonSerializer.Deserialize<PostRemovedEvent>(json, options),
            // If discriminator value is not in one of those cases, then throw
            _ => throw new JsonException($"typeDiscriminator {typeDiscriminator} is not supported yet")
        };
    }

    public override void Write(Utf8JsonWriter writer, BaseEvent value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}