namespace KaamkaazServices.Helper
{
    using Dapper;
    using KaamkaazServices.Models;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;

    /// <summary>
    /// Defines the <see cref="BlueColorDB" />
    /// </summary>
    public class BlueColorDB
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BlueColorDB"/> class.
        /// </summary>
        /// <param name="connectionString">The connectionString<see cref="string"/></param>
        public BlueColorDB(string connectionString)
        {
            ConnectionString = connectionString;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the ConnectionString
        /// </summary>
        public string ConnectionString { get; }

        public bool UpdateLocation(Location location)
        {
            string sql = $@"INSERT INTO [UserLocation] (UserId,Latitude,Longitude,City,Country,UpdatedOn) 
                            VALUES (
                                     {location.UserId},
                                     {location.Latitude},
                                     {location.Longitude},
                                    '{location.City}',
                                    '{location.Country}',                                    
                                    '{DateTime.UtcNow}'
                                   );
                            SELECT CAST(SCOPE_IDENTITY() as int)";
            int id = -1;
            try
            {
                var connection = new SqlConnection(ConnectionString);
                id = connection.Query<int>(sql).Single();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return id > 0;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The CreateUser
        /// </summary>
        /// <param name="user">The user<see cref="User"/></param>
        /// <param name="userId">The userId<see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
        public int CreateUser(User user, string userId)
        {
            string sql = $@"INSERT INTO [UserProfile] (UserId,ProfileData,IsActive,Name,Phone,AboutUser,CreateDate) 
                            VALUES (
                                    '{userId}',
                                    '{JsonConvert.SerializeObject(user.ProfileData)}',
                                    '{user.IsActive}',
                                    '{user.Name}',
                                    '{user.Phone}',
                                    '{user.AboutUser}',
                                    '{DateTime.UtcNow}'
                                   );
                            SELECT CAST(SCOPE_IDENTITY() as int)";
            int id = -1;
            try
            {
                var connection = new SqlConnection(ConnectionString);
                id = connection.Query<int>(sql).Single();
                if (id > 0 && user.Location != null)
                {
                    string sqlLocation = $@"INSERT INTO [UserLocation] (UserId,Latitude,Longitude,City,Country, UpdatedOn)
                                            VALUES (
                                                     {id},
                                                     {user.Location.Latitude},
                                                     {user.Location.Longitude},
                                                     '{user.Location.City}',
                                                     '{user.Location.Country}',
                                                     '{DateTime.UtcNow}'
                                                    );";
                    connection.Query(sqlLocation);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return id;
        }

        /// <summary>
        /// The GetProvidersInCity
        /// </summary>
        /// <param name="city">The city<see cref="string"/></param>
        /// <returns>The <see cref="List{Location}"/></returns>
        public List<Location> GetProvidersInCity(string city)
        {
            string sql = $@"SELECT UserId, Latitude, Longitude, City, Country,UpdatedOn
                            FROM UserLocation
                            WHERE City = '{city}'                            
                            Group by UserId,Latitude,Longitude,City,Country,UpdatedOn
                            Order By UpdatedOn desc;
                            ";
            var locations = new List<Location>();
            try
            {
                var connection = new SqlConnection(ConnectionString);
                locations = connection.Query<Location>(sql).ToList();
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return locations;
        }

        /// <summary>
        /// The GetUser
        /// </summary>
        /// <param name="userId">The userId<see cref="string"/></param>
        /// <returns>The <see cref="User"/></returns>
        public User GetUser(int userId)
        {
            string sql = $@"SELECT ProfileData, IsActive, Name, Phone, Name, AboutUser
                            FROM UserProfile
                            WHERE Id = {userId}";
            var user = new User();
            try
            {
                var connection = new SqlConnection(ConnectionString);
                user = connection.Query<User>(sql).Single();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return user;
        }

        /// <summary>
        /// The UpdateUser
        /// </summary>
        /// <param name="user">The user<see cref="User"/></param>
        /// <returns>The <see cref="int"/></returns>
        public int UpdateUser(User user)
        {
            string sql = $@" UPDATE UserProfile
                             SET ProfileData =  '{JsonConvert.SerializeObject(user.ProfileData)}',
                                 IsActive = '{user.IsActive}',
                                 Name = '{user.Name}',
                                 Phone = '{user.Phone}',
                                 AboutUser = '{user.AboutUser}'
                            WHERE UserId = '{user.UserId}';";
            var rowsAffected = -1;
            try
            {
                var connection = new SqlConnection(ConnectionString);
                rowsAffected = connection.Execute(sql);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return rowsAffected;
        }

        #endregion
    }
}
