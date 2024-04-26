using System.Text.Json;
using System.Text.Json.Serialization;

public class JsonDateTimeUTCConverter : JsonConverter<DateTime>
{
  public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    if (reader.TokenType != JsonTokenType.String)
    {
      throw new JsonException();
    }
    var value = reader.GetString();
    if (value == null)
    {
      throw new JsonException();
    }

    return DateTime.Parse(value);
  }

  public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
  {
    writer.WriteStringValue(value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
  }
}
