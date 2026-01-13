using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Write.API.Configs.Exception.classes;

namespace SocialNetwork.Write.API.Controllers;

public class AuthController : Controller
{
    [HttpGet("test-error")]
    public IActionResult TestError() 
    {
        throw new ModelNotFoundException("Usuário não encontrado no TiDB!");
    }
}