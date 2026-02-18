using AutoMapper;
using FluentAssertions;
using Moq;
using SocialNetwork.Contracts.Enums.Post;
using SocialNetwork.Write.API.Configs.Exception.classes;
using SocialNetwork.Write.API.dto.PostCategory;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Models.Enums.Post;
using SocialNetwork.Write.API.Modules.Post.Model;
using SocialNetwork.Write.API.Services.Providers;
using SocialNetwork.Write.API.Utils.UnitOfWork;

namespace SocialNetwork.Tests.Services.PostCategory;

public class PostCategoryServiceTest
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly PostCategoryService _service;

    public PostCategoryServiceTest()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _service = new PostCategoryService(_uowMock.Object);
    }
    
    static UserModel _user = new ()
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
    
    static PostModel _postMock = new ()
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
        UserId = _user.Id,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
    };
    
    static CategoryModel _category = new CategoryModel()
    {
        Id = Guid.NewGuid().ToString(),
        Name = "TI",
        Slug = "ti",
        Parent = null,
        Color = "#000000",
        CreatedAt = DateTime.UtcNow,
        Description = "anyDesc",
        DisplayOrder = 1,
        IconName = "a",
        IsActive = true,
        IsVisible = true,
        ParentId = null,
        UpdatedAt = DateTime.UtcNow,
    };

    private static PostCategoryModel _postCategory = new()
    {
        Id = Guid.NewGuid().ToString(),
        CategoryId = _category.Id,
        PostId = _postMock.Id,
        Order = 1,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
    };

    private static CreatePostCategoryDto _dto = new ()
    {
        CategoryId = _postCategory.CategoryId,
        PostId = _postCategory.PostId,
        Order = _postCategory.Order,
    };
    
    [Fact]
    public async Task ShouldReturnPostCategory_Success_WhenGetById()
    {
        _uowMock.Setup(x => x.PostCategoryRepository.GetByIdAsync(_postCategory.Id))
            .ReturnsAsync(_postCategory);

        PostCategoryModel model = await _service.GetByIdSimple(_postCategory.Id);
        model.Id.Should().Be(_postCategory.Id);
        
        _uowMock.Verify(x => x.PostCategoryRepository.GetByIdAsync(_postCategory.Id), Times.Once);
    }
    
    [Fact]
    public async Task ShouldThrowModelNotFoundException_Success_WhenGetById()
    {
        _uowMock.Setup(x => x.PostCategoryRepository.GetByIdAsync(_postMock.Id))
            .ReturnsAsync((PostCategoryModel?) null);

        Func<Task> func = async () => await _service.GetByIdSimple(_postMock.Id);

        await func.Should().ThrowAsync<ModelNotFoundException>()
            .WithMessage("Category not found");
        
        _uowMock.Verify(x => x.PostCategoryRepository.GetByIdAsync(_postMock.Id), Times.Once);
    }
    
    [Fact]
    public async Task ShouldReturnPostCategory_Success_WhenGetByPostIdAndCategoryId()
    {
        _uowMock.Setup(x => x.PostCategoryRepository.GetByPostIdAndCategoryId(_postCategory.PostId, _category.Id))
            .ReturnsAsync(_postCategory);

        PostCategoryModel model = await _service.GetByPostIdAndCategoryId(_postCategory.PostId, _category.Id);
        model.Id.Should().Be(_postCategory.Id);
        
        _uowMock.Verify(x => x.PostCategoryRepository.GetByPostIdAndCategoryId(_postCategory.PostId, _category.Id), Times.Once);
    }
    
    [Fact]
    public async Task ShouldThrowModelNotFoundException_Success_WhenGetByPostIdAndCategoryId()
    {
        _uowMock.Setup(x => x.PostCategoryRepository.GetByPostIdAndCategoryId(_postCategory.PostId, _category.Id))
            .ReturnsAsync((PostCategoryModel?) null);

        Func<Task> func = async () => await _service.GetByPostIdAndCategoryId(_postCategory.PostId, _category.Id);

        await func.Should().ThrowAsync<ModelNotFoundException>()
            .WithMessage("Category not found");
        
        _uowMock.Verify(x => x.PostCategoryRepository.GetByPostIdAndCategoryId(_postCategory.PostId, _category.Id), Times.Once);
    }

    [Fact]
    public async Task ShouldDeletePostCategory_Success()
    {
        _uowMock.Setup(x => x.PostCategoryRepository.DeleteAsync(_postCategory))
            .Returns(Task.CompletedTask);
        _uowMock.Setup(x => x.CommitAsync())
            .Returns(Task.CompletedTask);

        await _service.Delete(_postCategory);
        
        _uowMock.Verify(x => x.PostCategoryRepository.DeleteAsync(_postCategory), Times.Once);
        _uowMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task ShouldCreatePostCategory_Success()
    {
        _uowMock.Setup(x => x.Mapper.Map<PostCategoryModel>(_dto))
            .Returns(_postCategory);
        _uowMock.Setup(x => x.PostCategoryRepository.AddAsync(It.IsAny<PostCategoryModel>()))
            .ReturnsAsync(_postCategory);
        _uowMock.Setup(x => x.CommitAsync())
            .Returns(Task.CompletedTask);

        PostCategoryModel model = await _service.Create(_dto);
        model.Id.Should().Be(_postCategory.Id);
        
        _uowMock.Verify(x => x.PostCategoryRepository.AddAsync(It.IsAny<PostCategoryModel>()), Times.Once);
        _uowMock.Verify(x => x.Mapper.Map<PostCategoryModel>(_dto), Times.Once);
        _uowMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task ShouldReturn10WhenExecCountByPostIdAndCategoryId()
    {
        _uowMock.Setup(x => x.PostCategoryRepository.CountByPostIdAndCategoryId(_postCategory.PostId, _category.Id))
            .ReturnsAsync(10);

        int result = await _service.CountByPostIdAndCategoryId(_postCategory.PostId, _category.Id);
        result.Should().Be(10);
        
        _uowMock.Verify(x => x.PostCategoryRepository.CountByPostIdAndCategoryId(_postCategory.PostId, _category.Id), Times.Once);
    }
    
}