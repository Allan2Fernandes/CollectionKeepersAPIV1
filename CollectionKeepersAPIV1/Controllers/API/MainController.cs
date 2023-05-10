using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace CollectionKeepersAPIV1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MainController : ControllerBase
    {
        private readonly Serilog.ILogger log; 
        public MainController() 
        {
           log = Log.Logger.ForContext<MainController>();

        } 

        [HttpGet(nameof(CheckIfOnline))]
        public async Task<ActionResult<string>> CheckIfOnline()
        {
            log.Information("API Online!!!");
            return Ok("Online");
        }
    }
}
