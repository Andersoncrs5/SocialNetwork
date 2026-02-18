using System.Net;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Contracts.DTOs.Tag;
using SocialNetwork.Contracts.Utils.Res.http;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Modules.Tag.Dto;
using SocialNetwork.Write.API.Modules.Tag.Model;
using SocialNetwork.Write.API.Modules.Tag.Service.Interface;
using SocialNetwork.Write.API.Services.Interfaces;

namespace SocialNetwork.Write.API.Modules.Tag.Controller;

[ApiController]
[Route("api/v{version:apiVersion}/tag")]
[ApiVersion("1.0")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class TagController(
    IMapper mapper,
    ITagService service
    ) : Microsoft.AspNetCore.Mvc.Controller
{
    [HttpPost]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.Forbidden)]
    [ProducesResponseType(typeof(ResponseHttp<TagDto>), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ResponseHttp<IDictionary<string, string[]>>), (int)HttpStatusCode.BadRequest)]
    [Authorize(Roles = "MASTER_ROLE, SUPER_ADM_ROLE")]
    public async Task<IActionResult> Create([FromBody] CreateTagDto dto)
    {
        TagModel model = await service.CreateAsync(dto);

        return StatusCode(StatusCodes.Status201Created, new ResponseHttp<TagDto>(
            Data: mapper.Map<TagDto>(model),
            Message:"Tag created successfully",
            TraceId: HttpContext.TraceIdentifier,
            Success: true,
            Timestamp: DateTime.UtcNow
        ));
    }

    [HttpDelete("{id:required}")]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.Forbidden)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.NotFound)]
    [Authorize(Roles = "MASTER_ROLE, SUPER_ADM_ROLE")]
    public async Task<IActionResult> Delete([IsId] string id)
    {
        TagModel model = await service.GetByIdSimpleAsync(id);

        await service.DeleteAsync(model);

        return Ok(new ResponseHttp<object>(
            Data: null,
            Message:"Tag deleted successfully",
            TraceId: HttpContext.TraceIdentifier,
            Success: true,
            Timestamp: DateTime.UtcNow
        ));
    }

    [HttpPatch("{id:required}")]
    [ProducesResponseType(typeof(ResponseHttp<TagDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.Forbidden)]
    [ProducesResponseType(typeof(ResponseHttp<object>), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseHttp<IDictionary<string, string[]>>), (int)HttpStatusCode.BadRequest)]
    [Authorize(Roles = "MASTER_ROLE, SUPER_ADM_ROLE")]
    public async Task<IActionResult> Update([IsId] string id, UpdateTagDto dto)
    {
        TagModel model = await service.GetByIdSimpleAsync(id);

        TagModel tagUpdated = await service.UpdateAsync(model, dto);

        return Ok(new ResponseHttp<object>(
            Data: mapper.Map<TagDto>(tagUpdated),
            Message:"Tag updated successfully",
            TraceId: HttpContext.TraceIdentifier,
            Success: true,
            Timestamp: DateTime.UtcNow
        ));
    }
    
}