using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Modules.Post.Model;
using SocialNetwork.Write.API.Modules.Post.Service.Interface;

namespace SocialNetwork.Write.API.Modules.PostVote.Gateway;

public class PostVoteModuleGateway(IPostService postService)
{
    public async Task<bool> ExistsPostById([IsId] string id)
    {
        return await postService.ExistsByIdAsync(id);
    }
}