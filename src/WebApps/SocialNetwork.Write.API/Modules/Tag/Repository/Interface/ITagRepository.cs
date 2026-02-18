using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Modules.Tag.Model;
using SocialNetwork.Write.API.Repositories.Interfaces;

namespace SocialNetwork.Write.API.Modules.Tag.Repository.Interface;

public interface ITagRepository: IGenericRepository<TagModel>
{
    Task<bool> ExistsBySlug([SlugConstraint] string slug);
    Task<TagModel?> FindBySlug([SlugConstraint] string slug);
    Task<bool> ExistsByName([SlugConstraint] string name);
}