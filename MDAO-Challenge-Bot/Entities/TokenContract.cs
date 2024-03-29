﻿using MDAO_Challenge_Bot.Models;
using System.Numerics;

namespace MDAO_Challenge_Bot.Entities;
public class TokenContract
{
    public required string Address { get; init; }
    public required string Symbol { get; init; }
    public required byte Decimals { get; init; }

    public double DecimalsAdjust(BigInteger amount)
    {
        return Math.Exp(BigInteger.Log(amount) - BigInteger.Log(BigInteger.Pow(10, Decimals)));
    }
}
