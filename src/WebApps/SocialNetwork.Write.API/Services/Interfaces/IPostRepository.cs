using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Repositories.Interfaces;

namespace SocialNetwork.Write.API.Services.Interfaces;

public interface IPostRepository: IGenericRepository<PostModel>
{
    Task<bool> ExistsBySlug([SlugConstraint] string slug);
}