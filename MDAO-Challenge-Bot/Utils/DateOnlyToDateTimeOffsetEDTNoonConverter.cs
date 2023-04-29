using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MDAO_Challenge_Bot.Utils;
public class DateOnlyToDateTimeOffsetEDTNoonConverter : JsonConverter<DateTimeOffset>
{
    public TimeOnly Time { get; } = new TimeOnly(11, 59);
    public TimeSpan Offset { get; } = new TimeSpan(-4, 0, 0);

    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? rawDate = reader.GetString();

        return !DateOnly.TryParse(rawDate, out var date)
            ? DateTimeOffset.TryParse("4/28/2023, 3:59:00 PM UTC".Replace("UTC", "GMT"),
                CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out var timestamp)
                ? timestamp
                : DateTimeOffset.MinValue
            : new DateTimeOffset(date.ToDateTime(Time), Offset).UtcDateTime;
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

