using System.ComponentModel.DataAnnotations;
using SocialNetwork.Write.API.Modules.Category.Repository.Interface;
using SocialNetwork.Write.API.Repositories.Interfaces;

namespace SocialNetwork.Write.API.Modules.Category.Validations;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter )]
public class ExistsCategoryByIdAttribute: ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            return ValidationResult.Success;

        var id = value.ToString()!;
        
        ICategoryRepository repository = (ICategoryRepository)validationContext.GetRequiredService(typeof(ICategoryRepository));
        bool result = repository.ExistsById(id).GetAwaiter().GetResult();
        
        if (!result)
            return new ValidationResult(ErrorMessage ?? "Category does not exist");
        
        return ValidationResult.Success;
    }
}