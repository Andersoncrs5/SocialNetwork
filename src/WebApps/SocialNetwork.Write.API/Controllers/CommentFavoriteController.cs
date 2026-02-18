using System.Net;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Contracts.Utils.Res.http;
using SocialNetwork.Write.API.Configs.Exception.classes;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using SocialNetwork.Contracts.DTOs.CommentFavorite;
using SocialNetwork.Contracts.DTOs.PostFavorite;
using SocialNetwork.Write.API.Models.Enums.Post;
using SocialNetwork.Write.API.Modules.Comment.Service.Interface;
using SocialNetwork.Write.API.Utils.Classes;

namespace SocialNetwork.Write.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/comment-favorite")]
[ApiVersion("1.0")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class CommentFavoriteController(
    IMapper mapper,
    ICommentFavoriteService service,
    ICommentService commentService,
    IUserService userService
    ): Controller
{
    [HttpPost("{commentId:required}/toggle")]
    [ProducesResponseType(typeof(ResponseHttp<CommentFavoriteDto>), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.BadRequest)]
    [Authorize(Roles = "USER_ROLE")]
    public async Task<IActionResult> Toggle([IsId] string commentId)
    {
        string userId = User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value ?? throw new UnauthenticatedException();
        await userService.GetUserBySidSimple(userId);
        await commentService.GetByIdSimpleAsync(commentId);

        ResultToggle<CommentFavoriteModel?> toggle = await service.ToggleAsync(commentId, userId);
        
        string message = toggle.Action == AddedORRemoved.Added
            ? "Comment added with favorite successfully"
            : "Comment removed with favorite successfully";
    
        int status = toggle.Action == AddedORRemoved.Added
            ? StatusCodes.Status201Created
            : StatusCodes.Status200OK;
    
        CommentFavoriteDto? data = toggle.Action == AddedORRemoved.Added
            ? mapper.Map<CommentFavoriteDto>(toggle.Value)
            : null;
        
        return StatusCode(status, new ResponseHttp<CommentFavoriteDto>(
            Data: data,
            Message: message,
            TraceId: HttpContext.TraceIdentifier,
            Success: true,
            Timestamp: DateTime.UtcNow
        ));
    }
    
}