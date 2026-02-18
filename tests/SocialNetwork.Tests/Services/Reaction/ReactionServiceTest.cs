using FluentAssertions;
using Moq;
using SocialNetwork.Contracts.Enums.Reaction;
using SocialNetwork.Write.API.Configs.Exception.classes;
using SocialNetwork.Write.API.dto.Reaction;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Services.Providers;
using SocialNetwork.Write.API.Utils.UnitOfWork;

namespace SocialNetwork.Tests.Services.Reaction;

public class ReactionServiceTest
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly ReactionService _service;
    
    public ReactionServiceTest()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _service = new ReactionService(_uowMock.Object);
    }

    private static ReactionModel _reaction = new()
    {
        Id = Guid.NewGuid().ToString(),
        Name = "NameAny",
        Type = ReactionTypeEnum.Action,
        EmojiUrl = "https://hello.world.com",
        Active = true,
        Visible = true,
        DisplayOrder = 1,
        EmojiUnicode = "#745057849",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        Version = 1
    };

    [Fact]
    public async Task ShouldReturnReaction_Success()
    {
        _uowMock.Setup(x => x.ReactionRepository.GetByIdAsync(_reaction.Id))
            .ReturnsAsync(_reaction);
        
        ReactionModel result = await _service.GetByIdAsync(_reaction.Id);
        
        Assert.Equal(_reaction.Id, result.Id);
        Assert.Equal(_reaction.Name, result.Name);

        _uowMock.Verify(x => x.ReactionRepository.GetByIdAsync(_reaction.Id), Times.Once);
    }

    [Fact]
    public async Task ShouldReturnReaction_NotFound()
    {
        _uowMock.Setup(x => x.ReactionRepository.GetByIdAsync(_reaction.Id))
            .ReturnsAsync((ReactionModel?) null);
        
        Func<Task> act = async () => await _service.GetByIdAsync(_reaction.Id);
        
        await act.Should().ThrowAsync<ModelNotFoundException>()
            .WithMessage("Reaction not found");
        
        _uowMock.Verify(x => x.ReactionRepository.GetByIdAsync(_reaction.Id), Times.Once);
    }

    [Fact]
    public async Task ShouldReturnReaction_Added()
    {
        CreateReactionDto dto = new CreateReactionDto(){ Name = _reaction.Name, Type = _reaction.Type };
        _uowMock.Setup(x => x.Mapper.Map<ReactionModel>(It.IsAny<CreateReactionDto>()))
            .Returns(_reaction);
        _uowMock.Setup(x => x.ReactionRepository.AddAsync(It.IsAny<ReactionModel>()))
            .ReturnsAsync(_reaction);
        _uowMock.Setup(x => x.CommitAsync())
            .Returns(Task.CompletedTask);

        ReactionModel reactionModel = await _service.CreateAsync(dto);
        
        reactionModel.Should().BeEquivalentTo(_reaction);
        
        _uowMock.Verify(x => x.ReactionRepository.AddAsync(It.IsAny<ReactionModel>()), Times.Once);
        _uowMock.Verify(x => x.Mapper.Map<ReactionModel>(It.IsAny<CreateReactionDto>()), Times.Once);
        _uowMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task ShouldDeleteReaction()
    {
        _uowMock.Setup(x => x.ReactionRepository.DeleteAsync(_reaction))
            .Returns(Task.CompletedTask);
        _uowMock.Setup(x => x.CommitAsync())
            .Returns(Task.CompletedTask);
        
        await _service.DeleteAsync(_reaction);
        
        _uowMock.Verify(x => x.CommitAsync(), Times.Once);
        _uowMock.Verify(x => x.ReactionRepository.DeleteAsync(It.IsAny<ReactionModel>()), Times.Once);
    }
    
}