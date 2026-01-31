using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using SocialNetwork.Contracts.DTOs.User;
using SocialNetwork.Contracts.Utils.Res.http;
using SocialNetwork.Write.API.dto.Category;
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

        var json = JsonSerializer.Serialize(dto);
        _output.WriteLine($"JSON enviado: {json}");
        HttpResponseMessage message = await Client.PostAsJsonAsync(_url, dto);
        _output.WriteLine(message.Content.ReadAsStringAsync().Result);
        message.StatusCode.Should().Be(HttpStatusCode.Created);
        
        ResponseHttp<CategoryDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<CategoryDto>>();
        
        http.Should().NotBeNull();
        http.Data.Should().BeNull();
        
        http.Success.Should().BeTrue();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.DetailsError.Should().BeNull();

        http.Data.Id.Should().NotBeNullOrWhiteSpace();
        http.Data.Name.Should().Be(dto.Name);
        http.Data.Slug.Should().Be(dto.Slug);
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
    
}