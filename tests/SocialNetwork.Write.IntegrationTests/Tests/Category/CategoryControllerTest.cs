using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using SocialNetwork.Contracts.DTOs.User;
using SocialNetwork.Contracts.Utils.Res.http;
using SocialNetwork.Write.API.Modules.Category.Dto;
using SocialNetwork.Write.IntegrationTests.Config;
using SocialNetwork.Write.IntegrationTests.Tests.Utils.Classes;
using Xunit.Abstractions;

namespace SocialNetwork.Write.IntegrationTests.Tests.Category;

public class CategoryControllerTest: BaseIntegrationTest
{
    private readonly HelperTest _helper;
    private readonly ITestOutputHelper _output;
    private readonly string _url = "api/v1/category";

    public CategoryControllerTest(WriteApiFactory factory, ITestOutputHelper output) 
        : base(factory)
    {
        _output = output;
        _helper = new HelperTest(Client); 
    }

    [Fact]
    public async Task DeleteCategory_Success()
    {
        UserTestResult result = await _helper.LoginMaster();
        CategoryDto categoryDto = await _helper.CreateCategory(result);

        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", result.Tokens.Token);

        HttpResponseMessage message = await Client.DeleteAsync($"{_url}/{categoryDto.Id}");
        message.StatusCode.Should().Be(HttpStatusCode.OK);
        
        ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        
        http.Should().NotBeNull();
        http.Data.Should().BeNull();
        
        http.Success.Should().BeTrue();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.DetailsError.Should().BeNull();

        http.Data.Should().BeNull();
    }
    
    [Fact]
    public async Task DeleteCategory_Fail_CategoryNotFound()
    {
        UserTestResult result = await _helper.LoginMaster();

        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", result.Tokens.Token);

        HttpResponseMessage message = await Client.DeleteAsync($"{_url}/{Guid.NewGuid()}");
        message.StatusCode.Should().Be(HttpStatusCode.NotFound);
        
        ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        
        http.Should().NotBeNull();
        http.Data.Should().BeNull();
        http.Message.Should().NotBeNull();
        
        http.Success.Should().BeFalse();
        http.TraceId.Should().NotBeNullOrWhiteSpace();

        http.Data.Should().BeNull();
    }
    
    [Fact]
    public async Task DeleteCategory_Fail_RoleRequired()
    {
        UserTestResult newUser = await _helper.CreateNewUser();
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", newUser.Tokens.Token);

        HttpResponseMessage message = await Client.DeleteAsync($"{_url}/{Guid.NewGuid()}");
        
        message.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    [Fact]
    public async Task CreateCategory_Success()
    {
        UserTestResult result = await _helper.LoginMaster();

        CreateCategoryDto dto = new CreateCategoryDto()
        {
            Name = "category" + _helper.GenerateChars(),
            Slug = "category-" + _helper.GenerateChars().ToLower(),
            Description = null,
            IconName = null,
            Color = null,
            IsActive = true,
            IsVisible = true,
            DisplayOrder = 1,
            ParentId = null
        };

        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", result.Tokens.Token);

        HttpResponseMessage message = await Client.PostAsJsonAsync(_url, dto);
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
    }
    
    [Fact]
    public async Task CreateCategory_Success_WithParentId()
    {
        UserTestResult result = await _helper.LoginMaster();
        CategoryDto parent = await _helper.CreateCategory(result);

        CreateCategoryDto dto = new CreateCategoryDto()
        {
            Name = "category" + _helper.GenerateChars(),
            Slug = "category-" + _helper.GenerateChars().ToLower(),
            Description = null,
            IconName = null,
            Color = null,
            IsActive = true,
            IsVisible = true,
            DisplayOrder = 1,
            ParentId = parent.Id
        };

        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", result.Tokens.Token);

        HttpResponseMessage message = await Client.PostAsJsonAsync(_url, dto);
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
        http.Data.ParentId.Should().Be(dto.ParentId);
    }
    
    [Fact]
    public async Task CreateCategory_Fail_Unauthorized()
    {
        UserTestResult result = await _helper.CreateNewUser();

        CreateCategoryDto dto = new CreateCategoryDto()
        {
            Name = "category" + _helper.GenerateChars(),
            Slug = "category-" + _helper.GenerateChars().ToLower(),
            Description = null,
            IconName = null,
            Color = null,
            IsActive = true,
            IsVisible = true,
            DisplayOrder = 1,
            ParentId = null
        };

        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", result.Tokens.Token);

        HttpResponseMessage message = await Client.PostAsJsonAsync(_url, dto);
        message.StatusCode.Should().Be(HttpStatusCode.Forbidden);

    }
    
    [Fact]
    public async Task UpdateCategory_SuccessAllFields()
    {
        UserTestResult result = await _helper.LoginMaster();
        CategoryDto categoryDto = await _helper.CreateCategory(result);
    
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", result.Tokens.Token);
    
        UpdateCategoryDto dto = new UpdateCategoryDto()
        {
            Name = categoryDto.Name + "a",
            Slug = categoryDto.Slug + "a",
            Description = categoryDto.Description + "update",
            IconName = categoryDto.IconName + " fa-camera",
            Color = categoryDto.Color,
            IsActive = !categoryDto.IsActive,
            IsVisible = !categoryDto.IsVisible,
            ParentId = null,
            DisplayOrder = 10
        };
        
        HttpResponseMessage message = await Client.PatchAsJsonAsync($"{_url}/{categoryDto.Id}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.OK);
        
        ResponseHttp<CategoryDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<CategoryDto>>();
        
        http.Should().NotBeNull();
        http.Data.Should().NotBeNull();
        
        http.Success.Should().BeTrue();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.DetailsError.Should().BeNull();
        
        http.Data.Should().NotBeNull();

        http.Data.Id.Should().Be(categoryDto.Id);
        http.Data.Name.Should().Be(dto.Name);
        http.Data.Slug.Should().Be(dto.Slug);
        http.Data.Description.Should().Be(dto.Description);
        http.Data.IconName.Should().Be(dto.IconName);
        http.Data.Color.Should().Be(dto.Color);
        http.Data.IsActive.Should().Be(dto.IsActive.Value);
        http.Data.IsVisible.Should().Be(dto.IsVisible.Value);
        http.Data.ParentId.Should().Be(dto.ParentId);
        http.Data.DisplayOrder.Should().Be(dto.DisplayOrder);
    }
    
    [Fact]
    public async Task UpdateCategory_Success_JustFieldName()
    {
        UserTestResult result = await _helper.LoginMaster();
        CategoryDto categoryDto = await _helper.CreateCategory(result);
    
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", result.Tokens.Token);
    
        UpdateCategoryDto dto = new UpdateCategoryDto()
        {
            Name = categoryDto.Name + "a",
        };
        
        HttpResponseMessage message = await Client.PatchAsJsonAsync($"{_url}/{categoryDto.Id}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.OK);
        
        ResponseHttp<CategoryDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<CategoryDto>>();
        
        http.Should().NotBeNull();
        http.Data.Should().NotBeNull();
        
        http.Success.Should().BeTrue();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.DetailsError.Should().BeNull();

        http.Data.Should().NotBeNull();
        
        http.Data.Id.Should().Be(categoryDto.Id);
        http.Data.Name.Should().Be(dto.Name);
        http.Data.Slug.Should().Be(categoryDto.Slug);
        http.Data.Description.Should().Be(categoryDto.Description);
        http.Data.IconName.Should().Be(categoryDto.IconName);
        http.Data.Color.Should().Be(categoryDto.Color);
        http.Data.IsActive.Should().Be(categoryDto.IsActive);
        http.Data.IsVisible.Should().Be(categoryDto.IsVisible);
        http.Data.ParentId.Should().Be(categoryDto.ParentId);
        http.Data.DisplayOrder.Should().Be(categoryDto.DisplayOrder);
    }
    
    [Fact]
    public async Task UpdateCategory_Success_JustFieldDescription()
    {
        UserTestResult result = await _helper.LoginMaster();
        CategoryDto categoryDto = await _helper.CreateCategory(result);
    
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", result.Tokens.Token);
    
        UpdateCategoryDto dto = new UpdateCategoryDto()
        {
            Description = "descUpdate"
        };
        
        HttpResponseMessage message = await Client.PatchAsJsonAsync($"{_url}/{categoryDto.Id}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.OK);
        
        ResponseHttp<CategoryDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<CategoryDto>>();
        
        http.Should().NotBeNull();
        http.Data.Should().NotBeNull();
        
        http.Success.Should().BeTrue();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.DetailsError.Should().BeNull();

        http.Data.Should().NotBeNull();
        
        http.Data.Id.Should().Be(categoryDto.Id);
        http.Data.Name.Should().Be(categoryDto.Name);
        http.Data.Slug.Should().Be(categoryDto.Slug);
        http.Data.Description.Should().Be(dto.Description);
        http.Data.IconName.Should().Be(categoryDto.IconName);
        http.Data.Color.Should().Be(categoryDto.Color);
        http.Data.IsActive.Should().Be(categoryDto.IsActive);
        http.Data.IsVisible.Should().Be(categoryDto.IsVisible);
        http.Data.ParentId.Should().Be(categoryDto.ParentId);
        http.Data.DisplayOrder.Should().Be(categoryDto.DisplayOrder);
    }
    
    [Fact]
    public async Task UpdateCategory_Success_JustFieldIconName()
    {
        UserTestResult result = await _helper.LoginMaster();
        CategoryDto categoryDto = await _helper.CreateCategory(result);
    
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", result.Tokens.Token);
    
        UpdateCategoryDto dto = new UpdateCategoryDto()
        {
            IconName = "fa-solid fa-microchip",
        };
        
        HttpResponseMessage message = await Client.PatchAsJsonAsync($"{_url}/{categoryDto.Id}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.OK);
        
        ResponseHttp<CategoryDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<CategoryDto>>();
        
        http.Should().NotBeNull();
        http.Data.Should().NotBeNull();
        
        http.Success.Should().BeTrue();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.DetailsError.Should().BeNull();

        http.Data.Should().NotBeNull();
        
        http.Data.Id.Should().Be(categoryDto.Id);
        http.Data.Name.Should().Be(categoryDto.Name);
        http.Data.Slug.Should().Be(categoryDto.Slug);
        http.Data.Description.Should().Be(categoryDto.Description);
        http.Data.IconName.Should().Be(dto.IconName);
        http.Data.Color.Should().Be(categoryDto.Color);
        http.Data.IsActive.Should().Be(categoryDto.IsActive);
        http.Data.IsVisible.Should().Be(categoryDto.IsVisible);
        http.Data.ParentId.Should().Be(categoryDto.ParentId);
        http.Data.DisplayOrder.Should().Be(categoryDto.DisplayOrder);
    }
    
    [Fact]
    public async Task UpdateCategory_Success_JustFieldColor()
    {
        UserTestResult result = await _helper.LoginMaster();
        CategoryDto categoryDto = await _helper.CreateCategory(result);
    
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", result.Tokens.Token);
    
        UpdateCategoryDto dto = new UpdateCategoryDto()
        {
            Color = "000000",
        };
        
        HttpResponseMessage message = await Client.PatchAsJsonAsync($"{_url}/{categoryDto.Id}", dto);
        _output.WriteLine(await message.Content.ReadAsStringAsync());
        message.StatusCode.Should().Be(HttpStatusCode.OK);
        
        ResponseHttp<CategoryDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<CategoryDto>>();
        
        http.Should().NotBeNull();
        http.Data.Should().NotBeNull();
        
        http.Success.Should().BeTrue();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.DetailsError.Should().BeNull();

        http.Data.Should().NotBeNull();
        
        http.Data.Id.Should().Be(categoryDto.Id);
        http.Data.Name.Should().Be(categoryDto.Name);
        http.Data.Slug.Should().Be(categoryDto.Slug);
        http.Data.Description.Should().Be(categoryDto.Description);
        http.Data.IconName.Should().Be(categoryDto.IconName);
        http.Data.Color.Should().Be(dto.Color);
        http.Data.IsActive.Should().Be(categoryDto.IsActive);
        http.Data.IsVisible.Should().Be(categoryDto.IsVisible);
        http.Data.ParentId.Should().Be(categoryDto.ParentId);
        http.Data.DisplayOrder.Should().Be(categoryDto.DisplayOrder);
    }
    
    [Fact]
    public async Task UpdateCategory_Success_JustFieldSlug()
    {
        UserTestResult result = await _helper.LoginMaster();
        CategoryDto categoryDto = await _helper.CreateCategory(result);
    
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", result.Tokens.Token);
    
        UpdateCategoryDto dto = new UpdateCategoryDto()
        {
            Slug = categoryDto.Slug + this._helper.GenerateChars().ToLower(),
        };
        
        HttpResponseMessage message = await Client.PatchAsJsonAsync($"{_url}/{categoryDto.Id}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.OK);
        
        ResponseHttp<CategoryDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<CategoryDto>>();
        
        http.Should().NotBeNull();
        http.Data.Should().NotBeNull();
        
        http.Success.Should().BeTrue();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.DetailsError.Should().BeNull();

        http.Data.Should().NotBeNull();
        
        http.Data.Id.Should().Be(categoryDto.Id);
        http.Data.Name.Should().Be(categoryDto.Name);
        http.Data.Slug.Should().Be(dto.Slug);
        http.Data.Description.Should().Be(categoryDto.Description);
        http.Data.IconName.Should().Be(categoryDto.IconName);
        http.Data.Color.Should().Be(categoryDto.Color);
        http.Data.IsActive.Should().Be(categoryDto.IsActive);
        http.Data.IsVisible.Should().Be(categoryDto.IsVisible);
        http.Data.ParentId.Should().Be(categoryDto.ParentId);
        http.Data.DisplayOrder.Should().Be(categoryDto.DisplayOrder);
    }


    [Fact]
    public async Task UpdateCategory_Success_JustFieldIsActive()
    {
        UserTestResult result = await _helper.LoginMaster();
        CategoryDto categoryDto = await _helper.CreateCategory(result);
    
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", result.Tokens.Token);
    
        UpdateCategoryDto dto = new UpdateCategoryDto()
        {
            IsActive = !categoryDto.IsActive
        };
        
        HttpResponseMessage message = await Client.PatchAsJsonAsync($"{_url}/{categoryDto.Id}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.OK);
        
        ResponseHttp<CategoryDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<CategoryDto>>();
        
        http.Should().NotBeNull();
        http.Data.Should().NotBeNull();
        
        http.Success.Should().BeTrue();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.DetailsError.Should().BeNull();

        http.Data.Should().NotBeNull();
        
        http.Data.Id.Should().Be(categoryDto.Id);
        http.Data.Name.Should().Be(categoryDto.Name);
        http.Data.Slug.Should().Be(categoryDto.Slug);
        http.Data.Description.Should().Be(categoryDto.Description);
        http.Data.IconName.Should().Be(categoryDto.IconName);
        http.Data.Color.Should().Be(categoryDto.Color);
        http.Data.IsActive.Should().Be(dto.IsActive.Value);
        http.Data.IsVisible.Should().Be(categoryDto.IsVisible);
        http.Data.ParentId.Should().Be(categoryDto.ParentId);
        http.Data.DisplayOrder.Should().Be(categoryDto.DisplayOrder);
    }
    
    [Fact]
    public async Task UpdateCategory_Success_JustFieldIsVisible()
    {
        UserTestResult result = await _helper.LoginMaster();
        CategoryDto categoryDto = await _helper.CreateCategory(result);
    
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", result.Tokens.Token);
    
        UpdateCategoryDto dto = new UpdateCategoryDto()
        {
            IsVisible = !categoryDto.IsVisible
        };
        
        HttpResponseMessage message = await Client.PatchAsJsonAsync($"{_url}/{categoryDto.Id}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.OK);
        
        ResponseHttp<CategoryDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<CategoryDto>>();
        
        http.Should().NotBeNull();
        http.Data.Should().NotBeNull();
        
        http.Success.Should().BeTrue();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.DetailsError.Should().BeNull();

        http.Data.Should().NotBeNull();
        
        http.Data.Id.Should().Be(categoryDto.Id);
        http.Data.Name.Should().Be(categoryDto.Name);
        http.Data.Slug.Should().Be(categoryDto.Slug);
        http.Data.Description.Should().Be(categoryDto.Description);
        http.Data.IconName.Should().Be(categoryDto.IconName);
        http.Data.Color.Should().Be(categoryDto.Color);
        http.Data.IsActive.Should().Be(categoryDto.IsActive);
        http.Data.IsVisible.Should().Be(dto.IsVisible.Value);
        http.Data.ParentId.Should().Be(categoryDto.ParentId);
        http.Data.DisplayOrder.Should().Be(categoryDto.DisplayOrder);
    }
    
    [Fact]
    public async Task UpdateCategory_Success_JustFieldDisplayOrder()
    {
        UserTestResult result = await _helper.LoginMaster();
        CategoryDto categoryDto = await _helper.CreateCategory(result);
    
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", result.Tokens.Token);
    
        UpdateCategoryDto dto = new UpdateCategoryDto()
        {
            DisplayOrder = categoryDto.DisplayOrder + 1
        };
        
        HttpResponseMessage message = await Client.PatchAsJsonAsync($"{_url}/{categoryDto.Id}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.OK);
        
        ResponseHttp<CategoryDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<CategoryDto>>();
        
        http.Should().NotBeNull();
        http.Data.Should().NotBeNull();
        
        http.Success.Should().BeTrue();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.DetailsError.Should().BeNull();

        http.Data.Should().NotBeNull();
        
        http.Data.Id.Should().Be(categoryDto.Id);
        http.Data.Name.Should().Be(categoryDto.Name);
        http.Data.Slug.Should().Be(categoryDto.Slug);
        http.Data.Description.Should().Be(categoryDto.Description);
        http.Data.IconName.Should().Be(categoryDto.IconName);
        http.Data.Color.Should().Be(categoryDto.Color);
        http.Data.IsActive.Should().Be(categoryDto.IsActive);
        http.Data.IsVisible.Should().Be(categoryDto.IsVisible);
        http.Data.ParentId.Should().Be(categoryDto.ParentId);
        http.Data.DisplayOrder.Should().Be(dto.DisplayOrder);
    }
    
    [Fact]
    public async Task UpdateCategory_Success_TurnCategoryRoot()
    {
        UserTestResult result = await _helper.LoginMaster();
        CategoryDto parentDto = await _helper.CreateCategory(result);
        CategoryDto categoryDto = await _helper.CreateCategory(result, parentDto.Id);
    
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", result.Tokens.Token);
    
        UpdateCategoryDto dto = new UpdateCategoryDto()
        {
            BecameRoot = true
        };
        
        HttpResponseMessage message = await Client.PatchAsJsonAsync($"{_url}/{categoryDto.Id}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.OK);
        
        ResponseHttp<CategoryDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<CategoryDto>>();
        
        http.Should().NotBeNull();
        http.Data.Should().NotBeNull();
        
        http.Success.Should().BeTrue();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.DetailsError.Should().BeNull();

        http.Data.Should().NotBeNull();
        
        http.Data.Name.Should().Be(categoryDto.Name);
        http.Data.Slug.Should().Be(categoryDto.Slug);
        http.Data.Description.Should().Be(categoryDto.Description);
        http.Data.IconName.Should().Be(categoryDto.IconName);
        http.Data.Color.Should().Be(categoryDto.Color);
        http.Data.IsActive.Should().Be(categoryDto.IsActive);
        http.Data.IsVisible.Should().Be(categoryDto.IsVisible);
        http.Data.DisplayOrder.Should().Be(categoryDto.DisplayOrder);
        
        http.Data.ParentId.Should().BeNull();
    }
    
    [Fact]
    public async Task UpdateCategory_Success_JustParent()
    {
        UserTestResult result = await _helper.LoginMaster();
        CategoryDto parentDto = await _helper.CreateCategory(result);
        CategoryDto parentDto2 = await _helper.CreateCategory(result);
        CategoryDto categoryDto = await _helper.CreateCategory(result, parentDto.Id);
    
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", result.Tokens.Token);
    
        UpdateCategoryDto dto = new UpdateCategoryDto()
        {
            ParentId = parentDto2.Id
        };
        
        HttpResponseMessage message = await Client.PatchAsJsonAsync($"{_url}/{categoryDto.Id}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.OK);
        
        ResponseHttp<CategoryDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<CategoryDto>>();
        
        http.Should().NotBeNull();
        http.Data.Should().NotBeNull();
        
        http.Success.Should().BeTrue();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.DetailsError.Should().BeNull();

        http.Data.Should().NotBeNull();
        
        http.Data.Name.Should().Be(categoryDto.Name);
        http.Data.Slug.Should().Be(categoryDto.Slug);
        http.Data.Description.Should().Be(categoryDto.Description);
        http.Data.IconName.Should().Be(categoryDto.IconName);
        http.Data.Color.Should().Be(categoryDto.Color);
        http.Data.IsActive.Should().Be(categoryDto.IsActive);
        http.Data.IsVisible.Should().Be(categoryDto.IsVisible);
        http.Data.ParentId.Should().Be(dto.ParentId);
        http.Data.DisplayOrder.Should().Be(categoryDto.DisplayOrder);
        http.Data.ParentId.Should().Be(parentDto2.Id);
    }
    
}