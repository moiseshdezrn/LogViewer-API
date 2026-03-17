using LogViewer.Core.DTOs;
using LogViewer.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UnosquareTechnicalExam.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LogController : ControllerBase
    {
        private readonly ILogService _logService;
        private readonly ILogger<LogController> _logger;

        public LogController(ILogService logService, ILogger<LogController> logger)
        {
            _logService = logService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetLogs([FromQuery] LogQueryParameters parameters)
        {
            var result = await _logService.GetLogsAsync(parameters);
            return Ok(result);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetLogById(long id)
        {
            var log = await _logService.GetLogByIdAsync(id);

            if (log is null)
                return NotFound();

            return Ok(log);
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportLogs([FromQuery] LogQueryParameters parameters)
        {
            var csvBytes = await _logService.ExportLogsCsvAsync(parameters);
            return File(csvBytes, "text/csv", "logs_export.csv");
        }

        [HttpGet("stats/levels")]
        public async Task<IActionResult> GetLevelStats([FromQuery] LogStatsParameters parameters)
        {
            var stats = await _logService.GetLevelStatsAsync(parameters);
            return Ok(stats);
        }

        [HttpGet("stats/timeline")]
        public async Task<IActionResult> GetTimelineStats([FromQuery] LogStatsTimelineParameters parameters)
        {
            var stats = await _logService.GetTimelineStatsAsync(parameters);
            return Ok(stats);
        }
        [HttpGet("stats/errors-and-criticals-by-source")]
        public async Task<IActionResult> GetErrorAndCriticalsBySource([FromQuery] LogStatsParameters parameters)
        {
            var stats = await _logService.GetErrorAndCriticalsBySourceAsync(parameters);
            return Ok(stats);
        }

        [HttpGet("sources")]
        public async Task<IActionResult> GetSources()
        {
            var sources = await _logService.GetDistinctSourcesAsync();
            return Ok(sources);
        }

        [HttpGet("applications")]
        public async Task<IActionResult> GetApplications()
        {
            var applications = await _logService.GetDistinctApplicationsAsync();
            return Ok(applications);
        }

        [HttpGet("stats/daily")]
        public async Task<IActionResult> GetDailyStats()
        {
            var stats = await _logService.GetDailyStatsAsync();
            return Ok(stats);
        }
    }
}
