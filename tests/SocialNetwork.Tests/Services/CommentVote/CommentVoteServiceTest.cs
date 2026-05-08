using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SocialNetwork.Write.API.Configs.Exception.classes;
using SocialNetwork.Write.API.Models.Enums.Post;
using SocialNetwork.Write.API.Modules.CommentVote.Dto;
using SocialNetwork.Write.API.Modules.CommentVote.Model;
using SocialNetwork.Write.API.Modules.CommentVote.Service.provider;
using SocialNetwork.Write.API.Utils.Enums;
using SocialNetwork.Write.API.Utils.UnitOfWork;

namespace SocialNetwork.Tests.Services.CommentVote;

public class CommentVoteServiceTest
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly CommentVoteService _service;
    private readonly Mock<ILogger<CommentVoteService>> _loggerMock = new();
    private readonly string _userId;
    private readonly string _commentId;
    private readonly CommentVoteModel _vote; // Apenas declare aqui

    public CommentVoteServiceTest()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _service = new CommentVoteService(_uowMock.Object, _loggerMock.Object);
        _userId = Guid.NewGuid().ToString();
        _commentId = Guid.NewGuid().ToString();

        // Inicialize aqui
        _vote = new CommentVoteModel
        {
            Id = Guid.NewGuid().ToString(),
            CommentId = _commentId,
            UserId = _userId,
            Vote = VoteEnum.UPVOTE,
            UpdatedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
        };
    }

    [Fact]
    public async Task ShouldCreate_Success()
    {
        _uowMock.Setup(x => x.CommentVoteRepository.AddAsync(
            It.Is<CommentVoteModel>(v =>
                v.UserId == _vote.UserId &&
                v.CommentId == _vote.CommentId &&
                v.Vote == VoteEnum.UPVOTE))
        ).ReturnsAsync(_vote);

        _uowMock.Setup(x => x.CommitAsync())
            .Returns(Task.CompletedTask);

        var result = await _service.Create(
            _vote.UserId,
            _vote.CommentId,
            VoteEnum.UPVOTE
        );

        Assert.True(result.Success);
        Assert.Equal(StatusCodes.Status201Created, result.Status);
        Assert.NotNull(result.Value);

        var value = result.Value!;

        Assert.Equal(_vote.Id, value.Id);
        Assert.Equal(_vote.CommentId, value.CommentId);
        Assert.Equal(_vote.UserId, value.UserId);
        Assert.Equal(VoteEnum.UPVOTE, value.Vote);

        _uowMock.Verify(
            x => x.CommentVoteRepository.AddAsync(It.IsAny<CommentVoteModel>()),
            Times.Once
        );

        _uowMock.Verify(
            x => x.CommitAsync(),
            Times.Once
        );
    }

    [Fact]
    public async Task ShouldThrow_WhenForeignKeyViolation()
    {
        _uowMock.Setup(x => x.CommentVoteRepository.AddAsync(
            It.IsAny<CommentVoteModel>()))
        .ThrowsAsync(CreateDbUpdateException(1452));

        await Assert.ThrowsAsync<DbUpdateException>(() =>
            _service.Create(
                _vote.UserId,
                _vote.CommentId,
                VoteEnum.UPVOTE
            )
        );

        _uowMock.Verify(
            x => x.CommitAsync(),
            Times.Never
        );
    }

    [Fact]
    public async Task ShouldReturnConflict_WhenDuplicateVote()
    {
        _uowMock.Setup(x => x.CommentVoteRepository.AddAsync(
            It.IsAny<CommentVoteModel>()))
        .ThrowsAsync(CreateDuplicateEntryException());

        var result = await _service.Create(
            _vote.UserId,
            _vote.CommentId,
            VoteEnum.UPVOTE
        );

        Assert.False(result.Success);
        Assert.Equal(StatusCodes.Status409Conflict, result.Status);
        Assert.Equal(
            "You have already voted on this post.",
            result.Message
        );

        _uowMock.Verify(
            x => x.CommitAsync(),
            Times.Never
        );
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenDataTooLong()
    {
        _uowMock.Setup(x => x.CommentVoteRepository.AddAsync(
            It.IsAny<CommentVoteModel>()))
        .ThrowsAsync(CreateDataTooLongException());

        var result = await _service.Create(
            _vote.UserId,
            _vote.CommentId,
            VoteEnum.UPVOTE
        );

        Assert.False(result.Success);
        Assert.Equal(StatusCodes.Status400BadRequest, result.Status);
        Assert.Equal(
            "One of the fields is too long.",
            result.Message
        );

        _uowMock.Verify(
            x => x.CommitAsync(),
            Times.Never
        );
    }

    [Fact]
    public async Task ShouldThrowInternalServerError_WhenUnexpectedException()
    {
        _uowMock.Setup(x => x.CommentVoteRepository.AddAsync(
            It.IsAny<CommentVoteModel>()))
        .ThrowsAsync(new Exception("unexpected"));

        var exception = await Assert.ThrowsAsync<InternalServerErrorException>(() =>
            _service.Create(
                _vote.UserId,
                _vote.CommentId,
                VoteEnum.UPVOTE
            )
        );
        
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task ShouldDelete_Success()
    {
        _uowMock.Setup(x => x.CommentVoteRepository.DeleteById(_vote.Id))
            .ReturnsAsync(true);

        _uowMock.Setup(x => x.CommitAsync())
            .Returns(Task.CompletedTask);

        var result = await _service.DeleteById(_vote.Id);

        Assert.True(result.Success);
        Assert.Equal(StatusCodes.Status200OK, result.Status);
        Assert.Equal("Comment vote deleted successfully.", result.Message);

        _uowMock.Verify(x => x.CommentVoteRepository.DeleteById(_vote.Id), Times.Once);
        _uowMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task ShouldDelete_NotFound_WhenVoteDoesNotExist()
    {
        _uowMock.Setup(x => x.CommentVoteRepository.DeleteById(_vote.Id))
            .ReturnsAsync(false);

        var result = await _service.DeleteById(_vote.Id);

        Assert.False(result.Success);
        Assert.Equal(StatusCodes.Status404NotFound, result.Status);
        Assert.Equal("Comment vote not found.", result.Message);

        _uowMock.Verify(x => x.CommentVoteRepository.DeleteById(_vote.Id), Times.Once);
        _uowMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task ShouldDelete_Conflict_WhenForeignKeyViolation()
    {
        _uowMock.Setup(x => x.CommentVoteRepository.DeleteById(_vote.Id))
            .ThrowsAsync(CreateDbUpdateException(1451));

        var result = await _service.DeleteById(_vote.Id);

        Assert.False(result.Success);
        Assert.Equal(StatusCodes.Status409Conflict, result.Status);
        Assert.Equal("Cannot delete because it is referenced by other data.", result.Message);

        _uowMock.Verify(x => x.CommentVoteRepository.DeleteById(_vote.Id), Times.Once);
        _uowMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task ShouldDelete_Fail_WhenUnexpectedErrorOccurs()
    {
        _uowMock.Setup(x => x.CommentVoteRepository.DeleteById(_vote.Id))
            .ThrowsAsync(new Exception("unexpected"));

        var result = await _service.DeleteById(_vote.Id);

        Assert.False(result.Success);
        Assert.Equal(StatusCodes.Status500InternalServerError, result.Status);
        Assert.Equal("Error deleting post vote.", result.Message);

        _uowMock.Verify(x => x.CommentVoteRepository.DeleteById(_vote.Id), Times.Once);
        _uowMock.Verify(x => x.CommitAsync(), Times.Never);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task ShouldDelete_WithoutCommit_WhenCommitIsFalse()
    {
        _uowMock.Setup(x => x.CommentVoteRepository.DeleteById(_vote.Id))
            .ReturnsAsync(true);

        var result = await _service.DeleteById(_vote.Id, false);

        Assert.True(result.Success);
        Assert.Equal(StatusCodes.Status200OK, result.Status);

        _uowMock.Verify(x => x.CommentVoteRepository.DeleteById(_vote.Id), Times.Once);
        _uowMock.Verify(x => x.CommitAsync(), Times.Never);
    }
    
    [Fact]
    public async Task ShouldToggle_Add_WhenVoteDoesNotExist()
    {
        _uowMock
            .Setup(x => x.CommentVoteRepository.GetByCommentIdAndUserId(_commentId, _userId))
            .ReturnsAsync((CommentVoteModel?)null);

        var createdModel = new CommentVoteModel
        {
            Id = Guid.NewGuid().ToString(),
            CommentId = _commentId,
            UserId = _userId,
            Vote = VoteEnum.UPVOTE,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _uowMock
            .Setup(x => x.CommentVoteRepository.AddAsync(It.Is<CommentVoteModel>(m =>
                m.CommentId == _commentId &&
                m.UserId == _userId &&
                m.Vote == VoteEnum.UPVOTE)))
            .ReturnsAsync(createdModel);

        _uowMock
            .Setup(x => x.CommitAsync())
            .Returns(Task.CompletedTask);

        var dto = new ToggleCommentVoteDto(_commentId, VoteEnum.UPVOTE);

        var result = await _service.Toggle(dto, _userId);

        Assert.True(result.Success);
        Assert.Equal(StatusCodes.Status201Created, result.Status);
        Assert.NotNull(result.Value);
        Assert.Equal(ToggleStatus.Added, result.Value!.Action);
        Assert.NotNull(result.Value.Value);
        Assert.Equal(createdModel.Id, result.Value.Value!.Id);

        _uowMock.Verify(x => x.CommentVoteRepository.GetByCommentIdAndUserId(_commentId, _userId), Times.Once);
        _uowMock.Verify(x => x.CommentVoteRepository.AddAsync(It.IsAny<CommentVoteModel>()), Times.Once);
        _uowMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task ShouldToggle_Add_WithoutCommit_WhenCommitIsFalse()
    {
        _uowMock
            .Setup(x => x.CommentVoteRepository.GetByCommentIdAndUserId(_commentId, _userId))
            .ReturnsAsync((CommentVoteModel?)null);

        var createdModel = new CommentVoteModel
        {
            Id = Guid.NewGuid().ToString(),
            CommentId = _commentId,
            UserId = _userId,
            Vote = VoteEnum.UPVOTE,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _uowMock
            .Setup(x => x.CommentVoteRepository.AddAsync(It.IsAny<CommentVoteModel>()))
            .ReturnsAsync(createdModel);
        
        var dto = new ToggleCommentVoteDto(_commentId, VoteEnum.UPVOTE);
        
        var result = await _service.Toggle(dto, _userId, commit: false);

        Assert.True(result.Success);
        Assert.Equal(StatusCodes.Status201Created, result.Status);
        Assert.Equal(ToggleStatus.Added, result.Value!.Action);

        _uowMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task ShouldToggle_Update_WhenExistingVoteIsDifferent()
    {
        var existing = new CommentVoteModel
        {
            Id = Guid.NewGuid().ToString(),
            CommentId = _commentId,
            UserId = _userId,
            Vote = VoteEnum.DOWNVOTE,
            CreatedAt = DateTime.UtcNow.AddMinutes(-10),
            UpdatedAt = DateTime.UtcNow.AddMinutes(-10)
        };

        _uowMock
            .Setup(x => x.CommentVoteRepository.GetByCommentIdAndUserId(_commentId, _userId))
            .ReturnsAsync(existing);

        _uowMock
            .Setup(x => x.CommitAsync())
            .Returns(Task.CompletedTask);

        var dto = new ToggleCommentVoteDto(_commentId, VoteEnum.UPVOTE);
        
        var result = await _service.Toggle(dto, _userId);

        Assert.True(result.Success);
        Assert.Equal(StatusCodes.Status200OK, result.Status);
        Assert.NotNull(result.Value);
        Assert.Equal(ToggleStatus.Update, result.Value!.Action);

        _uowMock.Verify(x => x.CommentVoteRepository.GetByCommentIdAndUserId(_commentId, _userId), Times.Once);
        _uowMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task ShouldToggle_Remove_WhenExistingVoteIsTheSame()
    {
        var existing = new CommentVoteModel
        {
            Id = Guid.NewGuid().ToString(),
            CommentId = _commentId,
            UserId = _userId,
            Vote = VoteEnum.UPVOTE,
            CreatedAt = DateTime.UtcNow.AddMinutes(-10),
            UpdatedAt = DateTime.UtcNow.AddMinutes(-10)
        };

        _uowMock
            .Setup(x => x.CommentVoteRepository.GetByCommentIdAndUserId(_commentId, _userId))
            .ReturnsAsync(existing);

        _uowMock
            .Setup(x => x.CommentVoteRepository.DeleteById(existing.Id))
            .ReturnsAsync(true);

        _uowMock
            .Setup(x => x.CommitAsync())
            .Returns(Task.CompletedTask);

        var dto = new ToggleCommentVoteDto(_commentId, VoteEnum.UPVOTE);
        
        var result = await _service.Toggle(dto, _userId);

        Assert.True(result.Success);
        Assert.Equal(StatusCodes.Status200OK, result.Status);
        Assert.NotNull(result.Value);
        Assert.Equal(ToggleStatus.Removed, result.Value!.Action);
        Assert.Null(result.Value.Value);

        _uowMock.Verify(x => x.CommentVoteRepository.GetByCommentIdAndUserId(_commentId, _userId), Times.Once);
        _uowMock.Verify(x => x.CommentVoteRepository.DeleteById(existing.Id), Times.Once);
        _uowMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task ShouldToggle_Remove_WithoutCommit_WhenCommitIsFalse()
    {
        var existing = new CommentVoteModel
        {
            Id = Guid.NewGuid().ToString(),
            CommentId = _commentId,
            UserId = _userId,
            Vote = VoteEnum.UPVOTE,
            CreatedAt = DateTime.UtcNow.AddMinutes(-10),
            UpdatedAt = DateTime.UtcNow.AddMinutes(-10)
        };

        _uowMock
            .Setup(x => x.CommentVoteRepository.GetByCommentIdAndUserId(_commentId, _userId))
            .ReturnsAsync(existing);

        _uowMock
            .Setup(x => x.CommentVoteRepository.DeleteById(existing.Id))
            .ReturnsAsync(true);

        var dto = new ToggleCommentVoteDto(_commentId, VoteEnum.UPVOTE);
        
        var result = await _service.Toggle(dto, _userId, commit: false);

        Assert.True(result.Success);
        Assert.Equal(StatusCodes.Status200OK, result.Status);
        Assert.Equal(ToggleStatus.Removed, result.Value!.Action);

        _uowMock.Verify(x => x.CommitAsync(), Times.Never);
    }
    
    private static DbUpdateException CreateDbUpdateException(int number)
    {
        return new DbUpdateException(
            "Database error",
            new FakeMySqlException(number, "foreign key constraint fails")
        );
    }

    private static DbUpdateException CreateDuplicateEntryException()
    {
        return new DbUpdateException(
            "Duplicate entry",
            new FakeMySqlException(1062, "Duplicate entry")
        );
    }

    private static DbUpdateException CreateDataTooLongException()
    {
        return new DbUpdateException(
            "Data too long",
            new FakeMySqlException(1406, "Data too long")
        );
    }

    private sealed class FakeMySqlException : Exception
    {
        public int Number { get; }

        public FakeMySqlException(int number, string message)
            : base(message)
        {
            Number = number;
        }
    }
    
}