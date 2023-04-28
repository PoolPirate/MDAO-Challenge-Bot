using Common.Configuration;
using System.ComponentModel.DataAnnotations;

namespace MDAO_Challenge_Bot.Options;
public class AirtableOptions : Option
{
    public required bool Enabled { get; set; }
    [Required]
    public required Uri APIUrl { get; set; }
}
