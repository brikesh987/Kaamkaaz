namespace KaamkaazServices.Models
{
    using System;
    using System.Text;

    /// <summary>
    /// Defines the <see cref="Location" />
    /// </summary>
    public class Location
    {
        #region Properties

        /// <summary>
        /// Gets or sets the City
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the Country
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the Latitude
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the Longitude
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Gets or sets the UpdatedOn
        /// </summary>
        public DateTime UpdatedOn { get; set; }

        /// <summary>
        /// Gets or sets the UserId
        /// </summary>
        public int UserId { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The GetCacheKey
        /// </summary>
        /// <returns>The <see cref="string"/></returns>
        public string GetCacheKey()
        {
            var builder = new StringBuilder();
            builder.Append(City);
            builder.Append("_");
            builder.Append(Latitude);
            builder.Append("_");
            builder.Append(Longitude);
            return builder.ToString();
        }

        /// <summary>
        /// The IsValid
        /// </summary>
        /// <returns>The <see cref="bool"/></returns>
        public bool IsValid()
        {
            if (Latitude != 0 && Longitude != 0 && !string.IsNullOrWhiteSpace(City) && UserId > 0)
            {
                return true;
            }
            return false;
        }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="ServiceProvider" />
    /// </summary>
    public class ServiceProvider
    {
        #region Properties

        /// <summary>
        /// Gets or sets the AboutUser
        /// </summary>
        public string AboutUser { get; set; }

        /// <summary>
        /// Gets or sets the Location
        /// </summary>
        public Location Location { get; set; }

        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Phone
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Gets or sets the Service
        /// </summary>
        public string Service { get; set; }

        /// <summary>
        /// Gets or sets the UserId
        /// </summary>
        public int UserId { get; internal set; }

        #endregion
    }
}
