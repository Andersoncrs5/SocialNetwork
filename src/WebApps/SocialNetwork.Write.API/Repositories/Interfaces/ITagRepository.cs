using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Models;

namespace SocialNetwork.Write.API.Repositories.Interfaces;

public interface ITagRepository: IGenericRepository<TagModel>
{
    Task<bool> ExistsBySlug([SlugConstraint] string slug);
    Task<TagModel?> FindBySlug([SlugConstraint] string slug);
}