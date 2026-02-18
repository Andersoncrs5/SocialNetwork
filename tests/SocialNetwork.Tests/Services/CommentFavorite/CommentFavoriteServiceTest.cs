using FluentAssertions;
using Moq;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Models.Enums.Post;
using SocialNetwork.Write.API.Modules.CommentFavorite.Model;
using SocialNetwork.Write.API.Modules.CommentFavorite.Service.Provider;
using SocialNetwork.Write.API.Services.Providers;
using SocialNetwork.Write.API.Utils.Classes;
using SocialNetwork.Write.API.Utils.UnitOfWork;

namespace SocialNetwork.Tests.Services.CommentFavorite;

public class CommentFavoriteServiceTest
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly CommentFavoriteService _service;
    
    public CommentFavoriteServiceTest()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _service = new CommentFavoriteService(_uowMock.Object);
    }

    private static CommentFavoriteModel _favorite = new CommentFavoriteModel()
    {
        CommentId = Guid.NewGuid().ToString(),
        UserId = Guid.NewGuid().ToString(),
        Id = Guid.NewGuid().ToString(),
        CreatedAt = DateTime.UtcNow
    };
    
    [Fact]
    public async Task GetByCommentIdAndUserId_returns_null()
    {
        _uowMock.Setup(x => x.CommentFavoriteRepository.GetByCommentIdAndUserId(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((CommentFavoriteModel?) null);

        CommentFavoriteModel? model = await _service.GetByCommentIdAndUserId(_favorite.CommentId, _favorite.UserId);
        model.Should().BeNull();
        
        _uowMock.Verify(x => x.CommentFavoriteRepository.GetByCommentIdAndUserId(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }
    
    [Fact]
    public async Task GetByCommentIdAndUserId_returns_CommentFavoriteModel()
    {
        _uowMock.Setup(x => x.CommentFavoriteRepository.GetByCommentIdAndUserId(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(_favorite);

        CommentFavoriteModel? model = await _service.GetByCommentIdAndUserId(_favorite.CommentId, _favorite.UserId);
        model.Should().NotBeNull();
        model.Should().BeEquivalentTo(_favorite);
        
        _uowMock.Verify(x => x.CommentFavoriteRepository.GetByCommentIdAndUserId(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task ShouldDeleteCommentFavorite_Success()
    {
        _uowMock.Setup(x => x.CommentFavoriteRepository.DeleteAsync(_favorite)).Returns(Task.CompletedTask);
        _uowMock.Setup(x => x.CommitAsync());
        await _service.DeleteAsync(_favorite);
        
        _uowMock.Verify(x => x.CommentFavoriteRepository.DeleteAsync(_favorite), Times.Once);
        _uowMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task ShouldCreateFavorite_Success()
    {
        _uowMock.Setup(x => x.CommentFavoriteRepository.AddAsync(It.IsAny<CommentFavoriteModel>()))
            .ReturnsAsync(_favorite);
        _uowMock.Setup(x => x.CommitAsync());

        CommentFavoriteModel comment = await _service.CreateAsync(_favorite.CommentId, _favorite.UserId);
        
        comment.Should().NotBeNull();
        comment.Id.Should().Be(_favorite.Id);
        
        _uowMock.Verify(x => x.CommentFavoriteRepository.AddAsync(It.IsAny<CommentFavoriteModel>()), Times.Once);
    }

    [Fact]
    public async Task ShouldAddFavorite_Success_Toggle()
    {
        _uowMock.Setup(x => x.CommentFavoriteRepository.GetByCommentIdAndUserId(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((CommentFavoriteModel?)null);
        _uowMock.Setup(x => x.CommentFavoriteRepository.AddAsync(It.IsAny<CommentFavoriteModel>()))
            .ReturnsAsync(_favorite);
        _uowMock.Setup(x => x.CommitAsync());

        ResultToggle<CommentFavoriteModel?> toggle = await _service.ToggleAsync(_favorite.CommentId, _favorite.UserId);
        
        toggle.Should().NotBeNull();
        toggle.Value.Should().NotBeNull();
        toggle.Value.Should().BeEquivalentTo(_favorite);
        
        toggle.Action.Should().Be(AddedORRemoved.Added);

        _uowMock.Verify(x => x.CommentFavoriteRepository.AddAsync(It.IsAny<CommentFavoriteModel>()), Times.Once);
        _uowMock.Verify(x => x.CommentFavoriteRepository.GetByCommentIdAndUserId(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _uowMock.Verify(x => x.CommitAsync(), Times.Once);
        
    } 
    
    [Fact]
    public async Task ShouldRemoveFavorite_Success_Toggle()
    {
        _uowMock.Setup(x => x.CommentFavoriteRepository.GetByCommentIdAndUserId(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(_favorite);
        _uowMock.Setup(x => x.CommentFavoriteRepository.DeleteAsync(It.IsAny<CommentFavoriteModel>()))
            .Returns(Task.CompletedTask);
        _uowMock.Setup(x => x.CommitAsync());

        ResultToggle<CommentFavoriteModel?> toggle = await _service.ToggleAsync(_favorite.CommentId, _favorite.UserId);
        
        toggle.Should().NotBeNull();
        toggle.Value.Should().BeNull();
        
        toggle.Action.Should().Be(AddedORRemoved.Removed);

        _uowMock.Verify(x => x.CommentFavoriteRepository.DeleteAsync(It.IsAny<CommentFavoriteModel>()), Times.Once);
        _uowMock.Verify(x => x.CommentFavoriteRepository.GetByCommentIdAndUserId(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _uowMock.Verify(x => x.CommitAsync(), Times.Once);
        
    } 
    
}