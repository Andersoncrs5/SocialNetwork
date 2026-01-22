using System.Net;
using System.Security.Claims;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using SocialNetwork.Contracts.DTOs.User;
using SocialNetwork.Contracts.Utils.Res.http;
using SocialNetwork.Write.API.Configs.Exception.classes;
using SocialNetwork.Write.API.dto.User;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Services.Interfaces;
using SocialNetwork.Write.API.Utils;

namespace SocialNetwork.Write.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/user")]
[ApiVersion("1.0")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UserController(
    ITokenService tokenService,
    IUserService userService,
    IMapper mapper
    ) : Controller
{

    [HttpDelete]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Delete()
    {
        string? userId = User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value ?? throw new UnauthenticatedException();
        UserModel userBySid = await userService.GetUserBySidSimple(userId);
        
        await userService.DeleteUser(userBySid);

        return Ok(new ResponseHttp<object>(
            Data: null,
            Message: "User deleted",
            TraceId: HttpContext.TraceIdentifier,
            Success: true,
            Timestamp: DateTime.UtcNow
        ));
    }

    [HttpPatch]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseHttp<UserDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Update([FromBody] UpdateUserDto dto)
    {
        string? userId = User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value ?? throw new UnauthenticatedException();
        UserModel user = await userService.GetUserBySidSimple(userId);

        UserResult result = await userService.Update(dto, user);

        UserDto map = mapper.Map<UserDto>(result.User);

        return StatusCode(StatusCodes.Status200OK, new ResponseHttp<UserDto>(
            Data: map,
            "User updated",
            TraceId: HttpContext.TraceIdentifier,
            Success: true,
            Timestamp: DateTime.UtcNow
        ));
    }
    
}