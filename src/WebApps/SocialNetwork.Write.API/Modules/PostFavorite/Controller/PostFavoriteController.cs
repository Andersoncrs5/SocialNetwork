using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Contracts.DTOs.PostFavorite;
using SocialNetwork.Contracts.Utils.Res.http;
using SocialNetwork.Write.API.Configs.Exception.classes;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Modules.Post.Model;
using SocialNetwork.Write.API.Modules.Post.Service.Interface;
using SocialNetwork.Write.API.Modules.PostFavorite.Model;
using SocialNetwork.Write.API.Modules.PostFavorite.Service.Interface;
using SocialNetwork.Write.API.Modules.User.Model;
using SocialNetwork.Write.API.Modules.User.Service.Interface;
using SocialNetwork.Write.API.Services.Interfaces;

namespace SocialNetwork.Write.API.Modules.PostFavorite.Controller;

[ApiController]
[Route("api/v{version:apiVersion}/post-favorite")]
[ApiVersion("1.0")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class PostFavoriteController(
    IMapper mapper,
    IPostService postService,
    IPostFavoriteService service,
    IUserService userService
    ) : Microsoft.AspNetCore.Mvc.Controller
{
    [HttpPost("{postId:required}/toggle")]
    [ProducesResponseType(typeof(ResponseHttp<PostFavoriteDto>), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.BadRequest)]
    [Authorize(Roles = "USER_ROLE")]
    public async Task<IActionResult> Toggle([IsId] string postId)
    {
        string userId = User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value ?? throw new UnauthenticatedException();
        UserModel user = await userService.GetUserBySidSimple(userId);
        PostModel post = await postService.GetByIdSimpleAsync(postId);
    
        PostFavoriteModel? toggle = await service.GetByPostIdAndUserId(postId, userId);
    
        if (toggle is not null)
        {
            await service.DeleteAsync(toggle);
            
            return StatusCode(StatusCodes.Status200OK, new ResponseHttp<object>(
                Data: null,
                Message: "Post removed with favorite successfully",
                TraceId: HttpContext.TraceIdentifier,
                Success: true,
                Timestamp: DateTime.UtcNow
            ));
        }
        
        PostFavoriteModel model = await service.CreateAsync(post, user);
            
        return StatusCode(StatusCodes.Status201Created, new ResponseHttp<PostFavoriteDto>(
            Data: mapper.Map<PostFavoriteDto>(model),
            Message: "Post added with favorite successfully",
            TraceId: HttpContext.TraceIdentifier,
            Success: true,
            Timestamp: DateTime.UtcNow
        ));
    }
    
    /*
    [HttpPost("{postId:required}/toggle")]
    [ProducesResponseType(typeof(ResponseHttp<PostFavoriteDto>), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ResponseHttp<PostFavoriteDto?>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.BadRequest)]
    [Authorize(Roles = "USER_ROLE")]
    public async Task<IActionResult> Toggle([IsId] string postId)
    {
        string userId = User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value ?? throw new UnauthenticatedException();
        UserModel user = await userService.GetUserBySidSimple(userId);
        
        PostModel post = await postService.GetByIdSimpleAsync(postId);
    
        ResultToggle<PostFavoriteModel> toggle = await service.ToggleAsync(post, user);
    
        string message = toggle.Action == AddedORRemoved.Added
            ? "Post added with favorite successfully"
            : "Post removed with favorite successfully";
    
        int status = toggle.Action == AddedORRemoved.Added
            ? StatusCodes.Status201Created
            : StatusCodes.Status200OK;
    
        PostFavoriteDto? data = toggle.Action == AddedORRemoved.Added
            ? mapper.Map<PostFavoriteDto>(toggle.Value)
            : null;
        
        return StatusCode(status, new ResponseHttp<PostFavoriteDto>(
            Data: data,
            Message: message,
            TraceId: HttpContext.TraceIdentifier,
            Success: true,
            Timestamp: DateTime.UtcNow
        ));
    }
    */
    
}