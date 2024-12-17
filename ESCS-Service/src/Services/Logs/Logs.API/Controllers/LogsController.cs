﻿using Logs.API.Interfaces;
using Logs.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Logs.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly ILogService _logService;
        private readonly ILogger<LogsController> _logger;

        public LogsController(ILogService logService, ILogger<LogsController> logger)
        {
            _logService = logService;
            _logger = logger;
        }

        [HttpGet("fields")]
        public async Task<IActionResult> GetAllFields([FromQuery] string index)
        {
            // Flatten the properties to get all field paths
            var fields = await _logService.GetAllFields(index);

            return Ok(fields);
        }


        [HttpPost("search-by-user")]
        public async Task<IActionResult> GetLogsRequestByUserId([FromBody] SearchLogByUserIdRequest searchLogByUserIdRequest)
        {
            var paginatedLogs = await _logService.GetResponseLogByUser(searchLogByUserIdRequest);

            return Ok(paginatedLogs);
        }

        [HttpGet]
        public async Task<IActionResult> GetLogById([FromQuery] string id, [FromQuery] string index)
        {
            // Flatten the properties to get all field paths
            var log = await _logService.GetLogById(index, id);

            return Ok(log);
        }

        [HttpGet("response-statistic")]
        public async Task<IActionResult> GetUserResponseStatistic([FromQuery] FilterLogResponseRequestModel requestModel)
        {
            // Flatten the properties to get all field paths
            var log = await _logService.GetUserLogResponseStatistic(requestModel);

            return Ok(log);
        }

        [HttpGet("response-detail-statistic")]
        public async Task<IActionResult> GetUserResponseDetailStatistic([FromQuery] FilterLogResponseDetailRequestModel requestModel)
        {
            // Flatten the properties to get all field paths
            var log = await _logService.GetUserLogResponseDetailStatistic(requestModel);

            return Ok(log);
        }

        [HttpGet("endpoint-detail-statistic")]
        public async Task<IActionResult> GetUserResponseDetailStatistic([FromQuery] FilterServiceUsagelRequestModel requestModel)
        {
            // Flatten the properties to get all field paths
            var log = await _logService.GetServiceUsageStatistic(requestModel);

            return Ok(log);
        }
    }
}