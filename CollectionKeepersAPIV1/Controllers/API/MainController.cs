using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace CollectionKeepersAPIV1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MainController : ControllerBase
    {
    
        public MainController() 
        {
         
        } 

        [HttpGet(nameof(CheckIfOnline))]
        public async Task<ActionResult<string>> CheckIfOnline()
        {
            for(int i= 0; i<50; i++)
            {
                Log.Logger.Information("Logging in Main count = " + i);
            }
            return Ok("Online");
        }
    }
}
