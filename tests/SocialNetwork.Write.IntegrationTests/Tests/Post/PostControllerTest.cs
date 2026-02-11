using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using SocialNetwork.Contracts.DTOs.Post;
using SocialNetwork.Contracts.Enums.Post;
using SocialNetwork.Contracts.Utils.Enums;
using SocialNetwork.Contracts.Utils.Res.http;
using SocialNetwork.Write.API.dto.Posts;
using SocialNetwork.Write.API.Models.Enums.Post;
using SocialNetwork.Write.IntegrationTests.Config;
using SocialNetwork.Write.IntegrationTests.Tests.Utils.Classes;
using Xunit.Abstractions;

namespace SocialNetwork.Write.IntegrationTests.Tests.Post;

public class PostControllerTest: BaseIntegrationTest
{
    private readonly HelperTest _helper;
    private readonly ITestOutputHelper _output;
    private readonly string _url = "api/v1/post";

    public PostControllerTest(WriteApiFactory factory, ITestOutputHelper output) 
        : base(factory)
    {
        _output = output;
        _helper = new HelperTest(Client); 
    }

    [Fact]
    public async Task Post_CreatePost()
    {
        UserTestResult user = await _helper.CreateNewUser();

        CreatePostDto dto = new CreatePostDto()
        {
            Title = Guid.NewGuid().ToString(),
            Slug = _helper.GenerateChars().ToLower(),
            Content = string.Concat(Enumerable.Repeat("Content", 30)),
            Summary = "Summary",
            FeaturedImageUrl = "https://preview.redd.it/how-is-pochita-so-strong-v0-l01qqnevch8e1.jpeg?width=1080&crop=smart&auto=webp&s=656328dd522bc25a474c84dc53afe06935f5c262",
            Visibility = PostVisibilityEnum.Public,
            ReadingTime = 10,
            IsCommentsEnabled = true,
            ReadingLevel = ReadingLevelEnum.Medium,
            PostType = PostTypeEnum.Opinion,
            Language = LanguageEnum.English
        };
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", user.Tokens.Token);

        HttpResponseMessage message = await Client.PostAsJsonAsync($"{_url}", dto);
        _output.WriteLine("======================================================================");
        _output.WriteLine(message.Content.ReadAsStringAsync().Result);
        _output.WriteLine("======================================================================");
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
        http.Data.Language.Should().Be(dto.Language);
    }

    [Fact]
    public async Task Post_CreatePostOnPost()
    {
        UserTestResult user = await _helper.CreateNewUser();
        PostDto postDto = await _helper.CreatePostAsync(user);

        CreatePostDto dto = new CreatePostDto()
        {
            Title = Guid.NewGuid().ToString(),
            Slug = _helper.GenerateChars().ToLower(),
            Content = string.Concat(Enumerable.Repeat("Content", 30)),
            Summary = "Summary",
            FeaturedImageUrl = "https://preview.redd.it/how-is-pochita-so-strong-v0-l01qqnevch8e1.jpeg?width=1080&crop=smart&auto=webp&s=656328dd522bc25a474c84dc53afe06935f5c262",
            Visibility = PostVisibilityEnum.Public,
            ReadingTime = 10,
            IsCommentsEnabled = true,
            ReadingLevel = ReadingLevelEnum.Medium,
            PostType = PostTypeEnum.Opinion,
            Language = LanguageEnum.English,
            ParentId = postDto.Id
        };
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", user.Tokens.Token);

        HttpResponseMessage message = await Client.PostAsJsonAsync($"{_url}", dto);
        _output.WriteLine("======================================================================");
        _output.WriteLine(message.Content.ReadAsStringAsync().Result);
        _output.WriteLine("======================================================================");
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
        http.Data.Language.Should().Be(dto.Language);
        http.Data.ParentId.Should().Be(postDto.Id);
    }

    [Fact]
    public async Task ShouldDeletePost_Success()
    {
        UserTestResult user = await _helper.CreateNewUser();
        PostDto postDto = await _helper.CreatePostAsync(user);
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", user.Tokens.Token);

        HttpResponseMessage message = await Client.DeleteAsync($"{_url}/{postDto.Id}");
        message.StatusCode.Should().Be(HttpStatusCode.OK);

        ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        
        http.Should().NotBeNull();
        http.Success.Should().BeTrue();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.Data.Should().BeNull();
    }
   
    [Fact]
    public async Task ShouldDeletePost_Fail_BecauseForbidden()
    {
        UserTestResult user = await _helper.CreateNewUser();
        UserTestResult user2 = await _helper.CreateNewUser();
        PostDto postDto = await _helper.CreatePostAsync(user);
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", user2.Tokens.Token);

        HttpResponseMessage message = await Client.DeleteAsync($"{_url}/{postDto.Id}");
        message.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        
        http.Should().NotBeNull();
        http.Success.Should().BeFalse();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.Data.Should().BeNull();
    }
    
    [Fact]
    public async Task ShouldDeletePost_Fail_NotFound()
    {
        UserTestResult user = await _helper.CreateNewUser();
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", user.Tokens.Token);

        HttpResponseMessage message = await Client.DeleteAsync($"{_url}/{Guid.NewGuid()}");
        message.StatusCode.Should().Be(HttpStatusCode.NotFound);

        ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        
        http.Should().NotBeNull();
        http.Success.Should().BeFalse();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.Data.Should().BeNull();
    }
    
    [Fact]
    public async Task ShouldPatchPost_Success_AllFields()
    {
        UserTestResult user = await _helper.CreateNewUser();
        PostDto postDto = await _helper.CreatePostAsync(user);

        UpdatePostDto dto = new()
        {
            Title = postDto.Title + "updated",
            Slug = postDto.Slug + "-updated",
            Content = postDto.Content + "Update",
            Summary = postDto.Summary + "Update",
            Visibility = PostVisibilityEnum.Private,
            ReadingTime = postDto.ReadingTime + 1,
            IsCommentsEnabled = !postDto.IsCommentsEnabled,
            ReadingLevel = ReadingLevelEnum.Long,
            PostType = PostTypeEnum.Tutorial,
            FeaturedImageUrl = "https://i.pinimg.com/originals/29/8a/9b/298a9b03ccfc458c345658c16ee280e9.jpg",
            Language = LanguageEnum.English,
            Pinned = true
        };
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", user.Tokens.Token);

        HttpResponseMessage message = await Client.PatchAsJsonAsync($"{_url}/{postDto.Id}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.OK);
        
        ResponseHttp<PostDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<PostDto>>();
        
        http.Should().NotBeNull();
        http.Success.Should().BeTrue();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.Data.Should().NotBeNull();
        
        http.Data.Id.Should().Be(postDto.Id);
        http.Data.Title.Should().Be(dto.Title);
        http.Data.Slug.Should().Be(dto.Slug);
        http.Data.Content.Should().Be(dto.Content);
        http.Data.Summary.Should().Be(dto.Summary);
        http.Data.Visibility.Should().Be(dto.Visibility);
        http.Data.FeaturedImageUrl.Should().Be(dto.FeaturedImageUrl);
        http.Data.ReadingTime.Should().Be(dto.ReadingTime);
        http.Data.IsCommentsEnabled.Should().Be(dto.IsCommentsEnabled.Value);
        http.Data.ReadingLevel.Should().Be(dto.ReadingLevel);
        http.Data.PostType.Should().Be(dto.PostType);
        http.Data.Language.Should().Be(dto.Language);
        http.Data.Pinned.Should().Be(dto.Pinned.Value);
    }
    
    [Fact]
    public async Task ShouldPatchPost_Fail_PostNotFound()
    {
        UserTestResult user = await _helper.CreateNewUser();

        UpdatePostDto dto = new ();
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", user.Tokens.Token);

        HttpResponseMessage message = await Client.PatchAsJsonAsync($"{_url}/{Guid.NewGuid()}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.NotFound);
        
        ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        
        http.Should().NotBeNull();
        http.Success.Should().BeFalse();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.Data.Should().BeNull();
    }
    
    [Fact]
    public async Task ShouldPatchPost_Fail_ReturnForbBecauseAnotherUser()
    {
        UserTestResult user = await _helper.CreateNewUser();
        UserTestResult user2 = await _helper.CreateNewUser();
        
        PostDto postDto = await _helper.CreatePostAsync(user);

        UpdatePostDto dto = new ();
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", user2.Tokens.Token);

        HttpResponseMessage message = await Client.PatchAsJsonAsync($"{_url}/{postDto.Id}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        
        ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        
        http.Should().NotBeNull();
        http.Success.Should().BeFalse();
        http.Message.Should().NotBeNullOrWhiteSpace();
        
        http.Data.Should().BeNull();
    }
    
    [Fact]
    public async Task ShouldPatchPost_Success_JustFeaturedImageUrl()
    {
        UserTestResult user = await _helper.CreateNewUser();
        PostDto postDto = await _helper.CreatePostAsync(user);

        UpdatePostDto dto = new()
        {
            FeaturedImageUrl = "https://i.pinimg.com/originals/29/8a/9b/298a9b03ccfc458c345658c16ee280e9.jpg"
        };
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", user.Tokens.Token);

        HttpResponseMessage message = await Client.PatchAsJsonAsync($"{_url}/{postDto.Id}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.OK);
        
        ResponseHttp<PostDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<PostDto>>();
        
        http.Should().NotBeNull();
        http.Success.Should().BeTrue();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.Data.Should().NotBeNull();
        
        http.Data.Id.Should().Be(postDto.Id);
        http.Data.Title.Should().Be(postDto.Title);
        http.Data.Slug.Should().Be(postDto.Slug);
        http.Data.Content.Should().Be(postDto.Content);
        http.Data.Summary.Should().Be(postDto.Summary);
        http.Data.Visibility.Should().Be(postDto.Visibility);
        http.Data.FeaturedImageUrl.Should().Be(dto.FeaturedImageUrl);
        http.Data.ReadingTime.Should().Be(postDto.ReadingTime);
        http.Data.IsCommentsEnabled.Should().Be(postDto.IsCommentsEnabled);
        http.Data.ReadingLevel.Should().Be(postDto.ReadingLevel);
        http.Data.PostType.Should().Be(postDto.PostType);
    }
    
    [Fact]
    public async Task ShouldPatchPost_Success_JustLanguage()
    {
        UserTestResult user = await _helper.CreateNewUser();
        PostDto postDto = await _helper.CreatePostAsync(user);

        UpdatePostDto dto = new()
        {
            Language = LanguageEnum.German
        };
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", user.Tokens.Token);

        HttpResponseMessage message = await Client.PatchAsJsonAsync($"{_url}/{postDto.Id}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.OK);
        
        ResponseHttp<PostDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<PostDto>>();
        
        http.Should().NotBeNull();
        http.Success.Should().BeTrue();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.Data.Should().NotBeNull();
        
        http.Data.Id.Should().Be(postDto.Id);
        http.Data.Title.Should().Be(postDto.Title);
        http.Data.Slug.Should().Be(postDto.Slug);
        http.Data.Content.Should().Be(postDto.Content);
        http.Data.Summary.Should().Be(postDto.Summary);
        http.Data.Visibility.Should().Be(postDto.Visibility);
        http.Data.FeaturedImageUrl.Should().Be(postDto.FeaturedImageUrl);
        http.Data.ReadingTime.Should().Be(postDto.ReadingTime);
        http.Data.IsCommentsEnabled.Should().Be(postDto.IsCommentsEnabled);
        http.Data.ReadingLevel.Should().Be(postDto.ReadingLevel);
        http.Data.PostType.Should().Be(postDto.PostType);
        http.Data.Language.Should().Be(dto.Language);
    }
    
    [Fact]
    public async Task ShouldPatchPost_Success_JustPinned()
    {
        UserTestResult user = await _helper.CreateNewUser();
        PostDto postDto = await _helper.CreatePostAsync(user);

        UpdatePostDto dto = new()
        {
            Pinned = true
        };
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", user.Tokens.Token);

        HttpResponseMessage message = await Client.PatchAsJsonAsync($"{_url}/{postDto.Id}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.OK);
        
        ResponseHttp<PostDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<PostDto>>();
        
        http.Should().NotBeNull();
        http.Success.Should().BeTrue();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.Data.Should().NotBeNull();
        
        http.Data.Id.Should().Be(postDto.Id);
        http.Data.Title.Should().Be(postDto.Title);
        http.Data.Slug.Should().Be(postDto.Slug);
        http.Data.Content.Should().Be(postDto.Content);
        http.Data.Summary.Should().Be(postDto.Summary);
        http.Data.Visibility.Should().Be(postDto.Visibility);
        http.Data.FeaturedImageUrl.Should().Be(postDto.FeaturedImageUrl);
        http.Data.ReadingTime.Should().Be(postDto.ReadingTime);
        http.Data.IsCommentsEnabled.Should().Be(postDto.IsCommentsEnabled);
        http.Data.ReadingLevel.Should().Be(postDto.ReadingLevel);
        http.Data.PostType.Should().Be(postDto.PostType);
        http.Data.Language.Should().Be(postDto.Language);
        http.Data.Pinned.Should().Be(dto.Pinned.Value);
    }
    
    [Fact]
    public async Task ShouldPatchPost_Success_JustPostType()
    {
        UserTestResult user = await _helper.CreateNewUser();
        PostDto postDto = await _helper.CreatePostAsync(user);

        UpdatePostDto dto = new()
        {
            PostType = PostTypeEnum.Tutorial
        };
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", user.Tokens.Token);

        HttpResponseMessage message = await Client.PatchAsJsonAsync($"{_url}/{postDto.Id}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.OK);
        
        ResponseHttp<PostDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<PostDto>>();
        
        http.Should().NotBeNull();
        http.Success.Should().BeTrue();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.Data.Should().NotBeNull();
        
        http.Data.Id.Should().Be(postDto.Id);
        http.Data.Title.Should().Be(postDto.Title);
        http.Data.Slug.Should().Be(postDto.Slug);
        http.Data.Content.Should().Be(postDto.Content);
        http.Data.Summary.Should().Be(postDto.Summary);
        http.Data.Visibility.Should().Be(postDto.Visibility);
        http.Data.FeaturedImageUrl.Should().Be(postDto.FeaturedImageUrl);
        http.Data.ReadingTime.Should().Be(postDto.ReadingTime);
        http.Data.IsCommentsEnabled.Should().Be(postDto.IsCommentsEnabled);
        http.Data.ReadingLevel.Should().Be(postDto.ReadingLevel);
        http.Data.PostType.Should().Be(dto.PostType);
    }
    
    [Fact]
    public async Task ShouldPatchPost_Success_JustReadingLevel()
    {
        UserTestResult user = await _helper.CreateNewUser();
        PostDto postDto = await _helper.CreatePostAsync(user);

        UpdatePostDto dto = new()
        {
            ReadingLevel = ReadingLevelEnum.Long,
        };
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", user.Tokens.Token);

        HttpResponseMessage message = await Client.PatchAsJsonAsync($"{_url}/{postDto.Id}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.OK);
        
        ResponseHttp<PostDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<PostDto>>();
        
        http.Should().NotBeNull();
        http.Success.Should().BeTrue();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.Data.Should().NotBeNull();
        
        http.Data.Id.Should().Be(postDto.Id);
        http.Data.Title.Should().Be(postDto.Title);
        http.Data.Slug.Should().Be(postDto.Slug);
        http.Data.Content.Should().Be(postDto.Content);
        http.Data.Summary.Should().Be(postDto.Summary);
        http.Data.Visibility.Should().Be(postDto.Visibility);
        http.Data.FeaturedImageUrl.Should().Be(postDto.FeaturedImageUrl);
        http.Data.ReadingTime.Should().Be(postDto.ReadingTime);
        http.Data.IsCommentsEnabled.Should().Be(postDto.IsCommentsEnabled);
        http.Data.ReadingLevel.Should().Be(dto.ReadingLevel);
        http.Data.PostType.Should().Be(postDto.PostType);
    }
    
    [Fact]
    public async Task ShouldPatchPost_Success_JustIsCommentsEnabled()
    {
        UserTestResult user = await _helper.CreateNewUser();
        PostDto postDto = await _helper.CreatePostAsync(user);

        UpdatePostDto dto = new()
        {
            IsCommentsEnabled = !postDto.IsCommentsEnabled
        };
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", user.Tokens.Token);

        HttpResponseMessage message = await Client.PatchAsJsonAsync($"{_url}/{postDto.Id}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.OK);
        
        ResponseHttp<PostDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<PostDto>>();
        
        http.Should().NotBeNull();
        http.Success.Should().BeTrue();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.Data.Should().NotBeNull();
        
        http.Data.Id.Should().Be(postDto.Id);
        http.Data.Title.Should().Be(postDto.Title);
        http.Data.Slug.Should().Be(postDto.Slug);
        http.Data.Content.Should().Be(postDto.Content);
        http.Data.Summary.Should().Be(postDto.Summary);
        http.Data.Visibility.Should().Be(postDto.Visibility);
        http.Data.FeaturedImageUrl.Should().Be(postDto.FeaturedImageUrl);
        http.Data.ReadingTime.Should().Be(postDto.ReadingTime);
        http.Data.IsCommentsEnabled.Should().Be(dto.IsCommentsEnabled.Value);
        http.Data.ReadingLevel.Should().Be(postDto.ReadingLevel);
        http.Data.PostType.Should().Be(postDto.PostType);
    }
    
    [Fact]
    public async Task ShouldPatchPost_Success_JustReadingTime()
    {
        UserTestResult user = await _helper.CreateNewUser();
        PostDto postDto = await _helper.CreatePostAsync(user);

        UpdatePostDto dto = new()
        {
            ReadingTime = postDto.ReadingTime + 1
        };
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", user.Tokens.Token);

        HttpResponseMessage message = await Client.PatchAsJsonAsync($"{_url}/{postDto.Id}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.OK);
        
        ResponseHttp<PostDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<PostDto>>();
        
        http.Should().NotBeNull();
        http.Success.Should().BeTrue();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.Data.Should().NotBeNull();
        
        http.Data.Id.Should().Be(postDto.Id);
        http.Data.Title.Should().Be(postDto.Title);
        http.Data.Slug.Should().Be(postDto.Slug);
        http.Data.Content.Should().Be(postDto.Content);
        http.Data.Summary.Should().Be(postDto.Summary);
        http.Data.Visibility.Should().Be(postDto.Visibility);
        http.Data.FeaturedImageUrl.Should().Be(postDto.FeaturedImageUrl);
        http.Data.ReadingTime.Should().Be(dto.ReadingTime);
        http.Data.IsCommentsEnabled.Should().Be(postDto.IsCommentsEnabled);
        http.Data.ReadingLevel.Should().Be(postDto.ReadingLevel);
        http.Data.PostType.Should().Be(postDto.PostType);
    }
    
    [Fact]
    public async Task ShouldPatchPost_Success_JustVisibility()
    {
        UserTestResult user = await _helper.CreateNewUser();
        PostDto postDto = await _helper.CreatePostAsync(user);

        UpdatePostDto dto = new()
        {
            Visibility = PostVisibilityEnum.Private,
        };
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", user.Tokens.Token);

        HttpResponseMessage message = await Client.PatchAsJsonAsync($"{_url}/{postDto.Id}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.OK);
        
        ResponseHttp<PostDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<PostDto>>();
        
        http.Should().NotBeNull();
        http.Success.Should().BeTrue();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.Data.Should().NotBeNull();
        
        http.Data.Id.Should().Be(postDto.Id);
        http.Data.Title.Should().Be(postDto.Title);
        http.Data.Slug.Should().Be(postDto.Slug);
        http.Data.Content.Should().Be(postDto.Content);
        http.Data.Summary.Should().Be(postDto.Summary);
        http.Data.Visibility.Should().Be(dto.Visibility);
        http.Data.FeaturedImageUrl.Should().Be(postDto.FeaturedImageUrl);
        http.Data.ReadingTime.Should().Be(postDto.ReadingTime);
        http.Data.IsCommentsEnabled.Should().Be(postDto.IsCommentsEnabled);
        http.Data.ReadingLevel.Should().Be(postDto.ReadingLevel);
        http.Data.PostType.Should().Be(postDto.PostType);
    }
    
    [Fact]
    public async Task ShouldPatchPost_Success_JustSummary()
    {
        UserTestResult user = await _helper.CreateNewUser();
        PostDto postDto = await _helper.CreatePostAsync(user);

        UpdatePostDto dto = new()
        {
            Summary = postDto.Summary + "Update",
        };
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", user.Tokens.Token);

        HttpResponseMessage message = await Client.PatchAsJsonAsync($"{_url}/{postDto.Id}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.OK);
        
        ResponseHttp<PostDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<PostDto>>();
        
        http.Should().NotBeNull();
        http.Success.Should().BeTrue();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.Data.Should().NotBeNull();
        
        http.Data.Id.Should().Be(postDto.Id);
        http.Data.Title.Should().Be(postDto.Title);
        http.Data.Slug.Should().Be(postDto.Slug);
        http.Data.Content.Should().Be(postDto.Content);
        http.Data.Summary.Should().Be(dto.Summary);
        http.Data.Visibility.Should().Be(postDto.Visibility);
        http.Data.FeaturedImageUrl.Should().Be(postDto.FeaturedImageUrl);
        http.Data.ReadingTime.Should().Be(postDto.ReadingTime);
        http.Data.IsCommentsEnabled.Should().Be(postDto.IsCommentsEnabled);
        http.Data.ReadingLevel.Should().Be(postDto.ReadingLevel);
        http.Data.PostType.Should().Be(postDto.PostType);
    }
    
    [Fact]
    public async Task ShouldPatchPost_Success_JustContent()
    {
        UserTestResult user = await _helper.CreateNewUser();
        PostDto postDto = await _helper.CreatePostAsync(user);

        UpdatePostDto dto = new()
        {
            Content = postDto.Content + "Update",
        };
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", user.Tokens.Token);

        HttpResponseMessage message = await Client.PatchAsJsonAsync($"{_url}/{postDto.Id}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.OK);
        
        ResponseHttp<PostDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<PostDto>>();
        
        http.Should().NotBeNull();
        http.Success.Should().BeTrue();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.Data.Should().NotBeNull();
        
        http.Data.Id.Should().Be(postDto.Id);
        http.Data.Title.Should().Be(postDto.Title);
        http.Data.Slug.Should().Be(postDto.Slug);
        http.Data.Content.Should().Be(dto.Content);
        http.Data.Summary.Should().Be(postDto.Summary);
        http.Data.Visibility.Should().Be(postDto.Visibility);
        http.Data.FeaturedImageUrl.Should().Be(postDto.FeaturedImageUrl);
        http.Data.ReadingTime.Should().Be(postDto.ReadingTime);
        http.Data.IsCommentsEnabled.Should().Be(postDto.IsCommentsEnabled);
        http.Data.ReadingLevel.Should().Be(postDto.ReadingLevel);
        http.Data.PostType.Should().Be(postDto.PostType);
    }
    
    [Fact]
    public async Task ShouldPatchPost_Success_JustSlug()
    {
        UserTestResult user = await _helper.CreateNewUser();
        PostDto postDto = await _helper.CreatePostAsync(user);

        UpdatePostDto dto = new()
        {
            Slug = postDto.Slug + "-updated",
        };
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", user.Tokens.Token);

        HttpResponseMessage message = await Client.PatchAsJsonAsync($"{_url}/{postDto.Id}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.OK);
        
        ResponseHttp<PostDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<PostDto>>();
        
        http.Should().NotBeNull();
        http.Success.Should().BeTrue();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.Data.Should().NotBeNull();
        
        http.Data.Id.Should().Be(postDto.Id);
        http.Data.Title.Should().Be(postDto.Title);
        http.Data.Slug.Should().Be(dto.Slug);
        http.Data.Content.Should().Be(postDto.Content);
        http.Data.Summary.Should().Be(postDto.Summary);
        http.Data.Visibility.Should().Be(postDto.Visibility);
        http.Data.FeaturedImageUrl.Should().Be(postDto.FeaturedImageUrl);
        http.Data.ReadingTime.Should().Be(postDto.ReadingTime);
        http.Data.IsCommentsEnabled.Should().Be(postDto.IsCommentsEnabled);
        http.Data.ReadingLevel.Should().Be(postDto.ReadingLevel);
        http.Data.PostType.Should().Be(postDto.PostType);
    }
    
    [Fact]
    public async Task ShouldPatchPost_Success_JustTitle()
    {
        UserTestResult user = await _helper.CreateNewUser();
        PostDto postDto = await _helper.CreatePostAsync(user);

        UpdatePostDto dto = new()
        {
            Title = postDto.Title + "updated"
        };
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", user.Tokens.Token);

        HttpResponseMessage message = await Client.PatchAsJsonAsync($"{_url}/{postDto.Id}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.OK);
        
        ResponseHttp<PostDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<PostDto>>();
        
        http.Should().NotBeNull();
        http.Success.Should().BeTrue();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.Data.Should().NotBeNull();
        
        http.Data.Id.Should().Be(postDto.Id);
        http.Data.Title.Should().Be(dto.Title);
        http.Data.Slug.Should().Be(postDto.Slug);
        http.Data.Content.Should().Be(postDto.Content);
        http.Data.Summary.Should().Be(postDto.Summary);
        http.Data.Visibility.Should().Be(postDto.Visibility);
        http.Data.FeaturedImageUrl.Should().Be(postDto.FeaturedImageUrl);
        http.Data.ReadingTime.Should().Be(postDto.ReadingTime);
        http.Data.IsCommentsEnabled.Should().Be(postDto.IsCommentsEnabled);
        http.Data.ReadingLevel.Should().Be(postDto.ReadingLevel);
        http.Data.PostType.Should().Be(postDto.PostType);
    }
    
}