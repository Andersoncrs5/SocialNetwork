using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Modules.Post.Model;
using SocialNetwork.Write.API.Repositories.Interfaces;

namespace SocialNetwork.Write.API.Modules.Post.Repository.Interface;

public interface IPostRepository: IGenericRepository<PostModel>
{
    Task<bool> ExistsBySlug([SlugConstraint] string slug);
}