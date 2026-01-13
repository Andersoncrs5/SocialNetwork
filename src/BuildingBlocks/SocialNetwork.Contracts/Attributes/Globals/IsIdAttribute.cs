using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Contracts.Attributes.Globals;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class IsIdAttribute : ValidationAttribute
{
    public IsIdAttribute()
    {
        ErrorMessage = "The ID must be a valid, non-empty GUID.";
    }

    public override bool IsValid(object? value)
    {
        if (value == null) return false;

        if (value is Guid guidValue)
        {
            return guidValue != Guid.Empty;
        }

        if (value is string strValue && Guid.TryParse(strValue, out Guid parsedGuid))
        {
            return parsedGuid != Guid.Empty;
        }

        return false;
    }
}