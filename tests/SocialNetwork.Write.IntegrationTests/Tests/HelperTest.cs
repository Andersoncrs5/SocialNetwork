using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using MySqlX.XDevAPI;
using SocialNetwork.Contracts.DTOs.Comment;
using SocialNetwork.Contracts.DTOs.Post;
using SocialNetwork.Contracts.DTOs.PostCategory;
using SocialNetwork.Contracts.DTOs.PostFavorite;
using SocialNetwork.Contracts.DTOs.PostTag;
using SocialNetwork.Contracts.DTOs.Reaction;
using SocialNetwork.Contracts.DTOs.Tag;
using SocialNetwork.Contracts.DTOs.User;
using SocialNetwork.Contracts.Enums.Post;
using SocialNetwork.Contracts.Enums.Reaction;
using SocialNetwork.Contracts.Utils.Res.http;
using SocialNetwork.Write.API.dto.PostCategory;
using SocialNetwork.Write.API.dto.User;
using SocialNetwork.Write.API.Models.Enums.Post;
using SocialNetwork.Write.API.Modules.Category.Dto;
using SocialNetwork.Write.API.Modules.Comment.Dto;
using SocialNetwork.Write.API.Modules.Post.Dto;
using SocialNetwork.Write.API.Modules.PostTag.Dto;
using SocialNetwork.Write.API.Modules.Reaction.Dto;
using SocialNetwork.Write.API.Modules.Tag.Dto;
using SocialNetwork.Write.IntegrationTests.Config;
using SocialNetwork.Write.IntegrationTests.Tests.Utils.Classes;
using Xunit.Abstractions;

namespace SocialNetwork.Write.IntegrationTests.Tests;

public class HelperTest(HttpClient client)
{
    public async Task<ReactionDto> CreateReaction(UserTestResult result)
    {
        
        client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", result.Tokens.Token);

        CreateReactionDto dto = new CreateReactionDto()
        {
            Name = "reaction" + this.GenerateChars().ToLower(),
            Type = ReactionTypeEnum.Action,
            Active = true,
            DisplayOrder = 1,
            EmojiUnicode = "aaaaaaaa",
            EmojiUrl = "https://github.com/andersoncrs5",
            Visible = true
        };

        HttpResponseMessage message = await client.PostAsJsonAsync("api/v1/reaction", dto);
        message.StatusCode.Should().Be(HttpStatusCode.Created);

        ResponseHttp<ReactionDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<ReactionDto>>();
        
        http.Should().NotBeNull();
        http.Data.Should().NotBeNull();
        
        http.Success.Should().BeTrue();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.DetailsError.Should().BeNull();

        http.Data.Id.Should().NotBeNullOrWhiteSpace();
        http.Data.Name.Should().Be(dto.Name);
        http.Data.Type.Should().Be(dto.Type);
        http.Data.EmojiUrl.Should().Be(dto.EmojiUrl);
        http.Data.EmojiUnicode.Should().Be(dto.EmojiUnicode);
        http.Data.DisplayOrder.Should().Be(dto.DisplayOrder);
        http.Data.Active.Should().Be(dto.Active);
        http.Data.Visible.Should().Be(dto.Visible);
        
        return http.Data;
    }
    
    public async Task<PostFavoriteDto> CreatePostFavorite(UserTestResult user, PostDto postDto)
    {
        client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", user.Tokens.Token);

        HttpResponseMessage message = await client.PostAsync($"api/v1/post-favorite/{postDto.Id}/toggle", null); 
        message.StatusCode.Should().Be(HttpStatusCode.Created);

        ResponseHttp<PostFavoriteDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<PostFavoriteDto>>();
        http.Should().NotBeNull();
        http.Success.Should().BeTrue();
        
        http.Data.Should().NotBeNull();
        http.Data.Id.Should().NotBeNullOrWhiteSpace();
        http.Data.PostId.Should().Be(postDto.Id);
        http.Data.UserId.Should().Be(user.User.Id);
        
        return http.Data;
    }
    
    public async Task<CommentDto> CreateComment(UserTestResult user, PostDto postDto, string? parentId = null)
    {
        
        CreateCommentDto dto = new()
        {
            PostId = postDto.Id,
            Content = string.Concat(Enumerable.Repeat("AnyContent", 20)),
            ParentId = parentId
        };

        HttpResponseMessage message = await client.PostAsJsonAsync("api/v1/comment", dto);
        message.StatusCode.Should().Be(HttpStatusCode.Created);

        ResponseHttp<CommentDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<CommentDto>>();
        
        http.Should().NotBeNull();
        http.Success.Should().BeTrue();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        
        http.Data.Should().NotBeNull();
        
        http.Data.Id.Should().NotBeNullOrWhiteSpace();
        http.Data.Content.Should().Be(dto.Content);
        http.Data.PostId.Should().Be(dto.PostId);
        http.Data.ParentId.Should().Be(dto.ParentId);
        http.Data.UserId.Should().Be(user.User.Id);
        
        return http.Data;
    }
    
    public async Task<PostTagDto> CreateTagToPost(UserTestResult userTest, TagDto tagDto, PostDto postDto)
    {
        CreatePostTagDto dto = new ()
        {
            PostId = postDto.Id,
            TagId = tagDto.Id,
        };
        
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", userTest.Tokens.Token);

        HttpResponseMessage message = await client.PostAsJsonAsync($"api/v1/post-tag", dto);
        message.StatusCode.Should().Be(HttpStatusCode.Created);

        ResponseHttp<PostTagDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<PostTagDto>>();
        
        http.Should().NotBeNull();
        http.Message.Should().NotBeNullOrEmpty();
        http.TraceId.Should().NotBeNullOrEmpty();
        
        http.Success.Should().BeTrue();

        http.Data.Should().NotBeNull();
        http.Data.Id.Should().NotBeNullOrEmpty();
        http.Data.TagId.Should().Be(dto.TagId);
        http.Data.PostId.Should().Be(dto.PostId);
        
        return http.Data;
    }
    
    public async Task<PostCategoryDto> CreatePostCategory(CategoryDto categoryDto, PostDto postDto, UserTestResult userResult)
    {
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", userResult.Tokens.Token);

        CreatePostCategoryDto dto = new()
        {
            CategoryId = categoryDto.Id,
            PostId = postDto.Id,
            Order = 1
        };

        HttpResponseMessage message = await client.PostAsJsonAsync("api/v1/post-category", dto);
        message.StatusCode.Should().Be(HttpStatusCode.Created);
        
        ResponseHttp<PostCategoryDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<PostCategoryDto>>();
        
        http.Should().NotBeNull();
        
        http.Success.Should().BeTrue();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.DetailsError.Should().BeNull();

        http.Data.Should().NotBeNull();
        http.Data.CategoryId.Should().Be(dto.CategoryId);
        http.Data.PostId.Should().Be(dto.PostId);
        http.Data.Order.Should().Be(dto.Order);
        
        return http.Data;
    }
    
    public async Task<PostDto> CreatePostAsync(UserTestResult user)
    {
        CreatePostDto dto = new CreatePostDto()
        {
            Title = Guid.NewGuid().ToString(),
            Slug = GenerateChars().ToLower(),
            Content = string.Concat(Enumerable.Repeat("Content", 30)),
            Summary = "Summary",
            FeaturedImageUrl = "https://preview.redd.it/how-is-pochita-so-strong-v0-l01qqnevch8e1.jpeg?width=1080&crop=smart&auto=webp&s=656328dd522bc25a474c84dc53afe06935f5c262",
            Visibility = PostVisibilityEnum.Public,
            ReadingTime = 10,
            IsCommentsEnabled = true,
            ReadingLevel = ReadingLevelEnum.Medium,
            PostType = PostTypeEnum.Opinion
        };
        
        client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", user.Tokens.Token);

        HttpResponseMessage message = await client.PostAsJsonAsync($"api/v1/post", dto);
        message.StatusCode.Should().Be(HttpStatusCode.Created);

        ResponseHttp<PostDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<PostDto>>();
        
        http.Should().NotBeNull();
        http.Success.Should().BeTrue();
        http.Data.Should().NotBeNull();

        http.Data.Id.Should().NotBeNullOrWhiteSpace();
        http.Data.Title.Should().Be(dto.Title);
        http.Data.Slug.Should().Be(dto.Slug);
        http.Data.Content.Should().Be(dto.Content);
        http.Data.Summary.Should().Be(dto.Summary);
        http.Data.FeaturedImageUrl.Should().Be(dto.FeaturedImageUrl);
        http.Data.Visibility.Should().Be(dto.Visibility);
        http.Data.ReadingTime.Should().Be(dto.ReadingTime);
        http.Data.IsCommentsEnabled.Should().Be(dto.IsCommentsEnabled);
        http.Data.ReadingLevel.Should().Be(dto.ReadingLevel);
        http.Data.PostType.Should().Be(dto.PostType);
        http.Data.UserId.Should().Be(user.User.Id);
        
        return http.Data;
    }
    
    public async Task<TagDto> CreateTag(UserTestResult result)
    {
        CreateTagDto dto = new CreateTagDto()
        {
            Name = "TagName" + GenerateChars(),
            Slug = "tag-name" + GenerateChars().ToLower(),
            Description = "TagDescription",
            Color = "#000000",
            IsActive = true,
            IsSystem = true,
            IsVisible = true,
        };
        
        client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", result.Tokens.Token);

        HttpResponseMessage message = await client.PostAsJsonAsync("api/v1/tag", dto);
        message.StatusCode.Should().Be(HttpStatusCode.Created);
        
        ResponseHttp<TagDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<TagDto>>();
        
        http.Should().NotBeNull();
        http.Data.Should().NotBeNull();
        
        http.Success.Should().BeTrue();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.DetailsError.Should().BeNull();

        http.Data.Id.Should().NotBeNullOrWhiteSpace();
        http.Data.Name.Should().Be(dto.Name);
        http.Data.Slug.Should().Be(dto.Slug);
        http.Data.Description.Should().Be(dto.Description);
        http.Data.Color.Should().Be(dto.Color);
        http.Data.IsActive.Should().Be(dto.IsActive);
        http.Data.IsSystem.Should().Be(dto.IsSystem);
        http.Data.IsVisible.Should().Be(dto.IsVisible);
        
        return http.Data;
    }
    
    public async Task<CategoryDto> CreateCategory(UserTestResult result, string? parentId = null)
    {
        CreateCategoryDto dto = new CreateCategoryDto()
        {
            Name = "category" + GenerateChars(),
            Slug = "category-" + GenerateChars().ToLower(),
            Description = null,
            IconName = "fa-solid fa-microchip",
            Color = null,
            IsActive = true,
            IsVisible = true,
            DisplayOrder = 1,
            ParentId = parentId
        };

        client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", result.Tokens.Token);

        HttpResponseMessage message = await client.PostAsJsonAsync("api/v1/category", dto);
        message.StatusCode.Should().Be(HttpStatusCode.Created);
        
        ResponseHttp<CategoryDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<CategoryDto>>();
        
        http.Should().NotBeNull();
        http.Data.Should().NotBeNull();
        
        http.Success.Should().BeTrue();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.DetailsError.Should().BeNull();

        http.Data.Id.Should().NotBeNullOrWhiteSpace();
        http.Data.Name.Should().Be(dto.Name);
        http.Data.Slug.Should().Be(dto.Slug);
    
        return http.Data;
    }
    
    public async Task<UserTestResult> LoginMaster()
    {
        LoginUserDto dto = new LoginUserDto()
        {
            Email = "usersystem@example.com",
            Password = "Aw53653%54Fe!",
        };

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/v1/Auth/login", dto);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        ResponseHttp<ResponseTokens>? http = await response.Content.ReadFromJsonAsync<ResponseHttp<ResponseTokens>>();
        
        http.Should().NotBeNull();
        http.Data.Should().NotBeNull();
        http.Data.User.Should().NotBeNull();
        
        http.Data.Token.Should().NotBeNullOrEmpty();
        http.Data.RefreshToken.Should().NotBeNullOrEmpty();
        http.DetailsError.Should().BeNullOrWhiteSpace();
        http.Success.Should().BeTrue();

        CreateUserDto dtoRe = new CreateUserDto()
        {
            Email = http.Data.User.Email!,
            Username = http.Data.User.Username!,
            FullName = http.Data.User.FullName,
            PasswordHash = dto.Password
        };
        
        return new UserTestResult()
        {
            Dto = dtoRe ,
            Tokens = http.Data,
            User = http.Data.User,
        };
    }
    
    public async Task<UserTestResult> CreateNewUser()
    {
        long num = Random.Shared.NextInt64(1, 10000000000000);
        
        CreateUserDto dto = new CreateUserDto()
        {
            Email = $"user{num}@gmail.com",
            PasswordHash = "test6rA553e463$#%$%",
            Username = "pochita" + num,
            FullName = "pochita the chainsaw demon"
        };
        
        HttpResponseMessage response = await client.PostAsJsonAsync("/api/v1/Auth/register", dto);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        ResponseHttp<ResponseTokens>? http = await response.Content.ReadFromJsonAsync<ResponseHttp<ResponseTokens>>();
        
        http.Should().NotBeNull();
        http.Data.Should().NotBeNull();
        http.Data.User.Should().NotBeNull();
        
        http.Data.Token.Should().NotBeNullOrEmpty();
        http.Data.RefreshToken.Should().NotBeNullOrEmpty();
        http.DetailsError.Should().BeNullOrWhiteSpace();
        http.Success.Should().BeTrue();

        return new UserTestResult()
        {
            Dto = dto,
            Tokens = http.Data,
            User = http.Data.User,
        };

    }

    public string GenerateChars()
    {
        return string.Create(25, (object?)null, (buffer, _) =>
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                int asciiCode = Random.Shared.Next(65, 91); 
                buffer[i] = (char)asciiCode;
            }
        });
    }
    
}