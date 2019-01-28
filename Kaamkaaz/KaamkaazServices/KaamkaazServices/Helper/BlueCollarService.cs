namespace KaamkaazServices.Helper
{
    using GeoCoordinatePortable;
    using KaamkaazServices.Models;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="BlueCollarService" />
    /// </summary>
    public class BlueCollarService
    {
        #region Constants

        /// <summary>
        /// Defines the MAX_DISTANCE_IN_MILES
        /// </summary>
        private const double MAX_DISTANCE_IN_MILES = 5.0;

        #endregion

        #region Fields

        /// <summary>
        /// Defines the connectionString
        /// </summary>
        private readonly string connectionString;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BlueCollarService"/> class.
        /// </summary>
        /// <param name="connectionString">The connectionString<see cref="string"/></param>
        public BlueCollarService(string connectionString)
        {
            this.connectionString = connectionString;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The GetNearbyServiceProviders
        /// </summary>
        /// <param name="request">The request<see cref="ServiceProvidersRequest"/></param>
        /// <returns>The <see cref="List{ServiceProvider}"/></returns>
        public List<ServiceProvider> GetNearbyServiceProviders(ServiceProvidersRequest request)
        {
            var repository = new BlueColorDB(connectionString);
            var serviceprovidersInCity = repository.GetProvidersInCity(request.City);
            return GetNearestProviders(serviceprovidersInCity, request, repository);
        }

        /// <summary>
        /// The GetDistance
        /// </summary>
        /// <param name="request">The request<see cref="ServiceProvidersRequest"/></param>
        /// <param name="provider">The provider<see cref="Location"/></param>
        /// <returns>The <see cref="double"/></returns>
        private double GetDistance(ServiceProvidersRequest request, Location provider)
        {
            var providerLoc = new GeoCoordinate(request.Latitude, request.Longitude);
            var userLoc = new GeoCoordinate(provider.Latitude, provider.Longitude);

            var distanceInMeters = providerLoc.GetDistanceTo(userLoc);
            //convert to miles
            return (distanceInMeters / 1609.344);
        }

        /// <summary>
        /// The GetNearestProviders
        /// </summary>
        /// <param name="serviceprovidersInCity">The serviceprovidersInCity<see cref="List{Location}"/></param>
        /// <param name="request">The request<see cref="ServiceProvidersRequest"/></param>
        /// <param name="repository">The repository<see cref="BlueColorDB"/></param>
        /// <returns>The <see cref="List{ServiceProvider}"/></returns>
        private List<ServiceProvider> GetNearestProviders(List<Location> serviceprovidersInCity, ServiceProvidersRequest request, BlueColorDB repository)
        {
            var nearestproviders = new List<ServiceProvider>();
            foreach (var provider in serviceprovidersInCity)
            {
                try
                {
                    var distanceFromUser = GetDistance(request, provider);
                    if (distanceFromUser <= MAX_DISTANCE_IN_MILES)
                    {
                        //Add the provider
                        var user = repository.GetUser(provider.UserId);
                        nearestproviders.Add(new ServiceProvider()
                        {
                            AboutUser = user.AboutUser,
                            Name = user.Name,
                            Phone = user.Phone,
                            Service = JsonConvert.SerializeObject(user.ProfileData),
                            Location = new Location()
                            {
                                Latitude = provider.Latitude,
                                Longitude = provider.Longitude,
                                City = provider.City,
                                Country = provider.Country
                            }
                        });
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
            return nearestproviders;
        }

        #endregion
    }
}
