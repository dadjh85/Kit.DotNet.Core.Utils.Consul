using Microsoft.AspNetCore.Mvc;

namespace DemoUploadFileConsulWithRelativePath.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
       
        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
