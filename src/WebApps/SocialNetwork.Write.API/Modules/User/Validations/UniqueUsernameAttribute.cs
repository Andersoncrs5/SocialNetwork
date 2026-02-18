using System.ComponentModel.DataAnnotations;
using SocialNetwork.Write.API.Modules.User.Repository.Interface;
using SocialNetwork.Write.API.Repositories.Interfaces;

namespace SocialNetwork.Write.API.Modules.User.Validations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class UniqueUsernameAttribute: ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {

        if (value is not string username || string.IsNullOrWhiteSpace(username))
            return new ValidationResult("Username is required");
        
        IUserRepository userRepository = (IUserRepository)validationContext.GetRequiredService(typeof(IUserRepository));
        var exists = userRepository.ExistsByUsername(username).GetAwaiter().GetResult();

        if (exists)
            return new ValidationResult(ErrorMessage ?? "Username already exists");
            
        return ValidationResult.Success;
    }
}