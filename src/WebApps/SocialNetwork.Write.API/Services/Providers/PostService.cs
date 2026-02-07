
using AutoMapper;
using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Configs.Exception.classes;
using SocialNetwork.Write.API.dto.Posts;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Models.Enums.Post;
using SocialNetwork.Write.API.Services.Interfaces;
using SocialNetwork.Write.API.Utils.UnitOfWork;

namespace SocialNetwork.Write.API.Services.Providers;

public class PostService(IUnitOfWork uow, IMapper mapper): IPostService
{
    public async Task<PostModel?> GetByIdAsync([IsId] string id)
        => await uow.PostRepository.GetByIdAsync(id);
    public async Task<PostModel> GetByIdSimpleAsync([IsId] string id)
        => await uow.PostRepository.GetByIdAsync(id) ?? throw new ModelNotFoundException("Post not found");

    public async Task<bool> ExistsByIdAsync([IsId] string id)
        => await uow.PostRepository.ExistsById(id);

    public async Task<bool> ExistsBySlugAsync([SlugConstraint] string slug)
        => await uow.PostRepository.ExistsBySlug(slug);

    public async Task<PostModel> CreateAsync(CreatePostDto dto, UserModel user)
    {
        PostModel map = mapper.Map<PostModel>(dto);

        map.ModerationStatus = ModerationStatusEnum.PendingReview;
        map.HighlightStatus = PostHighlightStatusEnum.None;
        map.User = user;
        map.UserId = user.Id;
        
        PostModel model = await uow.PostRepository.AddAsync(map);
        await uow.CommitAsync();
        
        return model;
    }
    
    public async Task DeleteAsync(PostModel post)
    {
        await uow.PostRepository.DeleteAsync(post);
        await uow.CommitAsync();
    }

    public async Task<PostModel> UpdateAsync(PostModel post, UpdatePostDto dto)
    {
        if (!string.IsNullOrWhiteSpace(dto.Title)) post.Title = dto.Title;
        if (!string.IsNullOrWhiteSpace(dto.Content)) post.Content = dto.Content;
        if (!string.IsNullOrWhiteSpace(dto.Summary)) post.Summary = dto.Summary;
        if (!string.IsNullOrWhiteSpace(dto.FeaturedImageUrl)) post.FeaturedImageUrl = dto.FeaturedImageUrl;
        
        if (dto.Visibility.HasValue) post.Visibility = dto.Visibility.Value;
        if (dto.ReadingTime.HasValue) post.ReadingTime = dto.ReadingTime.Value;
        if (dto.IsCommentsEnabled.HasValue) post.IsCommentsEnabled = dto.IsCommentsEnabled.Value;
        if (dto.ReadingLevel.HasValue) post.ReadingLevel = dto.ReadingLevel.Value;
        if (dto.PostType.HasValue) post.PostType = dto.PostType.Value;

        if (!string.IsNullOrWhiteSpace(dto.Slug) && dto.Slug != post.Slug )
        {
            if (await uow.PostRepository.ExistsBySlug(dto.Slug))
                throw new ConflictValueException($"Slug {dto.Slug} already exists");
            
            post.Slug = dto.Slug;
        }
        
        PostModel postUpdated = await uow.PostRepository.Update(post);
        await uow.CommitAsync();
        
        return postUpdated;
    }
    
}