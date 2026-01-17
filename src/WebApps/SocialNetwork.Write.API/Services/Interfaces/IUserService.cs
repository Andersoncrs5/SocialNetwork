using System.Linq;
using System.Threading.Tasks;
using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.dto.User;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Utils;

namespace SocialNetwork.Write.API.Services.Interfaces;

public interface IUserService
{
    Task<UserModel?> GetUserBySid([IsId] string sid);
    Task<UserModel> GetUserBySidSimple([IsId] string sid);
    Task<bool> ExistsUserBySid([IsId] string sid);
    IQueryable<UserModel> GetIQueryable();
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
}