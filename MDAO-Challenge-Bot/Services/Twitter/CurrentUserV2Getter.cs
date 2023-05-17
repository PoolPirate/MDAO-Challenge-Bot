using Common.Services;
using Tweetinvi;
using Tweetinvi.Core.DTO;
using Tweetinvi.Models;

namespace MDAO_Challenge_Bot.Services.Twitter;
public class CurrentUserV2Getter : Singleton
{
    [Inject]
    private readonly ITwitterClient Client = null!;

    public async Task GetCurrentUserAsync()
    {
        var t = await Client.Execute.AdvanceRequestAsync<UserDTO>(
            (ITwitterRequest request) =>
            {
                request.Query.Url = $"https://api.twitter.com/2/users/me?expansions=pinned_tweet_id&user.fields=created_at";
                request.Query.HttpMethod = Tweetinvi.Models.HttpMethod.GET;
            }
        );
    }
}
