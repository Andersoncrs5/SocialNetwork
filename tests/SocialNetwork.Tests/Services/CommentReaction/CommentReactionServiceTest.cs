using FluentAssertions;
using Moq;
using SocialNetwork.Write.API.Configs.Exception.classes;
using SocialNetwork.Write.API.Modules.CommentReactions.Model;
using SocialNetwork.Write.API.Modules.CommentReactions.Service.Provider;
using SocialNetwork.Write.API.Utils.UnitOfWork;

namespace SocialNetwork.Tests.Services.CommentReaction;

public class CommentReactionServiceTest
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly CommentReactionService _service;
    
    public CommentReactionServiceTest()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _service = new CommentReactionService(_uowMock.Object);
    }

    private static CommentReactionModel _reaction = new()
    {
        Id = Guid.NewGuid().ToString(),
        CommentId = Guid.NewGuid().ToString(),
        UserId = Guid.NewGuid().ToString(),
        ReactionId = Guid.NewGuid().ToString(),
    };

    [Fact]
    public async Task ShouldReturnCommentReactionWhenGetByIdAsync()
    {
        _uowMock.Setup(x => x.CommentReactionRepository.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(_reaction);
        
        CommentReactionModel result = await _service.GetByIdSimpleAsync(_reaction.Id);
        
        Assert.Equal(_reaction.Id, result.Id);
        
        _uowMock.Verify(x => x.CommentReactionRepository.GetByIdAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task ShouldThrowNotFoundExceptionWhenGetByIdAsync()
    {
        _uowMock.Setup(x => x.CommentReactionRepository.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((CommentReactionModel?) null);
        
        Func<Task> act = async () => await _service.GetByIdSimpleAsync(_reaction.Id);

        await act.Should().ThrowAsync<ModelNotFoundException>()
            .WithMessage("Comment reaction not found");
        
        _uowMock.Verify(x => x.CommentReactionRepository.GetByIdAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task ShouldDeleteCommentReaction()
    {
        _uowMock.Setup(x => x.CommentReactionRepository.DeleteAsync(It.IsAny<CommentReactionModel>()))
            .Returns(Task.CompletedTask);
        _uowMock.Setup(x => x.CommitAsync())
            .Returns(Task.CompletedTask);

        await _service.DeleteAsync(_reaction);
        
        _uowMock.Verify(x => x.CommentReactionRepository.DeleteAsync(It.IsAny<CommentReactionModel>()), Times.Once);
        _uowMock.Verify(x => x.CommitAsync(), Times.Once);
    }
    
}