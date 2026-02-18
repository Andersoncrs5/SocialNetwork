using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using SocialNetwork.Contracts.DTOs.Post;
using SocialNetwork.Contracts.DTOs.PostCategory;
using SocialNetwork.Contracts.DTOs.User;
using SocialNetwork.Contracts.Utils.Res.http;
using SocialNetwork.Write.API.Modules.PostCategory.Dto;
using SocialNetwork.Write.IntegrationTests.Config;
using SocialNetwork.Write.IntegrationTests.Tests.Utils.Classes;
using Xunit.Abstractions;

namespace SocialNetwork.Write.IntegrationTests.Tests.PostCategory;

public class PostCategoryControllerTest: BaseIntegrationTest
{
    private readonly HelperTest _helper;
    private readonly ITestOutputHelper _output;
    private readonly string _url = "api/v1/post-category";

    public PostCategoryControllerTest(WriteApiFactory factory, ITestOutputHelper output) 
        : base(factory)
    {
        _helper = new HelperTest(Client);
        _output = output;
    }

    [Fact]
    public async Task CreatePostCategory_Success()
    {
        UserTestResult masterResult = await _helper.LoginMaster();
        CategoryDto categoryDto = await _helper.CreateCategory(masterResult);
        
        UserTestResult userResult = await _helper.CreateNewUser();
        PostDto postDto = await _helper.CreatePostAsync(userResult);
        
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", userResult.Tokens.Token);

        CreatePostCategoryDto dto = new()
        {
            CategoryId = categoryDto.Id,
            PostId = postDto.Id,
            Order = 1
        };

        HttpResponseMessage message = await Client.PostAsJsonAsync($"{_url}", dto);
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
    }

    [Fact]
    public async Task CreatePostCategory_Fail_ReturnForbBecauseAnotherUser()
    {
        UserTestResult masterResult = await _helper.LoginMaster();
        CategoryDto categoryDto = await _helper.CreateCategory(masterResult);
        
        UserTestResult userResult = await _helper.CreateNewUser();
        UserTestResult userResult2 = await _helper.CreateNewUser();
        PostDto postDto = await _helper.CreatePostAsync(userResult);
        
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", userResult2.Tokens.Token);

        CreatePostCategoryDto dto = new()
        {
            CategoryId = categoryDto.Id,
            PostId = postDto.Id,
            Order = 1
        };

        HttpResponseMessage message = await Client.PostAsJsonAsync($"{_url}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        
        ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        
        http.Should().NotBeNull();
        
        http.Success.Should().BeFalse();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.Message.Should().NotBeNullOrWhiteSpace();
        
        http.Data.Should().BeNull();
    }

    [Fact]
    public async Task CreatePostCategory_NotFoundPost()
    {
        UserTestResult masterResult = await _helper.LoginMaster();
        CategoryDto categoryDto = await _helper.CreateCategory(masterResult);
        
        UserTestResult userResult = await _helper.CreateNewUser();
        PostDto postDto = await _helper.CreatePostAsync(userResult);
        
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", userResult.Tokens.Token);

        CreatePostCategoryDto dto = new()
        {
            CategoryId = categoryDto.Id,
            PostId = Guid.NewGuid().ToString(),
            Order = 1
        };

        HttpResponseMessage message = await Client.PostAsJsonAsync($"{_url}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.NotFound);
        
        ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        
        http.Should().NotBeNull();
        
        http.Success.Should().BeFalse();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.DetailsError.Should().NotBeNull();

        http.Data.Should().BeNull();
    }
    
    [Fact]
    public async Task CreatePostCategory_NotFoundCategory()
    {
        UserTestResult masterResult = await _helper.LoginMaster();
        CategoryDto categoryDto = await _helper.CreateCategory(masterResult);
        
        UserTestResult userResult = await _helper.CreateNewUser();
        PostDto postDto = await _helper.CreatePostAsync(userResult);
        
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", userResult.Tokens.Token);

        CreatePostCategoryDto dto = new ()
        {
            CategoryId = Guid.NewGuid().ToString(),
            PostId = postDto.Id,
            Order = 1
        };

        HttpResponseMessage message = await Client.PostAsJsonAsync($"{_url}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.NotFound);
        
        ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        
        http.Should().NotBeNull();
        
        http.Success.Should().BeFalse();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.DetailsError.Should().NotBeNull();

        http.Data.Should().BeNull();
    }
    
    [Fact]
    public async Task DeletePostCategory()
    {
        UserTestResult masterResult = await _helper.LoginMaster();
        CategoryDto categoryDto = await _helper.CreateCategory(masterResult);
        
        UserTestResult userResult = await _helper.CreateNewUser();
        PostDto postDto = await _helper.CreatePostAsync(userResult);
        
        PostCategoryDto postCategoryDto = await _helper.CreatePostCategory(categoryDto, postDto, userResult);

        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", userResult.Tokens.Token);

        HttpResponseMessage message = await Client.DeleteAsync($"{_url}/{postCategoryDto.Id}");
        message.StatusCode.Should().Be(HttpStatusCode.OK);
        
        ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        
        http.Should().NotBeNull();
        
        http.Success.Should().BeTrue();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.DetailsError.Should().BeNull();

        http.Data.Should().BeNull();
    }
    
    [Fact]
    public async Task DeletePostCategory_Success_ReturnForbBecauseAnotherUser()
    {
        UserTestResult masterResult = await _helper.LoginMaster();
        CategoryDto categoryDto = await _helper.CreateCategory(masterResult);
        
        UserTestResult userResult = await _helper.CreateNewUser();
        UserTestResult userResult2 = await _helper.CreateNewUser();
        PostDto postDto = await _helper.CreatePostAsync(userResult);
        
        PostCategoryDto postCategoryDto = await _helper.CreatePostCategory(categoryDto, postDto, userResult);

        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", userResult2.Tokens.Token);

        HttpResponseMessage message = await Client.DeleteAsync($"{_url}/{postCategoryDto.Id}");
        message.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        
        ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        
        http.Should().NotBeNull();
        
        http.Success.Should().BeFalse();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.DetailsError.Should().NotBeNull();

        http.Data.Should().BeNull();
    }
    
    [Fact]
    public async Task DeletePostCategory_ReturnNotFound()
    {
        UserTestResult userResult = await _helper.CreateNewUser();
        
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", userResult.Tokens.Token);

        HttpResponseMessage message = await Client.DeleteAsync($"{_url}/{Guid.NewGuid()}");
        message.StatusCode.Should().Be(HttpStatusCode.NotFound);
        
        ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        
        http.Should().NotBeNull();
        
        http.Success.Should().BeFalse();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.DetailsError.Should().NotBeNull();

        http.Data.Should().BeNull();
    }
    
    [Fact]
    public async Task PatchPostCategory_Success_AllFields()
    {
        UserTestResult masterResult = await _helper.LoginMaster();
        CategoryDto categoryDto = await _helper.CreateCategory(masterResult);
        
        UserTestResult userResult = await _helper.CreateNewUser();
        PostDto postDto = await _helper.CreatePostAsync(userResult);
        
        PostCategoryDto postCategoryDto = await _helper.CreatePostCategory(categoryDto, postDto, userResult);

        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", userResult.Tokens.Token);

        UpdatePostCategoryDto dto = new()
        {
            Order = postCategoryDto.Order + 10, 
        };
        
        HttpResponseMessage message = await Client.PatchAsJsonAsync($"{_url}/{postCategoryDto.Id}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.OK);
        
        ResponseHttp<PostCategoryDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<PostCategoryDto>>();
        
        http.Should().NotBeNull();
        
        http.Success.Should().BeTrue();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.DetailsError.Should().BeNull();

        http.Data.Should().NotBeNull();
        
        http.Data.Id.Should().Be(postCategoryDto.Id);
        http.Data.Order.Should().Be(dto.Order);
    }

    [Fact]
    public async Task PatchPostCategory_Fail_ReturnForbAnotherUser()
    {
        UserTestResult masterResult = await _helper.LoginMaster();
        CategoryDto categoryDto = await _helper.CreateCategory(masterResult);
        
        UserTestResult userResult = await _helper.CreateNewUser();
        UserTestResult userResul2 = await _helper.CreateNewUser();
        PostDto postDto = await _helper.CreatePostAsync(userResult);
        
        PostCategoryDto postCategoryDto = await _helper.CreatePostCategory(categoryDto, postDto, userResult);

        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", userResul2.Tokens.Token);

        UpdatePostCategoryDto dto = new()
        {
            Order = postCategoryDto.Order + 10, 
        };
        
        HttpResponseMessage message = await Client.PatchAsJsonAsync($"{_url}/{postCategoryDto.Id}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        
        ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        
        http.Should().NotBeNull();
        
        http.Success.Should().BeFalse();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.Message.Should().NotBeNullOrWhiteSpace();

        http.Data.Should().BeNull();
    }

    [Fact]
    public async Task PatchPostCategory_Fail_ReturnNotFound()
    {
        UserTestResult userResult = await _helper.CreateNewUser();
        
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", userResult.Tokens.Token);

        UpdatePostCategoryDto dto = new()
        {
            Order = 10, 
        };
        
        HttpResponseMessage message = await Client.PatchAsJsonAsync($"{_url}/{Guid.NewGuid()}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.NotFound);
        
        ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        
        http.Should().NotBeNull();
        
        http.Success.Should().BeFalse();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.Message.Should().NotBeNullOrWhiteSpace();
        
        http.Data.Should().BeNull();
        
    }

    
}