using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using NCI.EasyObjects;

namespace EasyObjects.Console.BLL
{
    public class CustomersSchema : NCI.EasyObjects.Schema
    {
        private static List<SchemaItem> _entries;
        public static SchemaItem CustomerID = new SchemaItem("CustomerID", DbType.StringFixedLength, SchemaItemJustify.None, 5, false, true, false, false);
        public static SchemaItem CompanyName = new SchemaItem("CompanyName", DbType.String, SchemaItemJustify.None, 40, false, false, false, false);
        public static SchemaItem ContactName = new SchemaItem("ContactName", DbType.String, SchemaItemJustify.None, 30, true, false, false, false);
        public static SchemaItem ContactTitle = new SchemaItem("ContactTitle", DbType.String, SchemaItemJustify.None, 30, true, false, false, false);
        public static SchemaItem Address = new SchemaItem("Address", DbType.String, SchemaItemJustify.None, 60, true, false, false, false);
        public static SchemaItem City = new SchemaItem("City", DbType.String, SchemaItemJustify.None, 15, true, false, false, false);
        public static SchemaItem Region = new SchemaItem("Region", DbType.String, SchemaItemJustify.None, 15, true, false, false, false);
        public static SchemaItem PostalCode = new SchemaItem("PostalCode", DbType.String, SchemaItemJustify.None, 10, true, false, false, false);
        public static SchemaItem Country = new SchemaItem("Country", DbType.String, SchemaItemJustify.None, 15, true, false, false, false);
        public static SchemaItem Phone = new SchemaItem("Phone", DbType.String, SchemaItemJustify.None, 24, true, false, false, false);
        public static SchemaItem Fax = new SchemaItem("Fax", DbType.String, SchemaItemJustify.None, 24, true, false, false, false);

        public override List<SchemaItem> SchemaEntries
        {
            get
            {
                if (_entries == null || _entries.Count == 0)
                {
                    _entries = new List<SchemaItem>
                    {
                        CustomersSchema.CustomerID,
                        CustomersSchema.CompanyName,
                        CustomersSchema.ContactName,
                        CustomersSchema.ContactTitle,
                        CustomersSchema.Address,
                        CustomersSchema.City,
                        CustomersSchema.Region,
                        CustomersSchema.PostalCode,
                        CustomersSchema.Country,
                        CustomersSchema.Phone,
                        CustomersSchema.Fax,
                    };
                }

                return _entries;
            }
        }

        public static bool HasAutoKey { get => true; }
        public static bool HasRowID { get => false; }
    }

    public abstract class _Customers : EasyObject
    {
        public _Customers()
        {
            CustomersSchema _schema = new CustomersSchema();
            this.SchemaEntries = _schema.SchemaEntries;
            this.SchemaGlobal = "dbo";
        }

        public override void FlushData()
        {
            this._whereClause = null;
            this._aggregateClause = null;
            base.FlushData();
        }

        /// <summary>
        /// Loads the business object with info from the database, based on the requested primary key.
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns>A Boolean indicating success or failure of the query</returns>
        public bool LoadByPrimaryKey(string customerID)
        {
            switch (this.DefaultCommandType)
            {
                case CommandType.StoredProcedure:
                    ListDictionary parameters = new ListDictionary
                    {
                        { CustomersSchema.CustomerID.FieldName, customerID },
                    };

                    return base.LoadFromSql(this.SchemaStoredProcedureWithSeparator + "daab_GetCustomers", parameters, CommandType.StoredProcedure);

                case CommandType.Text:
                    this.Query.ClearAll();
                    this.Where.WhereClauseReset();
                    this.Where.CustomerID.Value = customerID;
                    return this.Query.Load();

                default:
                    throw new ArgumentException("Invalid CommandType", "commandType");
            }
        }

        /// <summary>
        /// Loads all records from the table.
        /// </summary>
        /// <returns>A Boolean indicating success or failure of the query</returns>
        public bool LoadAll()
        {
            switch (this.DefaultCommandType)
            {
                case CommandType.StoredProcedure:
                    return base.LoadFromSql(this.SchemaStoredProcedureWithSeparator + "daab_GetAllCustomers", null, CommandType.StoredProcedure);

                case CommandType.Text:
                    this.Query.ClearAll();
                    this.Where.WhereClauseReset();
                    return this.Query.Load();

                default:
                    throw new ArgumentException("Invalid CommandType", "commandType");
            }
        }

        /// <summary>
        /// Adds a new record to the internal table.
        /// </summary>
        public override void AddNew()
        {
            base.AddNew();
            this.ApplyDefaults();
        }

        /// <summary>
        /// Apply any default values to columns
        /// </summary>
        protected override void ApplyDefaults()
        {
        }
    }

    /// <summary>
    /// A class which represents the Customers table
    /// </summary>
	[Table("Customers")]
    public partial class CustomersModel
    {
        [Key]
        public virtual string CustomerID { get; set; }
        public virtual string CompanyName { get; set; }
        public virtual string ContactName { get; set; }
        public virtual string ContactTitle { get; set; }
        public virtual string Address { get; set; }
        public virtual string City { get; set; }
        public virtual string Region { get; set; }
        public virtual string PostalCode { get; set; }
        public virtual string Country { get; set; }
        public virtual string Phone { get; set; }
        public virtual string Fax { get; set; }
    }
}
