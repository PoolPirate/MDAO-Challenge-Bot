using System.Text.Json;
using System.Text.Json.Serialization;

namespace MDAO_Challenge_Bot.Utils;
public class DateOnlyToDateTimeOffsetEDTMidnightConverter : JsonConverter<DateTimeOffset>
{
    public TimeOnly Time { get; } = new TimeOnly(23, 59);
    public TimeSpan Offset { get; } = new TimeSpan(-4, 0, 0);

    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? rawDate = reader.GetString();

        return !DateOnly.TryParse(rawDate, out var date)
            ? DateTimeOffset.MinValue
            : new DateTimeOffset(date.ToDateTime(Time), Offset).UtcDateTime;
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

