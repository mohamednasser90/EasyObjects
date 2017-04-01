using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApp1
{
    /// <summary>
    /// Wrapper for Dapper that allows for basic CRUD operations
    /// </summary>
    public abstract class QueryBase<T>
    {
        private string _connectionString;

        /// <summary>
        /// 
        /// </summary>
        protected QueryBase(string connectionString)
        {
            _connectionString = connectionString;

            WhereParameters = new List<WhereParameterBase>();
            UpsertParameters = new List<ValueParameter>();
            SelectColumns = new List<string>();
            OrderByParameters = new List<OrderByParameter>();
            QueryParameters = new DynamicParameters();
            HasIdentity = false;
        }

        /// <summary>
        /// All columns defined on the inheriting type. This is property must be set in the 
        /// constructor of the inheriting type and is used when generating queries.
        /// </summary>
        public abstract IEnumerable<string> Columns { get; }

        /// <summary>
        /// The schema name that corresponds to the inheriting type. This property must be 
        /// set in the constructor of the inheriting type and is used when generating queries.
        /// </summary>
        protected string Schema { get; set; }

        /// <summary>
        /// The table name that corresponds to the inheriting type. This property must be 
        /// set in the constructor of the inheriting type and is used when generating queries.
        /// </summary>
        protected string TableName { get; set; }

        /// <summary>
        /// All columns that make up the primary key that corresponds to the table of the 
        /// inheriting type. This property must be set in the constructor of the inheriting 
        /// type and is used when generating queries.
        /// </summary>
        protected List<string> PrimaryKeyColumns { get; set; }

        /// <summary>
        /// Flag to indicate if the table of the inheriting type has an IDENTITY column
        /// (typically the primary key column)
        /// </summary>
        public bool HasIdentity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<WhereParameterBase> WhereParameters { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<ValueParameter> UpsertParameters { get; set; }

        /// <summary>
        /// List of SQL parameters to be used in the stored procedure
        /// <example>
        /// StoredProcedureParameters.Add("@b", dbType: DbType.Int32, direction: ParameterDirection.Output);
        /// </example>
        /// </summary>
        public DynamicParameters QueryParameters { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> SelectColumns { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual List<OrderByParameter> OrderByParameters { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long StartRow { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long MaxRowsToReturn { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<T> Select()
        {
            using (System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(_connectionString))
            {
                DynamicParameters parms = GetWhereParameters();
                string sql = GenerateSelectSql();

                return con.Query<T>(sql, parms);
            }
        }

        /// <summary>
        /// Returns the record count that matches the specified criteria as the type specified (must be numeric: short, int, long, uint, etc.)
        /// </summary>
        /// <typeparam name="U">Numeric type of the return value (short, int, long, etc.)</typeparam>
        /// <returns></returns>
        public virtual U SelectCount<U>() where U : struct
        {
            using (System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(_connectionString))
            {
                DynamicParameters parms = GetWhereParameters();
                string sql = GenerateSelectCountSql();

                return con.ExecuteScalar<U>(sql, parms);
            }
        }

        /// <summary>
        /// Inserts the InsertUpdate values and returns the identity of the inserted row
        /// </summary>
        /// <returns>int or long</returns>
        public virtual U Insert<U>() where U : struct
        {
            using (System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(_connectionString))
            {
                Dapper.SqlMapper.IDynamicParameters parms = GetInsertUpdateParameters();
                string sql = GenerateInsertSql();

                return con.Query<U>(sql, parms).First();
            }
        }

        /// <summary>
        /// Inserts the InsertUpdate values and returns the identity of the inserted row
        /// </summary>
        /// <returns>int or long</returns>
        public virtual int Insert()
        {
            using (System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(_connectionString))
            {
                Dapper.SqlMapper.IDynamicParameters parms = GetInsertUpdateParameters();
                string sql = GenerateInsertSql();

                return con.Execute(sql, parms);
            }
        }

        /// <summary>
        /// Updates the InsertUpdate values of the row specified and returns the number of rows affected
        /// </summary>
        /// <returns>int or long</returns>
        public virtual bool Update(long primaryKey)
        {
            using (System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(_connectionString))
            {
                DynamicParameters parms = GetInsertUpdateParameters();
                parms.Add(PrimaryKeyColumns.First(), primaryKey);

                string sql = GenerateUpdateSql(primaryKey);

                return con.Query<long>(sql, parms).First() > 0;
            }
        }

        /// <summary>
        /// Updates the InsertUpdate values of the row specified and returns the number of rows affected
        /// </summary>
        /// <returns>int or long</returns>
        public virtual bool Update(Dictionary<string, object> primaryKeys)
        {
            using (System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(_connectionString))
            {
                DynamicParameters parms = GetInsertUpdateParameters();

                foreach (string key in primaryKeys.Keys)
                {
                    parms.Add(key, primaryKeys[key]);
                }

                string sql = GenerateUpdateSql(primaryKeys);

                return con.Query<long>(sql, parms).First() > 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public virtual bool Delete(long primaryKey)
        {
            using (System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(_connectionString))
            {
                string sql = GenerateDeleteSql(primaryKey);

                return con.Query<long>(sql).First() > 0;
            }
        }

        /// <summary>
        /// Executes the specified stored procedure with any parameters in the StoredProcedureParameters collection and returns the output as the specified type. Output parameters
        /// and ReturnValues can be retrieved from the StoredProcedureParameters collection
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="procedureName">The name of the stored procedure to execute</param>
        /// <returns></returns>
        public virtual IEnumerable<U> ExecuteProcedure<U>(string procedureName)
        {
            using (System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(_connectionString))
            {
                return con.Query<U>(procedureName, QueryParameters, commandType: System.Data.CommandType.StoredProcedure);
            }
        }

        /// <summary>
        /// Executes a SQL statement that returns multiple result sets and returns a Dapper.SqlMapper.GridReader containing the results.
        /// Be sure to dispose of the GridReader and SqlConnection.
        /// </summary>
        /// <param name="con"></param>
        /// <param name="sql"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        /// <example>
        /// using (System.Data.SqlClient.SqlConnection con = ...
        ///        {
        ///            using (var multi = query.ExecuteMultiple(con, sql, CommandType.Text))
        ///            {
        ///             var hours = multi.Read();
        ///             var phones = multi.Read();
        ///             var options = multi.Read();
        ///            }
        ///        }
        /// </example>
        public virtual Dapper.SqlMapper.GridReader ExecuteMultiple(System.Data.SqlClient.SqlConnection con, string sql, System.Data.CommandType commandType)
        {
            return con.QueryMultiple(sql, QueryParameters, commandType: commandType);
        }

        /// <summary>
        /// Executes the specified stored procedure with any parameters in the StoredProcedureParameters collection. Output parameters and ReturnValues can be retrieved from 
        /// the StoredProcedureParameters collection
        /// </summary>
        /// <param name="procedureName"></param>
        public virtual void ExecuteProcedure(string procedureName)
        {
            using (System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(_connectionString))
            {
                con.Query(procedureName, QueryParameters, commandType: System.Data.CommandType.StoredProcedure);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="sql"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public virtual IEnumerable<U> ExecuteQuery<U>(string sql, System.Data.CommandType commandType)
        {
            using (System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(_connectionString))
            {
                return con.Query<U>(sql, QueryParameters, commandType: commandType);
            }
        }

        /// <summary>
        /// Generate an update statement from schema metadata and user parameters
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        protected string GenerateUpdateSql(long primaryKey)
        {
            string sql = string.Format("UPDATE [{0}].[{1}] SET {2} WHERE [{3}] = @{3}; SELECT @@ROWCOUNT;",
                Schema,
                TableName,
                GetUpdateValues(),
                PrimaryKeyColumns.First());

            return sql;
        }

        /// <summary>
        /// Generate an update statement from schema metadata and user parameters
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        protected string GenerateUpdateSql(Dictionary<string, object> primaryKeys)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("UPDATE [{0}].[{1}] SET {2} WHERE ",
                Schema,
                TableName,
                GetUpdateValues(),
                PrimaryKeyColumns.First());

            foreach (string key in primaryKeys.Keys)
            {
                if (sql.Length > 0) sql.Append(" AND");
                sql.AppendFormat(" [{0}] = @{0}", key, primaryKeys[key]);
            }

            sql.AppendLine(" SELECT @@ROWCOUNT;");

            return sql.ToString();
        }

        /// <summary>
        /// Generate an insert statement from schema metadata and user parameters
        /// </summary>
        /// <returns></returns>
        protected string GenerateInsertSql()
        {
            IEnumerable<string> insertColumns = GetInsertColumns();
            string sql = string.Format("INSERT INTO [{0}].[{1}] ({2}) VALUES ({3});{4}",
                Schema,
                TableName,
                string.Join(", ", UpsertParameters.Select(i => string.Format("[{0}]", i.Name))),
                GetInsertValues(), 
                HasIdentity ? " SELECT SCOPE_IDENTITY();" : "");

            return sql;
        }

        /// <summary>
        /// Generate a delete statement from schema metadata and user parameters
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        protected string GenerateDeleteSql(long primaryKey)
        {
            string sql = string.Format("DELETE FROM [{0}].[{1}] WHERE [{2}] = {3}; SELECT @@ROWCOUNT;",
                Schema,
                TableName,
                PrimaryKeyColumns.First(),
                primaryKey);

            return sql;
        }

        private string GetUpdateValues()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            foreach (ValueParameter item in UpsertParameters)
            {
                if (sb.Length > 0)
                {
                    sb.Append(", ");
                }

                sb.AppendFormat("[{0}] = @{0}", item.Name);
            }

            return sb.ToString();
        }

        private string GetInsertValues()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            foreach (ValueParameter item in UpsertParameters)
            {
                if (sb.Length > 0)
                {
                    sb.Append(", ");
                }

                sb.AppendFormat("@{0}", item.Name);
            }

            return sb.ToString();
        }

        private IEnumerable<string> GetInsertColumns()
        {
            return Columns.Where(i => !PrimaryKeyColumns.Contains(i));
        }

        /// <summary>
        /// Generate a select count statement from schema metadata and user parameters
        /// </summary>
        /// <returns></returns>
        protected string GenerateSelectCountSql()
        {
            return string.Format("SELECT COUNT([{0}]) FROM [{1}].[{2}] {3}",
                PrimaryKeyColumns.First(),
                Schema,
                TableName,
                GetWhereClause());
        }

        /// <summary>
        /// Generate a select statement from schema metadata and user parameters
        /// </summary>
        /// <returns></returns>
        protected string GenerateSelectSql()
        {
            //generate the paging sql if necessary
            if (StartRow >= 0 && MaxRowsToReturn > 0)
            {
                return string.Format("SELECT {0} FROM (SELECT ROW_NUMBER() OVER ({3}) AS rn, {0} FROM [{6}].[{1}] {2}) AS t0 WHERE rn BETWEEN {4} AND {5}",
                    SelectColumns.Any() ? string.Join(", ", SelectColumns.Select(i => string.Format("[{0}]", i))) : string.Join(", ", Columns.Select(i => string.Format("[{0}]", i))),
                    TableName,
                    GetWhereClause(),
                    GetOrderBy(),
                    StartRow,
                    MaxRowsToReturn,
                    Schema);
            }

            //generate non-paged sql
            return string.Format("SELECT {0} FROM [{1}].[{2}] {3} {4}",
                SelectColumns.Any() ? string.Join(", ", SelectColumns.Select(i => string.Format("[{0}]", i))) : string.Join(", ", Columns.Select(i => string.Format("[{0}]", i))),
                Schema,
                TableName,
                GetWhereClause(),
                GetOrderBy());
        }

        private string GetOrderBy()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            //use the specified order by if provided
            if (OrderByParameters.Any())
            {
                foreach (OrderByParameter item in OrderByParameters)
                {
                    if (sb.Length > 0)
                    {
                        sb.Append(", ");
                    }
                    else
                    {
                        sb.Append("ORDER BY ");
                    }

                    sb.AppendFormat("[{0}] {1}", item.SortColumnName, item.Direction == SortDirection.Ascending ? "ASC" : "DESC");
                }
                return sb.ToString();
            }

            //default order by to primary key
            foreach (string item in PrimaryKeyColumns)
            {
                if (sb.Length > 0)
                {
                    sb.Append(", ");
                }
                else
                {
                    sb.Append("ORDER BY ");
                }

                sb.AppendFormat("[{0}] ASC", item);
            }
            return sb.ToString();
        }

        private string GetWhereClause()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            bool addAnd = false;

            foreach (WhereParameter item in WhereParameters.OfType<WhereParameter>())
            {
                if (addAnd)
                {
                    sb.Append(" AND ");
                }

                switch (item.Operand)
                {
                    case WhereOperand.Equal:
                        sb.AppendFormat("[{0}] = @{0}", item.Name);
                        break;
                    case WhereOperand.NotEqual:
                        sb.AppendFormat("[{0}] <> @{0}", item.Name);
                        break;
                    case WhereOperand.IsNull:
                        sb.AppendFormat("[{0}] IS NULL", item.Name);
                        break;
                    case WhereOperand.IsNotNull:
                        sb.AppendFormat("[{0}] IS NOT NULL", item.Name);
                        break;
                    default:
                        break;
                }

                addAnd = true;
            }

            foreach (BetweenWhereParameter item in WhereParameters.OfType<BetweenWhereParameter>())
            {
                if (addAnd)
                {
                    sb.Append(" AND ");
                }
                sb.AppendFormat("[{0}] BETWEEN @{0}_start AND @{0}_end", item.Name);
            }

            if (sb.Length > 0)
            {
                sb.Insert(0, "WHERE ");
            }

            return sb.ToString();
        }

        private DynamicParameters GetInsertUpdateParameters()
        {
            Dapper.DynamicParameters parms = new Dapper.DynamicParameters();
            foreach (ValueParameter item in UpsertParameters)
            {
                parms.Add(item.Name, item.Value);
            }
            return parms;
        }

        private DynamicParameters GetWhereParameters()
        {
            Dapper.DynamicParameters parms = new Dapper.DynamicParameters();

            foreach (WhereParameter item in WhereParameters.OfType<WhereParameter>())
            {
                parms.Add(item.Name, item.Value);
            }
            foreach (BetweenWhereParameter item in WhereParameters.OfType<BetweenWhereParameter>())
            {
                parms.Add(string.Format("{0}_start", item.Name), item.StartValue);
                parms.Add(string.Format("{0}_end", item.Name), item.EndValue);
            }
            return parms;
        }
    }
}
