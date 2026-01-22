using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Configs.Exception.classes;
using SocialNetwork.Write.API.dto.User;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Services.Interfaces;
using SocialNetwork.Write.API.Utils;
using SocialNetwork.Write.API.Utils.UnitOfWork;

namespace SocialNetwork.Write.API.Services.Providers;

public class UserService(
    IUnitOfWork uow,
    IPasswordHasher<UserModel> passwordHasher,
    IMapper mapper
    ): IUserService
{
    private UserResult ReturnResult(IdentityResult result, UserModel? user)
    {
        if (result.Succeeded)
        {
            
            return new UserResult
            {
                Succeeded = true,
                User = user,
                Errors = null
            };
        }
        else
        {
            return new UserResult
            {
                Succeeded = false,
                User = user,
                Errors = result.Errors.Select(e => e.Description).ToList()
            };
        }
    }
    
    public async Task<UserModel?> GetUserBySid([IsId] string sid)
        => await uow.UserRepository.GetByIdAsync(sid);
    
    public async Task<UserModel> GetUserBySidSimple([IsId] string sid)
        => await uow.UserRepository.GetByIdAsync(sid) ?? throw new ModelNotFoundException("User not found");

    public async Task<bool> ExistsUserBySid([IsId] string sid)
        => await uow.UserRepository.ExistsByIdAsync(sid);
    
    public async Task<bool> CheckPassword(UserModel user, string password)
        => await uow.UserRepository.CheckPassword(user, password);
    
    public async Task<UserResult> DeleteUser(UserModel user)
    {
        var result = await uow.UserRepository.Delete(user);
        if (result.Succeeded)
            await uow.CommitAsync(); 
         
        return ReturnResult(result, null);
    }
    
    public async Task<UserModel?> GetUserByEmail([EmailConstraint] string email)
        => await uow.UserRepository.GetByEmail(email);
    
    public async Task<bool> ExistsUserByEmail([EmailConstraint] string email)
        => await uow.UserRepository.ExistsByEmail(email);
    
    public async Task<UserModel> GetUserByEmailSimple([EmailConstraint] string email)
        => await uow.UserRepository.GetByEmail(email) ?? throw new ModelNotFoundException("User not found");
    
    public async Task<UserModel?> GetUserByUsername(string username)
        => await uow.UserRepository.GetByUsername(username);
    
    public async Task<UserModel> GetUserByUsernameSimple(string username)
        => await uow.UserRepository.GetByUsername(username) ?? throw new ModelNotFoundException("User not found");
    
    public async Task<bool> ExistsUserByUsername(string username) 
        => await uow.UserRepository.ExistsByUsername(username);
    
    public async Task<UserModel?> GetUserByRefreshToken(string refreshToken)
        => await uow.UserRepository.GetByRefreshToken(refreshToken);

    public async Task<IList<string>> GetUserRoles(UserModel user)
        => await uow.UserRepository.GetRolesAsync(user);

    public async Task<UserResult> CreateUser(CreateUserDto dto)
    {
        UserModel user = mapper.Map<UserModel>(dto);

        IdentityResult result = await uow.UserRepository.Insert(user);
        
        var userCreated = await GetUserByEmail(dto.Email);
        return ReturnResult(result, userCreated);
    }
    
    public async Task<UserResult> UpdateSimple(UserModel user)
    {
        IdentityResult result = await uow.UserRepository.Update(user);
        
        UserModel model = await GetUserByEmailSimple(user.Email!);
        return ReturnResult(result, model);
    }

    public async Task<UserResult> Update(UpdateUserDto dto, UserModel user)
    {
        if (!string.IsNullOrWhiteSpace(dto.Username) && !dto.Username.Equals(user.UserName))
        {
            if (!await ExistsUserByUsername(dto.Username))
                user.UserName = dto.Username;
        }
    
        if (!string.IsNullOrWhiteSpace(dto.PasswordHash))
        {
            user.PasswordHash = passwordHasher.HashPassword(user, dto.PasswordHash);
            user.SecurityStamp = Guid.NewGuid().ToString();
        }
    
        user.FullName = dto.FullName ?? user.FullName;
        user.Bio = dto.Bio ?? user.Bio;
        user.CoverImageUrl = dto.CoverImageUrl ?? user.CoverImageUrl;
        user.ImageProfileUrl = dto.ImageProfileUrl ?? user.ImageProfileUrl;
        user.Language = dto.Language ?? user.Language;
        user.Country = dto.Country ?? user.Country;
    
        if (dto.BirthDate.HasValue) user.BirthDate = dto.BirthDate;
        if (dto.IsPrivate.HasValue) user.IsPrivate = dto.IsPrivate.Value;

        return await UpdateSimple(user);
    }
    
}