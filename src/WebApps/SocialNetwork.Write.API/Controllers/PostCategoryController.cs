using System.Net;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Contracts.DTOs.PostCategory;
using SocialNetwork.Contracts.Utils.Exceptions;
using SocialNetwork.Contracts.Utils.Res.http;
using SocialNetwork.Write.API.Configs.Exception.classes;
using SocialNetwork.Write.API.dto.PostCategory;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Modules.Category.Service.Interface;
using SocialNetwork.Write.API.Modules.Post.Model;
using SocialNetwork.Write.API.Modules.Post.Service.Interface;

namespace SocialNetwork.Write.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/post-category")]
[ApiVersion("1.0")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class PostCategoryController(
    IMapper mapper,
    IUserService userService,
    IPostService postService,
    IPostCategoryService service,
    ICategoryService categoryService
) : Controller
{
    [HttpPost]
    [ProducesResponseType(typeof(ResponseHttp<PostCategoryDto>),                  (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ResponseHttp<object>),                          (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseHttp<object>),                         (int)HttpStatusCode.Forbidden)]
    [ProducesResponseType(typeof(ResponseHttp<object>),                        (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseHttp<IDictionary<string, string[]>>), (int)HttpStatusCode.BadRequest)]
    [Authorize(Roles = "USER_ROLE")]
    public async Task<IActionResult> Create([FromBody] CreatePostCategoryDto dto)
    {
        string userId = User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value ?? throw new UnauthenticatedException();
        
        await categoryService.GetByIdSimple(dto.CategoryId);
        PostModel post = await postService.GetByIdSimpleAsync(dto.PostId);

        if (post.UserId != userId)
            throw new ResourceOwnerMismatchException("You don't have permission to add category this post");
        
        int result = await service.CountByPostIdAndCategoryId(dto.PostId, dto.CategoryId);

        if (result >= 10)
        {
            return StatusCode(StatusCodes.Status400BadRequest, new ResponseHttp<PostCategoryDto>(
                Data: null,
                Message:"Limit of 10 by post",
                TraceId: HttpContext.TraceIdentifier,
                Success: false,
                Timestamp: DateTime.UtcNow
            ));
        }
        
        PostCategoryModel categoryAdded = await service.Create(dto);
        
        return StatusCode(StatusCodes.Status201Created, new ResponseHttp<PostCategoryDto>(
            Data: mapper.Map<PostCategoryDto>(categoryAdded),
            Message:"Category added successfully",
            TraceId: HttpContext.TraceIdentifier,
            Success: true,
            Timestamp: DateTime.UtcNow
        ));
    }

    [HttpDelete("{id:required}")]
    [ProducesResponseType(typeof(ResponseHttp<object>),        (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseHttp<object>),  (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.Forbidden)]
    [ProducesResponseType(typeof(ResponseHttp<object>),(int)HttpStatusCode.BadRequest)]
    [Authorize(Roles = "USER_ROLE")]
    public async Task<IActionResult> Delete([IsId] string id)
    {
        string userId = User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value ?? throw new UnauthenticatedException();
        
        PostCategoryModel model = await service.GetByIdSimple(id);
        PostModel post = await postService.GetByIdSimpleAsync(model.PostId);

        if (post.UserId != userId)
            throw new ResourceOwnerMismatchException("You don't have permission to add category this post");

        await service.Delete(model);
        
        return StatusCode(StatusCodes.Status200OK, new ResponseHttp<object>(
            Data: null,
            Message:"Category removed successfully",
            TraceId: HttpContext.TraceIdentifier,
            Success: true,
            Timestamp: DateTime.UtcNow
        ));
    }

    [HttpPatch("{id:required}")]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.Forbidden)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseHttp<IDictionary<string, string[]>>), (int)HttpStatusCode.BadRequest)]
    [Authorize(Roles = "USER_ROLE")]
    public async Task<IActionResult> Update([IsId] string id, [FromBody] UpdatePostCategoryDto dto)
    {
        string userId = User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value ?? throw new UnauthenticatedException();
        
        PostCategoryModel model = await service.GetByIdSimple(id);
        PostModel post = await postService.GetByIdSimpleAsync(model.PostId);

        if (post.UserId != userId)
            throw new ResourceOwnerMismatchException("You don't have permission to add category this post");

        PostCategoryModel postCategoryUpdated = await service.Update(dto, model);

        return StatusCode(StatusCodes.Status200OK, new ResponseHttp<PostCategoryDto>(
            Data: mapper.Map<PostCategoryDto>(postCategoryUpdated),
            Message:"Resource updated successfully",
            TraceId: HttpContext.TraceIdentifier,
            Success: true,
            Timestamp: DateTime.UtcNow
        ));
    }
    
}