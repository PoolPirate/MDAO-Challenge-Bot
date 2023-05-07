using Common.Services;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tweetinvi;
using Tweetinvi.Models;

namespace MDAO_Challenge_Bot.Services.Twitter;
public class MessageV2Poster : Singleton
{
    [Inject]
    private readonly ITwitterClient Client = null!;

    public async Task PostMessageAsync(string text, long recipientId)
    {
        await Client.Execute.AdvanceRequestAsync(
            (ITwitterRequest request) =>
            {
                var messageParams = new MessageV2PostRequest()
                {
                    Text = text,
                };

                var jsonOptions = new JsonSerializerOptions()
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                var content = JsonContent.Create(messageParams, options: jsonOptions);

                request.Query.Url = $"https://api.twitter.com/2/dm_conversations/with/{recipientId}/messages";
                request.Query.HttpMethod = Tweetinvi.Models.HttpMethod.POST;
                request.Query.HttpContent = content;
            }
        );
    }

    private class MessageV2PostRequest
    {
        [JsonPropertyName("text")]
        public required string Text { get; set; } = string.Empty;

        
    }
}
