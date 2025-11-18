using CorarlERP.Web.Controllers;
using CorarlERP.Web.Health;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

//[Route("api/[controller]/[action]")]
[Route("api/[controller]")]
public class HealthController : CorarlERPControllerBase
{
    private readonly IHealthCheckService _healthCheckService;

    public HealthController(IHealthCheckService healthCheckService)
    {
        _healthCheckService = healthCheckService;
    }

    //[HttpGet]
    //public async Task<IActionResult> Get()
    //{
    //    var result = await _healthCheckService.CheckHealthAsync();
    //    return result.Status == "Healthy" ? Ok(result) : StatusCode(503, result);
    //}

    [HttpGet]
    public async Task<IActionResult> Check()
    {
        try
        {
            var result = await _healthCheckService.CheckHealthAsync();

            if (result.Status == "Healthy")
            {
                var responseObject = new
                {
                    Status = "Healthy",
                    Message = "OK Test",
                    TimeStamp = DateTime.UtcNow.ToString(),
                    Version = "Check_2025_05_26"
                };

                // Set Cache-Control headers
                Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";

                // ASP.NET Core automatically sets Content-Type to application/json for Ok()
                return Ok(responseObject);
            }
            else
            {
                var errorResponse = new
                {
                    Status = "Unhealthy",
                    Message = JsonConvert.SerializeObject(result.Checks),
                    TimeStamp = DateTime.UtcNow.ToString(),
                    Version = "Check_2025_05_26"
                };

                // Set Cache-Control headers
                Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";

                // Return 503 Internal Server Error with the error response
                return StatusCode(503, errorResponse);
            }
        }
        catch(Exception ex)
        {
            var errorResponse = new
            {
                Status = "Unhealthy",
                Message = ex.Message,
                TimeStamp = DateTime.UtcNow.ToString(),
                Version = "Check_2025_05_26"
            };

            // Set Cache-Control headers
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";

            // Return 500 Internal Server Error with the error response
            return StatusCode(500, errorResponse);
        }
    }
}