using System.ComponentModel.DataAnnotations;
using SocialNetwork.Write.API.Repositories.Interfaces;

namespace SocialNetwork.Write.API.Utils.Valids.Annotations.Reaction;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter )]
public class UniqueReactionNameAttribute: ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string name || string.IsNullOrWhiteSpace(name))
            return new ValidationResult("Name is required");

        IReactionRepository repository = (IReactionRepository)validationContext.GetRequiredService(typeof(IReactionRepository));
        
        bool result = repository.ExistsByName(name).GetAwaiter().GetResult();
        
        if (result)
            return new ValidationResult(ErrorMessage ?? "Name already exists");
        
        return ValidationResult.Success;
    }
}