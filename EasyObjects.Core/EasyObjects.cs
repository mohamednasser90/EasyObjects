using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace EasyObjects.Core
{
    public abstract class EasyObject
    {
        string _connectionString = string.Empty;

        public virtual IEnumerable<T> Select<T>(DynamicParameters parameterCollection)
        {
            string sql = "";

            using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                return sqlConnection.Query<T>(sql, parameterCollection, commandType: CommandType.Text);
            }
        }
    }
}