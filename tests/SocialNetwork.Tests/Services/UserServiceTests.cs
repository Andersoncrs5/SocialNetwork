using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using SocialNetwork.Write.API.Configs.Exception.classes;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Services.Providers;
using SocialNetwork.Write.API.Utils.UnitOfWork;
using Xunit;

namespace SocialNetwork.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IPasswordHasher<UserModel>> _passwordHasherMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _passwordHasherMock = new Mock<IPasswordHasher<UserModel>>();
        _mapperMock = new Mock<IMapper>();

        _userService = new UserService(
            _uowMock.Object, 
            _passwordHasherMock.Object, 
            _mapperMock.Object
        );
    }

    private readonly UserModel _user = new UserModel()
    {
        Id = Guid.NewGuid().ToString(),
        Email = "pochita@gmail.com",
        PasswordHash = "12345678",
        Bio = "any",
        AccessFailedCount = 0,
        BirthDate = DateTime.UtcNow.AddYears(-20),
        ConcurrencyStamp = null,
        Country = "BR",
        EmailConfirmed = true,
        CoverImageUrl = "",
        CreatedAt = DateTime.UtcNow,
        FailedLoginAttempts = 0,
        FullName = "pochita the chainsaw demon",
        UserName = "pochita"
    };

    // METHOD: ExistsUserBySid
    [Fact]
    public async Task ExistsUserBySid_ShouldReturnTrue() 
    {
        _uowMock.Setup(x => x.UserRepository.ExistsByIdAsync(_user.Id)).ReturnsAsync(true);

        bool exists = await _userService.ExistsUserBySid(_user.Id);
        
        exists.Should().BeTrue();
        
        _uowMock.Verify(x => x.UserRepository.ExistsByIdAsync(_user.Id), Times.Once);
    }
    
    [Fact]
    public async Task ExistsUserBySid_ShouldReturnFalse() 
    {
        _uowMock.Setup(x => x.UserRepository.ExistsByIdAsync(_user.Id)).ReturnsAsync(false);

        bool exists = await _userService.ExistsUserBySid(_user.Id);
        
        exists.Should().BeFalse();
        
        _uowMock.Verify(x => x.UserRepository.ExistsByIdAsync(_user.Id), Times.Once);
    }
    
    // METHOD: GetUserBySidSimple
    [Fact]
    public async Task GetUserByIdSimple_ShouldReturnUser()
    {
        _uowMock.Setup(x => x.UserRepository.GetByIdAsync(_user.Id)).ReturnsAsync(_user);

        UserModel simple = await _userService.GetUserBySidSimple(this._user.Id);
        
        simple.Should().BeEquivalentTo(_user);
        
        _uowMock.Verify(x => x.UserRepository.GetByIdAsync(_user.Id), Times.Once);
    }
    
    [Fact]
    public async Task GetUserBySidSimple_ShouldThrowException_WhenUserNotFound()
    {
        // Arrange
        string sid = "id-inexistente";
        _uowMock.Setup(x => x.UserRepository.GetByIdAsync(sid))
            .ReturnsAsync((UserModel?)null);

        var act = () => _userService.GetUserBySidSimple(sid);

        // Assert
        await act.Should().ThrowAsync<ModelNotFoundException>()
            .WithMessage("User not found");
    }

    [Fact]
    public async Task GetUserByEmailSimple_ShouldThrowModelNotFoundException_WhenUserDoesNotExist()
    {
        string email = "naoexiste@email.com";
        _uowMock.Setup(x => x.UserRepository.GetByEmail(email))
            .ReturnsAsync((UserModel?)null);

        Func<Task> act = () => _userService.GetUserByEmailSimple(email);

        await act.Should().ThrowAsync<ModelNotFoundException>()
            .WithMessage("User not found");

        _uowMock.Verify(x => x.UserRepository.GetByEmail(email), Times.Once);
        _uowMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetUserByEmailSimple_ShouldReturnUser()
    {
        string email = _user.Email!;
        _uowMock.Setup(x => x.UserRepository.GetByEmail(email))
            .ReturnsAsync(_user);

        UserModel simple = await _userService.GetUserByEmailSimple(email);
        
        simple.Should().BeEquivalentTo(_user);

        _uowMock.Verify(x => x.UserRepository.GetByEmail(email), Times.Once);
        _uowMock.VerifyNoOtherCalls();
    }
    
    // METHOD: GetUserBySid
    [Fact]
    public async Task GetUserBySid_ShouldReturnUser()
    {
        _uowMock.Setup(x => x.UserRepository.GetByIdAsync(this._user.Id)).ReturnsAsync(_user);

        UserModel? model = await _userService.GetUserBySid(this._user.Id);

        model.Should().NotBeNull();
        model.Should().BeEquivalentTo(_user);
        
        _uowMock.Verify(x => x.UserRepository.GetByIdAsync(this._user.Id), Times.Once);
    }
    
    [Fact]
    public async Task GetUserBySid_ShouldReturnNotFound()
    {
        _uowMock.Setup(x => x.UserRepository.GetByIdAsync(this._user.Id)).ReturnsAsync((UserModel?)null);

        UserModel? model = await _userService.GetUserBySid(this._user.Id);

        model.Should().BeNull();
        
        _uowMock.Verify(x => x.UserRepository.GetByIdAsync(this._user.Id), Times.Once);
    }
    
    [Fact]
    public async Task DeleteUser_ShouldNotCommit_WhenIdentityResultFails()
    {
        // Arrange
        var user = new UserModel { Id = "1" };
        _uowMock.Setup(x => x.UserRepository.Delete(user))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Error" }));

        // Act
        await _userService.DeleteUser(user);

        // Assert
        _uowMock.Verify(x => x.CommitAsync(), Times.Never);
        _uowMock.Verify(x => x.UserRepository.Delete(user), Times.Once);
        
        _uowMock.VerifyNoOtherCalls();
    }
    
    // METHOD: CheckPassword
    [Fact]
    public async Task CheckPassword_ShouldReturnTrue()
    {
        _uowMock.Setup(x => x.UserRepository.CheckPassword(_user, _user.PasswordHash!)).ReturnsAsync(true);

        bool checkPassword = await _userService.CheckPassword(_user, _user.PasswordHash!);
        
        checkPassword.Should().BeTrue();
        
        _uowMock.Verify(x => x.UserRepository.CheckPassword(_user, _user.PasswordHash!), Times.Once);
        _uowMock.VerifyNoOtherCalls();
    }
    
    [Fact]
    public async Task CheckPassword_ShouldReturnFalse()
    {
        _uowMock.Setup(x => x.UserRepository.CheckPassword(_user, _user.PasswordHash!)).ReturnsAsync(false);

        bool checkPassword = await _userService.CheckPassword(_user, _user.PasswordHash!);
        
        checkPassword.Should().BeFalse();
        
        _uowMock.Verify(x => x.UserRepository.CheckPassword(_user, _user.PasswordHash!), Times.Once);
        _uowMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetUserByEmail_ShouldReturnUser()
    {
        string email = _user.Email!;
        _uowMock.Setup(x => x.UserRepository.GetByEmail(email)).ReturnsAsync(_user);

        UserModel? model = await _userService.GetUserByEmail(email);

        model.Should().NotBeNull();
        model.Should().BeEquivalentTo(_user);
        
        _uowMock.Verify(x => x.UserRepository.GetByEmail(email), Times.Once);
        _uowMock.VerifyNoOtherCalls();
    }
    
    [Fact]
    public async Task GetUserByEmail_ShouldReturnNotFound()
    {
        string email = _user.Email!;
        _uowMock.Setup(x => x.UserRepository.GetByEmail(email)).ReturnsAsync((UserModel?)null);

        UserModel? model = await _userService.GetUserByEmail(email);

        model.Should().BeNull();
        
        _uowMock.Verify(x => x.UserRepository.GetByEmail(email), Times.Once);
        _uowMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExistsUserByEmail_ShouldReturnTrue()
    {
        string email = _user.Email!;
        _uowMock.Setup(x => x.UserRepository.ExistsByEmail(email)).ReturnsAsync(true);
        
        bool exists = await _userService.ExistsUserByEmail(email);
        exists.Should().BeTrue();
        
        _uowMock.Verify(x => x.UserRepository.ExistsByEmail(email), Times.Once);
        _uowMock.VerifyNoOtherCalls();
    }
    
    [Fact]
    public async Task ExistsUserByEmail_ShouldReturnFalse()
    {
        string email = _user.Email!;
        _uowMock.Setup(x => x.UserRepository.ExistsByEmail(email)).ReturnsAsync(false);
        
        bool exists = await _userService.ExistsUserByEmail(email);
        exists.Should().BeFalse();
        
        _uowMock.Verify(x => x.UserRepository.ExistsByEmail(email), Times.Once);
        _uowMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetUserByUsername_ShouldReturnUser() 
    {
        string username = _user.UserName!;
        _uowMock.Setup(x => x.UserRepository.GetByUsername(username)).ReturnsAsync(_user);

        UserModel? model = await _userService.GetUserByUsername(username);
        
        model.Should().NotBeNull();
        model.Should().BeEquivalentTo(_user);
        
        _uowMock.Verify(x => x.UserRepository.GetByUsername(username), Times.Once);
        _uowMock.VerifyNoOtherCalls();
    }
    
    [Fact]
    public async Task GetUserByUsername_ShouldReturnNotFound() 
    {
        string username = _user.UserName!;
        _uowMock.Setup(x => x.UserRepository.GetByUsername(username)).ReturnsAsync((UserModel?)null);

        UserModel? model = await _userService.GetUserByUsername(username);
        
        model.Should().BeNull();
        
        _uowMock.Verify(x => x.UserRepository.GetByUsername(username), Times.Once);
        _uowMock.VerifyNoOtherCalls();
    }
    
    // --- METHOD: GetUserByUsernameSimple ---
    [Fact]
    public async Task GetUserByUsernameSimple_ShouldReturnUser_WhenExists()
    {
        // Arrange
        string username = _user.UserName!;
        _uowMock.Setup(x => x.UserRepository.GetByUsername(username))
            .ReturnsAsync(_user);

        // Act
        UserModel result = await _userService.GetUserByUsernameSimple(username);

        // Assert
        result.Should().NotBeNull();
        result.UserName.Should().Be(username);
        _uowMock.Verify(x => x.UserRepository.GetByUsername(username), Times.Once);
    }

    [Fact]
    public async Task GetUserByUsernameSimple_ShouldThrowModelNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        string username = "ghost_user";
        _uowMock.Setup(x => x.UserRepository.GetByUsername(username))
            .ReturnsAsync((UserModel?)null);

        // Act
        Func<Task> act = () => _userService.GetUserByUsernameSimple(username);

        // Assert
        await act.Should().ThrowAsync<ModelNotFoundException>()
            .WithMessage("User not found");
        
        _uowMock.Verify(x => x.UserRepository.GetByUsername(username), Times.Once);
        _uowMock.VerifyNoOtherCalls();
    }

    // --- METHOD: ExistsUserByUsername ---
    [Fact]
    public async Task ExistsUserByUsername_ShouldReturnTrue_WhenUserExists()
    {
        // Arrange
        string username = _user.UserName!;
        _uowMock.Setup(x => x.UserRepository.ExistsByUsername(username))
            .ReturnsAsync(true);

        // Act
        bool exists = await _userService.ExistsUserByUsername(username);

        // Assert
        exists.Should().BeTrue();
        _uowMock.Verify(x => x.UserRepository.ExistsByUsername(username), Times.Once);
    }

    [Fact]
    public async Task ExistsUserByUsername_ShouldReturnFalse_WhenUserDoesNotExist()
    {
        // Arrange
        string username = "non_existent_user";
        _uowMock.Setup(x => x.UserRepository.ExistsByUsername(username))
            .ReturnsAsync(false);

        // Act
        bool exists = await _userService.ExistsUserByUsername(username);

        // Assert
        exists.Should().BeFalse();
        _uowMock.Verify(x => x.UserRepository.ExistsByUsername(username), Times.Once);
    }

    // --- METHOD: GetUserByRefreshToken ---
    [Fact]
    public async Task GetUserByRefreshToken_ShouldReturnUser_WhenTokenIsValid()
    {
        // Arrange
        string validToken = "valid-refresh-token-123";
        _user.RefreshToken = validToken;
        _uowMock.Setup(x => x.UserRepository.GetByRefreshToken(validToken))
            .ReturnsAsync(_user);

        // Act
        UserModel? result = await _userService.GetUserByRefreshToken(validToken);

        // Assert
        result.Should().NotBeNull();
        result!.RefreshToken.Should().Be(validToken);
        _uowMock.Verify(x => x.UserRepository.GetByRefreshToken(validToken), Times.Once);
    }

    [Fact]
    public async Task GetUserByRefreshToken_ShouldReturnNull_WhenTokenIsInvalid()
    {
        // Arrange
        string invalidToken = "expired-or-fake-token";
        _uowMock.Setup(x => x.UserRepository.GetByRefreshToken(invalidToken))
            .ReturnsAsync((UserModel?)null);

        // Act
        UserModel? result = await _userService.GetUserByRefreshToken(invalidToken);

        // Assert
        result.Should().BeNull();
        _uowMock.Verify(x => x.UserRepository.GetByRefreshToken(invalidToken), Times.Once);
        _uowMock.VerifyNoOtherCalls();
    }
}