using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Contracts.DTOs.CommentReaction;
using SocialNetwork.Contracts.Utils.Res.http;
using SocialNetwork.Write.API.Configs.Exception.classes;
using SocialNetwork.Write.API.Models.Enums.Post;
using SocialNetwork.Write.API.Modules.CommentReactions.Dto;
using SocialNetwork.Write.API.Modules.CommentReactions.Model;
using SocialNetwork.Write.API.Modules.CommentReactions.Service.Interface;
using SocialNetwork.Write.API.Utils.Classes;

namespace SocialNetwork.Write.API.Modules.CommentReactions.Controller;

[ApiController]
[Route("api/v{version:apiVersion}/comment-reaction")]
[ApiVersion("1.0")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class CommentReactionController(
    IMapper mapper,
    ICommentReactionService service
    ) : Microsoft.AspNetCore.Mvc.Controller
{

    [HttpPost("toggle")]
    [ProducesResponseType(typeof(ResponseHttp<CommentReactionDto>), (int) HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int) HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int) HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseHttp<CommentReactionDto?>), (int) HttpStatusCode.OK)]
    [Authorize(Roles = "USER_ROLE")]
    public async Task<IActionResult> Toggle(ToggleCommentReactionDto dto)
    {
        string userId = User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value ?? throw new UnauthenticatedException();
        ResultToggle<CommentReactionModel?> toggle = await service.ToggleAsync(dto, userId);

        string message = "";
        int status = StatusCodes.Status200OK;
        bool check = toggle.Action == ToggleStatus.Added || toggle.Action == ToggleStatus.Update;
            
        if (toggle.Action == ToggleStatus.Added) status = StatusCodes.Status201Created;

        CommentReactionDto? data = check
            ? mapper.Map<CommentReactionDto>(toggle.Value)
            : null;
        
        switch (toggle.Action)
        {
            case ToggleStatus.Added:
                message = "Reaction added to comment with successfully";
                break;
            case ToggleStatus.Update:
                message = "Reaction updated to comment with successfully";
                break;
            case ToggleStatus.Removed:
                message = "Reaction removed to comment with successfully";
                break;
        }
        
        return StatusCode(status, new ResponseHttp<CommentReactionDto>(
            Data: data,
            Message: message,
            TraceId: HttpContext.TraceIdentifier,
            Success: true,
            Timestamp: DateTime.UtcNow
        ));
        
    }
    
   
}