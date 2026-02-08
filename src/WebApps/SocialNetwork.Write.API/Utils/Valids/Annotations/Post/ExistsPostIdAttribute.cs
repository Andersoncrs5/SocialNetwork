using System.ComponentModel.DataAnnotations;
using SocialNetwork.Write.API.Repositories.Interfaces;

namespace SocialNetwork.Write.API.Utils.Valids.Annotations.Post;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter )]
public class ExistsPostIdAttribute: ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string id || string.IsNullOrWhiteSpace(id))
            return ValidationResult.Success;
        
        IPostRepository repository = (IPostRepository)validationContext.GetRequiredService(typeof(IPostRepository));
        
        bool result = repository.ExistsById(id).GetAwaiter().GetResult();
        
        if (!result)
            return new ValidationResult(ErrorMessage ?? "Post not found");
        
        return ValidationResult.Success;
    }
}