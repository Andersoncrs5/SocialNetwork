using System.ComponentModel.DataAnnotations;
using SocialNetwork.Write.API.Repositories.Interfaces;

namespace SocialNetwork.Write.API.Utils.Valids.Annotations.Category;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter )]
public class ExistsCategoryByIdAttribute: ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string id || !string.IsNullOrWhiteSpace(id))
            return new ValidationResult("Id is required");
        
        ICategoryRepository repository = (ICategoryRepository)validationContext.GetRequiredService(typeof(ICategoryRepository));
        bool result = repository.ExistsById(id).GetAwaiter().GetResult();
        
        if (!result)
            return new ValidationResult(ErrorMessage ?? "Category does not exist");
        
        return ValidationResult.Success;
    }
}