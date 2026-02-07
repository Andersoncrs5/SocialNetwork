using AutoMapper;
using FluentAssertions;
using Moq;
using SocialNetwork.Write.API.Configs.Exception.classes;
using SocialNetwork.Write.API.dto.Posts;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Models.Enums.Post;
using SocialNetwork.Write.API.Services.Providers;
using SocialNetwork.Write.API.Utils.UnitOfWork;

namespace SocialNetwork.Tests.Services.Posts;

public class PostServiceTest
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly PostService _postService;

    public PostServiceTest()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _postService = new PostService(_uowMock.Object,  _mapperMock.Object);
    }

    private readonly UserModel _user = new UserModel()
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
    
    private readonly PostModel _postMock = new ()
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

    private readonly CreatePostDto _dto = new()
    {
        Title = "TitleSimple",
        Slug = "slug-simple",
        Content = "ContentSimple",
        Summary = "SummarySimple",
        FeaturedImageUrl  = "FeaturedImageUrlSimple",
        Visibility = PostVisibilityEnum.Public,
        ReadingTime = 6,
        IsCommentsEnabled = true,
        ReadingLevel = ReadingLevelEnum.Short,
        PostType = PostTypeEnum.Article,
    };
    
    [Fact]
    public async Task ShouldReturnPost_Success()
    {
        _uowMock.Setup(x => x.PostRepository.GetByIdAsync(_postMock.Id)).ReturnsAsync(_postMock);

        PostModel? model = await _postService.GetByIdAsync(_postMock.Id);

        model.Should().NotBeNull();
        
        _uowMock.Verify(x => x.PostRepository.GetByIdAsync(_postMock.Id), Times.Once);
    }
    
    [Fact]
    public async Task ShouldReturnPost_Fail_NotFound()
    {
        _uowMock.Setup(x => x.PostRepository.GetByIdAsync(_postMock.Id)).ReturnsAsync((PostModel?)null);

        PostModel? model = await _postService.GetByIdAsync(_postMock.Id);

        model.Should().BeNull();
        
        _uowMock.Verify(x => x.PostRepository.GetByIdAsync(_postMock.Id), Times.Once);
    }

    [Fact]
    public async Task ShouldReturnTrue_Success_WhenExecExistsByIdAsync()
    {
        _uowMock.Setup(x => x.PostRepository.ExistsById(_postMock.Id)).ReturnsAsync(true);

        bool exists = await _postService.ExistsByIdAsync(_postMock.Id);
        exists.Should().BeTrue();
        _uowMock.Verify(x => x.PostRepository.ExistsById(_postMock.Id), Times.Once);
    }
    
    [Fact]
    public async Task ShouldReturnFalse_Success_WhenExecExistsByIdAsync()
    {
        _uowMock.Setup(x => x.PostRepository.ExistsById(_postMock.Id)).ReturnsAsync(true);

        bool exists = await _postService.ExistsByIdAsync(_postMock.Id);
        exists.Should().BeTrue();
        _uowMock.Verify(x => x.PostRepository.ExistsById(_postMock.Id), Times.Once);
    }
    
    [Fact]
    public async Task ShouldReturnTrue_Success_WhenExecExistsBySlugAsync()
    {
        _uowMock.Setup(x => x.PostRepository.ExistsBySlug(_postMock.Slug)).ReturnsAsync(true);

        bool exists = await _postService.ExistsBySlugAsync(_postMock.Slug);
        exists.Should().BeTrue();
        _uowMock.Verify(x => x.PostRepository.ExistsBySlug(_postMock.Slug), Times.Once);
    }
    
    [Fact]
    public async Task ShouldReturnFalse_Success_WhenExecExistsBySlugAsync()
    {
        _uowMock.Setup(x => x.PostRepository.ExistsBySlug(_postMock.Slug)).ReturnsAsync(true);

        bool exists = await _postService.ExistsBySlugAsync(_postMock.Slug);
        exists.Should().BeTrue();
        _uowMock.Verify(x => x.PostRepository.ExistsBySlug(_postMock.Slug), Times.Once);
    }

    [Fact]
    public async Task ShouldCreatePost_Success()
    {
        _mapperMock.Setup(x => x.Map<PostModel>(It.IsAny<CreatePostDto>())).Returns(_postMock);
        _uowMock.Setup(x => x.PostRepository.AddAsync(It.IsAny<PostModel>())).ReturnsAsync(_postMock);

        PostModel model = await _postService.CreateAsync(_dto, _user);
        
        model.Should().NotBeNull();
        model.Id.Should().Be(_postMock.Id);
        model.UserId.Should().Be(_user.Id);
        
        _uowMock.Verify(x => x.PostRepository.AddAsync(It.IsAny<PostModel>()), Times.Once);
        _uowMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task ShouldDeletePost_Success()
    {
        _uowMock.Setup(x => x.PostRepository.DeleteAsync(_postMock)).Returns(Task.CompletedTask);
        
        await _postService.DeleteAsync(_postMock);
        
        _uowMock.Verify(x => x.PostRepository.DeleteAsync(_postMock), Times.Once);
        _uowMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task ShouldThrowConflictValueException_Success()
    {
        UpdatePostDto dto = new UpdatePostDto();
        dto.Slug = _postMock.Slug + "-update";
        
        _uowMock.Setup(x => x.PostRepository.ExistsBySlug(dto.Slug)).ReturnsAsync(true);
        Func<Task> func = async () => await _postService.UpdateAsync(_postMock, dto);
        
        await func.Should().ThrowAsync<ConflictValueException>() 
            .WithMessage($"Slug {dto.Slug} already exists");
    }
    
}