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
    }
}