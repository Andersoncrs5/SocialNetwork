using System.Net;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Contracts.Utils.Res.http;
using SocialNetwork.Write.API.dto.Reaction;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Services.Interfaces;

namespace SocialNetwork.Write.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/reaction")]
[ApiVersion("1.0")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ReactionController(
    IMapper mapper,
    IReactionService service
): Controller
{
    [HttpPost]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.Forbidden)]
    [ProducesResponseType(typeof(ResponseHttp<ReactionDto>), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ResponseHttp<IDictionary<string, string[]>>), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.BadRequest)]
    [Authorize(Roles = "MASTER_ROLE, SUPER_ADM_ROLE")]
    public async Task<IActionResult> Create([FromBody] CreateReactionDto dto)
    {
        ReactionModel reactionModel = await service.CreateAsync(dto);
        
        return StatusCode(StatusCodes.Status201Created, new ResponseHttp<ReactionDto>(
            Data: mapper.Map<ReactionDto>(reactionModel),
            Message:"Reaction created successfully",
            TraceId: HttpContext.TraceIdentifier,
            Success: true,
            Timestamp: DateTime.UtcNow
        ));
    }

    [HttpDelete("{id:required}")]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.Forbidden)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.NotFound)]
    [Authorize(Roles = "MASTER_ROLE, SUPER_ADM_ROLE")]
    public async Task<IActionResult> Delete([IsId] string id)
    {
        ReactionModel reaction = await service.GetByIdAsync(id);
        
        await service.DeleteAsync(reaction);
        
        return StatusCode(StatusCodes.Status200OK, new ResponseHttp<object>(
            Data: null,
            Message:"Reaction deleted successfully",
            TraceId: HttpContext.TraceIdentifier,
            Success: true,
            Timestamp: DateTime.UtcNow
        ));
    }
    
    [HttpPatch("{id:required}")]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.Forbidden)]
    [ProducesResponseType(typeof(ResponseHttp<ReactionDto>), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ResponseHttp<IDictionary<string, string[]>>), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.NotFound)]
    [Authorize(Roles = "MASTER_ROLE, SUPER_ADM_ROLE")]
    public async Task<IActionResult> Patch([IsId] string id, [FromBody] UpdateReactionDto dto)
    {
        ReactionModel reaction = await service.GetByIdAsync(id);
        
        ReactionModel reactionModel = await service.UpdateAsync(dto, reaction);
        
        return StatusCode(StatusCodes.Status200OK, new ResponseHttp<ReactionDto>(
            Data: mapper.Map<ReactionDto>(reactionModel),
            Message:"Reaction updated successfully",
            TraceId: HttpContext.TraceIdentifier,
            Success: true,
            Timestamp: DateTime.UtcNow
        ));
    }
    
}