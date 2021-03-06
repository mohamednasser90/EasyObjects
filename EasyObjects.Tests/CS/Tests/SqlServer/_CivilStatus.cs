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

	public class CivilStatusSchema : NCI.EasyObjects.Schema
	{
		private static ArrayList _entries;
		public static SchemaItem CivilStatusID = new SchemaItem("CivilStatusID", DbType.Byte, true, false, false, true, false, false);
		public static SchemaItem CivilStatusEN = new SchemaItem("CivilStatusEN", DbType.AnsiString, SchemaItemJustify.None, 20, true, false, false, false);
		public static SchemaItem CivilStatusFR = new SchemaItem("CivilStatusFR", DbType.AnsiString, SchemaItemJustify.None, 20, true, false, false, false);
		public static SchemaItem CivilStatusES = new SchemaItem("CivilStatusES", DbType.AnsiString, SchemaItemJustify.None, 20, true, false, false, false);

		public override ArrayList SchemaEntries
		{
			get
			{
				if (_entries == null )
				{
					_entries = new ArrayList();
					_entries.Add(CivilStatusSchema.CivilStatusID);
					_entries.Add(CivilStatusSchema.CivilStatusEN);
					_entries.Add(CivilStatusSchema.CivilStatusFR);
					_entries.Add(CivilStatusSchema.CivilStatusES);
				}
				return _entries;
			}
		}
	}
	#endregion

	public abstract class _CivilStatus : EasyObject
	{

		public _CivilStatus()
		{
			CivilStatusSchema _schema = new CivilStatusSchema();
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
		/// <param name="CivilStatusID"></param>
		/// <returns>A Boolean indicating success or failure of the query</returns>
		public bool LoadByPrimaryKey(byte CivilStatusID)
		{
			switch(this.DefaultCommandType)
			{
				case CommandType.StoredProcedure:
					ListDictionary parameters = new ListDictionary();

					// Add in parameters
					parameters.Add(CivilStatusSchema.CivilStatusID.FieldName, CivilStatusID);

					return base.LoadFromSql(this.SchemaStoredProcedureWithSeparator + "daab_GetCivilStatus", parameters, CommandType.StoredProcedure);

				case CommandType.Text:
					this.Query.ClearAll();
					this.Where.WhereClauseReset();
					this.Where.CivilStatusID.Value = CivilStatusID;
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
					return base.LoadFromSql(this.SchemaStoredProcedureWithSeparator + "daab_GetAllCivilStatus", null, CommandType.StoredProcedure);

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

			// Create the Database object, using the default database service. The
			// default database service is determined through configuration.
			Database db = GetDatabase();

			switch(commandType)
			{
				case CommandType.StoredProcedure:
					string sqlCommand = this.SchemaStoredProcedureWithSeparator + "daab_AddCivilStatus";
					dbCommand = db.GetStoredProcCommand(sqlCommand);

					db.AddParameter(dbCommand, "CivilStatusID", DbType.Byte, 0, ParameterDirection.Output, true, 0, 0, "CivilStatusID", DataRowVersion.Default, Convert.DBNull);
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
						db.AddInParameter(dbCommand, "CivilStatusID", DbType.Byte, "CivilStatusID", DataRowVersion.Default);
					}
					else
					{
						db.AddParameter(dbCommand, "CivilStatusID", DbType.Byte, 0, ParameterDirection.Output, true, 0, 0, "CivilStatusID", DataRowVersion.Default, Convert.DBNull);
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
					string sqlCommand = this.SchemaStoredProcedureWithSeparator + "daab_UpdateCivilStatus";
					dbCommand = db.GetStoredProcCommand(sqlCommand);

					db.AddInParameter(dbCommand, "CivilStatusID", DbType.Byte, "CivilStatusID", DataRowVersion.Current);
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
					this.Where.CivilStatusID.Operator = WhereParameter.Operand.Equal;
					dbCommand = this.Query.GetUpdateCommandWrapper();

					dbCommand.Parameters.Clear();
					CreateParameters(db, dbCommand);
					db.AddInParameter(dbCommand, "CivilStatusID", DbType.Byte, "CivilStatusID", DataRowVersion.Current);
					
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
					string sqlCommand = this.SchemaStoredProcedureWithSeparator + "daab_DeleteCivilStatus";
					dbCommand = db.GetStoredProcCommand(sqlCommand);
					db.AddInParameter(dbCommand, "CivilStatusID", DbType.Byte, "CivilStatusID", DataRowVersion.Current);
					
					return dbCommand;

				case CommandType.Text:
					this.Query.ClearAll();
					this.Where.WhereClauseReset();
					this.Where.CivilStatusID.Operator = WhereParameter.Operand.Equal;
					dbCommand = this.Query.GetDeleteCommandWrapper();

					dbCommand.Parameters.Clear();
					db.AddInParameter(dbCommand, "CivilStatusID", DbType.Byte, "CivilStatusID", DataRowVersion.Current);
					
					return dbCommand;

				default:
					throw new ArgumentException("Invalid CommandType", "commandType");
			}
		}

		private void CreateParameters(Database db, DbCommand dbCommand)
		{
			db.AddInParameter(dbCommand, "CivilStatusEN", DbType.AnsiString, "CivilStatusEN", DataRowVersion.Current);
			db.AddInParameter(dbCommand, "CivilStatusFR", DbType.AnsiString, "CivilStatusFR", DataRowVersion.Current);
			db.AddInParameter(dbCommand, "CivilStatusES", DbType.AnsiString, "CivilStatusES", DataRowVersion.Current);
		}
		
		#region Properties
		public virtual byte CivilStatusID
		{
			get
			{
				return this.GetByte(CivilStatusSchema.CivilStatusID.FieldName);
	    	}
			set
			{
				this.SetByte(CivilStatusSchema.CivilStatusID.FieldName, value);
			}
		}
		public virtual string CivilStatusEN
		{
			get
			{
				return this.GetString(CivilStatusSchema.CivilStatusEN.FieldName);
	    	}
			set
			{
				this.SetString(CivilStatusSchema.CivilStatusEN.FieldName, value);
			}
		}
		public virtual string CivilStatusFR
		{
			get
			{
				return this.GetString(CivilStatusSchema.CivilStatusFR.FieldName);
	    	}
			set
			{
				this.SetString(CivilStatusSchema.CivilStatusFR.FieldName, value);
			}
		}
		public virtual string CivilStatusES
		{
			get
			{
				return this.GetString(CivilStatusSchema.CivilStatusES.FieldName);
	    	}
			set
			{
				this.SetString(CivilStatusSchema.CivilStatusES.FieldName, value);
			}
		}

		public override string TableName
		{
			get { return "CivilStatus"; }
		}
		
		#endregion		
		
		#region String Properties
	
		public virtual string s_CivilStatusID
	    {
			get
	        {
				return this.IsColumnNull(CivilStatusSchema.CivilStatusID.FieldName) ? string.Empty : base.GetByteAsString(CivilStatusSchema.CivilStatusID.FieldName);
			}
			set
	        {
				if(string.Empty == value)
					this.SetColumnNull(CivilStatusSchema.CivilStatusID.FieldName);
				else
					this.CivilStatusID = base.SetByteAsString(CivilStatusSchema.CivilStatusID.FieldName, value);
			}
		}

		public virtual string s_CivilStatusEN
	    {
			get
	        {
				return this.IsColumnNull(CivilStatusSchema.CivilStatusEN.FieldName) ? string.Empty : base.GetStringAsString(CivilStatusSchema.CivilStatusEN.FieldName);
			}
			set
	        {
				if(string.Empty == value)
					this.SetColumnNull(CivilStatusSchema.CivilStatusEN.FieldName);
				else
					this.CivilStatusEN = base.SetStringAsString(CivilStatusSchema.CivilStatusEN.FieldName, value);
			}
		}

		public virtual string s_CivilStatusFR
	    {
			get
	        {
				return this.IsColumnNull(CivilStatusSchema.CivilStatusFR.FieldName) ? string.Empty : base.GetStringAsString(CivilStatusSchema.CivilStatusFR.FieldName);
			}
			set
	        {
				if(string.Empty == value)
					this.SetColumnNull(CivilStatusSchema.CivilStatusFR.FieldName);
				else
					this.CivilStatusFR = base.SetStringAsString(CivilStatusSchema.CivilStatusFR.FieldName, value);
			}
		}

		public virtual string s_CivilStatusES
	    {
			get
	        {
				return this.IsColumnNull(CivilStatusSchema.CivilStatusES.FieldName) ? string.Empty : base.GetStringAsString(CivilStatusSchema.CivilStatusES.FieldName);
			}
			set
	        {
				if(string.Empty == value)
					this.SetColumnNull(CivilStatusSchema.CivilStatusES.FieldName);
				else
					this.CivilStatusES = base.SetStringAsString(CivilStatusSchema.CivilStatusES.FieldName, value);
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
				
				
				public WhereParameter CivilStatusID
				{
					get
					{
							WhereParameter wp = new WhereParameter(CivilStatusSchema.CivilStatusID);
							this._clause._entity.Query.AddWhereParameter(wp);
							return wp;
					}
				}

				public WhereParameter CivilStatusEN
				{
					get
					{
							WhereParameter wp = new WhereParameter(CivilStatusSchema.CivilStatusEN);
							this._clause._entity.Query.AddWhereParameter(wp);
							return wp;
					}
				}

				public WhereParameter CivilStatusFR
				{
					get
					{
							WhereParameter wp = new WhereParameter(CivilStatusSchema.CivilStatusFR);
							this._clause._entity.Query.AddWhereParameter(wp);
							return wp;
					}
				}

				public WhereParameter CivilStatusES
				{
					get
					{
							WhereParameter wp = new WhereParameter(CivilStatusSchema.CivilStatusES);
							this._clause._entity.Query.AddWhereParameter(wp);
							return wp;
					}
				}


				private WhereClause _clause;
			}
			#endregion
		
			public WhereParameter CivilStatusID
		    {
				get
		        {
					if(_CivilStatusID_W == null)
	        	    {
						_CivilStatusID_W = TearOff.CivilStatusID;
					}
					return _CivilStatusID_W;
				}
			}

			public WhereParameter CivilStatusEN
		    {
				get
		        {
					if(_CivilStatusEN_W == null)
	        	    {
						_CivilStatusEN_W = TearOff.CivilStatusEN;
					}
					return _CivilStatusEN_W;
				}
			}

			public WhereParameter CivilStatusFR
		    {
				get
		        {
					if(_CivilStatusFR_W == null)
	        	    {
						_CivilStatusFR_W = TearOff.CivilStatusFR;
					}
					return _CivilStatusFR_W;
				}
			}

			public WhereParameter CivilStatusES
		    {
				get
		        {
					if(_CivilStatusES_W == null)
	        	    {
						_CivilStatusES_W = TearOff.CivilStatusES;
					}
					return _CivilStatusES_W;
				}
			}

			private WhereParameter _CivilStatusID_W = null;
			private WhereParameter _CivilStatusEN_W = null;
			private WhereParameter _CivilStatusFR_W = null;
			private WhereParameter _CivilStatusES_W = null;

			public void WhereClauseReset()
			{
				_CivilStatusID_W = null;
				_CivilStatusEN_W = null;
				_CivilStatusFR_W = null;
				_CivilStatusES_W = null;

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
				
				
				public AggregateParameter CivilStatusID
				{
					get
					{
							AggregateParameter ap = new AggregateParameter(CivilStatusSchema.CivilStatusID);
							this._clause._entity.Query.AddAggregateParameter(ap);
							return ap;
					}
				}

				public AggregateParameter CivilStatusEN
				{
					get
					{
							AggregateParameter ap = new AggregateParameter(CivilStatusSchema.CivilStatusEN);
							this._clause._entity.Query.AddAggregateParameter(ap);
							return ap;
					}
				}

				public AggregateParameter CivilStatusFR
				{
					get
					{
							AggregateParameter ap = new AggregateParameter(CivilStatusSchema.CivilStatusFR);
							this._clause._entity.Query.AddAggregateParameter(ap);
							return ap;
					}
				}

				public AggregateParameter CivilStatusES
				{
					get
					{
							AggregateParameter ap = new AggregateParameter(CivilStatusSchema.CivilStatusES);
							this._clause._entity.Query.AddAggregateParameter(ap);
							return ap;
					}
				}


				private AggregateClause _clause;
			}
			#endregion
		
			public AggregateParameter CivilStatusID
		    {
				get
		        {
					if(_CivilStatusID_W == null)
	        	    {
						_CivilStatusID_W = TearOff.CivilStatusID;
					}
					return _CivilStatusID_W;
				}
			}

			public AggregateParameter CivilStatusEN
		    {
				get
		        {
					if(_CivilStatusEN_W == null)
	        	    {
						_CivilStatusEN_W = TearOff.CivilStatusEN;
					}
					return _CivilStatusEN_W;
				}
			}

			public AggregateParameter CivilStatusFR
		    {
				get
		        {
					if(_CivilStatusFR_W == null)
	        	    {
						_CivilStatusFR_W = TearOff.CivilStatusFR;
					}
					return _CivilStatusFR_W;
				}
			}

			public AggregateParameter CivilStatusES
		    {
				get
		        {
					if(_CivilStatusES_W == null)
	        	    {
						_CivilStatusES_W = TearOff.CivilStatusES;
					}
					return _CivilStatusES_W;
				}
			}

			private AggregateParameter _CivilStatusID_W = null;
			private AggregateParameter _CivilStatusEN_W = null;
			private AggregateParameter _CivilStatusFR_W = null;
			private AggregateParameter _CivilStatusES_W = null;

			public void AggregateClauseReset()
			{
				_CivilStatusID_W = null;
				_CivilStatusEN_W = null;
				_CivilStatusFR_W = null;
				_CivilStatusES_W = null;

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
