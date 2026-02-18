using System.Net;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Contracts.DTOs.User;
using SocialNetwork.Contracts.Utils.Res.http;
using SocialNetwork.Write.API.Modules.Category.Dto;
using SocialNetwork.Write.API.Modules.Category.Model;
using SocialNetwork.Write.API.Modules.Category.Service.Interface;

namespace SocialNetwork.Write.API.Modules.Category.Controller;

[ApiController]
[Route("api/v{version:apiVersion}/category")]
[ApiVersion("1.0")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class CategoryController(
    IMapper mapper,
    ICategoryService service
    ) : Microsoft.AspNetCore.Mvc.Controller
{
    [HttpPost]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.Forbidden)]
    [ProducesResponseType(typeof(ResponseHttp<CategoryDto>), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ResponseHttp<IDictionary<string, string[]>>), (int)HttpStatusCode.BadRequest)]
    [Authorize(Roles = "MASTER_ROLE, SUPER_ADM_ROLE")]
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
    { 
        CategoryModel model = await service.Create(dto);
        
        return StatusCode(StatusCodes.Status201Created, new ResponseHttp<CategoryDto>(
            Data: mapper.Map<CategoryDto>(model),
            Message:"Category created successfully",
            TraceId: HttpContext.TraceIdentifier,
            Success: true,
            Timestamp: DateTime.UtcNow
        ));
    }

    [HttpDelete("{id:required}")]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.Forbidden)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.NotFound)]
    [Authorize(Roles = "MASTER_ROLE, SUPER_ADM_ROLE")]
    public async Task<IActionResult> Delete([IsId] string id)
    {
        CategoryModel category = await service.GetByIdSimple(id);
        await service.Delete(category);
        
        return StatusCode(StatusCodes.Status200OK, new ResponseHttp<object>(
            Data: null,
            Message:"Category deleted successfully",
            TraceId: HttpContext.TraceIdentifier,
            Success: true,
            Timestamp: DateTime.UtcNow
        ));
    }

    [HttpPatch("{id:required}")]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.Forbidden)]
    [ProducesResponseType(typeof(ResponseHttp<CategoryDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseHttp<IDictionary<string, string[]>>), (int)HttpStatusCode.BadRequest)]
    [Authorize(Roles = "MASTER_ROLE, SUPER_ADM_ROLE")]
    public async Task<IActionResult> Patch([IsId] string id, [FromBody] UpdateCategoryDto dto)
    {
        CategoryModel category = await service.GetByIdSimple(id);

        CategoryModel model = await service.Update(dto, category);

        return StatusCode(StatusCodes.Status200OK, new ResponseHttp<CategoryDto>(
            Data: mapper.Map<CategoryDto>(model),
            Message:"Category updated successfully",
            TraceId: HttpContext.TraceIdentifier,
            Success: true,
            Timestamp: DateTime.UtcNow
        ));
    }

    
}