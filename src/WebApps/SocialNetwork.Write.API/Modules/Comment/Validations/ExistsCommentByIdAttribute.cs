using System.ComponentModel.DataAnnotations;
using SocialNetwork.Write.API.Modules.Comment.Repository.Interface;
using SocialNetwork.Write.API.Repositories.Interfaces;

namespace SocialNetwork.Write.API.Modules.Comment.Validations;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter )]
public class ExistsCommentByIdAttribute: ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string id || string.IsNullOrWhiteSpace(id))
            return ValidationResult.Success;
        
        ICommentRepository repository = (ICommentRepository)validationContext.GetRequiredService(typeof(ICommentRepository));
        
        bool result = repository.ExistsById(id).GetAwaiter().GetResult();
        
        if (!result)
            return new ValidationResult(ErrorMessage ?? "Comment not found");
        
        return ValidationResult.Success;
    }

}