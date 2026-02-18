using AutoMapper;
using FluentAssertions;
using Moq;
using SocialNetwork.Write.API.Configs.Exception.classes;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Modules.Category.Dto;
using SocialNetwork.Write.API.Modules.Category.Model;
using SocialNetwork.Write.API.Modules.Category.Service.Provider;
using SocialNetwork.Write.API.Services.Providers;
using SocialNetwork.Write.API.Utils.UnitOfWork;

namespace SocialNetwork.Tests.Services;

public class CategoryServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CategoryService _service;
    
    public CategoryServiceTests()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _service = new CategoryService(_uowMock.Object, _mapperMock.Object);
    }

    private readonly CategoryModel _category = new CategoryModel()
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

    private UpdateCategoryDto _dto = new UpdateCategoryDto()
    {
        Name = "NameUpdate",
        Slug = "name-update",
        ParentId = "b93f7c4e-28a1-4d37-8461-9c6a1e94e50d"
    };
    
    
    [Fact]
    public async Task GetByIdSimple_ShouldReturnCategory()
    {
        _uowMock.Setup(uow => uow.CategoryRepository.GetByIdAsync(_category.Id)).ReturnsAsync(_category);

        CategoryModel model = await _service.GetByIdSimple(_category.Id);
        Assert.Equal(_category.Id, model.Id);
        
        _uowMock.Verify(x => x.CategoryRepository.GetByIdAsync(_category.Id), Times.Once);
    }

    [Fact]
    public async Task GetByIdSimple_ShouldReturnNotFoundResult()
    {
        _uowMock.Setup(uow => uow.CategoryRepository.GetByIdAsync(_category.Id)).ReturnsAsync(() => null);
        
        Func<Task> func = async () => await _service.GetByIdSimple(_category.Id);

        await func.Should().ThrowAsync<ModelNotFoundException>()
            .WithMessage("Category not found");

    }

    [Fact]
    public async Task GetById_ShouldReturnCategory()
    {
        _uowMock.Setup(uow => uow.CategoryRepository.GetByIdAsync(_category.Id)).ReturnsAsync(_category);
        
        CategoryModel? model = await _service.GetById(_category.Id);
        
        model.Should().NotBeNull();
        model.Should().BeEquivalentTo(_category);
        
        _uowMock.Verify(x => x.CategoryRepository.GetByIdAsync(_category.Id), Times.Once);
    }
    
    [Fact]
    public async Task GetById_ShouldNull()
    {
        _uowMock.Setup(uow => uow.CategoryRepository.GetByIdAsync(_category.Id)).ReturnsAsync((CategoryModel?)null);
        
        CategoryModel? model = await _service.GetById(_category.Id);
        
        model.Should().BeNull();
        
        _uowMock.Verify(x => x.CategoryRepository.GetByIdAsync(_category.Id), Times.Once);
    }

    [Fact]
    public async Task Delete_ShouldDeleteCategory()
    {
        _uowMock.Setup(x => x.CategoryRepository.DeleteAsync(_category))
            .Returns(Task.CompletedTask);

        _uowMock.Setup(x => x.CommitAsync());
    
        await _service.Delete(_category);
    
        _uowMock.Verify(x => x.CategoryRepository.DeleteAsync(_category), Times.Once);
        _uowMock.Verify(x => x.CommitAsync(), Times.Once);
    }
    
    [Fact]
    public async Task Delete_ShouldExecuteInCorrectOrder()
    {
        var sequence = new MockSequence();

        _uowMock
            .InSequence(sequence)
            .Setup(x => x.CategoryRepository.DeleteAsync(_category))
            .Returns(Task.CompletedTask);

        _uowMock
            .InSequence(sequence)
            .Setup(x => x.CommitAsync());

        // Act
        await _service.Delete(_category);

        // Assert
        // Se a ordem for invertida no Service, o Moq lançará uma exceção aqui
        _uowMock.Verify(x => x.CategoryRepository.DeleteAsync(_category), Times.Once);
        _uowMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task ExistsById_ShouldReturnTrue()
    {
        _uowMock.Setup(x => x.CategoryRepository.ExistsById(_category.Id)).ReturnsAsync(true);

        bool exists = await _service.ExistsById(_category.Id);
        
        exists.Should().BeTrue();
        
        _uowMock.Verify(x => x.CategoryRepository.ExistsById(_category.Id), Times.Once);
    }
    
    [Fact]
    public async Task ExistsById_ShouldReturnFalse()
    {
        _uowMock.Setup(x => x.CategoryRepository.ExistsById(_category.Id)).ReturnsAsync(false);

        bool exists = await _service.ExistsById(_category.Id);
        
        exists.Should().BeFalse();
        
        _uowMock.Verify(x => x.CategoryRepository.ExistsById(_category.Id), Times.Once);
    }

    [Fact]
    public async Task GetByName_ShouldReturnCategory()
    {
        _uowMock.Setup(x => x.CategoryRepository.FindByName(_category.Name)).ReturnsAsync(_category);
        CategoryModel? model = await _service.GetByName(_category.Name);
        
        model.Should().NotBeNull();
        model.Should().BeEquivalentTo(_category);
        
        _uowMock.Verify(x => x.CategoryRepository.FindByName(_category.Name), Times.Once);
    }
    
    [Fact]
    public async Task GetByName_ShouldReturnNull()
    {
        _uowMock.Setup(x => x.CategoryRepository.FindByName(_category.Name)).ReturnsAsync((CategoryModel?)null);
        CategoryModel? model = await _service.GetByName(_category.Name);
        
        model.Should().BeNull();
        
        _uowMock.Verify(x => x.CategoryRepository.FindByName(_category.Name), Times.Once);
    }
    
    [Fact]
    public async Task ExistsByName_ShouldReturnTrue()
    {
        _uowMock.Setup(x => x.CategoryRepository.ExistsByName(_category.Name)).ReturnsAsync(true);

        bool exists = await _service.ExistsByName(_category.Name);
        
        exists.Should().BeTrue();
        
        _uowMock.Verify(x => x.CategoryRepository.ExistsByName(_category.Name), Times.Once);
    }
    
    [Fact]
    public async Task ExistsByName_ShouldReturnFalse()
    {
        _uowMock.Setup(x => x.CategoryRepository.ExistsByName(_category.Name)).ReturnsAsync(false);

        bool exists = await _service.ExistsByName(_category.Name);
        
        exists.Should().BeFalse();
        
        _uowMock.Verify(x => x.CategoryRepository.ExistsByName(_category.Name), Times.Once);
    }
    
    [Fact]
    public async Task ExistsBySlug_ShouldReturnTrue()
    {
        _uowMock.Setup(x => x.CategoryRepository.ExistsBySlug(_category.Slug)).ReturnsAsync(true);

        bool exists = await _service.ExistsBySlug(_category.Slug);
        
        exists.Should().BeTrue();
        
        _uowMock.Verify(x => x.CategoryRepository.ExistsBySlug(_category.Slug), Times.Once);
    }
    
    [Fact]
    public async Task ExistsBySlug_ShouldReturnFalse()
    {
        _uowMock.Setup(x => x.CategoryRepository.ExistsBySlug(_category.Slug)).ReturnsAsync(false);

        bool exists = await _service.ExistsBySlug(_category.Slug);
        
        exists.Should().BeFalse();
        
        _uowMock.Verify(x => x.CategoryRepository.ExistsBySlug(_category.Slug), Times.Once);
    }

    [Fact]
    public async Task Create_ShouldReturnCreatedCategory()
    {
        var sequence = new MockSequence();
        
        CreateCategoryDto dto = new CreateCategoryDto()
        {
            Name = _category.Name,
            Slug = _category.Slug,
            Description = _category.Description,
            IconName = _category.IconName,
            Color = _category.Color,
            IsActive = _category.IsActive,
            IsVisible = _category.IsVisible,
            DisplayOrder = _category.DisplayOrder,
            ParentId = _category.ParentId,
        };
        
        _mapperMock
            .InSequence(sequence)
            .Setup(x => x.Map<CategoryModel>(dto))
            .Returns(_category);
        _uowMock
            .InSequence(sequence)
            .Setup(x => x.CategoryRepository.AddAsync(It.IsAny<CategoryModel>()))
            .ReturnsAsync(_category);
        
        _uowMock
            .InSequence(sequence)
            .Setup(x => x.CommitAsync());

        CategoryModel model = await _service.Create(dto);
        
        model.Should().NotBeNull();
        model.Should().BeEquivalentTo(_category);
        
        _uowMock.Verify(x => x.CategoryRepository.AddAsync(It.IsAny<CategoryModel>()), Times.Once);
        _mapperMock.Verify(x => x.Map<CategoryModel>(dto), Times.Once);
        _uowMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateFieldName_ShouldThrowConflictValueException()
    {
        _uowMock.Setup(x => x.CategoryRepository.ExistsByName(_dto.Name!)).ReturnsAsync(true);

        Func<Task> func = async () => await _service.Update(_dto, _category);
        
        await func.Should().ThrowAsync<ConflictValueException>();
    }
    
    [Fact]
    public async Task UpdateFieldSlug_ShouldThrowConflictValueException()
    {
        _uowMock.Setup(x => x.CategoryRepository.ExistsBySlug(_dto.Slug!)).ReturnsAsync(true);

        Func<Task> func = async () => await _service.Update(_dto, _category);
        
        await func.Should().ThrowAsync<ConflictValueException>();
    }
    
    [Fact]
    public async Task Update_ShouldThrowSelfReferencingHierarchyException()
    {
        UpdateCategoryDto dto = new UpdateCategoryDto()
        {
            ParentId = _category.Id,
        };
        
        Func<Task> func = async () => await _service.Update(dto, _category);
        
        await func.Should().ThrowAsync<SelfReferencingHierarchyException>();
    }
    
    [Fact]
    public async Task Update_ShouldThrowModelNotFoundException()
    {
        _uowMock.Setup(x => x.CategoryRepository.ExistsById(_dto.ParentId!)).ReturnsAsync(false);
        
        Func<Task> func = async () => await _service.Update(_dto, _category);
        
        await func.Should().ThrowAsync<ModelNotFoundException>();
    }
    
}