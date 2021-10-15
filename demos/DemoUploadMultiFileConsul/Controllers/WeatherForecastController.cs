using Microsoft.AspNetCore.Mvc;

namespace DemoUploadMultiFileConsul.Controllers
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
