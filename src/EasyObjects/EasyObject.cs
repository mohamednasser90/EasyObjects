//===============================================================================
// NCI.EasyObjects library
// EasyObject class
//===============================================================================
// Copyright 2005 � Noonan Consulting Inc. All rights reserved.
// Adapted from Mike Griffin's dOOdads architecture. Used by permission.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace NCI.EasyObjects
{
    /// <summary>
    /// Provides the base functionality for an EasyObject.
    /// </summary>
    public abstract class EasyObject
    {
        // Internal error status variables
        int _errorCode = 0;
        string _errorMessage = String.Empty;

        // Internal data storage variables
        DataTable _dataTable = null;
        DataRow _dataRow = null;
        Hashtable _databases = new Hashtable();
        IEnumerator _enumerator = null;
        IDbTransaction _tx = null;

        // Internal schema entry variable
        List<SchemaItem> _schemaEntries;

        // Instance name variables, from Enterprise Library configuration files
        string _databaseInstanceName = string.Empty;
        string _dynamicQueryInstanceName = string.Empty;

        // Dynamic connection properties
        string _connectionString = string.Empty;
        string _connectionUserID = string.Empty;
        string _connectionPassword = string.Empty;
        string _connectionDatabase = string.Empty;
        string _connectionServer = string.Empty;
        bool _useIntegratedSecurity = false;

        // Query source for SELECT queries, such as a table or view
        string _querySource = string.Empty;
        CommandType _defaultCommandType = CommandType.StoredProcedure;

        // Internal storage for the dynamic query provider
        DynamicQuery _query;

        // Internal schema pointers
        private string _schemaGlobal = "";
        private string _tableViewSchema = "";
        private string _storedProcSchema = "";

        // Enable identity insert
        private bool _identityInsert = false;

        // Convert empty strings to NULLs
        private bool _convertEmptyStringToNull = true;

        // The command timeout value to use, the default is 30 seconds
        private int _commandTimeout = 30;

        // A reference to the table's primary key columns
        List<DataColumn> _primaryKeys = null;

        /// <summary>
        /// The default constructor.
        /// </summary>  
        public EasyObject() : this(new DynamicQueryProvider.SqlServerDynamicQuery())
        { }

        public EasyObject(DynamicQuery queryProvider) { }

        /// <summary>
        /// <para>Loads the internal DataTable with values from database Output parameters.</para>
        /// </summary>
        /// <param name="parameterCollection">The collection of parameters associated with the Command Wrapper</param>
        public virtual void Load(IDataParameterCollection parameterCollection)
        {
            foreach (IDataParameter param in parameterCollection)
            {
                if (((param.Direction == ParameterDirection.InputOutput)
                    || (param.Direction == ParameterDirection.Output)))
                {
                    _dataRow[param.ParameterName] = param.Value;
                }
            }
            _dataRow.AcceptChanges();
        }

        /// <summary>
        /// <para>Loads the internal DataTable.</para>
        /// </summary>
        /// <param name="dt">The DataTable to load the object with</param>
        public virtual void Load(DataTable dt)
        {
            //  Set internal reference to the datatable
            this.DataTable = dt;

            // Set the current row pointer and apply the 
            // primary keys from the schema
            if (dt.Rows.Count > 0)
            {
                _dataRow = dt.Rows[0];
                ApplyPrimaryKeys();
            }
        }

        /// <summary>
        /// <para>Loads the internal DataTable.</para>
        /// </summary>
        /// <param name="dt">The DataTable to load the object with</param>
        public virtual void Append(DataTable dt)
        {
            if (this.DataTable == null)
            {
                Load(dt);
                return;
            }

            Rewind();

            //  Set internal table and row pointers
            foreach (DataRow row in dt.Rows)
            {
                this.DataTable.ImportRow(row);
            }

            if (dt.Rows.Count > 0) _dataRow = dt.Rows[0];
        }

        /// <summary>
        /// <para>Loads the internal DataTable with columns based on the EasyObject's schema class.</para>
        /// </summary>
        public void LoadSchema()
        {
            DataColumn col;

            _dataTable = new DataTable(this.TableName);

            foreach (SchemaItem item in this.SchemaEntries)
            {
                col = _dataTable.Columns.Add(item.FieldName);
                col.AutoIncrement = item.IsAutoKey;

                //  Set the column datatype
                switch (item.DBType)
                {
                    case DbType.Guid:
                        col.DataType = typeof(System.Guid);
                        break;
                    case DbType.AnsiString:
                    case DbType.AnsiStringFixedLength:
                    case DbType.String:
                    case DbType.StringFixedLength:
                        col.DataType = typeof(string);
                        break;
                    case DbType.Binary:
                        col.DataType = typeof(byte[]);
                        break;
                    case DbType.Object:
                        col.DataType = typeof(object);
                        break;
                    case DbType.Byte:
                    case DbType.SByte:
                        col.DataType = typeof(byte);
                        break;
                    case DbType.Boolean:
                        col.DataType = typeof(bool);
                        break;
                    case DbType.Currency:
                    case DbType.Decimal:
                        col.DataType = typeof(Decimal);
                        break;
                    case DbType.Date:
                    case DbType.DateTime:
                        col.DataType = typeof(DateTime);
                        break;
                    case DbType.Time:
                        col.DataType = typeof(TimeSpan);
                        break;
                    case DbType.Double:
                    case DbType.VarNumeric:
                        col.DataType = typeof(double);
                        break;
                    case DbType.Int16:
                        col.DataType = typeof(short);
                        break;
                    case DbType.Int32:
                        col.DataType = typeof(int);
                        break;
                    case DbType.Int64:
                        col.DataType = typeof(long);
                        break;
                    case DbType.Single:
                        col.DataType = typeof(float);
                        break;
                }

                // Store a reference to the column if it is in the primary key
                if (item.IsInPrimaryKey)
                    this.PrimaryKeys.Add(col);
            }

            ApplyPrimaryKeys();
        }

        /// <summary>
        /// Apply the primary keys from the schema to the internal DataTable
        /// </summary>
        public virtual void ApplyPrimaryKeys()
        {
            if (this.PrimaryKeys.Count == 0 && _dataTable != null)
            {
                foreach (SchemaItem item in this.SchemaEntries)
                {
                    if (item.IsInPrimaryKey)
                    {
                        DataColumn col = _dataTable.Columns[item.FieldName];
                        col.AutoIncrement = item.IsAutoKey;
                        this.PrimaryKeys.Add(col);
                    }
                }
            }

            _dataTable.PrimaryKey = this.PrimaryKeys.ToArray();
        }

        /// <summary>
        /// A generic list of DataColumns that match the current schema's primary keys
        /// </summary>
        public virtual List<DataColumn> PrimaryKeys
        {
            get
            {
                if (_primaryKeys == null)
                    _primaryKeys = new List<DataColumn>();

                return _primaryKeys;
            }
        }

        /// <summary>
        /// After loading your BusinessEntity you can add custom columns, this is typically done to create a calculated column, however, it can
        /// be used to add a column just to hold state, it will not be saved to the database of course.
        /// </summary>
        /// <param name="name">The name of the Column</param>
        /// <param name="dataType">Use Type.GetType() as in Type.GetType("System.String")</param>
        /// <returns>The newly created DataColumn</returns>
        /// <example>
        /// VB.NET
        /// <code>
        /// Dim emps As New Employees
        /// If emps.LoadAll() Then
        ///
        ///    Dim col As DataColumn = emps.AddColumn("FullName", Type.GetType("System.String"))
        ///    col.Expression = Employees.ColumnNames.LastName + "+ ', ' + " + Employees.ColumnNames.FirstName
        ///
        ///    Dim fullName As String
        ///
        ///    Do
        ///        fullName = CType(emps.GetColumn("FullName"), String)
        ///    Loop Until Not emps.MoveNext
        ///    
        ///
        /// End If
        /// </code>
        /// C#
        /// <code>
        /// Employees emps = new Employees();
        ///	if(emps.LoadAll())
        ///	{
        ///		DataColumn col = emps.AddColumn("FullName", Type.GetType("System.String"));
        ///		col.Expression = Employees.ColumnNames.LastName + "+ ', ' + " + Employees.ColumnNames.FirstName;
        ///
        ///		string fullName;
        ///
        ///		do
        ///			fullName = emps.GetColumn("FullName") as string;
        ///		while(emps.MoveNext());
        ///	}
        /// </code>
        /// </example>
        public DataColumn AddColumn(string name, System.Type dataType)
        {
            DataColumn dc = null;

            if (_dataTable != null)
            {
                dc = new DataColumn(name, dataType);
                _dataTable.Columns.Add(dc);
            }

            return dc;
        }

        /// <summary>
        /// <para>Adds a new row to the internal DataTable.</para>
        /// </summary>
        /// <remarks>
        /// <para>If the internal DataTable is null (Nothing in Visual Basic), the DataTable is created from information stored in the EasyObject's Schema class.</para>
        /// </remarks>
        public virtual void AddNew()
        {
            if (_dataTable == null)
            {
                LoadSchema();
            }
            DataRow newRow = _dataTable.NewRow();
            _dataTable.Rows.Add(newRow);
            _dataRow = newRow;
        }

        /// <summary>
        /// <para>Marks the current row in the internal DataTable as deleted.</para>
        /// </summary>
        /// <remarks>
        /// <para>The row is not actually deleted until the EasyObject's Save() method is called.</para>
        /// </remarks>
        public virtual void MarkAsDeleted()
        {
            if (!(_dataRow == null))
            {
                _dataRow.Delete();
            }
        }

        /// <summary>
        /// <para>Marks every row in the internal DataTable as deleted.</para>
        /// </summary>
        /// <remarks>
        /// <para>The row is not actually deleted until the EasyObject's Save() method is called.</para>
        /// </remarks>
        public virtual void DeleteAll()
        {
            if (!(_dataTable == null))
            {
                foreach (DataRow row in _dataTable.Rows)
                {
                    row.Delete();
                }
            }
        }

        /// <summary>
        /// <para>Rolls back the internal DataTable to the state is was in when it was loaded, or the last time AcceptChanges was called.</para>
        /// </summary>
        public virtual void RejectChanges()
        {
            if (!(_dataTable == null))
            {
                _dataTable.RejectChanges();
            }
        }

        /// <summary>
        /// <para>Commits all the changes in the internal DataTable since the last time AcceptChanges was called.</para>
        /// </summary>
        public void AcceptChanges()
        {
            if (!(_dataTable == null))
            {
                _dataTable.AcceptChanges();
            }
        }

        /// <summary>
        /// <para>Gets the current state of the current row in the internal DataTable.</para>
        /// </summary>
        /// <returns>A DataRowState enumeration</returns>
        public DataRowState RowState()
        {
            if (_dataTable != null && _dataRow != null)
                return _dataRow.RowState;
            else
                return DataRowState.Detached;
        }

        /// <summary>
        /// <para>Gets a copy of internal DataTable containing all the changes to it since it was last loaded, or since AcceptChanges was last called.</para>
        /// </summary>
        public virtual void GetChanges()
        {
            if (!(_dataTable == null))
            {
                _dataTable = _dataTable.GetChanges();
            }
        }

        /// <summary>
        /// <para>Gets a copy of internal DataTable containing all the changes to it since it was last loaded, or since AcceptChanges was last called.</para>
        /// </summary>
        public virtual void GetChanges(DataRowState states)
        {
            if (!(_dataTable == null))
            {
                _dataTable = _dataTable.GetChanges(states);
            }
        }

        /// <summary>
        /// <para>Resets the internal DataTable to the first row.</para>
        /// </summary>
        public void Rewind()
        {
            _dataRow = null;
            _enumerator = null;
            if (!(_dataTable == null))
            {
                if ((_dataTable.DefaultView.Count > 0))
                {
                    _enumerator = _dataTable.DefaultView.GetEnumerator();
                    _enumerator.MoveNext();
                    DataRowView rowView = ((DataRowView)(_enumerator.Current));
                    _dataRow = rowView.Row;
                }
            }
        }

        /// <summary>
        /// <para>Moves the current row of internal DataTable.</para>
        /// </summary>
        /// <returns>True if the row was successfully changed, False for no more rows in the DataTable</returns>
        public bool MoveNext()
        {
            bool moved = false;
            if ((!(_enumerator == null)
                && _enumerator.MoveNext()))
            {
                DataRowView rowView = ((DataRowView)(_enumerator.Current));
                _dataRow = rowView.Row;
                moved = true;
            }
            return moved;
        }


        /// <summary>
        /// <para>Resets the EasyObject to an empty state.</para>
        /// </summary>
        public virtual void FlushData()
        {
            _dataRow = null;
            _dataTable = null;
            _enumerator = null;
            _query = null;
        }

        /// <summary>
        /// <para>Saves all the rows in the EasyObject.</para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// Any rows with a DataRowState of Added will call the Insert routine. Any rows with a DataRowState of 
        /// Modified will call the Update routine. Any rows with a DataRowState of Deleted will call the Deleted
        /// routine.
        /// </para>
        /// <para>
        /// All database calls will participate in the current transaction. If no transaction is present, then
        /// a new transaction will be created. The entire EasyObject will succeed or fail in one transaction.
        /// </para>
        /// </remarks>
        public virtual void Save(CommandType commandType)
        {
            if (_dataTable == null) { return; }

            // Call the PreSave
            this.PreSave();

            try
            {
                bool needToInsert = false;
                bool needToUpdate = false;
                bool needToDelete = false;
                foreach (DataRow row in _dataTable.Rows)
                {
                    switch (row.RowState)
                    {
                        case DataRowState.Added:
                            needToInsert = true;
                            break;
                        case DataRowState.Modified:
                            needToUpdate = true;
                            break;
                        case DataRowState.Deleted:
                            needToDelete = true;
                            break;
                    }
                }

                if (needToInsert || needToUpdate || needToDelete)
                {
                    IDbDataAdapter da = new SqlDataAdapter();

                    if (needToInsert) { da.InsertCommand = GetInsertCommand(commandType); }
                    if (needToUpdate) { da.UpdateCommand = GetUpdateCommand(commandType); }
                    if (needToDelete) { da.DeleteCommand = GetDeleteCommand(commandType); }


                    DbDataAdapter dbDataAdapter = da as DbDataAdapter;

                    // Perform the update
                    dbDataAdapter.Update(_dataTable);

                    this.AcceptChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Overload to save with a default <seealso cref="CommandType"/>
        /// </summary>
        /// <remarks>The default CommandType is set by calling <seealso cref="DefaultCommandType"/></remarks>
        public virtual void Save()
        {
            this.Save(this.DefaultCommandType);
        }

        /// <summary>
        /// <para>Load the EasyObject with the results from a SQL query or stored procedure.</para>
        /// </summary>
        /// <param name="sp">The name of the stored procedure to execute</param>
        /// <remarks>
        /// <para>
        /// The internal DataTable is loaded with any results returned from the stored procedure or query, but
        /// the columns may or may not line up with the EasyObject's Schema class. There is no restriction on the
        /// type or number of columns returned in the query.
        /// </para>
        /// <para>
        /// To see if any errors occurred from the database, see <see cref="ErrorMessage"/>.
        /// </para>
        /// </remarks>
        /// <returns>A boolean value indicating success or failure</returns>
        protected bool LoadFromSql(string sp)
        {
            return LoadFromSql(sp, null, this.DefaultCommandType);
        }

        /// <summary>
        /// <para>Load the EasyObject with the results from a SQL query or stored procedure.</para>
        /// </summary>
        /// <param name="sp">The name of the stored procedure to execute</param>
        /// <param name="parameters">The ListDictionary collection of parameters to send to the stored procedure</param>
        /// <remarks>
        /// <para>
        /// The internal DataTable is loaded with any results returned from the stored procedure or query, but
        /// the columns may or may not line up with the EasyObject's Schema class. There is no restriction on the
        /// type or number of columns returned in the query.
        /// </para>
        /// <para>
        /// To see if any errors occurred from the database, see <see cref="ErrorMessage"/>.
        /// </para>
        /// </remarks>
        /// <returns>A boolean value indicating success or failure</returns>
        protected bool LoadFromSql(string sp, ListDictionary parameters)
        {
            return LoadFromSql(sp, parameters, this.DefaultCommandType);
        }

        /// <summary>
        /// <para>Load the EasyObject with the results from a SQL query or stored procedure.</para>
        /// </summary>
        /// <param name="sp">The name of the stored procedure to execute</param>
        /// <param name="parameters">The ListDictionary collection of parameters to send to the stored procedure</param>
        /// <param name="commandType">A CommandType enumeration which tells the type of query to run, either StoredProcedure or Text</param>
        /// <remarks>
        /// <para>
        /// The internal DataTable is loaded with any results returned from the stored procedure or query, but
        /// the columns may or may not line up with the EasyObject's Schema class. There is no restriction on the
        /// type or number of columns returned in the query.
        /// </para>
        /// <para>
        /// To see if any errors occurred from the database, see <see cref="ErrorMessage"/>.
        /// </para>
        /// </remarks>
        /// <returns>A boolean value indicating success or failure</returns>
        protected bool LoadFromSql(string sp, ListDictionary parameters, CommandType commandType)
        {
            DbCommand dbCommand = new SqlCommand
            {
                CommandType = commandType,
                CommandText = sp,
                CommandTimeout = CommandTimeout > 0 ? CommandTimeout : 0
            };

            if (parameters != null)
            {
                foreach (DictionaryEntry param in parameters)
                {
                    AddInParameter(dbCommand, param.Key.ToString(), GetDbType(param.Value.GetType()), param.Value);
                }
            }

            return LoadFromSql(dbCommand);
        }

        /// <summary>
        /// Adds a new In <see cref="DbParameter"/> object to the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to add the in parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>                
        /// <remarks>
        /// <para>This version of the method is used when you can have the same parameter object multiple times with different values.</para>
        /// </remarks>        
        public void AddInParameter(DbCommand command,
                                   string name,
                                   DbType dbType)
        {
            AddParameter(command, name, dbType, ParameterDirection.Input, String.Empty, DataRowVersion.Default, null);
        }

        /// <summary>
        /// Adds a new In <see cref="DbParameter"/> object to the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The commmand to add the parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>                
        /// <param name="value"><para>The value of the parameter.</para></param>      
        public void AddInParameter(DbCommand command,
                                   string name,
                                   DbType dbType,
                                   object value)
        {
            AddParameter(command, name, dbType, ParameterDirection.Input, String.Empty, DataRowVersion.Default, value);
        }

        /// <summary>
        /// Adds a new In <see cref="DbParameter"/> object to the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to add the parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>                
        /// <param name="sourceColumn"><para>The name of the source column mapped to the DataSet and used for loading or returning the value.</para></param>
        /// <param name="sourceVersion"><para>One of the <see cref="DataRowVersion"/> values.</para></param>
        public void AddInParameter(DbCommand command,
                                   string name,
                                   DbType dbType,
                                   string sourceColumn,
                                   DataRowVersion sourceVersion)
        {
            AddParameter(command, name, dbType, 0, ParameterDirection.Input, true, 0, 0, sourceColumn, sourceVersion, null);
        }

        /// <summary>
        /// <para>Adds a new instance of a <see cref="DbParameter"/> object to the command.</para>
        /// </summary>
        /// <param name="command">The command to add the parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>        
        /// <param name="direction"><para>One of the <see cref="ParameterDirection"/> values.</para></param>                
        /// <param name="sourceColumn"><para>The name of the source column mapped to the DataSet and used for loading or returning the <paramref name="value"/>.</para></param>
        /// <param name="sourceVersion"><para>One of the <see cref="DataRowVersion"/> values.</para></param>
        /// <param name="value"><para>The value of the parameter.</para></param>    
        protected virtual void AddParameter(DbCommand command,
                                 string name,
                                 DbType dbType,
                                 ParameterDirection direction,
                                 string sourceColumn,
                                 DataRowVersion sourceVersion,
                                 object value)
        {
            AddParameter(command, name, dbType, 0, direction, false, 0, 0, sourceColumn, sourceVersion, value);
        }

        /// <summary>
        /// Adds a new In <see cref="DbParameter"/> object to the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to add the parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>
        /// <param name="size"><para>The maximum size of the data within the column.</para></param>
        /// <param name="direction"><para>One of the <see cref="ParameterDirection"/> values.</para></param>
        /// <param name="nullable"><para>Avalue indicating whether the parameter accepts <see langword="null"/> (<b>Nothing</b> in Visual Basic) values.</para></param>
        /// <param name="precision"><para>The maximum number of digits used to represent the <paramref name="value"/>.</para></param>
        /// <param name="scale"><para>The number of decimal places to which <paramref name="value"/> is resolved.</para></param>
        /// <param name="sourceColumn"><para>The name of the source column mapped to the DataSet and used for loading or returning the <paramref name="value"/>.</para></param>
        /// <param name="sourceVersion"><para>One of the <see cref="DataRowVersion"/> values.</para></param>
        /// <param name="value"><para>The value of the parameter.</para></param>       
        protected virtual void AddParameter(DbCommand command,
                                         string name,
                                         DbType dbType,
                                         int size,
                                         ParameterDirection direction,
                                         bool nullable,
                                         byte precision,
                                         byte scale,
                                         string sourceColumn,
                                         DataRowVersion sourceVersion,
                                         object value)
        {
            if (command == null) throw new ArgumentNullException("command");

            DbParameter parameter = CreateParameter(name, dbType, size, direction, nullable, precision, scale, sourceColumn, sourceVersion, value);
            command.Parameters.Add(parameter);
        }

        /// <summary>
        /// <para>Adds a new instance of a <see cref="DbParameter"/> object.</para>
        /// </summary>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>
        /// <param name="size"><para>The maximum size of the data within the column.</para></param>
        /// <param name="direction"><para>One of the <see cref="ParameterDirection"/> values.</para></param>
        /// <param name="nullable"><para>Avalue indicating whether the parameter accepts <see langword="null"/> (<b>Nothing</b> in Visual Basic) values.</para></param>
        /// <param name="precision"><para>The maximum number of digits used to represent the <paramref name="value"/>.</para></param>
        /// <param name="scale"><para>The number of decimal places to which <paramref name="value"/> is resolved.</para></param>
        /// <param name="sourceColumn"><para>The name of the source column mapped to the DataSet and used for loading or returning the <paramref name="value"/>.</para></param>
        /// <param name="sourceVersion"><para>One of the <see cref="DataRowVersion"/> values.</para></param>
        /// <param name="value"><para>The value of the parameter.</para></param>  
        /// <returns>A newly created <see cref="DbParameter"/> fully initialized with given parameters.</returns>
        protected DbParameter CreateParameter(string name,
                                              DbType dbType,
                                              int size,
                                              ParameterDirection direction,
                                              bool nullable,
                                              byte precision,
                                              byte scale,
                                              string sourceColumn,
                                              DataRowVersion sourceVersion,
                                              object value)
        {
            DbParameter param = CreateParameter(name);
            ConfigureParameter(param, name, dbType, size, direction, nullable, precision, scale, sourceColumn, sourceVersion, value);
            return param;
        }

        /// <summary>
        /// <para>Adds a new instance of a <see cref="DbParameter"/> object.</para>
        /// </summary>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <returns><para>An unconfigured parameter.</para></returns>
        protected DbParameter CreateParameter(string name)
        {
            return new SqlParameter
            {
                ParameterName = name
            };
        }

        /// <summary>
        /// Configures a given <see cref="DbParameter"/>.
        /// </summary>
        /// <param name="param">The <see cref="DbParameter"/> to configure.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>
        /// <param name="size"><para>The maximum size of the data within the column.</para></param>
        /// <param name="direction"><para>One of the <see cref="ParameterDirection"/> values.</para></param>
        /// <param name="nullable"><para>Avalue indicating whether the parameter accepts <see langword="null"/> (<b>Nothing</b> in Visual Basic) values.</para></param>
        /// <param name="precision"><para>The maximum number of digits used to represent the <paramref name="value"/>.</para></param>
        /// <param name="scale"><para>The number of decimal places to which <paramref name="value"/> is resolved.</para></param>
        /// <param name="sourceColumn"><para>The name of the source column mapped to the DataSet and used for loading or returning the <paramref name="value"/>.</para></param>
        /// <param name="sourceVersion"><para>One of the <see cref="DataRowVersion"/> values.</para></param>
        /// <param name="value"><para>The value of the parameter.</para></param>  
        protected virtual void ConfigureParameter(DbParameter param,
                                                  string name,
                                                  DbType dbType,
                                                  int size,
                                                  ParameterDirection direction,
                                                  bool nullable,
                                                  byte precision,
                                                  byte scale,
                                                  string sourceColumn,
                                                  DataRowVersion sourceVersion,
                                                  object value)
        {
            param.DbType = dbType;
            param.Size = size;
            param.Value = value ?? DBNull.Value;
            param.Direction = direction;
            param.IsNullable = nullable;
            param.SourceColumn = sourceColumn;
            param.SourceVersion = sourceVersion;
        }

        /// <summary>
        /// <para>Load the EasyObject with the results from a SQL query or stored procedure.</para>
        /// </summary>
        /// <param name="dbCommand">The command wrapper to use when calling ExecuteDataSet.</param>
        /// <returns>True if one or more rows was returned, false if not or an exception occurred.</returns>
        /// <remarks>
        /// <para>
        /// The internal DataTable is loaded with any results returned from the stored procedure or query, but
        /// the columns may or may not line up with the EasyObject's Schema class. There is no restriction on the
        /// type or number of columns returned in the query.
        /// </para>
        /// <para>
        /// To see if any errors occurred from the database, see <see cref="ErrorMessage"/>.
        /// </para>
        /// </remarks>
        /// <returns>A boolean value indicating success or failure</returns>
        protected bool LoadFromSql(DbCommand dbCommand)
        {
            DataTable dataTable = null;
            bool loaded = false;

            dbCommand.CommandTimeout = CommandTimeout > 0 ? CommandTimeout : 0;

            try
            {
                using (var con = new SqlConnection(this.ConnectionString))
                {
                    dbCommand.Connection = con;

                    using (var adapter = new SqlDataAdapter((SqlCommand)dbCommand))
                    {
                        adapter.Fill(dataTable);
                    }
                }

                // Load the object from the dataTable
                Load(dataTable);
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;
                return false;
            }
            finally
            {
                loaded = (this.RowCount > 0);
            }

            return loaded;
        }

        /// <summary>
        /// <para>Execute a SQL query or stored procedure without affecting the EasyObject.</para>
        /// </summary>
        /// <param name="sp">The name of the stored procedure to execute</param>
        protected void LoadFromSqlNoExec(string sp)
        {
            LoadFromSqlNoExec(sp, null, this.DefaultCommandType, this.CommandTimeout);
        }

        /// <summary>
        /// <para>Execute a SQL query or stored procedure without affecting the EasyObject.</para>
        /// </summary>
        /// <param name="sp">The name of the stored procedure to execute</param>
        /// <param name="parameters">The ListDictionary collection of parameters to send to the stored procedure</param>
        protected void LoadFromSqlNoExec(string sp, ListDictionary parameters)
        {
            LoadFromSqlNoExec(sp, parameters, this.DefaultCommandType, this.CommandTimeout);
        }

        /// <summary>
        /// <para>Execute a SQL query or stored procedure without affecting the EasyObject.</para>
        /// </summary>
        /// <param name="sp">The name of the stored procedure to execute</param>
        /// <param name="parameters">The ListDictionary collection of parameters to send to the stored procedure</param>
        /// <param name="commandType">A CommandType enumeration which tells the type of query to run, either StoredProcedure or Text</param>
        protected void LoadFromSqlNoExec(string sp, ListDictionary parameters, CommandType commandType)
        {
            LoadFromSqlNoExec(sp, parameters, commandType, this.CommandTimeout);
        }

        /// <summary>
        /// <para>Execute a SQL query or stored procedure without affecting the EasyObject.</para>
        /// </summary>
        /// <param name="sp">The name of the stored procedure to execute</param>
        /// <param name="parameters">The ListDictionary collection of parameters to send to the stored procedure</param>
        /// <param name="commandType">A CommandType enumeration which tells the type of query to run, either StoredProcedure or Text</param>
        /// <param name="commandTimeout">The command timeout value, 0 for infinite</param>
        protected void LoadFromSqlNoExec(string sp, ListDictionary parameters, CommandType commandType, int commandTimeout)
        {
            DbCommand dbCommand = new SqlCommand
            {
                CommandType = commandType,
                CommandText = sp,
                CommandTimeout = commandTimeout > 0 ? commandTimeout : 0
            };

            if (parameters != null)
            {
                foreach (DictionaryEntry param in parameters)
                {
                    AddInParameter(dbCommand, param.Key.ToString(), GetDbType(param.Value.GetType()), param.Value);
                }
            }

            LoadFromSqlNoExec(dbCommand);
        }

        /// <summary>
        /// <para>Execute a SQL query or stored procedure without affecting the EasyObject.</para>
        /// </summary>
        /// <param name="dbCommand">The command wrapper to use when calling ExecuteNonQuery.</param>
        protected void LoadFromSqlNoExec(DbCommand dbCommand)
        {
            try
            {
                using (var con = new SqlConnection(this.ConnectionString))
                {
                    dbCommand.Connection = con;
                    dbCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// <para>Return a value from a SQL query or stored procedure without affecting the EasyObject.</para>
        /// </summary>
        /// <param name="sp">The name of the stored procedure to execute</param>
        /// <returns>An object value from the database Command</returns>
        protected object LoadFromSqlScalar(string sp)
        {
            return LoadFromSqlScalar(sp, null, this.DefaultCommandType, this.CommandTimeout);
        }

        /// <summary>
        /// <para>Return a value from a SQL query or stored procedure without affecting the EasyObject.</para>
        /// </summary>
        /// <param name="sp">The name of the stored procedure to execute</param>
        /// <param name="parameters">The ListDictionary collection of parameters to send to the stored procedure</param>
        /// <returns>An object value from the database Command</returns>
        protected object LoadFromSqlScalar(string sp, ListDictionary parameters)
        {
            return LoadFromSqlScalar(sp, parameters, this.DefaultCommandType, this.CommandTimeout);
        }

        /// <summary>
        /// <para>Return a value from a SQL query or stored procedure without affecting the EasyObject.</para>
        /// </summary>
        /// <param name="sp">The name of the stored procedure to execute</param>
        /// <param name="parameters">The ListDictionary collection of parameters to send to the stored procedure</param>
        /// <param name="commandType">A CommandType enumeration which tells the type of query to run, either StoredProcedure or Text</param>
        /// <returns>An object value from the database Command</returns>
        protected object LoadFromSqlScalar(string sp, ListDictionary parameters, CommandType commandType)
        {
            return LoadFromSqlScalar(sp, parameters, commandType, this.CommandTimeout);
        }

        /// <summary>
        /// <para>Return a value from a SQL query or stored procedure without affecting the EasyObject.</para>
        /// </summary>
        /// <param name="sp">The name of the stored procedure to execute</param>
        /// <param name="parameters">The ListDictionary collection of parameters to send to the stored procedure</param>
        /// <param name="commandType">A CommandType enumeration which tells the type of query to run, either StoredProcedure or Text</param>
        /// <param name="commandTimeout">The command timeout value, 0 for infinite</param>
        /// <returns>An object value from the database Command</returns>
        protected object LoadFromSqlScalar(string sp, ListDictionary parameters, CommandType commandType, int commandTimeout)
        {
            DbCommand dbCommand = new SqlCommand
            {
                CommandType = commandType,
                CommandText = sp,
                CommandTimeout = commandTimeout > 0 ? commandTimeout : 0
            };

            if (parameters != null)
            {
                foreach (DictionaryEntry param in parameters)
                {
                    AddInParameter(dbCommand, param.Key.ToString(), GetDbType(param.Value.GetType()), param.Value);
                }
            }

            return LoadFromSqlScalar(dbCommand);
        }

        /// <summary>
        /// <para>Return a value from a SQL query or stored procedure without affecting the EasyObject.</para>
        /// </summary>
        /// <param name="dbCommand">The command wrapper to use when calling ExecuteNonQuery.</param>
        /// <returns>An object with the result of the SQL command.</returns>
        protected object LoadFromSqlScalar(DbCommand dbCommand)
        {
            object rc = 0;

            try
            {
                using (var con = new SqlConnection(this.ConnectionString))
                {
                    dbCommand.Connection = con;
                    rc = dbCommand.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return rc;
        }

        /// <summary>
        /// <para>Return an IDataReader from a SQL query or stored procedure without affecting the EasyObject.</para>
        /// </summary>
        /// <param name="sp">The name of the stored procedure to execute</param>
        /// <returns>An object value from the database Command</returns>
        protected IDataReader LoadFromSqlReader(string sp)
        {
            return LoadFromSqlReader(sp, null, this.DefaultCommandType, this.CommandTimeout);
        }

        /// <summary>
        /// <para>Return an IDataReader from a SQL query or stored procedure without affecting the EasyObject.</para>
        /// </summary>
        /// <param name="sp">The name of the stored procedure to execute</param>
        /// <param name="parameters">The ListDictionary collection of parameters to send to the stored procedure</param>
        /// <returns>An object value from the database Command</returns>
        protected IDataReader LoadFromSqlReader(string sp, ListDictionary parameters)
        {
            return LoadFromSqlReader(sp, parameters, this.DefaultCommandType, this.CommandTimeout);
        }

        /// <summary>
        /// <para>Return an IDataReader from a SQL query or stored procedure without affecting the EasyObject.</para>
        /// </summary>
        /// <param name="sp">The name of the stored procedure to execute</param>
        /// <param name="parameters">The ListDictionary collection of parameters to send to the stored procedure</param>
        /// <param name="commandType">A CommandType enumeration which tells the type of query to run, either StoredProcedure or Text</param>
        /// <returns>An object value from the database Command</returns>
        protected IDataReader LoadFromSqlReader(string sp, ListDictionary parameters, CommandType commandType)
        {
            return LoadFromSqlReader(sp, parameters, commandType, this.CommandTimeout);
        }

        /// <summary>
        /// <para>Return an IDataReader from a SQL query or stored procedure without affecting the EasyObject.</para>
        /// </summary>
        /// <param name="sp">The name of the stored procedure to execute</param>
        /// <param name="parameters">The ListDictionary collection of parameters to send to the stored procedure</param>
        /// <param name="commandType">A CommandType enumeration which tells the type of query to run, either StoredProcedure or Text</param>
        /// <param name="commandTimeout">The command timeout value, 0 for infinite</param>
        /// <returns>An object value from the database Command</returns>
        protected IDataReader LoadFromSqlReader(string sp, ListDictionary parameters, CommandType commandType, int commandTimeout)
        {
            DbCommand dbCommand = new SqlCommand
            {
                CommandType = commandType,
                CommandText = sp,
                CommandTimeout = commandTimeout > 0 ? commandTimeout : 0
            };

            if (parameters != null)
            {
                foreach (DictionaryEntry param in parameters)
                {
                    AddInParameter(dbCommand, param.Key.ToString(), GetDbType(param.Value.GetType()), param.Value);
                }
            }

            return LoadFromSqlReader(dbCommand);
        }

        /// <summary>
        /// <para>Return an IDataReader from a SQL query or stored procedure without affecting the EasyObject.</para>
        /// </summary>
        /// <param name="dbCommand">The command wrapper to use when calling ExecuteNonQuery.</param>
        /// <returns>An object with the result of the SQL command.</returns>
        protected IDataReader LoadFromSqlReader(DbCommand dbCommand)
        {
            IDataReader rc = null;

            try
            {
                using (var con = new SqlConnection(this.ConnectionString))
                {
                    dbCommand.Connection = con;
                    rc = dbCommand.ExecuteReader();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return rc;
        }

        /// <summary>
        /// <para>Maps a system type to a DbType enumeration.</para>
        /// </summary>
        /// <param name="sysType">The system type to translate</param>
        /// <returns>A DbType enumeration</returns>
        private DbType GetDbType(System.Type sysType)
        {
            switch (sysType.Name.ToLower(System.Globalization.CultureInfo.InvariantCulture))
            {
                case "guid":
                    return DbType.Guid;
                case "string":
                    return DbType.String;
                case "integer":
                case "int32":
                case "int":
                    return DbType.Int32;
                case "decimal":
                    return DbType.Decimal;
                case "byte[]":
                    return DbType.Binary;
                case "object":
                    return DbType.Object;
                case "byte":
                    return DbType.Byte;
                case "bool":
                    return DbType.Boolean;
                case "datetime":
                    return DbType.DateTime;
                case "timespan":
                    return DbType.Time;
                case "double":
                    return DbType.Double;
                case "short":
                case "int16":
                    return DbType.Int16;
                case "long":
                case "int64":
                    return DbType.Int64;
                case "float":
                    return DbType.Single;
                default:
                    return DbType.Object;
            }
        }

        /// <summary>
        /// <para>Perform any operations to the EasyObject before saving.</para>
        /// </summary>
        /// <remarks>
        /// This function used to be where the strings were "justified" before sending to the database. 
        /// That feature has now been deprecated because it's a remnant of some old code. But if you need
        /// to do some pre-processing before the Save, just override this method.
        /// </remarks>
        protected virtual void PreSave()
        {
        }

        /// <summary>
        /// <para>Perform any operations to the EasyObject after saving.</para>
        /// </summary>
        /// <remarks>This function has been deprecated and should not be used. It's a remnant of some old code.</remarks>
        protected virtual void PostSave(IDataParameterCollection ps)
        {
        }

        /// <summary>
        /// <para>Checks if a particular column in the EasyObject has a null value.</para>
        /// </summary>
        /// <remarks>The "s_" properties will automatically return a string value for the property, and String.Empty if the column value is null.</remarks>
        /// <example>
        /// <code>
        /// Employee emp = new Employee();
        /// if (!emp.IsColumnNull(EmployeeSchema.FirstName.FieldName)
        /// {
        ///		txtFirstName.Text = emp.FirstName
        /// }
        /// </code>
        /// </example>
        /// <returns>True if the column is null, False if not</returns>
        public virtual bool IsColumnNull(string fieldName)
        {
            return this.IsColumnNull(fieldName, false);
        }

        /// <summary>
        /// <para>Checks if a particular column in the EasyObject has a null value. This overload will optionally ignore if the column is missing from the current schema.</para>
        /// </summary>
        /// <remarks>The "s_" properties will automatically return a string value for the property, and String.Empty if the column value is null.</remarks>
        /// <example>
        /// <code>
        /// Employee emp = new Employee();
        /// if (!emp.IsColumnNull(EmployeeSchema.FirstName.FieldName)
        /// {
        ///		txtFirstName.Text = emp.FirstName
        /// }
        /// </code>
        /// </example>
        /// <returns>True if the column is null, False if not</returns>
        public virtual bool IsColumnNull(string fieldName, bool ignoreMissing)
        {
            if (ignoreMissing && !_dataTable.Columns.Contains(fieldName))
            {
                return true;
            }

            return _dataRow.IsNull(fieldName);
        }

        /// <summary>
        /// Use this method to set a column to DBNull.Value which will translate to NULL in your DBMS system.
        /// </summary>
        /// <param name="fieldName">The name of the column.</param>
        /// <remarks>Use the generated <see cref="Schema"/> class to call this function.</remarks>
        /// <example>
        /// VB.NET
        /// <code>
        /// emps.SetColumnNull(EmployeesSchema.MiddleInitial)
        /// </code>
        /// C#
        /// <code>
        /// emps.SetColumnNull(EmployeesSchema.MiddleInitial);
        /// </code>
        /// </example>
        public virtual void SetColumnNull(string fieldName)
        {
            _dataRow[fieldName] = Convert.DBNull;
        }

        /// <summary>
        /// Used by derived classes
        /// </summary>
        protected virtual DbCommand GetInsertCommand()
        {
            return GetInsertCommand(CommandType.StoredProcedure);
        }

        /// <summary>
        /// Used by derived classes
        /// </summary>
        protected virtual DbCommand GetInsertCommand(CommandType commandType)
        {
            return null;
        }

        /// <summary>
        /// Used by derived classes
        /// </summary>
        protected virtual DbCommand GetUpdateCommand()
        {
            return GetUpdateCommand(CommandType.StoredProcedure);
        }

        /// <summary>
        /// Used by derived classes
        /// </summary>
        protected virtual DbCommand GetUpdateCommand(CommandType commandType)
        {
            return null;
        }

        /// <summary>
        /// Used by derived classes
        /// </summary>
        protected virtual DbCommand GetDeleteCommand()
        {
            return GetDeleteCommand(CommandType.StoredProcedure);
        }

        /// <summary>
        /// Used by derived classes
        /// </summary>
        protected virtual DbCommand GetDeleteCommand(CommandType commandType)
        {
            return null;
        }

        /// <summary>
        /// Used by derived classes
        /// </summary>
        protected virtual void ApplyDefaults()
        {
        }

        /// <summary>
        /// This is the typeless version, this method should only be used for columns that you added via <see cref="AddColumn"/> or to access
        /// extra columns brought back by changing your <see cref="QuerySource"/> to a SQL View.
        /// </summary>
        /// <param name="columnName">The name of the column, "MyColumn"</param>
        /// <param name="Value">The value to set the column to</param>
        public virtual void SetColumn(string columnName, object Value)
        {
            _dataRow[columnName] = Value;
        }

        /// <summary>
        /// This is the typeless version, this method should only be used for columns that you added via <see cref="AddColumn"/> or to access
        /// extra columns brought back by changing your <see cref="QuerySource"/> to a SQL View.
        /// </summary>
        /// <param name="columnName">The name of the column, "MyColumn"</param>
        /// <returns>The value, you will have to typecast it to the proper type.</returns>
        public virtual object GetColumn(string columnName)
        {
            return _dataRow[columnName];
        }

        /// <summary>
        /// Mimics the old VB LSet function, which places a value within a padded string of a defined length.
        /// </summary>
        /// <param name="field">The value to place in the padded string</param>
        /// <param name="length">The maximum length of the padded string</param>
        /// <returns>A string containing the value and any string padding up to the length</returns>
        /// <remarks>
        /// If the length of the value is greater than the maximum padded length, then the original string is returned.
        /// </remarks>
        protected virtual string LSet(string field, int length)
        {
            if (field.Length >= length)
                return field;
            else
                return field.PadRight(length, ' ');
        }

        /// <summary>
        /// Mimics the old VB RSet function, which places a value within a padded string of a defined length.
        /// </summary>
        /// <param name="field">The value to place in the padded string</param>
        /// <param name="length">The maximum length of the padded string</param>
        /// <returns>A string containing the value and any string padding up to the length</returns>
        /// <remarks>
        /// If the length of the value is greater than the maximum padded length, then the original string is returned.
        /// </remarks>
        protected virtual string RSet(string field, int length)
        {
            if (field.Length >= length)
                return field;
            else
                return field.PadLeft(length, ' ');
        }

        /// <summary>
        /// Used by the Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name to store the value in</param>
        /// <returns>The value</returns>
        protected virtual string GetString(string fieldName)
        {
            if (_dataRow.IsNull(fieldName))
                return String.Empty;
            else
                return (string)_dataRow[fieldName];
        }

        /// <summary>
        /// Used by the Properties in your generated class
        /// </summary>
        /// <param name="fieldName">The name of the column</param>
        /// <param name="value">The value to store</param>
        protected virtual void SetString(string fieldName, string value)
        {
            SetString(fieldName, value, SchemaItemJustify.None, 0);
        }

        /// <summary>
        /// Stores a justified string in the internal DataTable. 
        /// </summary>
        /// <param name="fieldName">The name of the column</param>
        /// <param name="value">The value to store</param>
        /// <param name="justify">The <see cref="SchemaItemJustify"/> setting</param>
        /// <param name="length">The maximum length of the padded string</param>
        /// <remarks>This method has been depracated in anticipation of removing the SchemaItemJustify functionality.</remarks>
        protected virtual void SetString(string fieldName, string value, SchemaItemJustify justify, Int32 length)
        {
            if ((value.Trim() == String.Empty) && _convertEmptyStringToNull)
            {
                _dataRow[fieldName] = DBNull.Value;
                return;
            }
            switch (justify)
            {
                case SchemaItemJustify.None:
                    _dataRow[fieldName] = value.Trim();
                    break;
                case SchemaItemJustify.Left:
                    _dataRow[fieldName] = LSet(value.Trim(), length);
                    break;
                case SchemaItemJustify.Right:
                    _dataRow[fieldName] = RSet(value.Trim(), length);
                    break;
            }
        }

        /// <summary>
        /// Used by the Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name in the internal DataTable</param>
        /// <returns>The value stored in the column</returns>
        protected virtual bool GetBoolean(string fieldName)
        {
            return (bool)_dataRow[fieldName];
        }

        /// <summary>
        /// Used by the Properties in your generated class
        /// </summary>
        /// <param name="fieldName">The name of the column</param>
        /// <param name="value">The value to store</param>
        protected virtual void SetBoolean(string fieldName, bool value)
        {
            _dataRow[fieldName] = value;
        }

        /// <summary>
        /// Used by the Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name in the internal DataTable</param>
        /// <returns>The value stored in the column</returns>
        protected virtual short GetShort(string fieldName)
        {
            return (short)_dataRow[fieldName];
        }

        /// <summary>
        /// Used by the Properties in your generated class
        /// </summary>
        /// <param name="fieldName">The name of the column</param>
        /// <param name="value">The value to store</param>
        protected virtual void SetShort(string fieldName, short value)
        {
            _dataRow[fieldName] = value;
        }

        /// <summary>
        /// Used by the Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name in the internal DataTable</param>
        /// <returns>The value stored in the column</returns>
        protected virtual float GetSingle(string fieldName)
        {
            return (float)_dataRow[fieldName];
        }

        /// <summary>
        /// Used by the Properties in your generated class
        /// </summary>
        /// <param name="fieldName">The name of the column</param>
        /// <param name="value">The value to store</param>
        protected virtual void SetSingle(string fieldName, float value)
        {
            _dataRow[fieldName] = value;
        }

        /// <summary>
        /// Used by the Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name in the internal DataTable</param>
        /// <returns>The value stored in the column</returns>
        protected virtual Int32 GetInt32(string fieldName)
        {
            return (Int32)_dataRow[fieldName];
        }

        /// <summary>
        /// Used by the Properties in your generated class
        /// </summary>
        /// <param name="fieldName">The name of the column</param>
        /// <param name="value">The value to store</param>
        protected virtual void SetInt32(string fieldName, Int32 value)
        {
            _dataRow[fieldName] = value;
        }

        /// <summary>
        /// Used by the Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name in the internal DataTable</param>
        /// <returns>The value stored in the column</returns>
        protected virtual Int32 GetInteger(string fieldName)
        {
            return GetInt32(fieldName);
        }

        /// <summary>
        /// Used by the Properties in your generated class
        /// </summary>
        /// <param name="fieldName">The name of the column</param>
        /// <param name="value">The value to store</param>
        protected virtual void SetInteger(string fieldName, Int32 value)
        {
            SetInt32(fieldName, value);
        }

        /// <summary>
        /// Used by the Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name in the internal DataTable</param>
        /// <returns>The value stored in the column</returns>
        protected virtual long GetLong(string fieldName)
        {
            return (long)_dataRow[fieldName];
        }

        /// <summary>
        /// Used by the Properties in your generated class
        /// </summary>
        /// <param name="fieldName">The name of the column</param>
        /// <param name="value">The value to store</param>
        protected virtual void SetLong(string fieldName, long value)
        {
            _dataRow[fieldName] = value;
        }

        /// <summary>
        /// Used by the Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name in the internal DataTable</param>
        /// <returns>The value stored in the column</returns>
        protected virtual object GetObject(string fieldName)
        {
            return (object)_dataRow[fieldName];
        }

        /// <summary>
        /// Used by the Properties in your generated class
        /// </summary>
        /// <param name="fieldName">The name of the column</param>
        /// <param name="value">The value to store</param>
        protected virtual void SetObject(string fieldName, object value)
        {
            _dataRow[fieldName] = value;
        }

        /// <summary>
        /// Used by the Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name in the internal DataTable</param>
        /// <returns>The value stored in the column</returns>
        protected virtual byte[] GetByteArray(string fieldName)
        {
            return ((byte[])(_dataRow[fieldName]));
        }

        /// <summary>
        /// Used by the Properties in your generated class
        /// </summary>
        /// <param name="fieldName">The name of the column</param>
        /// <param name="value">The value to store</param>
        protected virtual void SetByteArray(string fieldName, byte[] value)
        {
            _dataRow[fieldName] = value;
        }

        /// <summary>
        /// Used by the Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name in the internal DataTable</param>
        /// <returns>The value stored in the column</returns>
        protected virtual byte GetByte(string fieldName)
        {
            return (byte)_dataRow[fieldName];
        }

        /// <summary>
        /// Used by the Properties in your generated class
        /// </summary>
        /// <param name="fieldName">The name of the column</param>
        /// <param name="value">The value to store</param>
        protected virtual void SetByte(string fieldName, byte value)
        {
            _dataRow[fieldName] = value;
        }

        /// <summary>
        /// Used by the Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name in the internal DataTable</param>
        /// <returns>The value stored in the column</returns>
        protected virtual float GetFloat(string fieldName)
        {
            return (float)_dataRow[fieldName];
        }

        /// <summary>
        /// Used by the Properties in your generated class
        /// </summary>
        /// <param name="fieldName">The name of the column</param>
        /// <param name="value">The value to store</param>
        protected virtual void SetFloat(string fieldName, float value)
        {
            _dataRow[fieldName] = value;
        }

        /// <summary>
        /// Used by the Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name in the internal DataTable</param>
        /// <returns>The value stored in the column</returns>
        protected virtual double GetDouble(string fieldName)
        {
            return (double)_dataRow[fieldName];
        }

        /// <summary>
        /// Used by the Properties in your generated class
        /// </summary>
        /// <param name="fieldName">The name of the column</param>
        /// <param name="value">The value to store</param>
        protected virtual void SetDouble(string fieldName, double value)
        {
            _dataRow[fieldName] = value;
        }

        /// <summary>
        /// Used by the Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name in the internal DataTable</param>
        /// <returns>The value stored in the column</returns>
        protected virtual Decimal GetDecimal(string fieldName)
        {
            return (Decimal)_dataRow[fieldName];
        }

        /// <summary>
        /// Used by the Properties in your generated class
        /// </summary>
        /// <param name="fieldName">The name of the column</param>
        /// <param name="value">The value to store</param>
        protected virtual void SetDecimal(string fieldName, Decimal value)
        {
            _dataRow[fieldName] = value;
        }

        /// <summary>
        /// Used by the Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name in the internal DataTable</param>
        /// <returns>The value stored in the column</returns>
        protected virtual DateTime GetDateTime(string fieldName)
        {
            return (DateTime)_dataRow[fieldName];
        }

        /// <summary>
        /// Used by the Properties in your generated class
        /// </summary>
        /// <param name="fieldName">The name of the column</param>
        /// <param name="value">The value to store</param>
        protected virtual void SetDateTime(string fieldName, DateTime value)
        {
            _dataRow[fieldName] = value;
        }

        /// <summary>
        /// Used by the Properties in your generated class
        /// </summary>
        /// <param name="fieldName">The name of the column</param>
        /// <param name="value">The value to store</param>
        /// <returns>The value stored in the column</returns>
        protected virtual void SetGuid(string fieldName, Guid value)
        {
            _dataRow[fieldName] = value;
        }

        /// <summary>
        /// Used by the Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name in the internal DataTable</param>
        protected virtual Guid GetGuid(string fieldName)
        {
            return (Guid)_dataRow[fieldName];
        }

        /// <summary>
        /// Used by the Properties in your generated class
        /// </summary>
        /// <param name="fieldName">The name of the column</param>
        /// <param name="value">The value to store</param>
        /// <returns>The value stored in the column</returns>
        protected virtual void SetTimeSpan(string fieldName, TimeSpan value)
        {
            _dataRow[fieldName] = value;
        }

        /// <summary>
        /// Used by the Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name in the internal DataTable</param>
        protected virtual TimeSpan GetTimeSpan(string fieldName)
        {
            return (TimeSpan)_dataRow[fieldName];
        }

        /// <summary>
        /// Retrieves the string value of the <see cref="SchemaItem"/>
        /// </summary>
        /// <param name="item">The <see cref="SchemaItem"/> to retrieve</param>
        /// <returns>A string value of the item, or empty if the column contains a NULL</returns>
        public virtual string GetItemString(SchemaItem item)
        {
            return GetItemString(item, String.Empty);
        }

        /// <summary>
        /// Retrieves the string value of the <see cref="SchemaItem"/>
        /// </summary>
        /// <param name="item">The <see cref="SchemaItem"/> to retrieve</param>
        /// <param name="defaultValue">If the column contains a NULL, this value will be returned instead</param>
        /// <returns>A string value of the item, or the default value if the column contains a NULL</returns>
        public virtual string GetItemString(SchemaItem item, string defaultValue)
        {
            if (IsColumnNull(item.FieldName))
            {
                return String.Empty;
            }
            else
            {
                switch (item.DBType)
                {
                    case DbType.DateTime:
                        return GetDateTime(item.FieldName).ToString("MM/dd/yyyy");
                    case DbType.Byte:
                        return GetByte(item.FieldName) == 0 ? defaultValue : GetString(item.FieldName);
                    case DbType.Decimal:
                    case DbType.Double:
                        return GetDouble(item.FieldName) == 0 ? defaultValue : GetString(item.FieldName);
                    case DbType.Int32:
                        return GetInt32(item.FieldName) == 0 ? defaultValue : GetString(item.FieldName);
                    default:
                        return GetString(item.FieldName);
                }
            }
        }

        /// <summary>
        /// Sets the value for a <see cref="SchemaItem"/> in the internal DataTable.
        /// </summary>
        /// <param name="item">A <see cref="SchemaItem"/> to set</param>
        /// <param name="value">The value to store in the column</param>
        public virtual void SetItem(SchemaItem item, string value)
        {
            if ((value.Trim().Length > 0))
            {
                switch (item.DBType)
                {
                    case DbType.DateTime:
                        SetDateTime(item.FieldName, DateTime.Parse(value));
                        break;
                    case DbType.Int32:
                        SetInt32(item.FieldName, int.Parse(value));
                        break;
                    case DbType.Decimal:
                    case DbType.Double:
                        SetDouble(item.FieldName, double.Parse(value));
                        break;
                    default:
                        SetString(item.FieldName, value);
                        break;
                }
            }
        }

        /// <summary>
        /// Used by the String Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name to store the value in</param>
        /// <returns>The value</returns>
        protected string GetGuidAsString(string fieldName)
        {
            return _dataRow[fieldName].ToString();
        }

        /// <summary>
        /// Used by the String Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name to store the value in</param>
        /// <param name="data">The Value</param>
        /// <returns>The Strong Type</returns>
        protected Guid SetGuidAsString(string fieldName, string data)
        {
            return new Guid(data);
        }

        /// <summary>
        /// Used by the String Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name to store the value in</param>
        /// <returns>The value</returns>
        protected string GetBoolAsString(string fieldName)
        {
            return _dataRow[fieldName].ToString();
        }

        /// <summary>
        /// Used by the String Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name to store the value in</param>
        /// <returns>The value</returns>
        protected string GetBooleanAsString(string fieldName)
        {
            return GetBoolAsString(fieldName);
        }

        /// <summary>
        /// Used by the String Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name to store the value in</param>
        /// <param name="data">The Value</param>
        /// <returns>The Strong Type</returns>
        protected bool SetBoolAsString(string fieldName, string data)
        {
            return Convert.ToBoolean(data);
        }

        /// <summary>
        /// Used by the String Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name to store the value in</param>
        /// <param name="data">The Value</param>
        /// <returns>The Strong Type</returns>
        protected bool SetBooleanAsString(string fieldName, string data)
        {
            return Convert.ToBoolean(data);
        }

        /// <summary>
        /// Used by the String Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name to store the value in</param>
        /// <returns>The value</returns>
        protected string GetStringAsString(string fieldName)
        {
            return (string)_dataRow[fieldName];
        }

        /// <summary>
        /// Used by the String Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name to store the value in</param>
        /// <param name="data">The Value</param>
        /// <returns>The Strong Type</returns>
        protected string SetStringAsString(string fieldName, string data)
        {
            return data;
        }

        /// <summary>
        /// Used by the String Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name to store the value in</param>
        /// <returns>The value</returns>
        protected string GetIntAsString(string fieldName)
        {
            return _dataRow[fieldName].ToString();
        }

        /// <summary>
        /// Used by the String Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name to store the value in</param>
        /// <returns>The value</returns>
        protected string GetIntegerAsString(string fieldName)
        {
            return GetIntAsString(fieldName);
        }

        /// <summary>
        /// Used by the String Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name to store the value in</param>
        /// <param name="data">The Value</param>
        /// <returns>The Strong Type</returns>
        protected int SetIntAsString(string fieldName, string data)
        {
            return Convert.ToInt32(data);
        }

        /// <summary>
        /// Used by the String Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name to store the value in</param>
        /// <param name="data">The Value</param>
        /// <returns>The Strong Type</returns>
        protected int SetIntegerAsString(string fieldName, string data)
        {
            return Convert.ToInt32(data);
        }

        /// <summary>
        /// Used by the String Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name to store the value in</param>
        /// <returns>The value</returns>
        protected string GetLongAsString(string fieldName)
        {
            return _dataRow[fieldName].ToString();
        }

        /// <summary>
        /// Used by the String Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name to store the value in</param>
        /// <param name="data">The Value</param>
        /// <returns>The Strong Type</returns>
        protected long SetLongAsString(string fieldName, string data)
        {
            return Convert.ToInt64(data);
        }

        /// <summary>
        /// Used by the String Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name to store the value in</param>
        /// <returns>The value</returns>
        protected string GetShortAsString(string fieldName)
        {
            return _dataRow[fieldName].ToString();
        }

        /// <summary>
        /// Used by the String Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name to store the value in</param>
        /// <param name="data">The Value</param>
        /// <returns>The Strong Type</returns>
        protected short SetShortAsString(string fieldName, string data)
        {
            return Convert.ToInt16(data);
        }

        /// <summary>
        /// Used by the String Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name to store the value in</param>
        /// <returns>The value</returns>
        protected string GetSingleAsString(string fieldName)
        {
            return _dataRow[fieldName].ToString();
        }

        /// <summary>
        /// Used by the String Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name to store the value in</param>
        /// <param name="data">The Value</param>
        /// <returns>The Strong Type</returns>
        protected float SetSingleAsString(string fieldName, string data)
        {
            return Convert.ToSingle(data);
        }

        /// <summary>
        /// Used by the String Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name to store the value in</param>
        /// <returns>The value</returns>
        protected string GetDateTimeAsString(string fieldName)
        {
            return ((DateTime)_dataRow[fieldName]).ToString(this.StringFormat);
        }

        /// <summary>
        /// Used by the String Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name to store the value in</param>
        /// <param name="data">The Value</param>
        /// <returns>The Strong Type</returns>
        protected DateTime SetDateTimeAsString(string fieldName, string data)
        {
            return Convert.ToDateTime(data);
        }

        /// <summary>
        /// Used by the String Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name to store the value in</param>
        /// <returns>The value</returns>
        protected string GetDecimalAsString(string fieldName)
        {
            return _dataRow[fieldName].ToString();
        }

        /// <summary>
        /// Used by the String Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name to store the value in</param>
        /// <param name="data">The Value</param>
        /// <returns>The Strong Type</returns>
        protected decimal SetDecimalAsString(string fieldName, string data)
        {
            return Convert.ToDecimal(data);
        }

        /// <summary>
        /// Used by the String Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name to store the value in</param>
        /// <returns>The value</returns>
        protected string GetFloatAsString(string fieldName)
        {
            return _dataRow[fieldName].ToString();
        }

        /// <summary>
        /// Used by the String Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name to store the value in</param>
        /// <param name="data">The Value</param>
        /// <returns>The Strong Type</returns>
        protected Single SetFloatAsString(string fieldName, string data)
        {
            return Convert.ToSingle(data);
        }

        /// <summary>
        /// Used by the String Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name to store the value in</param>
        /// <returns>The value</returns>
        protected string GetDoubleAsString(string fieldName)
        {
            return _dataRow[fieldName].ToString();
        }

        /// <summary>
        /// Used by the String Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name to store the value in</param>
        /// <param name="data">The Value</param>
        /// <returns>The Strong Type</returns>
        protected double SetDoubleAsString(string fieldName, string data)
        {
            return Convert.ToDouble(data);
        }

        /// <summary>
        /// Used by the String Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name to store the value in</param>
        /// <returns>The value</returns>
        protected string GetByteAsString(string fieldName)
        {
            return _dataRow[fieldName].ToString();
        }

        /// <summary>
        /// Used by the String Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name to store the value in</param>
        /// <param name="data">The Value</param>
        /// <returns>The Strong Type</returns>
        protected byte SetByteAsString(string fieldName, string data)
        {
            return Convert.ToByte(data);
        }

        /// <summary>
        /// Used by the String Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name to store the value in</param>
        /// <param name="data">The Value</param>
        /// <returns>The Strong Type</returns>
        protected TimeSpan SetTimeSpanAsString(string fieldName, string data)
        {
            return TimeSpan.Parse(data);
        }

        /// <summary>
        /// Used by the String Properties in your generated class. 
        /// </summary>
        /// <param name="fieldName">The column name to store the value in</param>
        /// <returns>The value</returns>
        protected string GetTimeSpanAsString(string fieldName)
        {
            return _dataRow[fieldName].ToString();
        }

        /// <summary>
        /// A format string used for displaying date values
        /// </summary>
        public string StringFormat = "MM/dd/yyyy";

        /// <summary>
        /// Allows for inserting primary key values that are normally autogenerated by the database
        /// <note>This may not be support for all database providers</note>
        /// </summary>
        public bool IdentityInsert
        {
            get { return _identityInsert; }
            set
            {
                _identityInsert = value;

                // Inserting IDENTITY values requires dynamic queries because
                // the stored procedures are not coded to support this yet
                if (_identityInsert) this.DefaultCommandType = CommandType.Text;
            }
        }

        /// <summary>
        /// Flag to convert empty strings to NULL when sending to the database
        /// </summary>
        public bool ConvertEmptyStringToNull
        {
            get { return _convertEmptyStringToNull; }
            set { _convertEmptyStringToNull = value; }
        }

        /// <summary>
        /// The command timeout to use when running queries against the database
        /// </summary>
        public int CommandTimeout
        {
            get { return _commandTimeout; }
            set { _commandTimeout = value; }
        }

        /// <summary>
        /// Returns the number of rows stored in the EasyObject
        /// </summary>
        public int RowCount
        {
            get
            {
                int count = 0;
                if (!(_dataTable == null))
                {
                    count = _dataTable.DefaultView.Count;
                }
                return count;
            }
        }

        /// <summary>
        /// Gets or sets the database instance name from the dataConfiguration.config file used to load the EasyObject.
        /// </summary>
        public string DatabaseInstanceName
        {
            get
            {
                return _databaseInstanceName;
            }
            set
            {
                _databaseInstanceName = value;
            }
        }

        /// <summary>
        /// Gets or set the dynamic query instance name from the dynamicQuerySettings.config file used to build dynamic queries.
        /// </summary>
        public string DynamicQueryInstanceName
        {
            get
            {
                return _dynamicQueryInstanceName;
            }
            set
            {
                _dynamicQueryInstanceName = value;
            }
        }

        /// <summary>
        /// Gets or sets the connection string used to connect to the database.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This will override any settings in dataConfiguration.config and instead connect directly using the connection
        /// string. Leave this field blank to use database instances from dataConfiguration.config.
        /// </para>
        /// <para>
        /// You can control the connection settings through the <see cref="UseIntegratedSecurity"/>, 
        /// <see cref="ConnectionUserID"/> and the <see cref="ConnectionPassword"/> properties.
        /// credentials.
        /// </para>
        /// </remarks>
        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }

        /// <summary>
        /// Gets or sets the user ID to use in the connect string for the database
        /// </summary>
        /// <remarks>
        /// </remarks>
        public string ConnectionUserID
        {
            get { return _connectionUserID; }
            set { _connectionUserID = value; }
        }

        /// <summary>
        /// Gets or sets the password to use in the connect string for the database
        /// </summary>
        /// <remarks>
        /// </remarks>
        public string ConnectionPassword
        {
            get { return _connectionPassword; }
            set { _connectionPassword = value; }
        }

        /// <summary>
        /// Gets or sets the database to connect to
        /// </summary>
        /// <remarks>
        /// </remarks>
        public string ConnectionDatabase
        {
            get { return _connectionDatabase; }
            set { _connectionDatabase = value; }
        }

        /// <summary>
        /// Gets or sets the database server to connect to
        /// </summary>
        /// <remarks>
        /// </remarks>
        public string ConnectionServer
        {
            get { return _connectionServer; }
            set { _connectionServer = value; }
        }

        /// <summary>
        /// Gets or sets the flag to use Integrated Security when connecting to the database
        /// </summary>
        /// <remarks>
        /// Defaults to false
        /// </remarks>
        public bool UseIntegratedSecurity
        {
            get { return _useIntegratedSecurity; }
            set { _useIntegratedSecurity = value; }
        }

        /// <summary>
        /// Gets or sets the internal error code.
        /// </summary>
        /// <remarks>This property is primarily set when an error has occurred after running a query.</remarks>
        public int ErrorCode
        {
            get
            {
                return _errorCode;
            }
            set
            {
                _errorCode = value;
            }
        }

        /// <summary>
        /// Gets or sets the internal error message.
        /// </summary>
        /// <remarks>This property is primarily set when an error has occurred after running a query.</remarks>
        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                _errorMessage = value;
            }
        }

        /// <summary>
        /// Returns a reference to the current row of the EasyObject.
        /// </summary>
        protected DataRow DataRow
        {
            get
            {
                return _dataRow;
            }
        }

        /// <summary>
        /// Returns a reference to the current DataTable for the EasyObject.
        /// </summary>
        protected internal DataTable DataTable
        {
            get
            {
                return _dataTable;
            }
            set
            {
                _dataTable = value;
                _dataTable.TableName = this.TableName;
                _dataRow = null;
                if (!(_dataTable == null))
                {
                    Rewind();
                }
            }
        }

        /// <summary>
        /// Returns a reference to the EasyObjects DefaultView.
        /// </summary>
        /// <remarks>This is primarily used for DataBind operations.</remarks>
        public DataView DefaultView
        {
            get
            {
                if (!(_dataTable == null))
                {
                    return _dataTable.DefaultView;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// After loading your EasyObject you can filter (temporarily hide) rows. To disable the filter 
        /// set this property to String.empty. After applying a filter, using Iteration via MoveNext will 
        /// properly respect any filter you have in place. See also <see cref="Sort"/>.
        /// </summary>
        /// <example>
        /// For a detailed explanation see the RowFilter property on ADO.NET's DataView.RowFilter property.
        /// <code>
        /// emps.Filter = "City = 'Berlin'";
        /// </code>
        /// </example>
        public string Filter
        {
            get
            {
                string _filter = "";

                if (_dataTable != null)
                {
                    _filter = _dataTable.DefaultView.RowFilter;
                }

                return _filter;
            }

            set
            {
                if (_dataTable != null)
                {
                    _dataTable.DefaultView.RowFilter = value;
                    Rewind();
                }
            }
        }

        /// <summary>
        /// Gets or sets a reference to the <see cref="SchemaEntries" /> for the EasyObject.
        /// </summary>
        /// <remarks>This is primarily set in the generated EasyObject class, but can be overridden in a derived class.</remarks>
        public List<SchemaItem> SchemaEntries
        {
            get
            {
                return _schemaEntries;
            }
            set
            {
                _schemaEntries = value;
            }
        }

        /// <summary>
        /// Returns the name of database table used to define the EasyObject.
        /// </summary>
        public virtual string TableName
        {
            get
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Used to set an alternate query source (such as a view) when building dynamic queries.
        /// </summary>
        /// <remarks>
        /// It is not necessary to load an EasyObject from the same table source. In fact, you can load an EasyObject
        /// from any table or view. However, not all of the strongly typed properties and methods are guaranteed to work it
        /// the query source is extremely divergent from the original table.
        /// </remarks>
        /// <example>
        /// VB.NET
        /// <code>
        /// Dim emps As New Employees
        /// 
        /// ' Load the object from a view instead of the table
        /// emps.QuerySource = "EmployeeDetailView"
        ///
        /// ' Only return the EmployeeID and LastName
        /// emps.Query.AddResultColumn(EmployeesSchema.EmployeeID)
        /// emps.Query.AddResultColumn(EmployeesSchema.LastName)
        ///
        /// ' Order by a column in the view but not the Employee table
        /// emps.Query.AddOrderBy("DateCertified", WhereParameter.Dir.ASC)
        ///
        /// emps.Query.Load()</code>
        ///	C#
        ///	<code>
        /// Employees emps = new Employees();
        ///
        /// // Load the object from a view instead of the table
        /// emps.QuerySource = "EmployeeDetailView";
        ///
        /// // Only return the EmployeeID and LastName
        /// emps.Query.AddResultColumn(EmployeesSchema.EmployeeID);
        /// emps.Query.AddResultColumn(EmployeesSchema.LastName);
        ///
        /// // Order by a column in the view but not the Employee table
        /// emps.Query.AddOrderBy("DateCertified", WhereParameter.Dir.ASC);
        ///
        /// emps.Query.Load();</code>
        /// </example>
        public virtual string QuerySource
        {
            get
            {
                if (string.Empty == _querySource) _querySource = this.TableName;
                return _querySource;
            }
            set
            {
                _querySource = value;
            }
        }

        /// <summary>
        /// Setting this is a quick way to set both <see cref="SchemaTableView"/> and <see cref="SchemaStoredProcedure"/>. at the same time.
        /// Setting this property does nothing more than assign both SchemaTableView and SchemaStoredProcedure, very rarely will you need to
        /// set SchemaTableView to one schema and SchemaStoredProcedure to a different schema. 
        /// </summary>
        virtual public string SchemaGlobal
        {
            get { return _schemaGlobal; }
            set
            {
                _schemaGlobal = value;
                SchemaTableView = value;
                SchemaStoredProcedure = value;
            }
        }

        /// <summary>
        /// This is the schema for the Table or View that will be accessed via Query.Load() and AddNew().  For instance, 
        /// if you set this to "HR" then Query.Load() will use "HR.EMPLOYEES" instead of just "EMPLOYEES". See <see cref="SchemaGlobal"/>.
        /// </summary>
        virtual public string SchemaTableView
        {
            get { return _tableViewSchema; }
            set { _tableViewSchema = value; }
        }

        /// <summary>
        /// This is the schema for the Table or View that will be accessed via Query.Load() and AddNew().  For instance, 
        /// if you set this to "HR" then Query.Load() will use "HR.EMPLOYEES" instead of just "EMPLOYEES". See <see cref="SchemaGlobal"/>.
        /// </summary>
        virtual public string SchemaTableViewWithSeparator
        {
            get
            {
                if (_tableViewSchema.Length > 0)
                {
                    return _tableViewSchema + this.SchemaSeparator;
                }
                else
                {
                    return _tableViewSchema;
                }
            }
        }

        /// <summary>
        /// This is the schema for the stored procedures that will be accessed by the EasyObject.  
        /// For instance, if you set this to "HR" then when you do an update your stored procedure will be 
        /// prefaced by "HR.MyStoredProc" See <see cref="SchemaGlobal"/>.
        /// </summary>
        virtual public string SchemaStoredProcedure
        {
            get
            {
                if (_storedProcSchema.Length > 0)
                {
                    return _storedProcSchema + this.SchemaSeparator;
                }
                else
                {
                    return _storedProcSchema;
                }
            }
            set { _storedProcSchema = value; }
        }

        /// <summary>
        /// This is the schema for the stored procedures that will be accessed by the EasyObject.  
        /// For instance, if you set this to "HR" then when you do an update your stored procedure will be 
        /// prefaced by "HR.MyStoredProc" See <see cref="SchemaGlobal"/>.
        /// </summary>
        virtual public string SchemaStoredProcedureWithSeparator
        {
            get
            {
                if (_storedProcSchema.Length > 0)
                {
                    return _storedProcSchema + this.SchemaSeparator;
                }
                else
                {
                    return _storedProcSchema;
                }
            }
        }

        /// <summary>
        /// Gets or sets a reference to the current EasyObject transaction.
        /// <note>This is primarily used internally.</note>
        /// </summary>
        protected IDbTransaction Transaction
        {
            get
            {
                return _tx;
            }
            set
            {
                _tx = value;
            }
        }

        /// <summary>
        /// Returns a reference to the EasyObject's <see cref="DynamicQuery" /> object.
        /// </summary>
        public DynamicQuery Query
        {
            get
            {
                if (_query == null)
                {
                    _query = new DynamicQueryProvider.SqlServerDynamicQuery(this);
                }

                return _query;
            }
        }

        /// <summary>
        /// Gets or sets the sort column for the EasyObject.
        /// </summary>
        public string Sort
        {
            get
            {
                string _sort = "";

                if (_dataTable != null)
                {
                    _sort = _dataTable.DefaultView.Sort;
                }

                return _sort;
            }

            set
            {
                if (_dataTable != null)
                {
                    _dataTable.DefaultView.Sort = value;
                    Rewind();
                }
            }
        }

        /// <summary>
        /// <para>Gets the string separator used between the schema and database object.</para>
        /// </summary>
        public virtual string SchemaSeparator
        {
            get { return "."; }
        }

        /// <summary>
        /// <para>Determines what CommandType will be used by default.</para>
        /// </summary>
        /// <remarks>Use this enumeration to toggle between stored procedures and inline SQL</remarks>
        public virtual CommandType DefaultCommandType
        {
            get { return _defaultCommandType; }
            set { _defaultCommandType = value; }
        }
    }
}