using System.Text.Json;
using System.Text.Json.Serialization;

namespace SpecBox.WebApi.Lib;

public class JsonGuidCrockfordConverter : JsonConverter<Guid>
{
  public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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

    return value.FromBase32CrockfordGuid();
  }

  public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
  {
    writer.WriteStringValue(value.ToBase32Crockford());
  }
}
