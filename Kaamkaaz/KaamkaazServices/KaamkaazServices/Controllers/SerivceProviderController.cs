namespace KaamkaazServices.Controllers
{
    using KaamkaazServices.Helper;
    using KaamkaazServices.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="SerivceProviderController" />
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SerivceProviderController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IMemoryCache memoryCache;

        public SerivceProviderController(IConfiguration configuration, IMemoryCache  memoryCache)
        {
            this.configuration = configuration;
            this.memoryCache = memoryCache;
        }
        #region Methods

        // GET: api/SerivceProvider
        /// <summary>
        /// The Get
        /// </summary>
        /// <param name="curLocation">The curLocation<see cref="ServiceProvidersRequest"/></param>
        /// <returns>The <see cref="IEnumerable{ServiceProvider}"/></returns>
        [HttpGet]
        
        public IEnumerable<ServiceProvider> Get(ServiceProvidersRequest curLocation)
        {
            if (curLocation == null || !curLocation.IsValid())
            {
                return new List<ServiceProvider>();
            }
            List<ServiceProvider> response;
            if (!memoryCache.TryGetValue(curLocation.GetCacheKey(),out response))
            {
                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                                        .SetSlidingExpiration(TimeSpan.FromMinutes(4));
                var service = new BlueCollarService(configuration.GetConnectionString("BlueColor"));
                response = service.GetNearbyServiceProviders(curLocation);
                memoryCache.Set<List<ServiceProvider>>(curLocation.GetCacheKey(), response);
            }
            return response;
        }

        #endregion
    }
}
