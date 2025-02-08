using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchedulerService.Domain;
using SchedulerService.Domain.Dto;
using SchedulerService.Domain.Extensions;
using SchedulerService.Domain.Mappers;
using SchedulerService.Domain.Response;
using SchedulerService.Services;
using SchedulerService.Services.Interfaces;

namespace SchedulerService.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/v1/institutions")]
public class InstitutionController : ControllerBase
{
    private readonly IInstitutionService _institutionService;
    private readonly IAdministratorService _administratorService;

    public InstitutionController(IInstitutionService institutionService, 
        IAdministratorService administratorService)
    {
        _institutionService = institutionService;
        _administratorService = administratorService;
    }

    [HttpGet("all")]
    [ProducesResponseType(typeof(IEnumerable<InstitutionResponse>), StatusCodes.Status200OK)]
    public async Task<ApiResponse> Get(CancellationToken ct)
    {
        var userId = User.GetIdFromClaims();
        var institutions = await _institutionService.GetAllAsync(userId, ct);
        return Response.Ok(InstitutionMapper.ToResponses(institutions));
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(InstitutionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> GetById(long id, CancellationToken ct)
    {
        var userId = User.GetIdFromClaims();
        
        var institution = await _institutionService.GetByIdAsync(id, userId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {id} is not found");
        
        return Response.Ok(InstitutionMapper.ToResponse(institution.Value));
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(InstitutionResponse), StatusCodes.Status201Created)]
    public async Task<ApiResponse> Create(InstitutionDto institutionDto, CancellationToken ct)
    {
        var userId = User.GetIdFromClaims();
        var institution = await _institutionService.CreateAsync(institutionDto, userId, ct);
        return Response.Created(InstitutionMapper.ToResponse((institution, UserRole.Owner)));
    }
    
    [HttpPut("update/{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ApiResponse> Update(long id, InstitutionDto institutionDto, CancellationToken ct)
    {
        // TODO test this as an admin
        var institution = await _institutionService.GetByIdAsync(id, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {id} is not found");
        
        var userId = User.GetIdFromClaims();

        if (!await _administratorService.IsUserAdminOrOwnerAsync(institution, userId, ct))
            return Response.Forbid();
        
        await _institutionService.UpdateAsync(institution, institutionDto, ct);
        return Response.Ok();
    }
    
    [HttpDelete("delete/{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Delete(long id, CancellationToken ct)
    {
        var userId = User.GetIdFromClaims();
        var deleted = await _institutionService.DeleteAsync(id, userId, ct);
        return deleted ? Response.Ok() : Response.NotFound($"Institution by id {id} is not found");
    }
}