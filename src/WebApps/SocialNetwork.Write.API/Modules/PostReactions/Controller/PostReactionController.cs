using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Contracts.DTOs.CommentReaction;
using SocialNetwork.Contracts.Utils.Res.http;
using SocialNetwork.Write.API.Configs.Exception.classes;
using SocialNetwork.Write.API.Modules.CommentReactions.Dto;
using SocialNetwork.Write.API.Modules.CommentReactions.Model;
using SocialNetwork.Write.API.Modules.PostReactions.Service.Interface;
using System.IdentityModel.Tokens.Jwt;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SocialNetwork.Write.API.Modules.PostReactions.Dto;
using SocialNetwork.Write.API.Modules.PostReactions.Model;
using SocialNetwork.Write.API.Utils.Classes;
using SocialNetwork.Write.API.Utils.result;

namespace SocialNetwork.Write.API.Modules.PostReactions.Controller;

[ApiController]
[Route("api/v{version:apiVersion}/post-reaction")]
[ApiVersion("1.0")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class PostReactionController(
    IPostReactionService service
    ) : Microsoft.AspNetCore.Mvc.Controller
{
    
    [HttpPost("toggle")]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int) HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int) HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int) HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseHttp<object?>), (int) HttpStatusCode.OK)]
    [Authorize(Roles = "USER_ROLE")]
    public async Task<IActionResult> Toggle(TogglePostReactionDto dto)
    {
        string userId = User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value ?? throw new UnauthenticatedException();
        Result<ResultToggle<PostReactionModel?>> toggle = await service.ToggleAsync(dto, userId);
        
        return StatusCode(toggle.Status, new ResponseHttp<object>(
            Data: null,
            Message: toggle.Message,
            TraceId: HttpContext.TraceIdentifier,
            Success: true,
            Timestamp: DateTime.UtcNow
        ));
    }
}