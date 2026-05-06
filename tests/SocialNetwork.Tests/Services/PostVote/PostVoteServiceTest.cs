using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SocialNetwork.Write.API.Models.Enums.Post;
using SocialNetwork.Write.API.Modules.Post.Service.Interface;
using SocialNetwork.Write.API.Modules.PostVote.Dto;
using SocialNetwork.Write.API.Modules.PostVote.Gateway;
using SocialNetwork.Write.API.Modules.PostVote.Model;
using SocialNetwork.Write.API.Modules.PostVote.Repository.Interface;
using SocialNetwork.Write.API.Modules.PostVote.Service.provider;
using SocialNetwork.Write.API.Utils.Enums;
using SocialNetwork.Write.API.Utils.result;
using SocialNetwork.Write.API.Utils.UnitOfWork;

namespace SocialNetwork.Tests.Services.PostVote;

public class PostVoteServiceTest
{
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<IPostVoteRepository> _repositoryMock = new();
    private readonly Mock<ILogger<PostVoteService>> _loggerMock = new();
    private readonly PostVoteService _service;
    private readonly Mock<IPostService> _postServiceMock = new();
    private readonly PostVoteModuleGateway _moduleGateway;
    
    public PostVoteServiceTest()
    {
        _moduleGateway = new PostVoteModuleGateway(_postServiceMock.Object);

        _uowMock.SetupGet(x => x.PostVoteRepository).Returns(_repositoryMock.Object);

        _service = new PostVoteService(
            _uowMock.Object,
            _loggerMock.Object,
            _moduleGateway
        );
    }
    
    private readonly PostVoteModel _vote = new()
    {
        Id = Guid.NewGuid().ToString(),
        PostId = Guid.NewGuid().ToString(),
        UserId = Guid.NewGuid().ToString(),
        Vote = VoteEnum.UPVOTE,
        UpdatedAt = DateTime.UtcNow,
        CreatedAt = DateTime.UtcNow,
    };

    [Fact]
    public async Task ShouldCreate_Success()
    {
        _repositoryMock
            .Setup(x => x.AddAsync(It.Is<PostVoteModel>(v =>
                v.UserId == _vote.UserId &&
                v.PostId == _vote.PostId &&
                v.Vote == VoteEnum.UPVOTE)))
            .ReturnsAsync(_vote);
        
        _postServiceMock
            .Setup(x => x.ExistsByIdAsync(_vote.PostId))
            .ReturnsAsync(true);

        _uowMock
            .Setup(x => x.CommitAsync())
            .Returns(Task.CompletedTask);

        var result = await _service.Create(_vote.UserId, _vote.PostId, VoteEnum.UPVOTE);

        Assert.True(result.Success);
        Assert.Equal(StatusCodes.Status201Created, result.Status);
        Assert.NotNull(result.Value);

        var value = result.Value!;
        Assert.Equal(_vote.Id, value.Id);
        Assert.Equal(_vote.PostId, value.PostId);
        Assert.Equal(_vote.UserId, value.UserId);
        Assert.Equal(VoteEnum.UPVOTE, value.Vote);

        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<PostVoteModel>()), Times.Once);
        _uowMock.Verify(x => x.CommitAsync(), Times.Once);
    }
    
    [Fact]
    public async Task ShouldCreate_Conflict_WhenDuplicateEntry()
    {
        _postServiceMock
            .Setup(x => x.ExistsByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        _repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<PostVoteModel>()))
            .ThrowsAsync(CreateDbUpdateException(1062));

        var result = await _service.Create(_vote.UserId, _vote.PostId, VoteEnum.UPVOTE);

        Assert.False(result.Success);
        Assert.Equal(StatusCodes.Status409Conflict, result.Status);
        Assert.Equal("You have already voted on this post.", result.Message);
        Assert.Null(result.Value);

        _uowMock.Verify(x => x.CommitAsync(), Times.Never);
    }
    
    [Fact]
    public async Task ShouldCreate_NotFound_WhenForeignKeyViolation()
    {
        _repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<PostVoteModel>()))
            .ThrowsAsync(CreateDbUpdateException(1452));

        var result = await _service.Create(_vote.UserId, _vote.PostId, VoteEnum.UPVOTE);

        Assert.False(result.Success);
        Assert.Equal(StatusCodes.Status404NotFound, result.Status);
        Assert.Null(result.Value);

        _uowMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task ShouldCreate_BadRequest_WhenDataTooLong()
    {
        _postServiceMock
            .Setup(x => x.ExistsByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        _repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<PostVoteModel>()))
            .ThrowsAsync(CreateDbUpdateException(1406));

        var result = await _service.Create(_vote.UserId, _vote.PostId, VoteEnum.UPVOTE);

        Assert.False(result.Success);
        Assert.Equal(StatusCodes.Status400BadRequest, result.Status);
        Assert.Equal("One of the fields is too long.", result.Message);
        Assert.Null(result.Value);

        _uowMock.Verify(x => x.CommitAsync(), Times.Never);
    }
    
    [Fact]
    public async Task ShouldCreate_InternalServerError_WhenUnexpectedException()
    {
        _postServiceMock
            .Setup(x => x.ExistsByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        _repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<PostVoteModel>()))
            .ThrowsAsync(new InvalidOperationException("unexpected"));

        var result = await _service.Create(_vote.UserId, _vote.PostId, VoteEnum.UPVOTE);

        Assert.False(result.Success);
        Assert.Equal(StatusCodes.Status500InternalServerError, result.Status);
        Assert.Equal("Error processing your vote.", result.Message);
        Assert.Null(result.Value);

        _uowMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task ShouldNotCommit_WhenCommitIsFalse()
    {
        _postServiceMock
            .Setup(x => x.ExistsByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        _repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<PostVoteModel>()))
            .ReturnsAsync(_vote);

        var result = await _service.Create(
            _vote.UserId,
            _vote.PostId,
            VoteEnum.UPVOTE,
            commit: false
        );

        Assert.True(result.Success);
        Assert.Equal(StatusCodes.Status201Created, result.Status);
        Assert.NotNull(result.Value);

        _uowMock.Verify(x => x.CommitAsync(), Times.Never);
    }
    
    [Fact]
    public async Task ShouldDeleteById_Success()
    {
        var id = _vote.Id;

        _repositoryMock
            .Setup(x => x.DeleteById(id))
            .ReturnsAsync(true);

        _uowMock
            .Setup(x => x.CommitAsync())
            .Returns(Task.CompletedTask);

        var result = await _service.DeleteById(id);

        Assert.True(result.Success);
        Assert.Equal(StatusCodes.Status200OK, result.Status);
        Assert.Equal("Post vote deleted successfully.", result.Message);

        _repositoryMock.Verify(x => x.DeleteById(id), Times.Once);
        _uowMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task ShouldDeleteById_NotFound_WhenEntityDoesNotExist()
    {
        var id = _vote.Id;

        _repositoryMock
            .Setup(x => x.DeleteById(id))
            .ReturnsAsync(false);

        var result = await _service.DeleteById(id);

        Assert.False(result.Success);
        Assert.Equal(StatusCodes.Status404NotFound, result.Status);
        Assert.Equal("Post vote not found.", result.Message);
        
        _repositoryMock.Verify(x => x.DeleteById(id), Times.Once);
        _uowMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task ShouldDeleteById_Conflict_WhenForeignKeyViolation()
    {
        var id = _vote.Id;

        _repositoryMock
            .Setup(x => x.DeleteById(id))
            .ThrowsAsync(CreateDbUpdateException(1451));

        var result = await _service.DeleteById(id);

        Assert.False(result.Success);
        Assert.Equal(StatusCodes.Status409Conflict, result.Status);
        Assert.Equal("Cannot delete because it is referenced by other data.", result.Message);
        
        _repositoryMock.Verify(x => x.DeleteById(id), Times.Once);
        _uowMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task ShouldDeleteById_InternalServerError_WhenUnexpectedException()
    {
        var id = _vote.Id;

        _repositoryMock
            .Setup(x => x.DeleteById(id))
            .ThrowsAsync(new InvalidOperationException("unexpected"));

        var result = await _service.DeleteById(id);

        Assert.False(result.Success);
        Assert.Equal(StatusCodes.Status500InternalServerError, result.Status);
        Assert.Equal("Error deleting post vote.", result.Message);
        
        _repositoryMock.Verify(x => x.DeleteById(id), Times.Once);
        _uowMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task ShouldDeleteById_NotCommit_WhenCommitIsFalse()
    {
        var id = _vote.Id;

        _repositoryMock
            .Setup(x => x.DeleteById(id))
            .ReturnsAsync(true);

        var result = await _service.DeleteById(id, commit: false);

        Assert.True(result.Success);
        Assert.Equal(StatusCodes.Status200OK, result.Status);
        
        _repositoryMock.Verify(x => x.DeleteById(id), Times.Once);
        _uowMock.Verify(x => x.CommitAsync(), Times.Never);
    }
    
    [Fact]
    public async Task ShouldToggle_Create_WhenNotExists()
    {
        _postServiceMock
            .Setup(x => x.ExistsByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        TogglePostVoteDto dto = new(_vote.PostId, VoteEnum.UPVOTE);

        _repositoryMock
            .Setup(x => x.GetByPostIdAndUserId(dto.postId, _vote.UserId))
            .ReturnsAsync((PostVoteModel?)null);

        _repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<PostVoteModel>()))
            .ReturnsAsync(_vote);

        _uowMock
            .Setup(x => x.CommitAsync())
            .Returns(Task.CompletedTask);

        var result = await _service.Toggle(dto, _vote.UserId);

        Assert.True(result.Success);
        Assert.Equal(StatusCodes.Status201Created, result.Status);

        Assert.Equal(ToggleStatus.Added, result.Value!.Action);

        _uowMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task ShouldToggle_Remove_WhenSameVote()
    {
        TogglePostVoteDto dto = new TogglePostVoteDto(_vote.PostId, VoteEnum.UPVOTE);

        _repositoryMock
            .Setup(x => x.GetByPostIdAndUserId(dto.postId, _vote.UserId))
            .ReturnsAsync(_vote);

        _repositoryMock
            .Setup(x => x.DeleteById(_vote.Id))
            .ReturnsAsync(true);

        _uowMock
            .Setup(x => x.CommitAsync())
            .Returns(Task.CompletedTask);

        var result = await _service.Toggle(dto, _vote.UserId);

        Assert.True(result.Success);
        Assert.Equal(StatusCodes.Status200OK, result.Status);

        var toggle = result.Value!;
        Assert.Equal(ToggleStatus.Removed, toggle.Action);
        Assert.Null(toggle.Value);

        _repositoryMock.Verify(x => x.DeleteById(_vote.Id), Times.Once);
        _uowMock.Verify(x => x.CommitAsync(), Times.Once);
    }
    
    [Fact]
    public async Task ShouldToggle_Fail_WhenCreateFails()
    {
        _postServiceMock
            .Setup(x => x.ExistsByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        TogglePostVoteDto dto = new(_vote.PostId, VoteEnum.UPVOTE);

        _repositoryMock
            .Setup(x => x.GetByPostIdAndUserId(dto.postId, _vote.UserId))
            .ReturnsAsync((PostVoteModel?)null);

        _repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<PostVoteModel>()))
            .ThrowsAsync(CreateDbUpdateException(1062));

        var result = await _service.Toggle(dto, _vote.UserId);

        Assert.False(result.Success);
        Assert.Equal(StatusCodes.Status409Conflict, result.Status);

        _uowMock.Verify(x => x.CommitAsync(), Times.Never);
    }
    
    [Fact]
    public async Task ShouldToggle_Fail_WhenDeleteFails()
    {
        TogglePostVoteDto dto = new TogglePostVoteDto(_vote.PostId, VoteEnum.UPVOTE);

        _repositoryMock
            .Setup(x => x.GetByPostIdAndUserId(dto.postId, _vote.UserId))
            .ReturnsAsync(_vote);

        _repositoryMock
            .Setup(x => x.DeleteById(_vote.Id))
            .ReturnsAsync(false);

        var result = await _service.Toggle(dto, _vote.UserId);

        Assert.False(result.Success);
        Assert.Equal(StatusCodes.Status404NotFound, result.Status);

        _uowMock.Verify(x => x.CommitAsync(), Times.Never);
    }
    
    [Fact]
    public async Task ShouldToggle_NotCommit_WhenCommitFalse()
    {
        _postServiceMock
            .Setup(x => x.ExistsByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        TogglePostVoteDto dto = new(_vote.PostId, VoteEnum.UPVOTE);

        _repositoryMock
            .Setup(x => x.GetByPostIdAndUserId(dto.postId, _vote.UserId))
            .ReturnsAsync((PostVoteModel?)null);

        _repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<PostVoteModel>()))
            .ReturnsAsync(_vote);

        var result = await _service.Toggle(dto, _vote.UserId, commit: false);

        Assert.True(result.Success);
        Assert.Equal(StatusCodes.Status201Created, result.Status);

        _uowMock.Verify(x => x.CommitAsync(), Times.Never);
    }
    
    private static DbUpdateException CreateDbUpdateException(int number)
    {
        return new DbUpdateException(
            "Database error",
            new FakeMySqlException(number, "fake mysql error")
        );
    }

    private sealed class FakeMySqlException : Exception
    {
        public int Number { get; }

        public FakeMySqlException(int number, string message) : base(message)
        {
            Number = number;
        }
    }
}