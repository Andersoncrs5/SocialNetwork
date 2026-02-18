using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Contracts.DTOs.Comment;
using SocialNetwork.Contracts.Utils.Exceptions;
using SocialNetwork.Contracts.Utils.Res.http;
using SocialNetwork.Write.API.Configs.Exception.classes;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Modules.Comment.Dto;
using SocialNetwork.Write.API.Modules.Comment.Model;
using SocialNetwork.Write.API.Modules.Comment.Service.Interface;
using SocialNetwork.Write.API.Modules.Post.Service.Interface;
using SocialNetwork.Write.API.Services.Interfaces;

namespace SocialNetwork.Write.API.Modules.Comment.Controller;

[ApiController]
[Route("api/v{version:apiVersion}/comment")]
[ApiVersion("1.0")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class CommentController(
    IMapper mapper,
    IUserService userService,
    IPostService postService,
    ICommentService service
): Microsoft.AspNetCore.Mvc.Controller
{
    [HttpPost]
    [ProducesResponseType(typeof(ResponseHttp<CommentDto>),                       (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ResponseHttp<object>),                        (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseHttp<IDictionary<string, string[]>>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Create(CreateCommentDto dto)
    {
        string userId = User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value ?? throw new UnauthenticatedException();
        UserModel user = await userService.GetUserBySidSimple(userId);

        CommentModel commentAdded = await service.CreateAsync(dto, user);
        
        return StatusCode(StatusCodes.Status201Created, new ResponseHttp<CommentDto>(
            Data: mapper.Map<CommentDto>(commentAdded),
            Message:"Comment added successfully",
            TraceId: HttpContext.TraceIdentifier,
            Success: true,
            Timestamp: DateTime.UtcNow
        ));
    }

    [HttpDelete("{commentId:required}")]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.Forbidden)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Delete([IsId] string commentId)
    {
        string userId = User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value ?? throw new UnauthenticatedException();

        CommentModel comment = await service.GetByIdSimpleAsync(commentId);
        
        if (comment.UserId != userId) 
            throw new ResourceOwnerMismatchException("You don't have permission to delete this comment");
        
        await service.DeleteAsync(comment);
        
        return StatusCode(StatusCodes.Status200OK, new ResponseHttp<object>(
            Data: null,
            Message:"Comment deleted successfully",
            TraceId: HttpContext.TraceIdentifier,
            Success: true,
            Timestamp: DateTime.UtcNow
        ));
    }

    [HttpPatch("{commentId:required}")]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.Forbidden)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseHttp<IDictionary<string, string[]>>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Update([IsId] string commentId, UpdateCommentDto dto)
    {
        string userId = User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value ?? throw new UnauthenticatedException();

        CommentModel comment = await service.GetByIdSimpleAsync(commentId);
        
        if (comment.UserId != userId) 
            throw new ResourceOwnerMismatchException("You don't have permission to delete this comment");

        CommentModel commentUpdated = await service.UpdateAsync(dto, comment);

        return StatusCode(StatusCodes.Status200OK, new ResponseHttp<object>(
            Data: mapper.Map<CommentDto>(commentUpdated),
            Message:"Comment updated successfully",
            TraceId: HttpContext.TraceIdentifier,
            Success: true,
            Timestamp: DateTime.UtcNow
        ));
    }
    
}