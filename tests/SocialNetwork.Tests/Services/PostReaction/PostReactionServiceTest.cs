using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SocialNetwork.Write.API.Modules.PostReactions.Dto;
using SocialNetwork.Write.API.Modules.PostReactions.Model;
using SocialNetwork.Write.API.Modules.PostReactions.Service.Provider;
using SocialNetwork.Write.API.Utils.result;
using SocialNetwork.Write.API.Utils.UnitOfWork;

namespace SocialNetwork.Tests.Services.PostReaction;

public class PostReactionServiceTest
{
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<ILogger<PostReactionService>> _loggerMock = new();
    private readonly PostReactionService _service;

    public PostReactionServiceTest()
    {
        _service = new PostReactionService(
            _uowMock.Object,
            _loggerMock.Object
        );
    }

    private readonly PostReactionModel model = new()
    {
        Id = Guid.NewGuid().ToString(),
        PostId = Guid.NewGuid().ToString(),
        UserId = Guid.NewGuid().ToString(),
        ReactionId = Guid.NewGuid().ToString(),
        CreatedAt = DateTime.Now,
        UpdatedAt = DateTime.Now
    };

    // CREATE
    [Fact]
    public async Task ShouldCreate_Success()
    {
        _uowMock.Setup(x => x.PostReactionRepository.AddAsync(
                It.Is<PostReactionModel>(v => 
                    v.PostId == model.PostId &&
                    v.UserId == model.UserId &&
                    v.ReactionId == model.ReactionId
                    )
                )
        ).ReturnsAsync(model);

        TogglePostReactionDto dto = new(model.PostId, model.ReactionId);

        Result<PostReactionModel> result = await _service.Create(dto, model.UserId);

        result.Success.Should().BeTrue();
        result.Status.Should().Be(201);
        
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(model.Id);
        
        _uowMock.Verify(x => x.PostReactionRepository.AddAsync(
            It.Is<PostReactionModel>(v => 
                v.PostId == model.PostId &&
                v.UserId == model.UserId &&
                v.ReactionId == model.ReactionId
            )
        ), Times.Once);
    }

    [Fact]
    public async Task ShouldCreate_Conflict_WhenDuplicateEntry()
    {
        var dto = new TogglePostReactionDto(model.PostId, model.ReactionId);

        _uowMock
            .Setup(x => x.PostReactionRepository.AddAsync(It.IsAny<PostReactionModel>()))
            .ThrowsAsync(CreateDbUpdateException(1062));

        var result = await _service.Create(dto, model.UserId);

        result.Success.Should().BeFalse();
        result.Status.Should().Be(StatusCodes.Status409Conflict);
        result.Message.Should().Be("You have already reacted on this post.");
        result.Value.Should().BeNull();

        _uowMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task ShouldCreate_Throw_WhenForeignKeyViolation()
    {
        var dto = new TogglePostReactionDto(model.PostId, model.ReactionId);

        _uowMock
            .Setup(x => x.PostReactionRepository.AddAsync(It.IsAny<PostReactionModel>()))
            .ThrowsAsync(CreateDbUpdateException(1452, "FOREIGN KEY constraint fails"));

        Func<Task> act = async () => await _service.Create(dto, model.UserId);

        await act.Should().ThrowAsync<DbUpdateException>();

        _uowMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task ShouldCreate_InternalServerError_WhenUnexpectedException()
    {
        var dto = new TogglePostReactionDto(model.PostId, model.ReactionId);

        _uowMock
            .Setup(x => x.PostReactionRepository.AddAsync(It.IsAny<PostReactionModel>()))
            .ThrowsAsync(new InvalidOperationException("unexpected"));

        var result = await _service.Create(dto, model.UserId);

        result.Success.Should().BeFalse();
        result.Status.Should().Be(StatusCodes.Status500InternalServerError);
        result.Message.Should().Be("Error processing your reaction.");
        result.Value.Should().BeNull();

        _uowMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task ShouldNotCommit_WhenCommitIsFalse()
    {
        var dto = new TogglePostReactionDto(model.PostId, model.ReactionId);

        _uowMock
            .Setup(x => x.PostReactionRepository.AddAsync(It.IsAny<PostReactionModel>()))
            .ReturnsAsync(model);

        var result = await _service.Create(dto, model.UserId, commit: false);

        result.Success.Should().BeTrue();
        result.Status.Should().Be(StatusCodes.Status201Created);
        result.Value.Should().NotBeNull();

        _uowMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    // DELETE
    [Fact]
    public async Task ShouldDeleteReaction_Success()
    {
        _uowMock.Setup(x => x.PostReactionRepository.Delete(model));
        _uowMock.Setup(x => x.CommitAsync());
        
        await _service.DeleteAsync(model);
        
        _uowMock.Verify(x => x.PostReactionRepository.Delete(model), Times.Once);
        _uowMock.Verify(x => x.CommitAsync(), Times.Once);
    }
    
    
    
    private static DbUpdateException CreateDbUpdateException(int number, string message = "Database error")
    {
        return new DbUpdateException(
            message,
            new FakeMySqlException(number, message)
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