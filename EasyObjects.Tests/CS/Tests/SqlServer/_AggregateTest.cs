/*
'===============================================================================
'  Generated From - CSharp_EasyObject_BusinessEntity.vbgen
' 
'  ** IMPORTANT  ** 
'  How to Generate your stored procedures:
' 
'  SQL      = SQL_DAAB_StoredProcs.vbgen
'  
'  This object is 'abstract' which means you need to inherit from it to be able
'  to instantiate it.  This is very easily done. You can override properties and
'  methods in your derived class, this allows you to regenerate this class at any
'  time and not worry about overwriting custom code. 
'
'  NEVER EDIT THIS FILE.
'
'  public class YourObject :  _YourObject
'  {
'
'  }
'
'===============================================================================
*/

// Generated by MyGeneration Version # (1.2.0.2)

using System;
using System.Data;
using System.Data.Common;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;
using System.Xml;
using System.IO;

using Microsoft.Practices.EnterpriseLibrary.Data;
using NCI.EasyObjects;

namespace NCI.EasyObjects.Tests.SQL
{

	#region Schema

	public class AggregateTestSchema : NCI.EasyObjects.Schema
	{
		private static ArrayList _entries;
		public static SchemaItem ID = new SchemaItem("ID", DbType.Int32, true, false, false, true, false, false);
		public static SchemaItem DepartmentID = new SchemaItem("DepartmentID", DbType.Int32, false, true, false, false, false, false);
		public static SchemaItem FirstName = new SchemaItem("FirstName", DbType.AnsiString, SchemaItemJustify.None, 25, true, false, false, false);
		public static SchemaItem LastName = new SchemaItem("LastName", DbType.AnsiString, SchemaItemJustify.None, 15, true, false, false, false);
		public static SchemaItem Age = new SchemaItem("Age", DbType.Int32, false, true, false, false, false, false);
		public static SchemaItem HireDate = new SchemaItem("HireDate", DbType.DateTime, false, true, false, false, false, false);
		public static SchemaItem Salary = new SchemaItem("Salary", DbType.Decimal, false, true, false, false, false, false);
		public static SchemaItem IsActive = new SchemaItem("IsActive", DbType.Boolean, false, true, false, false, false, true);
		public static SchemaItem IsActive2 = new SchemaItem("IsActive2", DbType.AnsiStringFixedLength, SchemaItemJustify.None, 1, false, false, false, true);
		public static SchemaItem DateCreated = new SchemaItem("DateCreated", DbType.DateTime, false, false, false, false, false, true);
		public static SchemaItem Ts = new SchemaItem("ts", DbType.Binary, false, true, true, false, false, false);

		public override ArrayList SchemaEntries
		{
			get
			{
				if (_entries == null )
				{
					_entries = new ArrayList();
					_entries.Add(AggregateTestSchema.ID);
					_entries.Add(AggregateTestSchema.DepartmentID);
					_entries.Add(AggregateTestSchema.FirstName);
					_entries.Add(AggregateTestSchema.LastName);
					_entries.Add(AggregateTestSchema.Age);
					_entries.Add(AggregateTestSchema.HireDate);
					_entries.Add(AggregateTestSchema.Salary);
					_entries.Add(AggregateTestSchema.IsActive);
					_entries.Add(AggregateTestSchema.IsActive2);
					_entries.Add(AggregateTestSchema.DateCreated);
					_entries.Add(AggregateTestSchema.Ts);
					AggregateTestSchema.Ts.IsComputed = true;
				}
				return _entries;
			}
		}
	}
	#endregion

	public abstract class _AggregateTest : EasyObject
	{

		public _AggregateTest()
		{
			AggregateTestSchema _schema = new AggregateTestSchema();
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
		/// <param name="ID"></param>
		/// <returns>A Boolean indicating success or failure of the query</returns>
		public bool LoadByPrimaryKey(int ID)
		{
			switch(this.DefaultCommandType)
			{
				case CommandType.StoredProcedure:
					ListDictionary parameters = new ListDictionary();

					// Add in parameters
					parameters.Add(AggregateTestSchema.ID.FieldName, ID);

					return base.LoadFromSql(this.SchemaStoredProcedureWithSeparator + "daab_GetAggregateTest", parameters, CommandType.StoredProcedure);

				case CommandType.Text:
					this.Query.ClearAll();
					this.Where.WhereClauseReset();
					this.Where.ID.Value = ID;
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
					return base.LoadFromSql(this.SchemaStoredProcedureWithSeparator + "daab_GetAllAggregateTest", null, CommandType.StoredProcedure);

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
			this.IsActive = true;
			this.IsActive2 = "Y";
			this.DateCreated = DateTime.Now;
		}

		protected override DbCommand GetInsertCommand(CommandType commandType)
		{	
			DbCommand dbCommand;

			// Create the Database object, using the default database service. The
			// default database service is determined through configuration.
			Database db = GetDatabase();

			switch(commandType)
			{
				case CommandType.StoredProcedure:
					string sqlCommand = this.SchemaStoredProcedureWithSeparator + "daab_AddAggregateTest";
					dbCommand = db.GetStoredProcCommand(sqlCommand);

					db.AddParameter(dbCommand, "ID", DbType.Int32, 0, ParameterDirection.Output, true, 0, 0, "ID", DataRowVersion.Default, Convert.DBNull);
					CreateParameters(db, dbCommand);
					
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
						db.AddInParameter(dbCommand, "ID", DbType.Int32, "ID", DataRowVersion.Default);
					}
					else
					{
						db.AddParameter(dbCommand, "ID", DbType.Int32, 0, ParameterDirection.Output, true, 0, 0, "ID", DataRowVersion.Default, Convert.DBNull);
					}
					CreateParameters(db, dbCommand);

					return dbCommand;

				default:
					throw new ArgumentException("Invalid CommandType", "commandType");
			}
		}

		protected override DbCommand GetUpdateCommand(CommandType commandType)
		{
            DbCommand dbCommand;

			// Create the Database object, using the default database service. The
			// default database service is determined through configuration.
			Database db = GetDatabase();

			switch(commandType)
			{
				case CommandType.StoredProcedure:
					string sqlCommand = this.SchemaStoredProcedureWithSeparator + "daab_UpdateAggregateTest";
					dbCommand = db.GetStoredProcCommand(sqlCommand);

					db.AddInParameter(dbCommand, "ID", DbType.Int32, "ID", DataRowVersion.Current);
					CreateParameters(db, dbCommand);
					
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
					this.Where.ID.Operator = WhereParameter.Operand.Equal;
					this.Where.Ts.Operator = WhereParameter.Operand.Equal;
					dbCommand = this.Query.GetUpdateCommandWrapper();

					dbCommand.Parameters.Clear();
					CreateParameters(db, dbCommand);
					db.AddInParameter(dbCommand, "ID", DbType.Int32, "ID", DataRowVersion.Current);
					
					return dbCommand;

				default:
					throw new ArgumentException("Invalid CommandType", "commandType");
			}
		}

		protected override DbCommand GetDeleteCommand(CommandType commandType)
		{
            DbCommand dbCommand;

			// Create the Database object, using the default database service. The
			// default database service is determined through configuration.
			Database db = GetDatabase();

			switch(commandType)
			{
				case CommandType.StoredProcedure:
					string sqlCommand = this.SchemaStoredProcedureWithSeparator + "daab_DeleteAggregateTest";
					dbCommand = db.GetStoredProcCommand(sqlCommand);
					db.AddInParameter(dbCommand, "ID", DbType.Int32, "ID", DataRowVersion.Current);
					
					return dbCommand;

				case CommandType.Text:
					this.Query.ClearAll();
					this.Where.WhereClauseReset();
					this.Where.ID.Operator = WhereParameter.Operand.Equal;
					dbCommand = this.Query.GetDeleteCommandWrapper();

					dbCommand.Parameters.Clear();
					db.AddInParameter(dbCommand, "ID", DbType.Int32, "ID", DataRowVersion.Current);
					
					return dbCommand;

				default:
					throw new ArgumentException("Invalid CommandType", "commandType");
			}
		}

		private void CreateParameters(Database db, DbCommand dbCommand)
		{
			db.AddInParameter(dbCommand, "DepartmentID", DbType.Int32, "DepartmentID", DataRowVersion.Current);
			db.AddInParameter(dbCommand, "FirstName", DbType.AnsiString, "FirstName", DataRowVersion.Current);
			db.AddInParameter(dbCommand, "LastName", DbType.AnsiString, "LastName", DataRowVersion.Current);
			db.AddInParameter(dbCommand, "Age", DbType.Int32, "Age", DataRowVersion.Current);
			db.AddInParameter(dbCommand, "HireDate", DbType.DateTime, "HireDate", DataRowVersion.Current);
			db.AddInParameter(dbCommand, "Salary", DbType.Decimal, "Salary", DataRowVersion.Current);
			db.AddInParameter(dbCommand, "IsActive", DbType.Boolean, "IsActive", DataRowVersion.Current);
			db.AddInParameter(dbCommand, "IsActive2", DbType.AnsiStringFixedLength, "IsActive2", DataRowVersion.Current);
			db.AddInParameter(dbCommand, "DateCreated", DbType.DateTime, "DateCreated", DataRowVersion.Current);
			db.AddParameter(dbCommand, "ts", DbType.Binary, 8, ParameterDirection.InputOutput, false, 0, 0, "ts", DataRowVersion.Current, null);
		}
		
		#region Properties
		public virtual int ID
		{
			get
			{
				return this.GetInteger(AggregateTestSchema.ID.FieldName);
	    	}
			set
			{
				this.SetInteger(AggregateTestSchema.ID.FieldName, value);
			}
		}
		public virtual int DepartmentID
		{
			get
			{
				return this.GetInteger(AggregateTestSchema.DepartmentID.FieldName);
	    	}
			set
			{
				this.SetInteger(AggregateTestSchema.DepartmentID.FieldName, value);
			}
		}
		public virtual string FirstName
		{
			get
			{
				return this.GetString(AggregateTestSchema.FirstName.FieldName);
	    	}
			set
			{
				this.SetString(AggregateTestSchema.FirstName.FieldName, value);
			}
		}
		public virtual string LastName
		{
			get
			{
				return this.GetString(AggregateTestSchema.LastName.FieldName);
	    	}
			set
			{
				this.SetString(AggregateTestSchema.LastName.FieldName, value);
			}
		}
		public virtual int Age
		{
			get
			{
				return this.GetInteger(AggregateTestSchema.Age.FieldName);
	    	}
			set
			{
				this.SetInteger(AggregateTestSchema.Age.FieldName, value);
			}
		}
		public virtual DateTime HireDate
		{
			get
			{
				return this.GetDateTime(AggregateTestSchema.HireDate.FieldName);
	    	}
			set
			{
				this.SetDateTime(AggregateTestSchema.HireDate.FieldName, value);
			}
		}
		public virtual decimal Salary
		{
			get
			{
				return this.GetDecimal(AggregateTestSchema.Salary.FieldName);
	    	}
			set
			{
				this.SetDecimal(AggregateTestSchema.Salary.FieldName, value);
			}
		}
		public virtual bool IsActive
		{
			get
			{
				return this.GetBoolean(AggregateTestSchema.IsActive.FieldName);
	    	}
			set
			{
				this.SetBoolean(AggregateTestSchema.IsActive.FieldName, value);
			}
		}
		public virtual string IsActive2
		{
			get
			{
				return this.GetString(AggregateTestSchema.IsActive2.FieldName);
	    	}
			set
			{
				this.SetString(AggregateTestSchema.IsActive2.FieldName, value);
			}
		}
		public virtual DateTime DateCreated
		{
			get
			{
				return this.GetDateTime(AggregateTestSchema.DateCreated.FieldName);
	    	}
			set
			{
				this.SetDateTime(AggregateTestSchema.DateCreated.FieldName, value);
			}
		}
		public virtual byte[] Ts
		{
			get
			{
				return this.GetByteArray(AggregateTestSchema.Ts.FieldName);
	    	}
			set
			{
				this.SetByteArray(AggregateTestSchema.Ts.FieldName, value);
			}
		}

		public override string TableName
		{
			get { return "AggregateTest"; }
		}
		
		#endregion		
		
		#region String Properties
	
		public virtual string s_ID
	    {
			get
	        {
				return this.IsColumnNull(AggregateTestSchema.ID.FieldName) ? string.Empty : base.GetIntegerAsString(AggregateTestSchema.ID.FieldName);
			}
			set
	        {
				if(string.Empty == value)
					this.SetColumnNull(AggregateTestSchema.ID.FieldName);
				else
					this.ID = base.SetIntegerAsString(AggregateTestSchema.ID.FieldName, value);
			}
		}

		public virtual string s_DepartmentID
	    {
			get
	        {
				return this.IsColumnNull(AggregateTestSchema.DepartmentID.FieldName) ? string.Empty : base.GetIntegerAsString(AggregateTestSchema.DepartmentID.FieldName);
			}
			set
	        {
				if(string.Empty == value)
					this.SetColumnNull(AggregateTestSchema.DepartmentID.FieldName);
				else
					this.DepartmentID = base.SetIntegerAsString(AggregateTestSchema.DepartmentID.FieldName, value);
			}
		}

		public virtual string s_FirstName
	    {
			get
	        {
				return this.IsColumnNull(AggregateTestSchema.FirstName.FieldName) ? string.Empty : base.GetStringAsString(AggregateTestSchema.FirstName.FieldName);
			}
			set
	        {
				if(string.Empty == value)
					this.SetColumnNull(AggregateTestSchema.FirstName.FieldName);
				else
					this.FirstName = base.SetStringAsString(AggregateTestSchema.FirstName.FieldName, value);
			}
		}

		public virtual string s_LastName
	    {
			get
	        {
				return this.IsColumnNull(AggregateTestSchema.LastName.FieldName) ? string.Empty : base.GetStringAsString(AggregateTestSchema.LastName.FieldName);
			}
			set
	        {
				if(string.Empty == value)
					this.SetColumnNull(AggregateTestSchema.LastName.FieldName);
				else
					this.LastName = base.SetStringAsString(AggregateTestSchema.LastName.FieldName, value);
			}
		}

		public virtual string s_Age
	    {
			get
	        {
				return this.IsColumnNull(AggregateTestSchema.Age.FieldName) ? string.Empty : base.GetIntegerAsString(AggregateTestSchema.Age.FieldName);
			}
			set
	        {
				if(string.Empty == value)
					this.SetColumnNull(AggregateTestSchema.Age.FieldName);
				else
					this.Age = base.SetIntegerAsString(AggregateTestSchema.Age.FieldName, value);
			}
		}

		public virtual string s_HireDate
	    {
			get
	        {
				return this.IsColumnNull(AggregateTestSchema.HireDate.FieldName) ? string.Empty : base.GetDateTimeAsString(AggregateTestSchema.HireDate.FieldName);
			}
			set
	        {
				if(string.Empty == value)
					this.SetColumnNull(AggregateTestSchema.HireDate.FieldName);
				else
					this.HireDate = base.SetDateTimeAsString(AggregateTestSchema.HireDate.FieldName, value);
			}
		}

		public virtual string s_Salary
	    {
			get
	        {
				return this.IsColumnNull(AggregateTestSchema.Salary.FieldName) ? string.Empty : base.GetDecimalAsString(AggregateTestSchema.Salary.FieldName);
			}
			set
	        {
				if(string.Empty == value)
					this.SetColumnNull(AggregateTestSchema.Salary.FieldName);
				else
					this.Salary = base.SetDecimalAsString(AggregateTestSchema.Salary.FieldName, value);
			}
		}

		public virtual string s_IsActive
	    {
			get
	        {
				return this.IsColumnNull(AggregateTestSchema.IsActive.FieldName) ? string.Empty : base.GetBooleanAsString(AggregateTestSchema.IsActive.FieldName);
			}
			set
	        {
				if(string.Empty == value)
					this.SetColumnNull(AggregateTestSchema.IsActive.FieldName);
				else
					this.IsActive = base.SetBooleanAsString(AggregateTestSchema.IsActive.FieldName, value);
			}
		}

		public virtual string s_IsActive2
	    {
			get
	        {
				return this.IsColumnNull(AggregateTestSchema.IsActive2.FieldName) ? string.Empty : base.GetStringAsString(AggregateTestSchema.IsActive2.FieldName);
			}
			set
	        {
				if(string.Empty == value)
					this.SetColumnNull(AggregateTestSchema.IsActive2.FieldName);
				else
					this.IsActive2 = base.SetStringAsString(AggregateTestSchema.IsActive2.FieldName, value);
			}
		}

		public virtual string s_DateCreated
	    {
			get
	        {
				return this.IsColumnNull(AggregateTestSchema.DateCreated.FieldName) ? string.Empty : base.GetDateTimeAsString(AggregateTestSchema.DateCreated.FieldName);
			}
			set
	        {
				if(string.Empty == value)
					this.SetColumnNull(AggregateTestSchema.DateCreated.FieldName);
				else
					this.DateCreated = base.SetDateTimeAsString(AggregateTestSchema.DateCreated.FieldName, value);
			}
		}


		#endregion		
	
		#region Where Clause
		public class WhereClause
		{
			public WhereClause(EasyObject entity)
			{
				this._entity = entity;
			}
			
			public TearOffWhereParameter TearOff
			{
				get
				{
					if(_tearOff == null)
					{
						_tearOff = new TearOffWhereParameter(this);
					}

					return _tearOff;
				}
			}

			#region TearOff's
			public class TearOffWhereParameter
			{
				public TearOffWhereParameter(WhereClause clause)
				{
					this._clause = clause;
				}
				
				
				public WhereParameter ID
				{
					get
					{
							WhereParameter wp = new WhereParameter(AggregateTestSchema.ID);
							this._clause._entity.Query.AddWhereParameter(wp);
							return wp;
					}
				}

				public WhereParameter DepartmentID
				{
					get
					{
							WhereParameter wp = new WhereParameter(AggregateTestSchema.DepartmentID);
							this._clause._entity.Query.AddWhereParameter(wp);
							return wp;
					}
				}

				public WhereParameter FirstName
				{
					get
					{
							WhereParameter wp = new WhereParameter(AggregateTestSchema.FirstName);
							this._clause._entity.Query.AddWhereParameter(wp);
							return wp;
					}
				}

				public WhereParameter LastName
				{
					get
					{
							WhereParameter wp = new WhereParameter(AggregateTestSchema.LastName);
							this._clause._entity.Query.AddWhereParameter(wp);
							return wp;
					}
				}

				public WhereParameter Age
				{
					get
					{
							WhereParameter wp = new WhereParameter(AggregateTestSchema.Age);
							this._clause._entity.Query.AddWhereParameter(wp);
							return wp;
					}
				}

				public WhereParameter HireDate
				{
					get
					{
							WhereParameter wp = new WhereParameter(AggregateTestSchema.HireDate);
							this._clause._entity.Query.AddWhereParameter(wp);
							return wp;
					}
				}

				public WhereParameter Salary
				{
					get
					{
							WhereParameter wp = new WhereParameter(AggregateTestSchema.Salary);
							this._clause._entity.Query.AddWhereParameter(wp);
							return wp;
					}
				}

				public WhereParameter IsActive
				{
					get
					{
							WhereParameter wp = new WhereParameter(AggregateTestSchema.IsActive);
							this._clause._entity.Query.AddWhereParameter(wp);
							return wp;
					}
				}

				public WhereParameter IsActive2
				{
					get
					{
							WhereParameter wp = new WhereParameter(AggregateTestSchema.IsActive2);
							this._clause._entity.Query.AddWhereParameter(wp);
							return wp;
					}
				}

				public WhereParameter DateCreated
				{
					get
					{
							WhereParameter wp = new WhereParameter(AggregateTestSchema.DateCreated);
							this._clause._entity.Query.AddWhereParameter(wp);
							return wp;
					}
				}

				public WhereParameter Ts
				{
					get
					{
							WhereParameter wp = new WhereParameter(AggregateTestSchema.Ts);
							this._clause._entity.Query.AddWhereParameter(wp);
							return wp;
					}
				}


				private WhereClause _clause;
			}
			#endregion
		
			public WhereParameter ID
		    {
				get
		        {
					if(_ID_W == null)
	        	    {
						_ID_W = TearOff.ID;
					}
					return _ID_W;
				}
			}

			public WhereParameter DepartmentID
		    {
				get
		        {
					if(_DepartmentID_W == null)
	        	    {
						_DepartmentID_W = TearOff.DepartmentID;
					}
					return _DepartmentID_W;
				}
			}

			public WhereParameter FirstName
		    {
				get
		        {
					if(_FirstName_W == null)
	        	    {
						_FirstName_W = TearOff.FirstName;
					}
					return _FirstName_W;
				}
			}

			public WhereParameter LastName
		    {
				get
		        {
					if(_LastName_W == null)
	        	    {
						_LastName_W = TearOff.LastName;
					}
					return _LastName_W;
				}
			}

			public WhereParameter Age
		    {
				get
		        {
					if(_Age_W == null)
	        	    {
						_Age_W = TearOff.Age;
					}
					return _Age_W;
				}
			}

			public WhereParameter HireDate
		    {
				get
		        {
					if(_HireDate_W == null)
	        	    {
						_HireDate_W = TearOff.HireDate;
					}
					return _HireDate_W;
				}
			}

			public WhereParameter Salary
		    {
				get
		        {
					if(_Salary_W == null)
	        	    {
						_Salary_W = TearOff.Salary;
					}
					return _Salary_W;
				}
			}

			public WhereParameter IsActive
		    {
				get
		        {
					if(_IsActive_W == null)
	        	    {
						_IsActive_W = TearOff.IsActive;
					}
					return _IsActive_W;
				}
			}

			public WhereParameter IsActive2
		    {
				get
		        {
					if(_IsActive2_W == null)
	        	    {
						_IsActive2_W = TearOff.IsActive2;
					}
					return _IsActive2_W;
				}
			}

			public WhereParameter DateCreated
		    {
				get
		        {
					if(_DateCreated_W == null)
	        	    {
						_DateCreated_W = TearOff.DateCreated;
					}
					return _DateCreated_W;
				}
			}

			public WhereParameter Ts
		    {
				get
		        {
					if(_Ts_W == null)
	        	    {
						_Ts_W = TearOff.Ts;
					}
					return _Ts_W;
				}
			}

			private WhereParameter _ID_W = null;
			private WhereParameter _DepartmentID_W = null;
			private WhereParameter _FirstName_W = null;
			private WhereParameter _LastName_W = null;
			private WhereParameter _Age_W = null;
			private WhereParameter _HireDate_W = null;
			private WhereParameter _Salary_W = null;
			private WhereParameter _IsActive_W = null;
			private WhereParameter _IsActive2_W = null;
			private WhereParameter _DateCreated_W = null;
			private WhereParameter _Ts_W = null;

			public void WhereClauseReset()
			{
				_ID_W = null;
				_DepartmentID_W = null;
				_FirstName_W = null;
				_LastName_W = null;
				_Age_W = null;
				_HireDate_W = null;
				_Salary_W = null;
				_IsActive_W = null;
				_IsActive2_W = null;
				_DateCreated_W = null;
				_Ts_W = null;

				this._entity.Query.FlushWhereParameters();

			}
	
			private EasyObject _entity;
			private TearOffWhereParameter _tearOff;
			
		}
	
		public WhereClause Where
		{
			get
			{
				if(_whereClause == null)
				{
					_whereClause = new WhereClause(this);
				}
		
				return _whereClause;
			}
		}
		
		private WhereClause _whereClause = null;	
		#endregion
		
		#region Aggregate Clause
		public class AggregateClause
		{
			public AggregateClause(EasyObject entity)
			{
				this._entity = entity;
			}
			
			public TearOffAggregateParameter TearOff
			{
				get
				{
					if(_tearOff == null)
					{
						_tearOff = new TearOffAggregateParameter(this);
					}

					return _tearOff;
				}
			}

			#region TearOff's
			public class TearOffAggregateParameter
			{
				public TearOffAggregateParameter(AggregateClause clause)
				{
					this._clause = clause;
				}
				
				
				public AggregateParameter ID
				{
					get
					{
							AggregateParameter ap = new AggregateParameter(AggregateTestSchema.ID);
							this._clause._entity.Query.AddAggregateParameter(ap);
							return ap;
					}
				}

				public AggregateParameter DepartmentID
				{
					get
					{
							AggregateParameter ap = new AggregateParameter(AggregateTestSchema.DepartmentID);
							this._clause._entity.Query.AddAggregateParameter(ap);
							return ap;
					}
				}

				public AggregateParameter FirstName
				{
					get
					{
							AggregateParameter ap = new AggregateParameter(AggregateTestSchema.FirstName);
							this._clause._entity.Query.AddAggregateParameter(ap);
							return ap;
					}
				}

				public AggregateParameter LastName
				{
					get
					{
							AggregateParameter ap = new AggregateParameter(AggregateTestSchema.LastName);
							this._clause._entity.Query.AddAggregateParameter(ap);
							return ap;
					}
				}

				public AggregateParameter Age
				{
					get
					{
							AggregateParameter ap = new AggregateParameter(AggregateTestSchema.Age);
							this._clause._entity.Query.AddAggregateParameter(ap);
							return ap;
					}
				}

				public AggregateParameter HireDate
				{
					get
					{
							AggregateParameter ap = new AggregateParameter(AggregateTestSchema.HireDate);
							this._clause._entity.Query.AddAggregateParameter(ap);
							return ap;
					}
				}

				public AggregateParameter Salary
				{
					get
					{
							AggregateParameter ap = new AggregateParameter(AggregateTestSchema.Salary);
							this._clause._entity.Query.AddAggregateParameter(ap);
							return ap;
					}
				}

				public AggregateParameter IsActive
				{
					get
					{
							AggregateParameter ap = new AggregateParameter(AggregateTestSchema.IsActive);
							this._clause._entity.Query.AddAggregateParameter(ap);
							return ap;
					}
				}

				public AggregateParameter IsActive2
				{
					get
					{
							AggregateParameter ap = new AggregateParameter(AggregateTestSchema.IsActive2);
							this._clause._entity.Query.AddAggregateParameter(ap);
							return ap;
					}
				}

				public AggregateParameter DateCreated
				{
					get
					{
							AggregateParameter ap = new AggregateParameter(AggregateTestSchema.DateCreated);
							this._clause._entity.Query.AddAggregateParameter(ap);
							return ap;
					}
				}

				public AggregateParameter Ts
				{
					get
					{
							AggregateParameter ap = new AggregateParameter(AggregateTestSchema.Ts);
							this._clause._entity.Query.AddAggregateParameter(ap);
							return ap;
					}
				}


				private AggregateClause _clause;
			}
			#endregion
		
			public AggregateParameter ID
		    {
				get
		        {
					if(_ID_W == null)
	        	    {
						_ID_W = TearOff.ID;
					}
					return _ID_W;
				}
			}

			public AggregateParameter DepartmentID
		    {
				get
		        {
					if(_DepartmentID_W == null)
	        	    {
						_DepartmentID_W = TearOff.DepartmentID;
					}
					return _DepartmentID_W;
				}
			}

			public AggregateParameter FirstName
		    {
				get
		        {
					if(_FirstName_W == null)
	        	    {
						_FirstName_W = TearOff.FirstName;
					}
					return _FirstName_W;
				}
			}

			public AggregateParameter LastName
		    {
				get
		        {
					if(_LastName_W == null)
	        	    {
						_LastName_W = TearOff.LastName;
					}
					return _LastName_W;
				}
			}

			public AggregateParameter Age
		    {
				get
		        {
					if(_Age_W == null)
	        	    {
						_Age_W = TearOff.Age;
					}
					return _Age_W;
				}
			}

			public AggregateParameter HireDate
		    {
				get
		        {
					if(_HireDate_W == null)
	        	    {
						_HireDate_W = TearOff.HireDate;
					}
					return _HireDate_W;
				}
			}

			public AggregateParameter Salary
		    {
				get
		        {
					if(_Salary_W == null)
	        	    {
						_Salary_W = TearOff.Salary;
					}
					return _Salary_W;
				}
			}

			public AggregateParameter IsActive
		    {
				get
		        {
					if(_IsActive_W == null)
	        	    {
						_IsActive_W = TearOff.IsActive;
					}
					return _IsActive_W;
				}
			}

			public AggregateParameter IsActive2
		    {
				get
		        {
					if(_IsActive2_W == null)
	        	    {
						_IsActive2_W = TearOff.IsActive2;
					}
					return _IsActive2_W;
				}
			}

			public AggregateParameter DateCreated
		    {
				get
		        {
					if(_DateCreated_W == null)
	        	    {
						_DateCreated_W = TearOff.DateCreated;
					}
					return _DateCreated_W;
				}
			}

			public AggregateParameter Ts
		    {
				get
		        {
					if(_Ts_W == null)
	        	    {
						_Ts_W = TearOff.Ts;
					}
					return _Ts_W;
				}
			}

			private AggregateParameter _ID_W = null;
			private AggregateParameter _DepartmentID_W = null;
			private AggregateParameter _FirstName_W = null;
			private AggregateParameter _LastName_W = null;
			private AggregateParameter _Age_W = null;
			private AggregateParameter _HireDate_W = null;
			private AggregateParameter _Salary_W = null;
			private AggregateParameter _IsActive_W = null;
			private AggregateParameter _IsActive2_W = null;
			private AggregateParameter _DateCreated_W = null;
			private AggregateParameter _Ts_W = null;

			public void AggregateClauseReset()
			{
				_ID_W = null;
				_DepartmentID_W = null;
				_FirstName_W = null;
				_LastName_W = null;
				_Age_W = null;
				_HireDate_W = null;
				_Salary_W = null;
				_IsActive_W = null;
				_IsActive2_W = null;
				_DateCreated_W = null;
				_Ts_W = null;

				this._entity.Query.FlushAggregateParameters();

			}
	
			private EasyObject _entity;
			private TearOffAggregateParameter _tearOff;
			
		}
	
		public AggregateClause Aggregate
		{
			get
			{
				if(_aggregateClause == null)
				{
					_aggregateClause = new AggregateClause(this);
				}
		
				return _aggregateClause;
			}
		}
		
		private AggregateClause _aggregateClause = null;	
		#endregion
	}
}