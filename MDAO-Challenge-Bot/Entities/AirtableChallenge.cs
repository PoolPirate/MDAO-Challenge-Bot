﻿using MDAO_Challenge_Bot.Utils;
using System.Text.Json.Serialization;

namespace MDAO_Challenge_Bot.Models;
public class AirtableChallenge
{
    public long Id { get; init; }

    public required string Title { get; init; }

    public required string Status { get; init; }

    [JsonPropertyName("start_date")]
    [JsonConverter(typeof(DateOnlyToDateTimeOffsetEDTNoonConverter))]
    public DateTimeOffset StartTimestamp { get; init; }

    [JsonPropertyName("end_date")]
    [JsonConverter(typeof(DateOnlyToDateTimeOffsetEDTMidnightConverter))]
    public DateTimeOffset EndTimestamp { get; init; }

    public long? TweetId { get; private set; }

    public void SetTweetId(long tweetId)
    {
        if (TweetId is not null)
        {
            throw new InvalidOperationException("TweetId already set");
        }

        TweetId = tweetId;
    }
}
