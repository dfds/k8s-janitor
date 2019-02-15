using Microsoft.AspNetCore.Mvc;

namespace RolemapperService.WebApi.Controllers
{
    [Route("healthz")]
    public class HealthController : ControllerBase
    {
        [HttpGet("")]
        public OkResult IsHealthy()
        {
            return Ok();
        }
    }
}