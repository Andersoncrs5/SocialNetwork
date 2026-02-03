using AutoMapper;
using FluentAssertions;
using Moq;
using SocialNetwork.Write.API.Configs.Exception.classes;
using SocialNetwork.Write.API.dto.Category;
using SocialNetwork.Write.API.dto.Tag;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Services.Providers;
using SocialNetwork.Write.API.Utils.UnitOfWork;

namespace SocialNetwork.Tests.Services.Tags;

public class TagServiceTest
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly TagService _tagService;
    
    public TagServiceTest()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _tagService = new TagService(_uowMock.Object, _mapperMock.Object);
    }

    private readonly TagModel _tag = new ()
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

    private readonly CreateTagDto _dto = new ()
    {
        Name = "test",
        Slug = "test",
        Description = "testDesc",
        Color = "#000000",
        IsActive = true,
        IsVisible = true,
        IsSystem = true,
    };
    
    [Fact]
    public async Task GetByIdSimple_ShouldReturnTag()
    {
        _uowMock.Setup(x => x.TagRepository.GetByIdAsync(_tag.Id)).ReturnsAsync(_tag);

        TagModel model = await _tagService.GetByIdSimpleAsync(_tag.Id);
        
        model.Should().BeEquivalentTo(_tag);
        
        _uowMock.Verify(x => x.TagRepository.GetByIdAsync(_tag.Id), Times.Once);
        _uowMock.Verify(x => x.CommitAsync(), Times.Never);
    }
    
    [Fact]
    public async Task GetByIdSimple_ShouldReturnNotFound()
    {
        _uowMock.Setup(x => x.TagRepository.GetByIdAsync(_tag.Id)).ReturnsAsync((TagModel?) null);

        Func<Task> func = async () => await _tagService.GetByIdSimpleAsync(_tag.Id);
        
        await func.Should().ThrowAsync<ModelNotFoundException>()
            .WithMessage("Tag not found");
        
        _uowMock.Verify(x => x.TagRepository.GetByIdAsync(_tag.Id), Times.Once);
        _uowMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task ExistsById_ShouldReturnTrue()
    {
        _uowMock.Setup(x => x.TagRepository.ExistsById(_tag.Id)).ReturnsAsync(true);

        bool exists = await _tagService.ExistsByIdAsync(_tag.Id);
        
        exists.Should().BeTrue();

        _uowMock.Verify(x => x.TagRepository.ExistsById(_tag.Id), Times.Once);
        _uowMock.Verify(x => x.CommitAsync(), Times.Never);
    }
    
    [Fact]
    public async Task ExistsById_ShouldReturnFalse()
    {
        _uowMock.Setup(x => x.TagRepository.ExistsById(_tag.Id)).ReturnsAsync(false);

        bool exists = await _tagService.ExistsByIdAsync(_tag.Id);
        
        exists.Should().BeFalse();

        _uowMock.Verify(x => x.TagRepository.ExistsById(_tag.Id), Times.Once);
        _uowMock.Verify(x => x.CommitAsync(), Times.Never);
    }
    
    [Fact]
    public async Task ExistsByName_ShouldReturnTrue()
    {
        _uowMock.Setup(x => x.TagRepository.ExistsByName(_tag.Name)).ReturnsAsync(true);

        bool exists = await _tagService.ExistsByNameAsync(_tag.Name);
        
        exists.Should().BeTrue();

        _uowMock.Verify(x => x.TagRepository.ExistsByName(_tag.Name), Times.Once);
        _uowMock.Verify(x => x.CommitAsync(), Times.Never);
    }
    
    [Fact]
    public async Task ExistsByName_ShouldReturnFalse()
    {
        _uowMock.Setup(x => x.TagRepository.ExistsByName(_tag.Name)).ReturnsAsync(false);

        bool exists = await _tagService.ExistsByNameAsync(_tag.Name);
        
        exists.Should().BeFalse();

        _uowMock.Verify(x => x.TagRepository.ExistsByName(_tag.Name), Times.Once);
        _uowMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Delete_ShouldDeleteTag()
    {
        _uowMock.Setup(x => x.TagRepository.DeleteAsync(_tag)).Returns(Task.CompletedTask);
        
        await _tagService.DeleteAsync(_tag);
        _uowMock.Verify(x => x.TagRepository.DeleteAsync(_tag), Times.Once);
        
        _uowMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Create_ShouldCreateTag()
    {
        _mapperMock.Setup(x => x.Map<TagModel>(_dto)).Returns(_tag);
        _uowMock.Setup(x => x.TagRepository.AddAsync(_tag)).ReturnsAsync(_tag);
        
        TagModel model = await _tagService.CreateAsync(_dto);
        model.Should().BeEquivalentTo(_tag);
        
        _uowMock.Verify(x => x.TagRepository.AddAsync(_tag), Times.Once);
        _uowMock.Verify(x => x.CommitAsync(), Times.Once);
        _mapperMock.Verify(x => x.Map<TagModel>(_dto), Times.Once);
    }

    [Fact]
    public async Task Update_ShouldThrowConflictValueException_WhenUpdateFieldName()
    {
        UpdateTagDto dto = new ();
        dto.Name = _tag.Name + "a";
        
        _uowMock.Setup(x => x.TagRepository.ExistsByName(dto.Name)).ReturnsAsync(true);

        Func<Task> func = async () => await _tagService.UpdateAsync(_tag, dto);

        await func.Should().ThrowAsync<ConflictValueException>()
            .WithMessage($"Name: {dto.Name} already in use");
    }
    
    [Fact]
    public async Task Update_ShouldThrowConflictValueException_WhenUpdateFieldSlug()
    {
        UpdateTagDto dto = new ();
        dto.Slug = _tag.Slug + "a";
        
        _uowMock.Setup(x => x.TagRepository.ExistsBySlug(dto.Slug)).ReturnsAsync(true);

        Func<Task> func = async () => await _tagService.UpdateAsync(_tag, dto);

        await func.Should().ThrowAsync<ConflictValueException>()
            .WithMessage($"Slug: {dto.Slug} already in use");
    }
    
}