using Common.Services;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tweetinvi;
using Tweetinvi.Models;

namespace MDAO_Challenge_Bot.Services.Twitter;
public class TweetsV2Poster : Singleton
{
    [Inject]
    private readonly ITwitterClient Client = null!;

    public async Task<long> PostTweetAsync(string text, long? inReplyToTweetId = null)
    {
        var response = await Client.Execute.AdvanceRequestAsync<PostTweetResponse>(
            (ITwitterRequest request) =>
            {
                var tweetParams = new TweetV2PostRequest()
                {
                    Text = text,
                    Reply = inReplyToTweetId is null
                        ? null
                        : new TweetsV2ReplyPostRequest()
                        {
                            InReplyToTweetId = inReplyToTweetId.Value.ToString()
                        }
                };

                var jsonOptions = new JsonSerializerOptions()
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };
                
                var content = JsonContent.Create(tweetParams, options: jsonOptions);

                request.Query.Url = "https://api.twitter.com/2/tweets";
                request.Query.HttpMethod = Tweetinvi.Models.HttpMethod.POST;
                request.Query.HttpContent = content;
            }
        );

        return long.Parse(response.Model.Data.Id);
    }


    private record PostTweetResponse(TweetData Data);
    private record TweetData(string Text, string Id);

    private class TweetV2PostRequest
    {
        [JsonPropertyName("text")]
        public required string Text { get; set; } = string.Empty;

        [JsonPropertyName("reply")]
        public TweetsV2ReplyPostRequest? Reply { get; set; } = null!;
    }

    private class TweetsV2ReplyPostRequest
    {
        [JsonPropertyName("in_reply_to_tweet_id")]
        public required string InReplyToTweetId { get; set; }
    }
}
