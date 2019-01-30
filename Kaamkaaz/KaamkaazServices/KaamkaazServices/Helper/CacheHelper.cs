using KaamkaazServices.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaamkaazServices.Helper
{
    public class CacheHelper
    {
        private readonly IMemoryCache memoryCache;
        private readonly IConfiguration configuration;

        public CacheHelper(IMemoryCache memoryCache, IConfiguration configuration)
        {
            this.memoryCache = memoryCache;
            this.configuration = configuration;
        }
        //public List<ServiceProvider> GetServiceProvidersNearby(string key, IMemoryCache memoryCache)
        //{
        //    List<ServiceProvider> response;
        //    if (!memoryCache.TryGetValue(key, out response))
        //    {
        //        // Set cache options.
        //        var cacheEntryOptions = new MemoryCacheEntryOptions()
        //                                .SetSlidingExpiration(TimeSpan.FromMinutes(4));
        //        var service = new BlueCollarService(configuration.GetConnectionString("BlueColor"));
        //        response = service.GetNearbyServiceProviders(curLocation);
        //        memoryCache.Set<List<ServiceProvider>>(curLocation.GetCacheKey(), response);
        //    }

        //    return response;
        //}
    }
}
