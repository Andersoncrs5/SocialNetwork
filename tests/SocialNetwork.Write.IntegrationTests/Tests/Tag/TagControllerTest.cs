using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using SocialNetwork.Contracts.DTOs.Tag;
using SocialNetwork.Contracts.Utils.Res.http;
using SocialNetwork.Write.API.Modules.Tag.Dto;
using SocialNetwork.Write.IntegrationTests.Config;
using SocialNetwork.Write.IntegrationTests.Tests.Utils.Classes;
using Xunit.Abstractions;

namespace SocialNetwork.Write.IntegrationTests.Tests.Tag;

public class TagControllerTest: BaseIntegrationTest
{
    private readonly HelperTest _helper;
    private readonly ITestOutputHelper _output;
    private readonly string _url = "api/v1/tag";

    public TagControllerTest(WriteApiFactory factory, ITestOutputHelper output) 
        : base(factory)
    {
        _output = output;
        _helper = new HelperTest(Client); 
    }
    
    [Fact]
    public async Task CreateTag_Success()
    {
        UserTestResult result = await _helper.LoginMaster();

        CreateTagDto dto = new CreateTagDto()
        {
            Name = "TagName" + _helper.GenerateChars(),
            Slug = "tag-name" + _helper.GenerateChars().ToLower(),
            Description = "TagDescription",
            Color = "#000000",
            IsActive = true,
            IsSystem = true,
            IsVisible = true,
        };
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", result.Tokens.Token);

        HttpResponseMessage message = await Client.PostAsJsonAsync(_url, dto);
        _output.WriteLine(message.Content.ReadAsStringAsync().Result);
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
    }
    
    [Fact]
    public async Task CreateTag_Fail_ShouldReturnStatusForbidden()
    {
        UserTestResult result = await _helper.CreateNewUser();

        CreateTagDto dto = new CreateTagDto()
        {
            Name = "TagName" + _helper.GenerateChars(),
            Slug = "tag-name" + _helper.GenerateChars().ToLower(),
            Description = "TagDescription",
            Color = "#000000",
            IsActive = true,
            IsSystem = true,
            IsVisible = true,
        };
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", result.Tokens.Token);

        HttpResponseMessage message = await Client.PostAsJsonAsync(_url, dto);
        message.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    [Fact]
    public async Task Delete_Success()
    {
        UserTestResult result = await _helper.LoginMaster();
        TagDto tagDto = await _helper.CreateTag(result);
        
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", result.Tokens.Token);
        
        HttpResponseMessage message = await Client.DeleteAsync($"{_url}/{tagDto.Id}");
        message.StatusCode.Should().Be(HttpStatusCode.OK);
        
        ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        
        http.Should().NotBeNull();
        http.Data.Should().BeNull();
        
        http.Success.Should().BeTrue();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.DetailsError.Should().BeNull();
    }
    
    [Fact]
    public async Task Delete_Fail_ShouldReturnStatus404NotFound()
    {
        UserTestResult result = await _helper.LoginMaster();
        
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", result.Tokens.Token);
        
        HttpResponseMessage message = await Client.DeleteAsync($"{_url}/{Guid.NewGuid()}");
        message.StatusCode.Should().Be(HttpStatusCode.NotFound);
        
        ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        
        http.Should().NotBeNull();
        http.Data.Should().BeNull();
        
        http.Success.Should().BeFalse();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.Message.Should().NotBeNullOrWhiteSpace();
    }
    
    [Fact]
    public async Task Delete_Fail_ShouldReturnStatusForbidden()
    {
        UserTestResult result = await _helper.CreateNewUser();
        
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", result.Tokens.Token);
        
        HttpResponseMessage message = await Client.DeleteAsync($"{_url}/{Guid.NewGuid()}");
        message.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    [Fact]
    public async Task Update_Success_ShouldUpdateAllFields()
    {
        UserTestResult result = await _helper.LoginMaster();
        TagDto tagDto = await _helper.CreateTag(result);
        
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", result.Tokens.Token);

        UpdateTagDto dto = new ()
        {
            Name = "TagName" + "updated",
            Slug = "tag-name" + "updated",
            Description = "TagDescription" + "updated",
            Color = "#000001",
            IsActive = !tagDto.IsActive,
            IsSystem = !tagDto.IsSystem,
            IsVisible = !tagDto.IsVisible,
        };
        
        HttpResponseMessage message = await Client.PatchAsJsonAsync($"{_url}/{tagDto.Id}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.OK);
        
        ResponseHttp<TagDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<TagDto>>();
        
        http.Should().NotBeNull();
        http.Data.Should().NotBeNull();
        
        http.Success.Should().BeTrue();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.DetailsError.Should().BeNull();
        
        http.Data.Id.Should().Be(tagDto.Id);
        http.Data.Name.Should().Be(dto.Name);
        http.Data.Slug.Should().Be(dto.Slug);
        http.Data.Description.Should().Be(dto.Description);
        http.Data.Color.Should().Be(dto.Color);
        http.Data.IsActive.Should().Be(dto.IsActive.HasValue);
        http.Data.IsSystem.Should().Be(dto.IsSystem.HasValue);
        http.Data.IsVisible.Should().Be(dto.IsVisible.HasValue);
    }

    [Fact]
    public async Task Update_Success_ShouldUpdateJustName()
    {
        UserTestResult result = await _helper.LoginMaster();
        TagDto tagDto = await _helper.CreateTag(result);
        
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", result.Tokens.Token);

        UpdateTagDto dto = new ()
        {
            Name = "TagNameUpdated"
        };
        
        HttpResponseMessage message = await Client.PatchAsJsonAsync($"{_url}/{tagDto.Id}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.OK);
        
        ResponseHttp<TagDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<TagDto>>();
        
        http.Should().NotBeNull();
        http.Data.Should().NotBeNull();
        
        http.Success.Should().BeTrue();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.DetailsError.Should().BeNull();
        
        http.Data.Id.Should().Be(tagDto.Id);
        http.Data.Name.Should().Be(dto.Name);
        http.Data.Slug.Should().Be(tagDto.Slug);
        http.Data.Description.Should().Be(tagDto.Description);
        http.Data.Color.Should().Be(tagDto.Color);
        http.Data.IsActive.Should().Be(tagDto.IsActive);
        http.Data.IsSystem.Should().Be(tagDto.IsSystem);
        http.Data.IsVisible.Should().Be(tagDto.IsVisible);
    }

    [Fact]
    public async Task Update_Success_ShouldUpdateJustSlug()
    {
        UserTestResult result = await _helper.LoginMaster();
        TagDto tagDto = await _helper.CreateTag(result);
        
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", result.Tokens.Token);

        UpdateTagDto dto = new ()
        {
            Slug = "tag-name-updated"
        };
        
        HttpResponseMessage message = await Client.PatchAsJsonAsync($"{_url}/{tagDto.Id}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.OK);
        
        ResponseHttp<TagDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<TagDto>>();
        
        http.Should().NotBeNull();
        http.Data.Should().NotBeNull();
        
        http.Success.Should().BeTrue();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.DetailsError.Should().BeNull();
        
        http.Data.Id.Should().Be(tagDto.Id);
        http.Data.Name.Should().Be(tagDto.Name);
        http.Data.Slug.Should().Be(dto.Slug);
        http.Data.Description.Should().Be(tagDto.Description);
        http.Data.Color.Should().Be(tagDto.Color);
        http.Data.IsActive.Should().Be(tagDto.IsActive);
        http.Data.IsSystem.Should().Be(tagDto.IsSystem);
        http.Data.IsVisible.Should().Be(tagDto.IsVisible);
    }

    [Fact]
    public async Task Update_Success_ShouldUpdateJustDescription()
    {
        UserTestResult result = await _helper.LoginMaster();
        TagDto tagDto = await _helper.CreateTag(result);
        
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", result.Tokens.Token);

        UpdateTagDto dto = new ()
        {
            Description = "TagDescription" + "updated",
        };
        
        HttpResponseMessage message = await Client.PatchAsJsonAsync($"{_url}/{tagDto.Id}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.OK);
        
        ResponseHttp<TagDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<TagDto>>();
        
        http.Should().NotBeNull();
        http.Data.Should().NotBeNull();
        
        http.Success.Should().BeTrue();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.DetailsError.Should().BeNull();
        
        http.Data.Id.Should().Be(tagDto.Id);
        http.Data.Name.Should().Be(tagDto.Name);
        http.Data.Slug.Should().Be(tagDto.Slug);
        http.Data.Description.Should().Be(dto.Description);
        http.Data.Color.Should().Be(tagDto.Color);
        http.Data.IsActive.Should().Be(tagDto.IsActive);
        http.Data.IsSystem.Should().Be(tagDto.IsSystem);
        http.Data.IsVisible.Should().Be(tagDto.IsVisible);
    }

    [Fact]
    public async Task Update_Success_ShouldUpdateJustColor()
    {
        UserTestResult result = await _helper.LoginMaster();
        TagDto tagDto = await _helper.CreateTag(result);
        
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", result.Tokens.Token);

        UpdateTagDto dto = new ()
        {
            Color = "#000001",
        };
        
        HttpResponseMessage message = await Client.PatchAsJsonAsync($"{_url}/{tagDto.Id}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.OK);
        
        ResponseHttp<TagDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<TagDto>>();
        
        http.Should().NotBeNull();
        http.Data.Should().NotBeNull();
        
        http.Success.Should().BeTrue();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.DetailsError.Should().BeNull();
        
        http.Data.Id.Should().Be(tagDto.Id);
        http.Data.Name.Should().Be(tagDto.Name);
        http.Data.Slug.Should().Be(tagDto.Slug);
        http.Data.Description.Should().Be(tagDto.Description);
        http.Data.Color.Should().Be(dto.Color);
        http.Data.IsActive.Should().Be(tagDto.IsActive);
        http.Data.IsSystem.Should().Be(tagDto.IsSystem);
        http.Data.IsVisible.Should().Be(tagDto.IsVisible);
    }

    [Fact]
    public async Task Update_Success_ShouldUpdateJustIsActive()
    {
        UserTestResult result = await _helper.LoginMaster();
        TagDto tagDto = await _helper.CreateTag(result);
        
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", result.Tokens.Token);

        UpdateTagDto dto = new ()
        {
            IsActive = !tagDto.IsActive,
        };
        
        HttpResponseMessage message = await Client.PatchAsJsonAsync($"{_url}/{tagDto.Id}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.OK);
        
        ResponseHttp<TagDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<TagDto>>();
        
        http.Should().NotBeNull();
        http.Data.Should().NotBeNull();
        
        http.Success.Should().BeTrue();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.DetailsError.Should().BeNull();
        
        http.Data.Id.Should().Be(tagDto.Id);
        http.Data.Name.Should().Be(tagDto.Name);
        http.Data.Slug.Should().Be(tagDto.Slug);
        http.Data.Description.Should().Be(tagDto.Description);
        http.Data.Color.Should().Be(tagDto.Color);
        http.Data.IsActive.Should().Be(dto.IsActive.HasValue);
        http.Data.IsSystem.Should().Be(tagDto.IsSystem);
        http.Data.IsVisible.Should().Be(tagDto.IsVisible);
    }

    [Fact]
    public async Task Update_Success_ShouldUpdateJustIsSystem()
    {
        UserTestResult result = await _helper.LoginMaster();
        TagDto tagDto = await _helper.CreateTag(result);
        
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", result.Tokens.Token);

        UpdateTagDto dto = new ()
        {
            IsSystem = !tagDto.IsSystem,
        };
        
        HttpResponseMessage message = await Client.PatchAsJsonAsync($"{_url}/{tagDto.Id}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.OK);
        
        ResponseHttp<TagDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<TagDto>>();
        
        http.Should().NotBeNull();
        http.Data.Should().NotBeNull();
        
        http.Success.Should().BeTrue();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.DetailsError.Should().BeNull();
        
        http.Data.Id.Should().Be(tagDto.Id);
        http.Data.Name.Should().Be(tagDto.Name);
        http.Data.Slug.Should().Be(tagDto.Slug);
        http.Data.Description.Should().Be(tagDto.Description);
        http.Data.Color.Should().Be(tagDto.Color);
        http.Data.IsActive.Should().Be(tagDto.IsActive);
        http.Data.IsSystem.Should().Be(dto.IsSystem.HasValue);
        http.Data.IsVisible.Should().Be(tagDto.IsVisible);
    }

    [Fact]
    public async Task Update_Success_ShouldUpdateJustFieldIsVisible()
    {
        UserTestResult result = await _helper.LoginMaster();
        TagDto tagDto = await _helper.CreateTag(result);
        
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", result.Tokens.Token);

        UpdateTagDto dto = new ()
        {
            IsVisible = !tagDto.IsVisible,
        };
        
        HttpResponseMessage message = await Client.PatchAsJsonAsync($"{_url}/{tagDto.Id}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.OK);
        
        ResponseHttp<TagDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<TagDto>>();
        
        http.Should().NotBeNull();
        http.Data.Should().NotBeNull();
        
        http.Success.Should().BeTrue();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.DetailsError.Should().BeNull();
        
        http.Data.Id.Should().Be(tagDto.Id);
        http.Data.Name.Should().Be(tagDto.Name);
        http.Data.Slug.Should().Be(tagDto.Slug);
        http.Data.Description.Should().Be(tagDto.Description);
        http.Data.Color.Should().Be(tagDto.Color);
        http.Data.IsActive.Should().Be(tagDto.IsActive);
        http.Data.IsSystem.Should().Be(tagDto.IsSystem);
        http.Data.IsVisible.Should().Be(dto.IsVisible.HasValue);
    }

    
}