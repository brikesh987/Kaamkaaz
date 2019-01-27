namespace KaamkaazServices.Helper
{
    using Dapper;
    using KaamkaazServices.Models;
    using Newtonsoft.Json;
    using System;
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
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return rowsAffected;
        }

        public User GetUser(string userId)
        {
            string sql = $@"SELECT ProfileData, IsActive, Name, Phone, Name, AboutUser
                            FROM UserProfile
                            WHERE UserId = '{userId}'";
            var user = new User();
            try
            {
                var connection = new SqlConnection(ConnectionString);
                user = connection.Query<User>(sql).Single();
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return user;
        }
        #endregion
    }

}
