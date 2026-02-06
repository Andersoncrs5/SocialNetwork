using System.ComponentModel.DataAnnotations;
using SocialNetwork.Write.API.Repositories.Interfaces;

namespace SocialNetwork.Write.API.Utils.Valids.Annotations.Post;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter )]
public class UniquePostSlugAttribute: ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string slug || string.IsNullOrWhiteSpace(slug))
            return new ValidationResult("Slug is required");

        IPostRepository repository = (IPostRepository)validationContext.GetRequiredService(typeof(IPostRepository));
        
        bool result = repository.ExistsBySlug(slug).GetAwaiter().GetResult();
        
        if (result)
            return new ValidationResult(ErrorMessage ?? "Slug already exists");
        
        return ValidationResult.Success;
    }
}