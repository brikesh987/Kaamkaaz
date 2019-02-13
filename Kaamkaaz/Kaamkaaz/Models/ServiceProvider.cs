namespace Kaamkaaz.Models
{
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
