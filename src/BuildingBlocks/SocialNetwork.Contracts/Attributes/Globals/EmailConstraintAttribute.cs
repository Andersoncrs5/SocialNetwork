using System;
using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Contracts.Attributes.Globals;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class EmailConstraintAttribute : ValidationAttribute
{
    private readonly EmailAddressAttribute _emailAddressAttribute = new();

    public EmailConstraintAttribute()
    {
        ErrorMessage = "Invalid email address";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var strValue = value as string;

        if (string.IsNullOrWhiteSpace(strValue))
            return new ValidationResult("Email cannot be empty");

        if (strValue.Length < 5 || strValue.Length > 150)
            return new ValidationResult("Email must be between 5 and 150 characters");

        if (!_emailAddressAttribute.IsValid(value))
            return new ValidationResult("Invalid email format");
        

        return ValidationResult.Success;
    }
}