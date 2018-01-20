﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
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

        public static bool HasAutoKey { get => false; }
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
			switch(this.DefaultCommandType)
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
			switch(this.DefaultCommandType)
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

		protected override DbCommand GetInsertCommand(CommandType commandType)
		{	
			DbCommand dbCommand;

			switch(commandType)
			{
				case CommandType.StoredProcedure:
					string sqlCommand = this.SchemaStoredProcedureWithSeparator + "daab_AddCustomers";
                    dbCommand = new SqlCommand
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandText = sqlCommand,
                        CommandTimeout = CommandTimeout > 0 ? CommandTimeout : 0
                    };

					CreateParameters(dbCommand);
					
					return dbCommand;

				case CommandType.Text:
					this.Query.ClearAll();
					this.Where.WhereClauseReset();
					foreach(SchemaItem item in this.SchemaEntries)
					{
						if (!item.IsComputed)
						{
							if ((item.IsAutoKey && this.IdentityInsert) || !item.IsAutoKey)
							{
								this.Query.AddInsertColumn(item);
							}
						}
					}
					dbCommand = this.Query.GetInsertCommandWrapper();

					dbCommand.Parameters.Clear();
					if (this.IdentityInsert)
					{
					}
					else
					{
					}
					CreateParameters(dbCommand);

					return dbCommand;

				default:
					throw new ArgumentException("Invalid CommandType", "commandType");
			}
		}

		protected override DbCommand GetUpdateCommand(CommandType commandType)
		{
            DbCommand dbCommand;

			switch(commandType)
			{
				case CommandType.StoredProcedure:
					string sqlCommand = this.SchemaStoredProcedureWithSeparator + "daab_UpdateCustomers";
                    dbCommand = new SqlCommand
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandText = sqlCommand,
                        CommandTimeout = CommandTimeout > 0 ? CommandTimeout : 0
                    };

					CreateParameters(dbCommand);
					
					return dbCommand;

				case CommandType.Text:
					this.Query.ClearAll();
					foreach(SchemaItem item in this.SchemaEntries)
					{
						if (!(item.IsAutoKey || item.IsComputed))
						{
							this.Query.AddUpdateColumn(item);
						}
					}

					this.Where.WhereClauseReset();
					this.Where.CustomerID.Operator = WhereParameter.Operand.Equal;
					dbCommand = this.Query.GetUpdateCommandWrapper();

					dbCommand.Parameters.Clear();
					CreateParameters(dbCommand);
					
					return dbCommand;

				default:
					throw new ArgumentException("Invalid CommandType", "commandType");
			}
		}

		protected override DbCommand GetDeleteCommand(CommandType commandType)
		{
            DbCommand dbCommand;

			switch(commandType)
			{
				case CommandType.StoredProcedure:
					string sqlCommand = this.SchemaStoredProcedureWithSeparator + "daab_DeleteCustomers";
                    dbCommand = new SqlCommand
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandText = sqlCommand,
                        CommandTimeout = CommandTimeout > 0 ? CommandTimeout : 0
                    };

					base.AddInParameter(dbCommand, "CustomerID", DbType.StringFixedLength, "CustomerID", DataRowVersion.Current);
					
					return dbCommand;

				case CommandType.Text:
					this.Query.ClearAll();
					this.Where.WhereClauseReset();
					this.Where.CustomerID.Operator = WhereParameter.Operand.Equal;
					dbCommand = this.Query.GetDeleteCommandWrapper();

					dbCommand.Parameters.Clear();
					base.AddInParameter(dbCommand, "CustomerID", DbType.StringFixedLength, "CustomerID", DataRowVersion.Current);
					
					return dbCommand;

				default:
					throw new ArgumentException("Invalid CommandType", "commandType");
			}
		}

		private void CreateParameters(DbCommand dbCommand)
		{
			base.AddInParameter(dbCommand, "CustomerID", DbType.StringFixedLength, "CustomerID", DataRowVersion.Current);
			base.AddInParameter(dbCommand, "CompanyName", DbType.String, "CompanyName", DataRowVersion.Current);
			base.AddInParameter(dbCommand, "ContactName", DbType.String, "ContactName", DataRowVersion.Current);
			base.AddInParameter(dbCommand, "ContactTitle", DbType.String, "ContactTitle", DataRowVersion.Current);
			base.AddInParameter(dbCommand, "Address", DbType.String, "Address", DataRowVersion.Current);
			base.AddInParameter(dbCommand, "City", DbType.String, "City", DataRowVersion.Current);
			base.AddInParameter(dbCommand, "Region", DbType.String, "Region", DataRowVersion.Current);
			base.AddInParameter(dbCommand, "PostalCode", DbType.String, "PostalCode", DataRowVersion.Current);
			base.AddInParameter(dbCommand, "Country", DbType.String, "Country", DataRowVersion.Current);
			base.AddInParameter(dbCommand, "Phone", DbType.String, "Phone", DataRowVersion.Current);
			base.AddInParameter(dbCommand, "Fax", DbType.String, "Fax", DataRowVersion.Current);
		}

		#region Properties
						public virtual string CustomerID
						{
							get
							{
				return this.GetString(CustomersSchema.CustomerID.FieldName);
					    	}
							set
							{
				this.SetString(CustomersSchema.CustomerID.FieldName, value);
							}
						}
						public virtual string CompanyName
						{
							get
							{
				return this.GetString(CustomersSchema.CompanyName.FieldName);
					    	}
							set
							{
				this.SetString(CustomersSchema.CompanyName.FieldName, value);
							}
						}
						public virtual string ContactName
						{
							get
							{
				return this.GetString(CustomersSchema.ContactName.FieldName);
					    	}
							set
							{
				this.SetString(CustomersSchema.ContactName.FieldName, value);
							}
						}
						public virtual string ContactTitle
						{
							get
							{
				return this.GetString(CustomersSchema.ContactTitle.FieldName);
					    	}
							set
							{
				this.SetString(CustomersSchema.ContactTitle.FieldName, value);
							}
						}
						public virtual string Address
						{
							get
							{
				return this.GetString(CustomersSchema.Address.FieldName);
					    	}
							set
							{
				this.SetString(CustomersSchema.Address.FieldName, value);
							}
						}
						public virtual string City
						{
							get
							{
				return this.GetString(CustomersSchema.City.FieldName);
					    	}
							set
							{
				this.SetString(CustomersSchema.City.FieldName, value);
							}
						}
						public virtual string Region
						{
							get
							{
				return this.GetString(CustomersSchema.Region.FieldName);
					    	}
							set
							{
				this.SetString(CustomersSchema.Region.FieldName, value);
							}
						}
						public virtual string PostalCode
						{
							get
							{
				return this.GetString(CustomersSchema.PostalCode.FieldName);
					    	}
							set
							{
				this.SetString(CustomersSchema.PostalCode.FieldName, value);
							}
						}
						public virtual string Country
						{
							get
							{
				return this.GetString(CustomersSchema.Country.FieldName);
					    	}
							set
							{
				this.SetString(CustomersSchema.Country.FieldName, value);
							}
						}
						public virtual string Phone
						{
							get
							{
				return this.GetString(CustomersSchema.Phone.FieldName);
					    	}
							set
							{
				this.SetString(CustomersSchema.Phone.FieldName, value);
							}
						}
						public virtual string Fax
						{
							get
							{
				return this.GetString(CustomersSchema.Fax.FieldName);
					    	}
							set
							{
				this.SetString(CustomersSchema.Fax.FieldName, value);
							}
						}
		public override string TableName
		{
			get { return "Customers"; }
		}
		
		#endregion		
		
		#region String Properties
#pragma warning disable IDE1006 // Naming Styles
		public virtual string s_CustomerID
		{
			get
			{
				return this.IsColumnNull("CustomersSchema.CustomerID.FieldName") ? string.Empty : base.GetStringAsString(CustomersSchema.CustomerID.FieldName);
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this.SetColumnNull(CustomersSchema.CustomerID.FieldName);
				}
				else
				{
					this.CustomerID = base.SetStringAsString(CustomersSchema.CustomerID.FieldName, value);
				}
			}
		}
		public virtual string s_CompanyName
		{
			get
			{
				return this.IsColumnNull("CustomersSchema.CompanyName.FieldName") ? string.Empty : base.GetStringAsString(CustomersSchema.CompanyName.FieldName);
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this.SetColumnNull(CustomersSchema.CompanyName.FieldName);
				}
				else
				{
					this.CompanyName = base.SetStringAsString(CustomersSchema.CompanyName.FieldName, value);
				}
			}
		}
		public virtual string s_ContactName
		{
			get
			{
				return this.IsColumnNull("CustomersSchema.ContactName.FieldName") ? string.Empty : base.GetStringAsString(CustomersSchema.ContactName.FieldName);
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this.SetColumnNull(CustomersSchema.ContactName.FieldName);
				}
				else
				{
					this.ContactName = base.SetStringAsString(CustomersSchema.ContactName.FieldName, value);
				}
			}
		}
		public virtual string s_ContactTitle
		{
			get
			{
				return this.IsColumnNull("CustomersSchema.ContactTitle.FieldName") ? string.Empty : base.GetStringAsString(CustomersSchema.ContactTitle.FieldName);
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this.SetColumnNull(CustomersSchema.ContactTitle.FieldName);
				}
				else
				{
					this.ContactTitle = base.SetStringAsString(CustomersSchema.ContactTitle.FieldName, value);
				}
			}
		}
		public virtual string s_Address
		{
			get
			{
				return this.IsColumnNull("CustomersSchema.Address.FieldName") ? string.Empty : base.GetStringAsString(CustomersSchema.Address.FieldName);
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this.SetColumnNull(CustomersSchema.Address.FieldName);
				}
				else
				{
					this.Address = base.SetStringAsString(CustomersSchema.Address.FieldName, value);
				}
			}
		}
		public virtual string s_City
		{
			get
			{
				return this.IsColumnNull("CustomersSchema.City.FieldName") ? string.Empty : base.GetStringAsString(CustomersSchema.City.FieldName);
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this.SetColumnNull(CustomersSchema.City.FieldName);
				}
				else
				{
					this.City = base.SetStringAsString(CustomersSchema.City.FieldName, value);
				}
			}
		}
		public virtual string s_Region
		{
			get
			{
				return this.IsColumnNull("CustomersSchema.Region.FieldName") ? string.Empty : base.GetStringAsString(CustomersSchema.Region.FieldName);
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this.SetColumnNull(CustomersSchema.Region.FieldName);
				}
				else
				{
					this.Region = base.SetStringAsString(CustomersSchema.Region.FieldName, value);
				}
			}
		}
		public virtual string s_PostalCode
		{
			get
			{
				return this.IsColumnNull("CustomersSchema.PostalCode.FieldName") ? string.Empty : base.GetStringAsString(CustomersSchema.PostalCode.FieldName);
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this.SetColumnNull(CustomersSchema.PostalCode.FieldName);
				}
				else
				{
					this.PostalCode = base.SetStringAsString(CustomersSchema.PostalCode.FieldName, value);
				}
			}
		}
		public virtual string s_Country
		{
			get
			{
				return this.IsColumnNull("CustomersSchema.Country.FieldName") ? string.Empty : base.GetStringAsString(CustomersSchema.Country.FieldName);
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this.SetColumnNull(CustomersSchema.Country.FieldName);
				}
				else
				{
					this.Country = base.SetStringAsString(CustomersSchema.Country.FieldName, value);
				}
			}
		}
		public virtual string s_Phone
		{
			get
			{
				return this.IsColumnNull("CustomersSchema.Phone.FieldName") ? string.Empty : base.GetStringAsString(CustomersSchema.Phone.FieldName);
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this.SetColumnNull(CustomersSchema.Phone.FieldName);
				}
				else
				{
					this.Phone = base.SetStringAsString(CustomersSchema.Phone.FieldName, value);
				}
			}
		}
		public virtual string s_Fax
		{
			get
			{
				return this.IsColumnNull("CustomersSchema.Fax.FieldName") ? string.Empty : base.GetStringAsString(CustomersSchema.Fax.FieldName);
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this.SetColumnNull(CustomersSchema.Fax.FieldName);
				}
				else
				{
					this.Fax = base.SetStringAsString(CustomersSchema.Fax.FieldName, value);
				}
			}
		}
#pragma warning restore IDE1006 // Naming Styles
		#endregion
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
