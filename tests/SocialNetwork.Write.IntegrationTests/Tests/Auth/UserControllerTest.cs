using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using SocialNetwork.Contracts.DTOs.User;
using SocialNetwork.Contracts.Utils.Res.http;
using SocialNetwork.Write.API.dto.User;
using SocialNetwork.Write.IntegrationTests.Config;
using SocialNetwork.Write.IntegrationTests.Tests.Utils.Classes;
using Xunit.Abstractions;

namespace SocialNetwork.Write.IntegrationTests.Tests.Auth;

public class UserControllerTest : BaseIntegrationTest
{
    private readonly HelperTest _helper;
    private readonly ITestOutputHelper _output;
    private readonly string _url = "api/v1/user";

    public UserControllerTest(WriteApiFactory factory, ITestOutputHelper output) 
        : base(factory)
    {
        _output = output;
        _helper = new HelperTest(Client); 
    }

    [Fact]
    public async Task DeleteUser_Success()
    {
        UserTestResult result = await _helper.CreateNewUser();
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", result.Tokens.Token);

        HttpResponseMessage message = await Client.DeleteAsync(_url);
        _output.WriteLine(await message.Content.ReadAsStringAsync());
        message.StatusCode.Should().Be(HttpStatusCode.OK);

        ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        
        http.Should().NotBeNull();
        http.Data.Should().BeNull();
        
        http.Success.Should().BeTrue();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.DetailsError.Should().BeNull();
    }

    [Fact]
    public async Task UpdateUser_Success_AllFields()
    {
        UserTestResult result = await _helper.CreateNewUser();

        UpdateUserDto dto = new UpdateUserDto()
        {
            Bio = "AnyBio",
            BirthDate = DateTime.UtcNow.AddYears(-30),
            Country = "USA",
            CoverImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTQ0SFRurRwxC7u88oa0JQRTFzo0wn7XTud4Q&s",
            FullName = "pochita the chainsaw demon",
            ImageProfileUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTQ0SFRurRwxC7u88oa0JQRTFzo0wn7XTud4Q&s",
            IsPrivate = false,
            Language = "EN",
            PasswordHash = "AnbbgygyyGTD4866756%%$%f",
            Username = $"{result.User.Username}1"
       };
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", result.Tokens.Token);

        HttpResponseMessage message = await Client.PatchAsJsonAsync(_url, dto);
        message.StatusCode.Should().Be(HttpStatusCode.OK);

        ResponseHttp<UserDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<UserDto>>();
        
        http.Should().NotBeNull();
        http.DetailsError.Should().BeNull();
        http.Success.Should().BeTrue();
        http.TraceId.Should().NotBeNullOrWhiteSpace();

        http.Data.Should().NotBeNull();
        http.Data.Id.Should().Be(result.User.Id);
        http.Data.Email.Should().Be(result.User.Email);
    
        http.Data.Bio.Should().Be(dto.Bio);
        http.Data.Country.Should().Be(dto.Country);
        http.Data.CoverImageUrl.Should().Be(dto.CoverImageUrl);
        http.Data.FullName.Should().Be(dto.FullName);
        http.Data.ImageProfileUrl.Should().Be(dto.ImageProfileUrl);
        http.Data.Language.Should().Be(dto.Language);
        http.Data.Username.Should().Be(dto.Username);

        http.Data.IsPrivate.Should().Be(dto.IsPrivate!.Value);
    
        http.Data.BirthDate.Should().BeCloseTo(dto.BirthDate!.Value, TimeSpan.FromSeconds(1));
        
    }
    
    [Fact]
    public async Task UpdateUser_Success_JustFieldBio()
    {
        UserTestResult result = await _helper.CreateNewUser();

        UpdateUserDto dto = new UpdateUserDto()
        {
            Bio = "AnyBio",
            BirthDate = null,
            Country = null,
            CoverImageUrl = null,
            FullName = null,
            ImageProfileUrl = null,
            IsPrivate = null,
            Language = null,
            PasswordHash = null,
            Username = null,
       };
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", result.Tokens.Token);

        HttpResponseMessage message = await Client.PatchAsJsonAsync(_url, dto);
        message.StatusCode.Should().Be(HttpStatusCode.OK);

        ResponseHttp<UserDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<UserDto>>();
        
        http.Should().NotBeNull();
        http.DetailsError.Should().BeNull();
        http.Success.Should().BeTrue();
        http.TraceId.Should().NotBeNullOrWhiteSpace();

        http.Data.Should().NotBeNull();
        http.Data.Id.Should().Be(result.User.Id);
        http.Data.Email.Should().Be(result.User.Email);
    
        http.Data.Bio.Should().Be(dto.Bio);
        http.Data.Country.Should().BeNull();
        http.Data.CoverImageUrl.Should().BeNull();
        http.Data.ImageProfileUrl.Should().BeNull();
        http.Data.Language.Should().BeNull();
        http.Data.Username.Should().Be(result.User.Username);

        http.Data.IsPrivate.Should().BeFalse();
    
        http.Data.BirthDate.Should().BeNull();
    }
    
    [Fact]
    public async Task UpdateUser_Success_JustFieldBirthDate()
    {
        UserTestResult result = await _helper.CreateNewUser();

        UpdateUserDto dto = new UpdateUserDto()
        {
            Bio = null,
            BirthDate = DateTime.UtcNow.AddYears(-30),
            Country = null,
            CoverImageUrl = null,
            FullName = null,
            ImageProfileUrl = null,
            IsPrivate = null,
            Language = null,
            PasswordHash = null,
            Username = null,
       };
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", result.Tokens.Token);

        HttpResponseMessage message = await Client.PatchAsJsonAsync(_url, dto);
        message.StatusCode.Should().Be(HttpStatusCode.OK);

        ResponseHttp<UserDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<UserDto>>();
        
        http.Should().NotBeNull();
        http.DetailsError.Should().BeNull();
        http.Success.Should().BeTrue();
        http.TraceId.Should().NotBeNullOrWhiteSpace();

        http.Data.Should().NotBeNull();
        http.Data.Id.Should().Be(result.User.Id);
        http.Data.Email.Should().Be(result.User.Email);
    
        http.Data.Bio.Should().BeNull();
        http.Data.Country.Should().BeNull();
        http.Data.CoverImageUrl.Should().BeNull();
        http.Data.ImageProfileUrl.Should().BeNull();
        http.Data.Language.Should().BeNull();
        http.Data.Username.Should().Be(result.User.Username);

        http.Data.IsPrivate.Should().BeFalse();
    
        http.Data.BirthDate.Should().BeCloseTo(dto.BirthDate!.Value, TimeSpan.FromSeconds(1));
        
    }
    
    [Fact]
    public async Task UpdateUser_Fail_UsernameAlreadyExists()
    {
        UserTestResult user1 = await _helper.CreateNewUser();
        UserTestResult user2 = await _helper.CreateNewUser();

        UpdateUserDto dto = new UpdateUserDto()
        {
            Username = user1.User.Username 
        };
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", user2.Tokens.Token);

        HttpResponseMessage message = await Client.PatchAsJsonAsync(_url, dto);
        _output.WriteLine(await message.Content.ReadAsStringAsync());
        
        message.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        
        http.Should().NotBeNull();
        http.Success.Should().BeFalse();
        http.Message.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task UpdateUser_Fail_Validation_BioTooLong()
    {
        UserTestResult result = await _helper.CreateNewUser();

        // String gigantesca para estourar o limite de 600/800 caracteres
        UpdateUserDto dto = new UpdateUserDto()
        {
            Bio = new string('A', 901) 
        };
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", result.Tokens.Token);

        HttpResponseMessage message = await Client.PatchAsJsonAsync(_url, dto);
        
        // Deve disparar o seu InvalidModelStateResponseFactory configurado no Program.cs
        message.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var response = await message.Content.ReadFromJsonAsync<ResponseHttp<IEnumerable<string>>>();
        
        response.Should().NotBeNull();
        response.Success.Should().BeFalse();
        response.Data.Should().NotBeEmpty(); // Contém os erros de validação
    }

    [Fact]
    public async Task UpdateUser_Fail_Unauthorized()
    {
        // Tenta atualizar sem o header de Authorization
        UpdateUserDto dto = new UpdateUserDto() { Bio = "New Bio" };
        
        Client.DefaultRequestHeaders.Authorization = null; 

        HttpResponseMessage message = await Client.PatchAsJsonAsync(_url, dto);
        
        message.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    
    
}