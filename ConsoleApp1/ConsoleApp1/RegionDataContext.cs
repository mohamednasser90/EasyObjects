using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApp1
{
    public class RegionDataContext
    {
        private static readonly RegionDataContext instance = new RegionDataContext();
        private string _connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=Northwind;Integrated Security=True;";

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        // Adapted from Jon Skeet: http://csharpindepth.com/Articles/General/Singleton.aspx
        static RegionDataContext() { }

        public RegionDataContext() { }

        public static RegionDataContext Instance { get { return instance; } }

        /// <summary>
        /// Common Select Method for Region
        /// </summary>
        /// <returns></returns>
        public virtual List<Region> Select()
        {
            return Select(null);
        }

        /// <summary>
        /// Common SelectByPk Method for Region
        /// </summary>
        /// <returns></returns>
        public virtual Region SelectByPk(int id)
        {
            return Select(new Region { RegionID = id }).FirstOrDefault();
        }

        /// <summary>
        /// Common Select Method for Region
        /// </summary>
        /// <param name="sortExpression"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual List<Region> Select(Region model)
        {
            RegionQuery query = new RegionQuery(_connectionString);
            SetSelectQueryParams(model, query);

            return query.Select().ToList();
        }

        /// <summary>
        /// Insert Method for Region
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual void Insert(Region model)
        {
            RegionQuery query = new RegionQuery(_connectionString);
            SetInsertUpdateQueryParams(model, query);
            int rows = query.Insert();

            if (rows == 0)
            {
                throw new InvalidOperationException("Insert statement affected 0 rows");
            }
        }

        /// <summary>
        /// Update method for Region
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual void Update(Region model)
        {
            RegionQuery query = new RegionQuery(_connectionString);
            SetInsertUpdateQueryParams(model, query);

            if (!query.Update(model.RegionID))
            {
                throw new InvalidOperationException("Update statement affected 0 rows");
            }
        }

        /// <summary>
        /// Delete Method for ActivationRequest
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual void Delete(Region model)
        {
            RegionQuery query = new RegionQuery(_connectionString);

            if (!query.Delete(model.RegionID))
            {
                throw new InvalidOperationException("Delete statement affected 0 rows");
            }
        }

        /// <summary>
        /// Common By Id Delete Method for ActivationRequest
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual void DeleteByPk(int id)
        {
            Delete(new Region { RegionID = id });
        }

        /// <summary>
        /// Sets the WHERE parameters on the SQL query from the passed in item
        /// </summary>
        /// <param name="model">The model object that holds values we want to query on</param>
        /// <param name="query">The query object for constructing the SQL query</param>
        /// <param name="sortExpression">The columns and directions to sort on using the DataManager supported format</param>
        internal static void SetSelectQueryParams(Region model, RegionQuery query, string sortExpression = null)
        {
            //
            // Parse the DataManager-style sort expression
            //

            if (!string.IsNullOrWhiteSpace(sortExpression))
            {
                ParseSortExpression(query, sortExpression);
            }


            //
            // If no item was passed in then we are doing a SELECT * type of query
            //

            if (model == null)
            {
                return;
            }


            //
            // Primary key columns
            //

            if (model.RegionID > default(int))
            {
                query.WhereParameters.Add(new WhereParameter() { Name = RegionQuery.ColumnNames.RegionID, Value = model.RegionID, Operand = WhereOperand.Equal });
            }


            //
            // Member columns
            //

            if (!string.IsNullOrWhiteSpace(model.RegionDescription))
            {
                query.WhereParameters.Add(new WhereParameter() { Name = RegionQuery.ColumnNames.RegionDescription, Value = model.RegionDescription, Operand = WhereOperand.Equal });
            }
        }

        /// <summary>
        /// Adds the ValueParameters to the SQL query
        /// </summary>
        /// <param name="item">The model object that holds values we want to query on</param>
        /// <param name="query">The query object for constructing the INSERT/UPDATE statement for the SQL query</param>
        internal static void SetInsertUpdateQueryParams(Region item, RegionQuery query)
        {
            //
            // Primary key columns (not including IDENTITY)
            //            

            query.UpsertParameters.Add(CreateNullableParameter(RegionQuery.ColumnNames.RegionID, item.RegionID));

            //
            // Foreign key columns
            //            

            //
            // Member columns
            //

            query.UpsertParameters.Add(CreateNullableParameter(RegionQuery.ColumnNames.RegionDescription, item.RegionDescription));
        }

        /// <summary>
        /// Adds the ORDER BY parameters to the SQL query
        /// </summary>
        /// <param name="query">The query object for constructing the SQL query</param>
        /// <param name="sortExpression">The columns and directions to sort on using the DataManager supported format</param>
        internal static void ParseSortExpression(RegionQuery query, string sortExpression)
        {
            if (string.IsNullOrWhiteSpace(sortExpression))
            {
                return;
            }

            string[] columns = sortExpression.Split(new[] { ',' });

            foreach (string col in columns)
            {
                string[] pair = col.Split(new[] { ' ' });

                OrderByParameter param = new OrderByParameter
                {
                    SortColumnName = pair[0],
                    Direction = SortDirection.Ascending
                };

                if (pair.Length == 2 && !string.IsNullOrWhiteSpace(pair[1]) && pair[1].Contains("DESC"))
                {
                    param.Direction = SortDirection.Descending;
                }

                query.OrderByParameters.Add(param);
            }
        }

        /// <summary>
        /// Creates a ValueParameter and will use the null value if 
        /// the value is of a specific type and value combination.
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="value">
        /// If this value is a numeric type and the value is -1 a null will be used.
        /// If this value is of type string and the value is an empty string "" or null then null will be used.
        /// </param>
        /// <returns></returns>
        internal static ValueParameter CreateNullableParameter(string columnName, object value)
        {
            var nullParameter = new ValueParameter() { Name = columnName, Value = null };

            if (value == null)
                return nullParameter;

            if ((value.GetType() == typeof(Int16) || value.GetType() == typeof(Int32) ||
                value.GetType() == typeof(Int64)) && (value.ToString() == "-1"))
                return nullParameter;

            if (value.GetType() == typeof(string) && string.IsNullOrEmpty(value.ToString()))
                return nullParameter;

            if (value.GetType() == typeof(Guid) && (Guid)value == default(Guid))
                return nullParameter;

            if (value.GetType() == typeof(DateTime) && (DateTime)value == default(DateTime))
                return nullParameter;

            return new ValueParameter() { Name = columnName, Value = value }; ;
        }
    }
}
