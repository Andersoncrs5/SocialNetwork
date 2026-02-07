using System.Net;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using SocialNetwork.Contracts.DTOs.User;
using SocialNetwork.Contracts.Utils.Exceptions;
using SocialNetwork.Contracts.Utils.Res.http;
using SocialNetwork.Write.API.Configs.Exception.classes;
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
        IRoleService roleService,
        IMapper mapper
    ) : Controller
{

    [HttpPost("register")]
    [SwaggerOperation(Summary = "Create user", Tags = ["Auth"])]
    [ProducesResponseType(typeof(ResponseHttp<ResponseTokens>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseHttp<IDictionary<string, string[]>>), (int)HttpStatusCode.BadRequest)]
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
                Success: false,
                Timestamp: DateTime.UtcNow
            ));
        }

        UserModel userCurrent = result.User!;

        RoleModel role = await roleService.GetByNameSimple("USER_ROLE");
        await userService.AddRole(role, userCurrent);
        
        UserModel user = await userService.GetUserByEmailSimple(userCurrent.Email!);

        IList<string> roles = await userService.GetUserRoles(user);

        ResponseTokens tokens = tokenService.CreateTokens(user, roles);
        
        user.RefreshToken = tokens.RefreshToken;
        user.RefreshTokenExpiryTime = tokens.ExpiredAtRefreshToken;

        UserResult simple = await userService.UpdateSimple(user);

        tokens.User = mapper.Map<UserDto>(simple.User);
        
        return StatusCode(StatusCodes.Status201Created, new ResponseHttp<ResponseTokens>(
            Data: tokens,
            Message: "User created",
            Success: true,
            Timestamp: DateTime.UtcNow,
            TraceId: HttpContext.TraceIdentifier
        ));
    }
    
    [HttpPost("login")]
    [SwaggerOperation(Summary = "Login user", Tags = ["Auth"])]
    [ProducesResponseType(typeof(ResponseHttp<ResponseTokens>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseHttp<IDictionary<string, string[]>>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
    {
        var user = await userService.GetUserByEmail(dto.Email);
        var traceId = HttpContext.TraceIdentifier;

         var invalidCredentialsResponse = Unauthorized(new ResponseHttp<object>(
            null, "Invalid credentials", traceId,  false, DateTime.UtcNow));

        if (user is null) return invalidCredentialsResponse;

        if (user.LockoutEnd > DateTimeOffset.UtcNow)
        {
            return StatusCode(423, new ResponseHttp<object>( 
                null, $"Account locked until {user.LockoutEnd}", traceId, false, DateTime.UtcNow));
        }

        bool isPasswordValid = await userService.CheckPassword(user, dto.Password);

        if (!isPasswordValid)
        {
            user.AccessFailedCount += 1;
        
            if (user.AccessFailedCount >= 3)
            {
                user.LockoutEnd = DateTimeOffset.UtcNow.AddHours(5);
                user.AccessFailedCount = 0; 
            }
        
            await userService.UpdateSimple(user);
            return invalidCredentialsResponse;
        }

        user.AccessFailedCount = 0;
        user.LockoutEnd = null;
    
        var roles = await userService.GetUserRoles(user);
        var tokens = tokenService.CreateTokens(user, roles);
    
        user.RefreshToken = tokens.RefreshToken;
        user.RefreshTokenExpiryTime = tokens.ExpiredAtRefreshToken;
    
        UserResult simple = await userService.UpdateSimple(user);

        tokens.User = mapper.Map<UserDto>(simple.User);

        return Ok(new ResponseHttp<ResponseTokens>(tokens, "Login success", traceId, true, DateTime.UtcNow));
    }

    [HttpPost("logout")]
    [SwaggerOperation(Summary = "Logout user", Tags = ["Auth"])]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> Logout()
    {
        string? userId = User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value ?? throw new UnauthenticatedException();
        UserModel userBySid = await userService.GetUserBySidSimple(userId);
        
        userBySid.RefreshToken = null;
        userBySid.RefreshTokenExpiryTime = null;
        userBySid.AccessFailedCount = 0;
        
        await userService.UpdateSimple(userBySid);
        
        return Ok(new ResponseHttp<object>(
            Data: null,
            Message: "Logout success",
            TraceId: HttpContext.TraceIdentifier,
            Success: true,
            Timestamp: DateTime.UtcNow
        ));
    }

    [HttpGet("refresh-token/{refreshToken}")]
    [SwaggerOperation(Summary = "Refresh Token", Tags = ["Auth"])]
    [ProducesResponseType(typeof(ResponseHttp<ResponseTokens>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.Forbidden)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> RefreshToken(string refreshToken)
    {
        UserModel user = await userService.GetUserByRefreshToken(refreshToken) 
                         ?? throw new ModelNotFoundException("Refresh token not found or invalid.");
        
        if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            throw new ForbiddenException("Refresh token has expired. Please login again.");
        
        IList<string> roles = await userService.GetUserRoles(user);

        ResponseTokens tokens = tokenService.CreateTokens(user, roles);
        
        UserDto userDto = mapper.Map<UserDto>(user);
        
        tokens.User = userDto;

        return Ok(new ResponseHttp<ResponseTokens>(
            tokens, 
            "Refresh with success", 
            HttpContext.TraceIdentifier, 
            true, 
            DateTime.UtcNow
        ));
    }
    
}