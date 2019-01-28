namespace KaamkaazServices.Controllers
{
    using KaamkaazServices.Helper;
    using KaamkaazServices.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="SerivceProviderController" />
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SerivceProviderController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public SerivceProviderController(IConfiguration configuration)
        {
            this.configuration = configuration;
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
            var service = new BlueCollarService(configuration.GetConnectionString("BlueColor"));
            return service.GetNearbyServiceProviders(curLocation);
        }

        #endregion
    }
}
