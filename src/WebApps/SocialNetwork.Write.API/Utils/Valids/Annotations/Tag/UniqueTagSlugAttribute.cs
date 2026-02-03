using System.ComponentModel.DataAnnotations;
using SocialNetwork.Write.API.Repositories.Interfaces;

namespace SocialNetwork.Write.API.Utils.Valids.Annotations.Tag;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter )]
public class UniqueTagSlugAttribute: ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string slug || string.IsNullOrWhiteSpace(slug))
            return new ValidationResult("Slug is required");

        ITagRepository repository = (ITagRepository)validationContext.GetRequiredService(typeof(ITagRepository));

        bool result = repository.ExistsBySlug(slug).GetAwaiter().GetResult();
        
        if (result)
            return new ValidationResult("Slug is unique");
        
        return ValidationResult.Success;
    }
}