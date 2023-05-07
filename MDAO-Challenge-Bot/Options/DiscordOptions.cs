using Common.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDAO_Challenge_Bot.Options;
public class DiscordOptions : Option
{
    public required string DiscordWebhookURL { get; set; }
    public required bool ShareAirtable { get; set; }
    public required bool ShareLaborMarkets { get; set; }
}
