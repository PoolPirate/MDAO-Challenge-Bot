using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDAO_Challenge_Bot.Entities.LaborMarketSubscriptions;
public class DiscordWebhookSubscription : LaborMarketSubscription
{
    public required Uri DiscordWebhookURL { get; init; }

	public DiscordWebhookSubscription()
	{
	}
}
