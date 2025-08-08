using System.Text.Json;
using System.Text.Json.Serialization;

namespace AktWeb.Functions.Model.Converters;

public class FlexibleDoubleConverter : JsonConverter<double>
{
    public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Handle numbers directly
        if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetDouble();
        }

        // Handle strings that represent numbers
        if (reader.TokenType == JsonTokenType.String)
        {
            var stringValue = reader.GetString();
            if (double.TryParse(stringValue, out double result))
            {
                return result;
            }

            throw new JsonException($"Unable to convert \"{stringValue}\" to double.");
        }

        throw new JsonException($"Unexpected token parsing double. Token: {reader.TokenType}");
    }

    public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value);
    }
}
