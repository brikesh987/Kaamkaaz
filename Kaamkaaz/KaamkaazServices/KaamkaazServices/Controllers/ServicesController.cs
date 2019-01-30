namespace KaamkaazServices.Controllers
{
    using KaamkaazServices.Helper;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Configuration;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="ServicesController" />
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        #region Fields

        /// <summary>
        /// Defines the cacheHelper
        /// </summary>
        private readonly CacheHelper cacheHelper;

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
        /// Initializes a new instance of the <see cref="ServicesController"/> class.
        /// </summary>
        /// <param name="configuration">The configuration<see cref="IConfiguration"/></param>
        /// <param name="memoryCache">The memoryCache<see cref="IMemoryCache"/></param>
        public ServicesController(IConfiguration configuration, IMemoryCache memoryCache)
        {
            this.configuration = configuration;
            this.memoryCache = memoryCache;
            cacheHelper = new CacheHelper(memoryCache, configuration);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The Get
        /// </summary>
        /// <param name="country">The country<see cref="string"/></param>
        /// <returns>The <see cref="IEnumerable{string}"/></returns>
        [HttpGet]
        public IEnumerable<string> Get(string country)
        {
            if (string.IsNullOrWhiteSpace(country))
            {
                return new List<string>();
            }

            return cacheHelper.GetServices(country);
        }

        #endregion
    }
}
