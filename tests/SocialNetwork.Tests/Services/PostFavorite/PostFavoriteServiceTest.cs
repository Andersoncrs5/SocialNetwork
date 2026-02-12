using FluentAssertions;
using Moq;
using SocialNetwork.Contracts.Enums.Post;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Models.Enums.Post;
using SocialNetwork.Write.API.Services.Providers;
using SocialNetwork.Write.API.Utils.Classes;
using SocialNetwork.Write.API.Utils.UnitOfWork;

namespace SocialNetwork.Tests.Services.PostFavorite;

public class PostFavoriteServiceTest
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly PostFavoriteService _service;
    
    public PostFavoriteServiceTest()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _service = new PostFavoriteService(_uowMock.Object);
    }

    private static UserModel _user = new UserModel()
    {
        Id = "69b078e9-13ac-481e-bf28-f3767be0493f",
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
        FullName = "pochita the chainsaw demon",
        UserName = "pochita"
    };
    
    private static PostModel _postMock = new ()
    {
        Id = Guid.NewGuid().ToString(),
        Title = "TitleSimple",
        Slug = "slug-simple",
        Content = "ContentSimple",
        Summary = "SummarySimple",
        FeaturedImageUrl  = "FeaturedImageUrlSimple",
        Visibility = PostVisibilityEnum.Public,
        ReadingTime = 6,
        RankingScore = 3.1415,
        IsCommentsEnabled = true,
        HighlightStatus = PostHighlightStatusEnum.None,
        ModerationStatus = ModerationStatusEnum.PendingReview,
        ReadingLevel = ReadingLevelEnum.Short,
        PostType = PostTypeEnum.Article,
        UserId = "69b078e9-13ac-481e-bf28-f3767be0493f",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
    };

    private static PostFavoriteModel favorite = new()
    {
        Id = Guid.NewGuid().ToString(),
        PostId = _postMock.Id,
        UserId = _user.Id,
        User = _user,
        Post = _postMock,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
    };
    
    [Fact]
    public async Task ShouldReturnPostFavorite_Success_WhenGetByUserIdAndPostId()
    {
        _uowMock.Setup(x => x.PostFavoriteRepository.GetByPostIdAndUserId(favorite.PostId, favorite.UserId))
            .ReturnsAsync(favorite);
        
        PostFavoriteModel? result = await _service.GetByPostIdAndUserId(favorite.PostId, favorite.UserId);

        result.Should().NotBeNull();
        result.Id.Should().Be(favorite.Id);
        
        _uowMock.Verify(x => x.PostFavoriteRepository.GetByPostIdAndUserId(_postMock.Id, favorite.UserId), Times.Once);
    }
    
    [Fact]
    public async Task ShouldReturnNull_Success_WhenGetByUserIdAndPostId()
    {
        _uowMock.Setup(x => x.PostFavoriteRepository.GetByPostIdAndUserId(favorite.PostId, favorite.UserId))
            .ReturnsAsync((PostFavoriteModel?) null);
        
        PostFavoriteModel? result = await _service.GetByPostIdAndUserId(favorite.PostId, favorite.UserId);

        result.Should().BeNull();
        
        _uowMock.Verify(x => x.PostFavoriteRepository.GetByPostIdAndUserId(_postMock.Id, favorite.UserId), Times.Once);
    }

    [Fact]
    public async Task ShouldDeletePostFavorite_Success()
    {
        _uowMock.Setup(x => x.PostFavoriteRepository.DeleteAsync(favorite))
            .Returns(Task.CompletedTask);
        _uowMock.Setup(x => x.CommitAsync())
            .Returns(Task.CompletedTask);

        await _service.DeleteAsync(favorite);
        
        _uowMock.Verify(x => x.PostFavoriteRepository.DeleteAsync(favorite), Times.Once);
        _uowMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task ShouldCreatePostFavorite_Success()
    {
        _uowMock.Setup(x => x.PostFavoriteRepository.AddAsync(It.IsAny<PostFavoriteModel>()))
            .ReturnsAsync(favorite);
        _uowMock.Setup(x => x.CommitAsync())
            .Returns(Task.CompletedTask);

        PostFavoriteModel model = await _service.CreateAsync(_postMock, _user);
        
        model.Should().NotBeNull();
        model.Id.Should().Be(favorite.Id);
        
        _uowMock.Verify(x => x.PostFavoriteRepository.AddAsync(It.IsAny<PostFavoriteModel>()), Times.Once);
        _uowMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task ShouldAddedPostFavorite_Success_Toggle()
    {
        _uowMock.Setup(x => x.PostFavoriteRepository.GetByPostIdAndUserId(_postMock.Id, _user.Id))
            .ReturnsAsync((PostFavoriteModel?)null);
        _uowMock.Setup(x => x.PostFavoriteRepository.AddAsync(It.IsAny<PostFavoriteModel>()))
            .ReturnsAsync(favorite);
        _uowMock.Setup(x => x.ExecuteTransactionAsync(It.IsAny<Func<Task>>()))
            .Returns<Func<Task>>(async (operation) => await operation());

        ResultToggle<PostFavoriteModel> toggle = await _service.ToggleAsync(_postMock, _user);
        toggle.Should().NotBeNull();

        toggle.Action.Should().Be(AddedORRemoved.Added);
        toggle.Value.Should().NotBeNull();
        toggle.Value.Id.Should().Be(favorite.Id);
        
        _uowMock.Verify(x => x.PostFavoriteRepository.AddAsync(It.IsAny<PostFavoriteModel>()), Times.Once);
        _uowMock.Verify(x => x.PostFavoriteRepository.GetByPostIdAndUserId(_postMock.Id, _user.Id), Times.Once);
    }
    
    [Fact]
    public async Task ShouldRemovedPostFavorite_Success_Toggle()
    {
        _uowMock.Setup(x => x.PostFavoriteRepository.GetByPostIdAndUserId(_postMock.Id, _user.Id))
            .ReturnsAsync(favorite);
        _uowMock.Setup(x => x.PostFavoriteRepository.DeleteAsync(favorite))
            .Returns(Task.CompletedTask);
        _uowMock.Setup(x => x.ExecuteTransactionAsync(It.IsAny<Func<Task>>()))
            .Returns<Func<Task>>(async (operation) => await operation());

        ResultToggle<PostFavoriteModel> toggle = await _service.ToggleAsync(_postMock, _user);
        toggle.Should().NotBeNull();

        toggle.Action.Should().Be(AddedORRemoved.Removed);
        toggle.Value.Should().BeNull();
        
        _uowMock.Verify(x => x.PostFavoriteRepository.DeleteAsync(favorite), Times.Once);
        _uowMock.Verify(x => x.PostFavoriteRepository.GetByPostIdAndUserId(_postMock.Id, _user.Id), Times.Once);
    }
    
}