using AutoMapper;
using FluentAssertions;
using Moq;
using SocialNetwork.Write.API.Configs.Exception.classes;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Modules.Role.Model;
using SocialNetwork.Write.API.Modules.Role.Service.Provider;
using SocialNetwork.Write.API.Services.Providers;
using SocialNetwork.Write.API.Utils.UnitOfWork;

namespace SocialNetwork.Tests.Services.Roles;

public class RoleServiceTest
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly RoleService _roleService;

    public RoleServiceTest()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _roleService = new RoleService(_uowMock.Object);
    }

    private readonly RoleModel _roleModel = new()
    {
        Id = Guid.NewGuid().ToString(),
        Name = "TI",
        Description = "anyDesc",
    };

    [Fact]
    public async Task ShouldReturnRoleFromCache_Success()
    {
        _uowMock.Setup(x => x.RedisService.GetAsync<RoleModel>(_roleModel.Name!))
            .ReturnsAsync(_roleModel);

        RoleModel roleModel = await _roleService.GetByNameSimple(_roleModel.Name!);
        
        roleModel.Id.Should().Be(_roleModel.Id);
        
        _uowMock.Verify(x => x.RedisService.GetAsync<RoleModel>(_roleModel.Name!),  Times.Once);
        _uowMock.Verify(x => x.RoleRepository.GetByNameAsync(_roleModel.Name!),  Times.Never);
        _uowMock.Verify(x => x.RedisService.CreateAsync(_roleModel.Name!, It.IsAny<RoleModel>(), null),  Times.Never);
    }
    
    [Fact]
    public async Task ShouldReturnRoleFromDB_Success()
    {
        _uowMock.Setup(x => x.RedisService.GetAsync<RoleModel>(_roleModel.Name!))
            .ReturnsAsync((RoleModel?)null);
        _uowMock.Setup(x => x.RoleRepository.GetByNameAsync(_roleModel.Name!))
            .ReturnsAsync(_roleModel);
        _uowMock.Setup(x => x.RedisService.CreateAsync(_roleModel.Name!, It.IsAny<RoleModel>(), null))
            .ReturnsAsync(true);

        RoleModel roleModel = await _roleService.GetByNameSimple(_roleModel.Name!);
        
        roleModel.Id.Should().Be(_roleModel.Id);
        
        _uowMock.Verify(x => x.RedisService.GetAsync<RoleModel>(_roleModel.Name!),  Times.Once);
        _uowMock.Verify(x => x.RoleRepository.GetByNameAsync(_roleModel.Name!),  Times.Once);
        _uowMock.Verify(x => x.RedisService.CreateAsync(_roleModel.Name!, It.IsAny<RoleModel>(), null),  Times.Once);
    }
    
    [Fact]
    public async Task ShouldThrowModelNotFound_Success()
    {
        _uowMock.Setup(x => x.RedisService.GetAsync<RoleModel>(_roleModel.Name!))
            .ReturnsAsync((RoleModel?)null);
        _uowMock.Setup(x => x.RoleRepository.GetByNameAsync(_roleModel.Name!))
            .ReturnsAsync((RoleModel?)null);
       
        Func<Task> func = async () => await _roleService.GetByNameSimple(_roleModel.Name!);

        await func.Should().ThrowAsync<ModelNotFoundException>()
            .WithMessage($"Role {_roleModel.Name} not found");
        
        _uowMock.Verify(x => x.RedisService.GetAsync<RoleModel>(_roleModel.Name!),  Times.Once);
        _uowMock.Verify(x => x.RoleRepository.GetByNameAsync(_roleModel.Name!),  Times.Once);
        _uowMock.Verify(x => x.RedisService.CreateAsync(_roleModel.Name!, It.IsAny<RoleModel>(), null),  Times.Never);
    }
    
    
    
}