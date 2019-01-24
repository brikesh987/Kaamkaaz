using KaamkaazServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Data.SqlClient;
using Dapper;

namespace KaamkaazServices.Helper
{
    public class BlueColorDB
    {
        public BlueColorDB(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public string ConnectionString { get; }
        public string CreateUser(User user,string userId)
        {
            string sql = $@"INSERT INTO [UserProfile] (UserId,ProfileData,IsActive,Name,Phone,AboutUser,CreateDate) 
                            VALUES (
                                    {userId},
                                    {JsonConvert.SerializeObject(user.ServicesOffered)},
                                    {user.IsActive},
                                    {user.Name},
                                    {user.PhoneNumber},
                                    {user.AboutUser},
                                    {DateTime.UtcNow}
                                   );
                            SELECT CAST(SCOPE_IDENTITY() as int)";
            try
            {
                var connection = new SqlConnection(ConnectionString);
                var id = connection.Query<int>(sql).Single();
            } catch (Exception e) {

            }
            
            return "";
        }
    }
}
