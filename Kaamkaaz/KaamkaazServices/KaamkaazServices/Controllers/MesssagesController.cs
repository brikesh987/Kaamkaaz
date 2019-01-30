using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KaamkaazServices.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KaamkaazServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MesssagesController : ControllerBase
    {

        // GET: api/Messsage/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST: api/Messsage
        [HttpPost]
        public ActionResult Post(BroadcastRequest broadcastMessage)
        {
            //TODO: Implement
            if (broadcastMessage == null || !broadcastMessage.IsValid())
            {
                return BadRequest();
            }
            //Implement
            return Ok();
        }

        
    }
}
