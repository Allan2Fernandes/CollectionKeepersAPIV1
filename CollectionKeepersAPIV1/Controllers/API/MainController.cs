﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CollectionKeepersAPIV1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MainController : ControllerBase
    {

        [HttpGet(nameof(CheckIfOnline))]
        public async Task<ActionResult<string>> CheckIfOnline()
        {
            return Ok("Online");
        }
    }
}
