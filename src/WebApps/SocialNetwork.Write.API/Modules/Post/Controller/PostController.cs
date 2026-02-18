using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Contracts.DTOs.Post;
using SocialNetwork.Contracts.Utils.Exceptions;
using SocialNetwork.Contracts.Utils.Res.http;
using SocialNetwork.Write.API.Configs.Exception.classes;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Modules.Post.Dto;
using SocialNetwork.Write.API.Modules.Post.Model;
using SocialNetwork.Write.API.Modules.Post.Service.Interface;
using SocialNetwork.Write.API.Modules.User.Model;
using SocialNetwork.Write.API.Modules.User.Service.Interface;
using SocialNetwork.Write.API.Services.Interfaces;

namespace SocialNetwork.Write.API.Modules.Post.Controller;

[ApiController]
[Route("api/v{version:apiVersion}/post")]
[ApiVersion("1.0")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class PostController(
    IMapper mapper,
    IPostService service,
    IUserService userService
    ) : Microsoft.AspNetCore.Mvc.Controller
{
    [HttpPost]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.Forbidden)]
    [ProducesResponseType(typeof(ResponseHttp<PostDto>), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ResponseHttp<IDictionary<string, string[]>>), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.BadRequest)]
    [Authorize(Roles = "USER_ROLE")]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostDto dto)
    {
        string userId = User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value ?? throw new UnauthenticatedException();
        UserModel user = await userService.GetUserBySidSimple(userId);
        
        PostModel postCreated = await service.CreateAsync(dto, user);
        
        return StatusCode(StatusCodes.Status201Created, new ResponseHttp<PostDto>(
            Data: mapper.Map<PostDto>(postCreated),
            Message:"Post created successfully",
            TraceId: HttpContext.TraceIdentifier,
            Success: true,
            Timestamp: DateTime.UtcNow
        ));
    }
    
    [HttpDelete("{id:required}")]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.Forbidden)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.BadRequest)]
    [Authorize(Roles = "USER_ROLE")]
    public async Task<IActionResult> DeletePost([IsId] string id)
    {
        string userId = User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value ?? throw new UnauthenticatedException();
        
        PostModel post = await service.GetByIdSimpleAsync(id);

        if (post.UserId != userId)
            throw new ResourceOwnerMismatchException("You don't have permission to delete this post");
        
        await service.DeleteAsync(post);
        
        return StatusCode(StatusCodes.Status200OK, new ResponseHttp<PostDto>(
            Data: null,
            Message:"Post deleted successfully",
            TraceId: HttpContext.TraceIdentifier,
            Success: true,
            Timestamp: DateTime.UtcNow
        ));
    }

    [HttpPatch("{id:required}")]
    [ProducesResponseType(typeof(ResponseHttp<PostDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseHttp<object>),  (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseHttp<object>),  (int)HttpStatusCode.Forbidden)]
    [ProducesResponseType(typeof(ResponseHttp<object>),  (int)HttpStatusCode.BadRequest)]
    [Authorize(Roles = "USER_ROLE")]
    public async Task<IActionResult> PatchPost([IsId] string id, [FromBody] UpdatePostDto dto)
    {
        string userId = User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value ?? throw new UnauthenticatedException();
        
        PostModel post = await service.GetByIdSimpleAsync(id);

        if (post.UserId != userId)
            throw new ResourceOwnerMismatchException("You don't have permission to delete this post");

        PostModel postUpdate = await service.UpdateAsync(post, dto);
        
        return StatusCode(StatusCodes.Status200OK, new ResponseHttp<PostDto>(
            Data: mapper.Map<PostDto>(postUpdate),
            Message:"Post updated successfully",
            TraceId: HttpContext.TraceIdentifier,
            Success: true,
            Timestamp: DateTime.UtcNow
        ));
    }
    
    
}