using System.Net;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Contracts.DTOs.User;
using SocialNetwork.Contracts.Utils.Res.http;
using SocialNetwork.Write.API.dto.Category;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Services.Interfaces;

namespace SocialNetwork.Write.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/user")]
[ApiVersion("1.0")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class CategoryController(
    IMapper mapper,
    ICategoryService service
    ) : Controller
{
    [HttpPost]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ResponseHttp<UserDto>), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ResponseHttp<IEnumerable<string>>), (int)HttpStatusCode.OK)]
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
    
    
}