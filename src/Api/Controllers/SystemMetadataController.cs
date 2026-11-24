using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using LedgerAI.Api.Data.Dtos.Common;

namespace LedgerAI.Api.Controllers;

[ApiController]
[Route("api/metadata")]
public class SystemMetadataController : ControllerBase
{
    private readonly IHostEnvironment _environment;
    private readonly ILogger<SystemMetadataController> _logger;

    public SystemMetadataController(IHostEnvironment environment, ILogger<SystemMetadataController> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<ApiResponseDto<object>> GetMetadata()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var version = assembly.GetName().Version?.ToString() ?? "1.0.0";

        var metadata = new
        {
            Application = "LedgerAI Web API",
            Version = version,
            Framework = ".NET 9",
            Status = "Healthy",
            Environment = _environment.EnvironmentName,
            DatabaseEngine = "PostgreSQL 16+",
            ServerTimeUtc = DateTimeOffset.UtcNow
        };

        return Ok(ApiResponseDto<object>.Ok(metadata, "System metadata retrieved successfully."));
    }
}
