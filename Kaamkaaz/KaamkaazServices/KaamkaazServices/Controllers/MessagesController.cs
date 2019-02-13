using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using KaamkaazServices.Helper;
using KaamkaazServices.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace KaamkaazServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IMemoryCache memoryCache;

        public MessagesController(IConfiguration configuration, IMemoryCache memoryCache)
        {
            this.configuration = configuration;
            this.memoryCache = memoryCache;
        }

        // GET: api/Messsage/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST: api/Messsage
        [HttpPost]
        [Authorize(AuthenticationSchemes = "amx")]
        public ActionResult Post(BroadcastRequest broadcastMessage)
        {
            //TODO: Implement
            if (broadcastMessage == null || !broadcastMessage.IsValid())
            {
                return BadRequest("Broadcast message is not valid.");
            }
            //Implement
            var repository = new BlueColorDB(configuration.GetConnectionString("BlueColor"));
            int messageId = repository.SaveMessage(broadcastMessage);
            //find recepients and add meesage for them
            //check if the nearby service proviers are in the cache
            List<ServiceProvider> providersNearby;
            string key = broadcastMessage.CurrentLocation.GetCacheKey();
            if (!memoryCache.TryGetValue(key, out providersNearby))
            {
                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                                        .SetSlidingExpiration(TimeSpan.FromMinutes(4));
                var service = new BlueCollarService(configuration.GetConnectionString("BlueColor"));
                var serviceRequest = new ServiceProvidersRequest() { City = broadcastMessage.CurrentLocation.City,
                                            Longitude = broadcastMessage.CurrentLocation.Longitude,
                                            Latitude = broadcastMessage.CurrentLocation.Latitude,
                                            Service = broadcastMessage.ServiceRequested }; ;
                providersNearby = service.GetNearbyServiceProviders(serviceRequest);
                memoryCache.Set<List<ServiceProvider>>(key, providersNearby);
            }
            //Add the message to be sent to each of the nearby provider;
            providersNearby.Where(y => y.UserId  != broadcastMessage.UserId).ToList().ForEach(x => repository.AddMessageRecipient(messageId, x.UserId));
            return Ok();
        }

        
    }
}
