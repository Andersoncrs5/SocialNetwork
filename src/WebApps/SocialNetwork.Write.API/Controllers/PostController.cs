using System.Net;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Contracts.Utils.Res.http;
using SocialNetwork.Write.API.dto.Posts;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Services.Interfaces;

namespace SocialNetwork.Write.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/post")]
[ApiVersion("1.0")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class PostController(
    IMapper mapper,
    IPostService service
    ) : Controller
{
    [HttpPost]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.Forbidden)]
    [ProducesResponseType(typeof(ResponseHttp<PostDto>), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ResponseHttp<IDictionary<string, string[]>>), (int)HttpStatusCode.BadRequest)]
    [Authorize(Roles = "USER_ROLE")]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostDto dto)
    {
        PostModel postCreated = await service.CreateAsync(dto);
        
        return StatusCode(StatusCodes.Status201Created, new ResponseHttp<PostDto>(
            Data: mapper.Map<PostDto>(postCreated),
            Message:"Post created successfully",
            TraceId: HttpContext.TraceIdentifier,
            Success: true,
            Timestamp: DateTime.UtcNow
        ));
    }
    
    
        
}