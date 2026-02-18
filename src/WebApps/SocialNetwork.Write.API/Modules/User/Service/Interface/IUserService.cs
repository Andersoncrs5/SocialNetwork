using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Modules.Role.Model;
using SocialNetwork.Write.API.Modules.User.Dto;
using SocialNetwork.Write.API.Modules.User.Model;
using SocialNetwork.Write.API.Utils;

namespace SocialNetwork.Write.API.Modules.User.Service.Interface;

public interface IUserService
{
    Task<UserResult> UpdateSimple(UserModel user);
    Task<UserResult> Update(UpdateUserDto dto, UserModel user);
    Task<UserModel?> GetUserBySid([IsId] string sid);
    Task<UserModel> GetUserBySidSimple([IsId] string sid);
    Task<bool> ExistsUserBySid([IsId] string sid);
    Task<bool> CheckPassword(UserModel user, string password);
    Task<UserResult> DeleteUser(UserModel user);
    Task<UserModel?> GetUserByEmail([EmailConstraint] string email);
    Task<bool> ExistsUserByEmail([EmailConstraint] string email);
    Task<UserModel> GetUserByEmailSimple([EmailConstraint] string email);
    Task<UserModel?> GetUserByUsername(string username);
    Task<UserModel> GetUserByUsernameSimple(string username);
    Task<bool> ExistsUserByUsername(string username);
    Task<UserModel?> GetUserByRefreshToken(string refreshToken);
    Task<IList<string>> GetUserRoles(UserModel user);
    Task<UserResult> CreateUser(CreateUserDto dto);
    Task<bool> AddRole(RoleModel role, UserModel user);
    Task<bool> RemoveRole(RoleModel role, UserModel user);
}