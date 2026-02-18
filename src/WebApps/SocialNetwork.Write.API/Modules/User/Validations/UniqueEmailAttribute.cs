using System.ComponentModel.DataAnnotations;
using SocialNetwork.Write.API.Modules.User.Repository.Interface;
using SocialNetwork.Write.API.Repositories.Interfaces;

namespace SocialNetwork.Write.API.Modules.User.Validations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class UniqueEmailAttribute: ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string email || string.IsNullOrWhiteSpace(email))
            return new ValidationResult("Email is required");
        
        IUserRepository userRepository = (IUserRepository)validationContext.GetRequiredService(typeof(IUserRepository));
        
        var exists = userRepository.ExistsByEmail(email).GetAwaiter().GetResult();
        
        if (exists)
            return new ValidationResult(ErrorMessage ?? "Email already exists");
        
        return ValidationResult.Success;
    }
}