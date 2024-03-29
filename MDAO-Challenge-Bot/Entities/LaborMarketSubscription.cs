﻿using MDAO_Challenge_Bot.Models;

namespace MDAO_Challenge_Bot.Entities;
public abstract class LaborMarketSubscription
{
    public required long Id { get; init; }
    public required string LaborMarketAddress { get; init; }
    public required LaborMarketSubscriptionType Type { get; init; }

    public virtual LaborMarket? LaborMarket { get; set; } //Navigation Property
}
