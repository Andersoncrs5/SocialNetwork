using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SocialNetwork.Contracts.Attributes.Globals;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class SlugConstraintAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var slug = value as string;

        if (string.IsNullOrWhiteSpace(slug))
            return ValidationResult.Success;

        var regex = new Regex("^[a-z0-9]+(?:-[a-z0-9]+)*$");

        if (regex.IsMatch(slug))
            return ValidationResult.Success;

        return new ValidationResult( ErrorMessage ?? "Slug invalid");
    }
}