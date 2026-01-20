using System.Net;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Contracts.Utils.Res.http;
using SocialNetwork.Write.API.dto.User;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Services.Interfaces;
using SocialNetwork.Write.API.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace SocialNetwork.Write.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/auth")]
[ApiVersion("1.0")]
public class AuthController(
        ITokenService tokenService,
        IUserService userService,
        IMapper mapper
    ) : Controller
{

    [HttpPost("register")]
    [SwaggerOperation(Summary = "Create user", Tags = ["Auth"])]
    [ProducesResponseType(typeof(ResponseHttp<IEnumerable<string>>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseHttp<ResponseTokens>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseHttp<IEnumerable<string>>), (int)HttpStatusCode.BadRequest)] 
    public async Task<IActionResult> RegisterUser([FromBody] CreateUserDto dto)
    {
        UserResult result = await userService.CreateUser(dto);

        if (!result.Succeeded) 
        {
            return BadRequest(new ResponseHttp<IEnumerable<string>>(
                Data: result.Errors,
                Message: "Registration failed",
                TraceId: HttpContext.TraceIdentifier,
                ErrorCode: result.Errors?.Count() ?? 0,
                Success: false,
                Timestamp: DateTime.UtcNow
            ));
        }

        UserModel user = result.User!;
        
        IList<string> roles = await userService.GetUserRoles(user);

        ResponseTokens tokens = tokenService.CreateTokens(user, roles);
        
        user.RefreshToken = tokens.RefreshToken;
        user.RefreshTokenExpiryTime = tokens.ExpiredAtRefreshToken;
        
        await userService.UpdateSimple(user);
        
        return StatusCode(StatusCodes.Status201Created, new ResponseHttp<ResponseTokens>(
            Data: tokens,
            ErrorCode: 0,
            Message: "User created",
            Success: true,
            Timestamp: DateTime.UtcNow,
            TraceId: HttpContext.TraceIdentifier
        ));
    }
    
    [HttpPost("login")]
    [SwaggerOperation(Summary = "Login user", Tags = ["Auth"])]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ResponseHttp<ResponseTokens>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
    {
        var user = await userService.GetUserByEmail(dto.Email);
        var traceId = HttpContext.TraceIdentifier;

         var invalidCredentialsResponse = Unauthorized(new ResponseHttp<object>(
            null, "Invalid credentials", traceId, 1, false, DateTime.UtcNow));

        if (user is null) return invalidCredentialsResponse;

        if (user.LockoutEnd > DateTime.UtcNow)
        {
            return StatusCode(423, new ResponseHttp<object>( 
                null, $"Account locked until {user.LockoutEnd}", traceId, 2, false, DateTime.UtcNow));
        }

        bool isPasswordValid = await userService.CheckPassword(user, dto.Password);

        if (!isPasswordValid)
        {
            user.FailedLoginAttempts += 1;
        
            if (user.FailedLoginAttempts >= 3)
            {
                user.LockoutEnd = DateTime.UtcNow.AddHours(5);
                user.FailedLoginAttempts = 0; 
            }
        
            await userService.UpdateSimple(user);
            return invalidCredentialsResponse;
        }

        user.FailedLoginAttempts = 0;
        user.LockoutEnd = null;
    
        var roles = await userService.GetUserRoles(user);
        var tokens = tokenService.CreateTokens(user, roles);
    
        user.RefreshToken = tokens.RefreshToken;
        user.RefreshTokenExpiryTime = tokens.ExpiredAtRefreshToken;
    
        await userService.UpdateSimple(user);

        return Ok(new ResponseHttp<ResponseTokens>(tokens, "Login success", traceId, 0, true, DateTime.UtcNow));
    }
    
    
    
}