using Common.Configuration;
using System.ComponentModel.DataAnnotations;

namespace MDAO_Challenge_Bot.Options;
public class GoogleOptions : Option
{
    public required string ServiceAccountCredentialFile { get; set; }

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!File.Exists(ServiceAccountCredentialFile))
        {
            yield return new ValidationResult($"ServiceAccountCredentialFile could not be found at {ServiceAccountCredentialFile}");
        }
    }
}
