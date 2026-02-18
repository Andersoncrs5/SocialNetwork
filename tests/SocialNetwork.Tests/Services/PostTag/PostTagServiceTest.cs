using AutoMapper;
using FluentAssertions;
using Moq;
using SocialNetwork.Contracts.Enums.Post;
using SocialNetwork.Write.API.Configs.Exception.classes;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Models.Enums.Post;
using SocialNetwork.Write.API.Modules.Post.Model;
using SocialNetwork.Write.API.Modules.PostTag.Dto;
using SocialNetwork.Write.API.Modules.PostTag.Model;
using SocialNetwork.Write.API.Modules.PostTag.Service.Provider;
using SocialNetwork.Write.API.Modules.Tag.Model;
using SocialNetwork.Write.API.Services.Providers;
using SocialNetwork.Write.API.Utils.UnitOfWork;

namespace SocialNetwork.Tests.Services.PostTag;

public class PostTagServiceTest
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly PostTagService _postTagService;
    
    public PostTagServiceTest()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _postTagService = new PostTagService(_uowMock.Object);
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
    
    static TagModel _tag = new ()
    {
        Id = Guid.NewGuid().ToString(),
        Name = "test",
        Slug = "test",
        Description = "testDesc",
        Color = "#000000",
        IsActive = true,
        IsVisible = true,
        IsSystem = true,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
    };

    static PostTagModel _postTag = new()
    {
        Id = Guid.NewGuid().ToString(),
        PostId = _postMock.Id,
        TagId = _tag.Id,
        Post = _postMock,
        Tag = _tag,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
    };
    
    [Fact]
    public async Task ShouldReturnPostTag_Success()
    {
        _uowMock.Setup(x => x.PostTagRepository.GetByIdAsync(_postMock.Id)).ReturnsAsync(_postTag);

        PostTagModel model = await _postTagService.GetByIdAsync(_postMock.Id);

        model.Id.Should().Be(_postTag.Id);

        _uowMock.Verify(x => x.PostTagRepository.GetByIdAsync(_postMock.Id), Times.Once);
    }

    [Fact]
    public async Task ShouldThrowModelNotFound_Success()
    {
        _uowMock.Setup(x => x.PostTagRepository.GetByIdAsync(_postMock.Id))
            .ReturnsAsync((PostTagModel?) null);
        
        Func<Task> act = async () => await _postTagService.GetByIdAsync(_postMock.Id);

        await act.Should().ThrowAsync<ModelNotFoundException>()
            .WithMessage("Tag not found");
        
        _uowMock.Verify(x => x.PostTagRepository.GetByIdAsync(_postMock.Id), Times.Once);
    }

    [Fact]
    public async Task ShouldReturn10_Success()
    {
        _uowMock.Setup(x => x.PostTagRepository.CountByPostIdAndTagId(_postMock.Id, _tag.Id))
            .ReturnsAsync(10);
        
        int count = await _postTagService.CountByPostIdAndTagId(_postMock.Id, _tag.Id);
        count.Should().Be(10);

        _uowMock.Verify(x => x.PostTagRepository.CountByPostIdAndTagId(_postMock.Id, _tag.Id), Times.Once);
    }

    [Fact]
    public async Task ShouldReturnPostTag_Success_WhenGetByPostIdAndTagId()
    {
        _uowMock.Setup(x => x.PostTagRepository.GetByPostIdAndTagId(_postTag.PostId, _tag.Id))
            .ReturnsAsync(_postTag);

        PostTagModel? model = await _postTagService.GetByPostIdAndTagId(_postTag.PostId, _tag.Id);
        
        model.Should().NotBeNull();
        model.Id.Should().Be(_postTag.Id);
        
        _uowMock.Verify(x => x.PostTagRepository.GetByPostIdAndTagId(_postTag.PostId, _tag.Id), Times.Once);
    }
    
    [Fact]
    public async Task ShouldReturnNull_Success_WhenGetByPostIdAndTagId()
    {
        _uowMock.Setup(x => x.PostTagRepository.GetByPostIdAndTagId(_postTag.PostId, _tag.Id))
            .ReturnsAsync((PostTagModel?) null);

        PostTagModel? model = await _postTagService.GetByPostIdAndTagId(_postTag.PostId, _tag.Id);
        
        model.Should().BeNull();
        
        _uowMock.Verify(x => x.PostTagRepository.GetByPostIdAndTagId(_postTag.PostId, _tag.Id), Times.Once);
    }

    [Fact]
    public async Task ShouldDelete_Success()
    {
        _uowMock.Setup(x => x.PostTagRepository.DeleteAsync(_postTag))
            .Returns(Task.CompletedTask);
        _uowMock.Setup(x => x.CommitAsync());
        
        await _postTagService.Delete(_postTag);
        
        _uowMock.Verify(x => x.PostTagRepository.DeleteAsync(_postTag), Times.Once);
        _uowMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task CreatePostTag_Success()
    {
        _uowMock.Setup(x => x.Mapper.Map<PostTagModel>(It.IsAny<CreatePostTagDto>()))
            .Returns(_postTag);
        _uowMock.Setup(x => x.PostTagRepository.AddAsync(It.IsAny<PostTagModel>()))
            .ReturnsAsync(_postTag);
        _uowMock.Setup(x => x.CommitAsync());

        CreatePostTagDto dto = new ()
        {
            PostId = _postMock.Id,
            TagId = _tag.Id,
        };

        PostTagModel tagModel = await _postTagService.Create(dto);
        tagModel.Id.Should().Be(_postTag.Id);
        
        _uowMock.Verify(x => x.PostTagRepository.AddAsync(It.IsAny<PostTagModel>()), Times.Once);
        _uowMock.Verify(x => x.Mapper.Map<PostTagModel>(It.IsAny<CreatePostTagDto>()), Times.Once);
        _uowMock.Verify(x => x.CommitAsync(), Times.Once);
    }
    
}