using Common.Services;
using MDAO_Challenge_Bot.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MDAO_Challenge_Bot.Services.Sharing;
public class SharingTaskRunner : Scoped
{
    [Inject]
    private readonly ChallengeDBContext DbContext = null!;
    [Inject]
    private readonly SharingService SharingService = null!;
    [Inject]
    private readonly ILogger<SharingTaskRunner> Logger = null!;

    public async Task ShareAirtableChallengeAsync(long challengeId)
    {
        var challenge = await DbContext.AirtableChallenges
            .Where(x => x.Id == challengeId)
            .SingleOrDefaultAsync();

        if (challenge is null)
        {
            Logger.LogWarning("Sharing AirtableChallenge failed! Challenge no longer exists Id={id}", challengeId);
            return;
        }

        await SharingService.ShareAirtableChallengeAsync(challenge);
    }

    public async Task ShareLaborMarketRequest(long requestId)
    {
        var request = await DbContext.LaborMarketRequests
            .Include(x => x.LaborMarket)
            .Include(x => x.ProviderPaymentToken)
            .Include(x => x.ReviewerPaymentToken)
            .Where(x => x.Id == requestId)
            .SingleOrDefaultAsync();

        if (request is null)
        {
            Logger.LogWarning("Sharing LaborMarketRequest failed! Request no longer exists Id={id}", requestId);
            return;
        }

        await SharingService.ShareLaborMarketRequestAsync(request.LaborMarket!, request, request.ProviderPaymentToken!);
    }
}
