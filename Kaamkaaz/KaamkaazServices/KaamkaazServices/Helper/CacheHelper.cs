namespace KaamkaazServices.Helper
{
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="CacheHelper" />
    /// </summary>
    public class CacheHelper
    {
        #region Fields

        /// <summary>
        /// Defines the configuration
        /// </summary>
        private readonly IConfiguration configuration;

        /// <summary>
        /// Defines the memoryCache
        /// </summary>
        private readonly IMemoryCache memoryCache;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheHelper"/> class.
        /// </summary>
        /// <param name="memoryCache">The memoryCache<see cref="IMemoryCache"/></param>
        /// <param name="configuration">The configuration<see cref="IConfiguration"/></param>
        public CacheHelper(IMemoryCache memoryCache, IConfiguration configuration)
        {
            this.memoryCache = memoryCache;
            this.configuration = configuration;
        }

        #endregion

        #region Methods

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
        /// <summary>
        /// The GetServices
        /// </summary>
        /// <param name="country">The country<see cref="string"/></param>
        /// <returns>The <see cref="List{string}"/></returns>
        public List<string> GetServices(string country)
        {
            List<string> services;
            if (!memoryCache.TryGetValue(country, out services))
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                                        .SetSlidingExpiration(TimeSpan.FromHours(4));
                var repository = new BlueColorDB(configuration.GetConnectionString("BlueColor"));
                services = repository.GetServices(country);
                memoryCache.Set<List<string>>(country, services);
            }
            return services;
        }

        #endregion
    }
}
