using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Configs.Exception.classes;
using SocialNetwork.Write.API.dto.PostTag;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Services.Interfaces;
using SocialNetwork.Write.API.Utils.UnitOfWork;

namespace SocialNetwork.Write.API.Services.Providers;

public class PostTagService(IUnitOfWork uow): IPostTagService
{
    public async Task<PostTagModel> GetByIdAsync([IsId] string id) 
        => await uow.PostTagRepository.GetByIdAsync(id) ?? throw new ModelNotFoundException("Tag not found");

    public async Task<PostTagModel?> GetByPostIdAndTagId([IsId] string postId, [IsId] string tagId)
        => await uow.PostTagRepository.GetByPostIdAndTagId(postId, tagId);
    
    public async Task<int> CountByPostIdAndTagId([IsId] string postId, [IsId] string tagId)
        => await uow.PostTagRepository.CountByPostIdAndTagId(postId, tagId);

    public async Task Delete(PostTagModel tag, bool commit = true)
    {
        await uow.PostTagRepository.DeleteAsync(tag);
        
        if (commit) await uow.CommitAsync();
    }

    public async Task<PostTagModel> Create(CreatePostTagDto dto, bool commit = true)
    {
        PostTagModel model = uow.Mapper.Map<PostTagModel>(dto);

        PostTagModel postTagAdded = await uow.PostTagRepository.AddAsync(model);
        if (commit) await uow.CommitAsync();
        
        return postTagAdded;
    }
    
}