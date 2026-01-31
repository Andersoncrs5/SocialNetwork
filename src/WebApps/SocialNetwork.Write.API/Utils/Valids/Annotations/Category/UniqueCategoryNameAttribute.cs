using System.ComponentModel.DataAnnotations;
using SocialNetwork.Write.API.Repositories.Interfaces;

namespace SocialNetwork.Write.API.Utils.Valids.Annotations.Category;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter )]
public class UniqueCategoryNameAttribute: ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string name || string.IsNullOrWhiteSpace(name))
            return new ValidationResult("Name is required");

        ICategoryRepository repository = (ICategoryRepository)validationContext.GetRequiredService(typeof(ICategoryRepository));
        bool result = repository.ExistsByName(name).GetAwaiter().GetResult();
        
        if (result)
            return new ValidationResult(ErrorMessage ?? "Name already exists");
        
        return ValidationResult.Success;
    }
}