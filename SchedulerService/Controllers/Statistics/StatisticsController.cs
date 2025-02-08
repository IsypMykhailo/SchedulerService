using System.Linq.Expressions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchedulerService.Domain.Extensions;
using SchedulerService.Domain.Models;
using SchedulerService.Domain.Response;
using SchedulerService.Services.Interfaces;

namespace SchedulerService.Controllers.Statistics;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/v1/institutions/{institutionId:long}/")]
public class StatisticsController : ControllerBase
{
    private readonly IAdministratorService _administratorService;
    private readonly IInstitutionService _institutionService;
    private readonly IGroupStudentService _groupStudentService;

    public StatisticsController(IAdministratorService administratorService,
        IInstitutionService institutionService,
        IGroupStudentService groupStudentService)
    {
        _administratorService = administratorService;
        _institutionService = institutionService;
        _groupStudentService = groupStudentService;
    }
    
    [HttpGet("personnel-count")]
    [ProducesResponseType(typeof(IEnumerable<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Get(long institutionId, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId,
            new Expression<Func<Institution, object?>>[]
            {
                e => e.Administrators,
                e => e.Teachers
            }, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var userId = User.GetIdFromClaims();
        
        if (!await _administratorService.IsUserBelongsToInstitutionAsync(institution, userId, ct))
            return Response.Forbid();

        var studentsCount = await _groupStudentService.GetStudentsCountAsync(institution, ct);
        
        return Response.Ok(new
        {
            AdminsCount = institution.Administrators.Count, 
            TeachersCount = institution.Teachers.Count,
            StudentsCount = studentsCount
        });
    }
}