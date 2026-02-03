using System.ComponentModel.DataAnnotations;
using SocialNetwork.Write.API.Repositories.Interfaces;

namespace SocialNetwork.Write.API.Utils.Valids.Annotations.Tag;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter )]
public class UniqueTagNameAttribute: ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string name || string.IsNullOrWhiteSpace(name))
            return new ValidationResult("Name is required");

        ITagRepository repository = (ITagRepository)validationContext.GetRequiredService(typeof(ITagRepository));
        bool result = repository.ExistsByName(name).GetAwaiter().GetResult();
        
        if (result)
            return new ValidationResult("Tag name already exists");
        
        return ValidationResult.Success;
    }
}