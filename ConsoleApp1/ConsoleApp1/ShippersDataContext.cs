using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApp1
{
    public class ShippersDataContext
    {
        private static readonly ShippersDataContext instance = new ShippersDataContext();
        private string _connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=Northwind;Integrated Security=True;";

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        // Adapted from Jon Skeet: http://csharpindepth.com/Articles/General/Singleton.aspx
        static ShippersDataContext() { }

        public ShippersDataContext() { }

        public static ShippersDataContext Instance { get { return instance; } }

        /// <summary>
        /// Common Select Method for Shippers
        /// </summary>
        /// <returns></returns>
        public virtual List<Shippers> Select()
        {
            return Select(null);
        }

        /// <summary>
        /// Common SelectByPk Method for Shippers
        /// </summary>
        /// <returns></returns>
        public virtual Shippers SelectByPk(int id)
        {
            return Select(new Shippers { ShipperID = id }).FirstOrDefault();
        }

        /// <summary>
        /// Common Select Method for Shippers
        /// </summary>
        /// <param name="sortExpression"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual List<Shippers> Select(Shippers model)
        {
            ShippersQuery query = new ShippersQuery(_connectionString);
            SetSelectQueryParams(model, query);

            return query.Select().ToList();
        }

        /// <summary>
        /// Insert Method for Shippers
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual void Insert(Shippers model)
        {
            ShippersQuery query = new ShippersQuery(_connectionString);
            SetInsertUpdateQueryParams(model, query);
            model.ShipperID = query.Insert<int>();

            if (model.ShipperID == default(int))
            {
                throw new InvalidOperationException("Insert statement affected 0 rows");
            }
        }

        /// <summary>
        /// Update method for Shippers
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual void Update(Shippers model)
        {
            ShippersQuery query = new ShippersQuery(_connectionString);
            SetInsertUpdateQueryParams(model, query);

            if (!query.Update(model.ShipperID))
            {
                throw new InvalidOperationException("Update statement affected 0 rows");
            }
        }

        /// <summary>
        /// Delete Method for ActivationRequest
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual void Delete(Shippers model)
        {
            ShippersQuery query = new ShippersQuery(_connectionString);

            if (!query.Delete(model.ShipperID))
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
            Delete(new Shippers { ShipperID = id });
        }

        /// <summary>
        /// Sets the WHERE parameters on the SQL query from the passed in item
        /// </summary>
        /// <param name="model">The model object that holds values we want to query on</param>
        /// <param name="query">The query object for constructing the SQL query</param>
        /// <param name="sortExpression">The columns and directions to sort on using the DataManager supported format</param>
        internal static void SetSelectQueryParams(Shippers model, ShippersQuery query, string sortExpression = null)
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

            if (model.ShipperID > default(int))
            {
                query.WhereParameters.Add(new WhereParameter() { Name = ShippersQuery.ColumnNames.ShipperID, Value = model.ShipperID, Operand = WhereOperand.Equal });
            }


            //
            // Member columns
            //

            if (!string.IsNullOrWhiteSpace(model.CompanyName))
            {
                query.WhereParameters.Add(new WhereParameter() { Name = ShippersQuery.ColumnNames.CompanyName, Value = model.CompanyName, Operand = WhereOperand.Equal });
            }

            if (!string.IsNullOrWhiteSpace(model.Phone))
            {
                query.WhereParameters.Add(new WhereParameter() { Name = ShippersQuery.ColumnNames.Phone, Value = model.Phone, Operand = WhereOperand.Equal });
            }
        }

        /// <summary>
        /// Adds the ValueParameters to the SQL query
        /// </summary>
        /// <param name="item">The model object that holds values we want to query on</param>
        /// <param name="query">The query object for constructing the INSERT/UPDATE statement for the SQL query</param>
        internal static void SetInsertUpdateQueryParams(Shippers item, ShippersQuery query)
        {
            //
            // Primary key columns (not including IDENTITY)
            //            

            //
            // Foreign key columns
            //            

            //
            // Member columns
            //

            query.UpsertParameters.Add(CreateNullableParameter(ShippersQuery.ColumnNames.CompanyName, item.CompanyName));
            query.UpsertParameters.Add(CreateNullableParameter(ShippersQuery.ColumnNames.Phone, item.Phone));
        }

        /// <summary>
        /// Adds the ORDER BY parameters to the SQL query
        /// </summary>
        /// <param name="query">The query object for constructing the SQL query</param>
        /// <param name="sortExpression">The columns and directions to sort on using the DataManager supported format</param>
        internal static void ParseSortExpression(ShippersQuery query, string sortExpression)
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
