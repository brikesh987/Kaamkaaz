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
    public class SerivceProviderController : ControllerBase
    {
        // GET: api/SerivceProvider
        [HttpGet]
        public IEnumerable<ServiceProvider> Get(ServiceProvidersRequest curLocation)
        {
            return new List<ServiceProvider>();
        }        
    }
}
