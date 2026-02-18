using FluentAssertions;
using Moq;
using SocialNetwork.Contracts.Enums.Post;
using SocialNetwork.Write.API.Configs.Exception.classes;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Models.Enums.Post;
using SocialNetwork.Write.API.Modules.Comment.Dto;
using SocialNetwork.Write.API.Modules.Comment.Model;
using SocialNetwork.Write.API.Modules.Comment.Service.Provider;
using SocialNetwork.Write.API.Modules.Post.Model;
using SocialNetwork.Write.API.Services.Providers;
using SocialNetwork.Write.API.Utils.UnitOfWork;

namespace SocialNetwork.Tests.Services.Comment;

public class CommentServiceTest
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly CommentService _service;
    
    public CommentServiceTest()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _service = new CommentService(_uowMock.Object);
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

    static CommentModel _comment = new()
    {
        Id = Guid.NewGuid().ToString(),
        Content = "ContentSimple",
        PostId = _postMock.Id,
        UserId = _user.Id,
        SentimentScore = 0.0,
        IsEdited = false,
        ModerationStatus = ModerationStatusEnum.PendingReview
    };

    [Fact]
    public async Task ShouldReturnComment_Success_GetById()
    {
        _uowMock.Setup(uow => uow.CommentRepository.GetByIdAsync(_comment.Id))
            .ReturnsAsync(_comment);

        CommentModel comment = await _service.GetByIdSimpleAsync(_comment.Id);
        Assert.Equal(_comment.Id, comment.Id);
        
        _uowMock.Verify(uow => uow.CommentRepository.GetByIdAsync(_comment.Id), Times.Once);
    }

    [Fact]
    public async Task ShouldThrowModelNotFound_Success_GetById()
    {
        _uowMock.Setup(uow => uow.CommentRepository.GetByIdAsync(_comment.Id))
            .ReturnsAsync((CommentModel?) null);
        
        Func<Task> act = async () => await _service.GetByIdSimpleAsync(_comment.Id);
        
        await act.Should().ThrowAsync<ModelNotFoundException>()
            .WithMessage("Comment not found");
        
        _uowMock.Verify(uow => uow.CommentRepository.GetByIdAsync(_comment.Id), Times.Once);
    }

    [Fact]
    public async Task ShouldDeleteReturnComment()
    {
        _uowMock.Setup(uow => uow.CommentRepository.DeleteAsync(_comment))
            .Returns(Task.CompletedTask);
        _uowMock.Setup(uow => uow.CommitAsync());
        
        await _service.DeleteAsync(_comment);
        
        _uowMock.Verify(uow => uow.CommentRepository.DeleteAsync(_comment), Times.Once);
    }

    [Fact]
    public async Task ShouldReturnTrue_Success_ExistsById()
    {
        _uowMock.Setup(x => x.CommentRepository.ExistsById(_comment.Id))
            .ReturnsAsync(true);

        bool exists = await _service.ExistsById(_comment.Id);
        exists.Should().BeTrue();
        
        _uowMock.Verify(uow => uow.CommentRepository.ExistsById(_comment.Id), Times.Once);
    }

    [Fact]
    public async Task ShouldReturnCommentCreated_Success()
    {
        CreateCommentDto dto = new()
        {
            Content = _comment.Content,
            ParentId = _comment.ParentId,
            PostId = _comment.PostId,
        };
        
        _uowMock.Setup(x => x.Mapper.Map<CommentModel>(dto)).Returns(_comment);
        _uowMock.Setup(x => x.CommentRepository.AddAsync(It.IsAny<CommentModel>()))
            .ReturnsAsync(_comment);
        _uowMock.Setup(x => x.CommitAsync());

        CommentModel model = await _service.CreateAsync(dto, _user);
        model.Should().BeEquivalentTo(_comment);
        
        _uowMock.Verify(uow => uow.CommentRepository.AddAsync(_comment), Times.Once);
        _uowMock.Verify(uow => uow.CommitAsync(), Times.Once);
        _uowMock.Verify(uow => uow.Mapper.Map<CommentModel>(dto), Times.Once);
    }
    
}