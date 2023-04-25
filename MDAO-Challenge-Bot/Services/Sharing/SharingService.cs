using Common.Services;
using MDAO_Challenge_Bot.Models;

namespace MDAO_Challenge_Bot.Services.Sharing;
public class SharingService : Singleton
{
    [Inject]
    private readonly DiscordSharingClient DiscordSharingClient = null!;
    [Inject]
    private readonly TwitterSharingClient TwitterSharingClient = null!;

    public async Task ShareAirtableChallengeAsync(AirtableChallenge challenge)
    {
        await DiscordSharingClient.ShareAsync(challenge);
    }
}
