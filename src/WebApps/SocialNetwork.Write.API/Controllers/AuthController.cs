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
        
        return Ok(new ResponseHttp<ResponseTokens>(
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
        UserModel? user = await userService.GetUserByEmail(dto.Email);

        if (user is null)
            return Unauthorized();

        bool checkPassword = await userService.CheckPassword(user, dto.Password);
        
        if (!checkPassword)
            return Unauthorized();

        IList<string> roles = await userService.GetUserRoles(user);

        ResponseTokens tokens = tokenService.CreateTokens(user, roles);

        ResponseHttp<ResponseTokens> response = new ResponseHttp<ResponseTokens>(
            Data: tokens,
            ErrorCode: 0,
            Message: "Login success",
            Success: true,
            Timestamp: DateTime.UtcNow,
            TraceId: HttpContext.TraceIdentifier
        );

        return Ok(response);
    }
    
}