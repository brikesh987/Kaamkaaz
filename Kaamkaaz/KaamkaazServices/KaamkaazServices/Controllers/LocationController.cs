namespace KaamkaazServices.Controllers
{
    using KaamkaazServices.Helper;
    using KaamkaazServices.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Defines the <see cref="LocationController" />
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : Controller
    {
        private readonly IConfiguration configuration;

        public LocationController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        #region Methods

        /// <summary>
        /// The Post
        /// </summary>
        /// <param name="location">The location<see cref="Location"/></param>
        /// <returns>The <see cref="ActionResult"/></returns>
        [HttpPost]
        public ActionResult Post(Location location)
        {
            if (location == null || !location.IsValid())
            {
                return BadRequest("Location information is not valid.");
            }
            var repository = new BlueColorDB(configuration.GetConnectionString("BlueColor"));
            if (repository.UpdateLocation(location))
                return Ok();
            else
                return BadRequest("An error occurred while processing the request.");
        }
        #endregion
    }
}
