using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Contracts.DTOs.PostTag;
using SocialNetwork.Contracts.Utils.Exceptions;
using SocialNetwork.Contracts.Utils.Res.http;
using SocialNetwork.Write.API.Configs.Exception.classes;
using SocialNetwork.Write.API.Modules.Post.Model;
using SocialNetwork.Write.API.Modules.Post.Service.Interface;
using SocialNetwork.Write.API.Modules.PostTag.Dto;
using SocialNetwork.Write.API.Modules.PostTag.Model;
using SocialNetwork.Write.API.Modules.PostTag.Service.Interface;
using SocialNetwork.Write.API.Modules.Tag.Service.Interface;
using SocialNetwork.Write.API.Modules.User.Service.Interface;
using SocialNetwork.Write.API.Services.Interfaces;

namespace SocialNetwork.Write.API.Modules.PostTag.Controller;

[ApiController]
[Route("api/v{version:apiVersion}/post-tag")]
[ApiVersion("1.0")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class PostTagController(
    IPostService postService,
    ITagService tagService,
    IUserService userService,
    IPostTagService service,
    IMapper mapper
    ): Microsoft.AspNetCore.Mvc.Controller
{
    [HttpPost]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.Forbidden)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseHttp<PostTagDto>), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ResponseHttp<IDictionary<string, string[]>>), (int)HttpStatusCode.BadRequest)]
    [Authorize(Roles = "USER_ROLE")]
    public async Task<IActionResult> Create(CreatePostTagDto dto)
    {
        string userId = User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value ?? throw new UnauthenticatedException();
        
        PostModel post = await postService.GetByIdSimpleAsync(dto.PostId);
        await tagService.GetByIdSimpleAsync(dto.TagId);
        
        if (post.UserId != userId)
            throw new ResourceOwnerMismatchException("You don't have permission to add tag this post");
        
        int result = await service.CountByPostIdAndTagId(dto.PostId, dto.TagId);

        if (result >= 10) 
        {
            return StatusCode(StatusCodes.Status400BadRequest, new ResponseHttp<object>(
                Data: null,
                Message:"Limit of 10 by post",
                TraceId: HttpContext.TraceIdentifier,
                Success: false,
                Timestamp: DateTime.UtcNow
            ));
        }
        
        PostTagModel tagAdded = await service.Create(dto);
        
        return StatusCode(StatusCodes.Status201Created, new ResponseHttp<PostTagDto>(
            Data: mapper.Map<PostTagDto>(tagAdded),
            Message:"Tag added successfully",
            TraceId: HttpContext.TraceIdentifier,
            Success: true,
            Timestamp: DateTime.UtcNow
        ));
    }

    [HttpDelete("{id:required}")]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.Forbidden)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.NotFound)]
    [Authorize(Roles = "USER_ROLE")]
    public async Task<IActionResult> Delete([IsId] string id)
    {
        string userId = User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value ?? throw new UnauthenticatedException();

        PostTagModel model = await service.GetByIdAsync(id);
        
        PostModel post = await postService.GetByIdSimpleAsync(model.PostId);
        
        if (post.UserId != userId)
            throw new ResourceOwnerMismatchException("You don't have permission to add tag this post");

        await service.Delete(model);
        
        return StatusCode(StatusCodes.Status200OK, new ResponseHttp<PostTagDto>(
            Data: null,
            Message:"Tag removed successfully",
            TraceId: HttpContext.TraceIdentifier,
            Success: true,
            Timestamp: DateTime.UtcNow
        ));
    }
    
    
}