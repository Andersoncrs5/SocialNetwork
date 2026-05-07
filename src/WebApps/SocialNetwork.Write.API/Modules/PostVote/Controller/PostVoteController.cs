using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Write.API.Configs.Exception.classes;
using SocialNetwork.Write.API.Modules.PostVote.Dto;
using SocialNetwork.Write.API.Modules.PostVote.Model;
using SocialNetwork.Write.API.Modules.PostVote.Service.Interface;
using SocialNetwork.Write.API.Utils.Classes;
using SocialNetwork.Write.API.Utils.result;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using SocialNetwork.Contracts.DTOs.CommentReaction;
using SocialNetwork.Contracts.DTOs.PostTag;
using SocialNetwork.Contracts.Utils.Res.http;
using SocialNetwork.Write.API.Models.Enums.Post;

namespace SocialNetwork.Write.API.Modules.PostVote.Controller;

[ApiController]
[Route("api/v{version:apiVersion}/post-vote")]
[ApiVersion("1.0")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class PostVoteController(
    IPostVoteService service
    ): Microsoft.AspNetCore.Mvc.Controller
{
    
    [HttpPost("toggle")]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.Forbidden)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseHttp<IDictionary<string, string[]>>), (int)HttpStatusCode.BadRequest)]
    [Authorize(Roles = "USER_ROLE")]
    public async Task<IActionResult> Toggle(TogglePostVoteDto dto)
    {
        string userId = User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value ?? throw new UnauthenticatedException();
        
        Result<ResultToggle<PostVoteModel>> result = await service.Toggle(dto, userId);

        if (!result.Success) 
        {
            return StatusCode(result.Status, new ResponseHttp<object>(
                Data: null,
                Message: result.Message,
                TraceId: HttpContext.TraceIdentifier,
                Success: false,
                Timestamp: DateTime.UtcNow,
                Errors: result.Errors
            ));
        }

        ResultToggle<PostVoteModel> toggle = result.Value!;
        
        string message = "";
        int status = StatusCodes.Status200OK;
        bool check = toggle.Action == ToggleStatus.Added || toggle.Action == ToggleStatus.Update;
            
        if (toggle.Action == ToggleStatus.Added) status = StatusCodes.Status201Created;
        
        switch (toggle.Action)
        {
            case ToggleStatus.Added:
                message = "Vote added to post with successfully";
                break;
            case ToggleStatus.Update:
                message = "Vote updated to post with successfully";
                break;
            case ToggleStatus.Removed:
                message = "Vote removed to post with successfully";
                break;
        }
        
        return StatusCode(status, new ResponseHttp<object>(
            Data: null,
            Message: message,
            TraceId: HttpContext.TraceIdentifier,
            Success: true,
            Timestamp: DateTime.UtcNow
        ));
        
    }
    
}