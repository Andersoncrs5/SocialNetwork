using System.ComponentModel.DataAnnotations;
using SocialNetwork.Write.API.Repositories.Interfaces;

namespace SocialNetwork.Write.API.Utils.Valids.Annotations.Category;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter )]
public class UniqueCategorySlugAttribute: ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string slug || !string.IsNullOrWhiteSpace(slug))
            return new ValidationResult("Slug is required");

        ICategoryRepository repository = (ICategoryRepository)validationContext.GetRequiredService(typeof(ICategoryRepository));
        bool result = repository.ExistsBySlug(slug).GetAwaiter().GetResult();
        
        if (result)
            return new ValidationResult(ErrorMessage ?? "Slug already exists");
        
        return ValidationResult.Success;
    }
}