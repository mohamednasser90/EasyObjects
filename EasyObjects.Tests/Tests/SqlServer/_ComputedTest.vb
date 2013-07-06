'===============================================================================
'  Generated From - VbNet_EasyObject_BusinessEntity.vbgen
' 
'  ** IMPORTANT  ** 
'  
'  This object is 'MustInherit' which means you need to inherit from it to be able
'  to instantiate it.  This is very easily done. You can override properties and
'  methods in your derived class, this allows you to regenerate this class at any
'  time and not worry about overwriting custom code. 
'
'  NEVER EDIT THIS FILE.
'
'  Public Class YourObject 
'      Inherits _YourObject
'
'  End Class
'
'===============================================================================

' Generated by MyGeneration Version # (1.2.0.2)

Imports System
Imports System.Data
Imports System.Data.Common
Imports System.Configuration
Imports System.Collections
Imports System.Collections.Specialized
Imports System.Xml
Imports System.IO

Imports Microsoft.Practices.EnterpriseLibrary.Data
Imports NCI.EasyObjects


NameSpace EasyObjects.Tests.SQL

#Region " Schema "

Public Class ComputedTestSchema
	Inherits NCI.EasyObjects.Schema

    Private Shared _entries As ArrayList
	Public Shared RecordID As New SchemaItem("RecordID", DbType.Int32, True, False, False, True, False, False)
	Public Shared Integer1 As New SchemaItem("Integer1", DbType.Int32, False, True, False, False, False, False)
	Public Shared Integer2 As New SchemaItem("Integer2", DbType.Int32, False, True, False, False, False, False)
	Public Shared String1 As New SchemaItem("String1", DbType.AnsiString, SchemaItemJustify.None, 5, True, False, False, False)
	Public Shared String2 As New SchemaItem("String2", DbType.AnsiString, SchemaItemJustify.None, 5, True, False, False, False)
	Public Shared Computed1 As New SchemaItem("Computed1", DbType.Int32, False, True, True, False, False, False)
	Public Shared Computed2 As New SchemaItem("Computed2", DbType.AnsiString, SchemaItemJustify.None, 11, True, False, False, False)

    Public Overrides ReadOnly Property SchemaEntries() As ArrayList
        Get
            If _entries Is Nothing Then
                _entries = New ArrayList()
				_entries.Add(ComputedTestSchema.RecordID)
				_entries.Add(ComputedTestSchema.Integer1)
				_entries.Add(ComputedTestSchema.Integer2)
				_entries.Add(ComputedTestSchema.String1)
				_entries.Add(ComputedTestSchema.String2)
				_entries.Add(ComputedTestSchema.Computed1)
				ComputedTestSchema.Computed1.IsComputed = True
				_entries.Add(ComputedTestSchema.Computed2)
				ComputedTestSchema.Computed2.IsComputed = True

            End If
            Return _entries
        End Get
    End Property

End Class

#End Region

Public MustInherit Class _ComputedTest
    Inherits EasyObject

    Sub New()
        Dim _schema As New ComputedTestSchema()
        Me.SchemaEntries = _schema.SchemaEntries
		Me.SchemaGlobal = "dbo"
    End Sub

	Public Overrides Sub FlushData()
		Me._whereClause = Nothing
		Me._aggregateClause = Nothing
		MyBase.FlushData()
	End Sub
		
	''' <summary>
	''' Loads the business object with info from the database, based on the requested primary key.
	''' </summary>
	''' <param name="RecordID"></param>
	''' <returns>A Boolean indicating success or failure of the query</returns>
	Public Function LoadByPrimaryKey(ByVal RecordID As Integer) As Boolean
		
		Select Case Me.DefaultCommandType
			Case CommandType.StoredProcedure
				Dim parameters As ListDictionary = New ListDictionary

				' Add in parameters
				parameters.Add(ComputedTestSchema.RecordID.FieldName, RecordID)

				Return MyBase.LoadFromSql(Me.SchemaStoredProcedureWithSeparator & "daab_GetComputedTest", parameters, CommandType.StoredProcedure)
				
			Case CommandType.Text
                Me.Query.ClearAll()
                Me.Where.WhereClauseReset()
				Me.Where.RecordID.Value = RecordID
				Return Me.Query.Load()

			Case Else
				Throw New ArgumentException("Invalid CommandType", "commandType")
				
		End Select
		
	End Function

    ''' <summary>
    ''' Loads all records from the table.
    ''' </summary>
    ''' <returns>A Boolean indicating success or failure of the query</returns>
    Public Function LoadAll() As Boolean
	
		Select Case Me.DefaultCommandType
		
			Case CommandType.StoredProcedure
				Dim parameters As ListDictionary = Nothing
				Return MyBase.LoadFromSql(Me.SchemaStoredProcedureWithSeparator & "daab_GetAllComputedTest", parameters, CommandType.StoredProcedure)
				
			Case CommandType.Text
                Me.Query.ClearAll()
                Me.Where.WhereClauseReset()
				Return Me.Query.Load()
			
			Case Else
				Throw New ArgumentException("Invalid CommandType", "commandType")
				
		End Select

    End Function

    ''' <summary>
    ''' Adds a new record to the internal table.
    ''' </summary>
	Public Overrides Sub AddNew()
		MyBase.AddNew()
		Me.ApplyDefaults()
	End Sub

	''' <summary>
	''' Apply any default values to columns
	''' </summary>
	Protected Overrides Sub ApplyDefaults()
	End Sub

    Protected Overrides Function GetInsertCommand(commandType As CommandType) As DbCommand
	
		Dim dbCommand As DbCommand
		
        ' Create the Database object, using the default database service. The
        ' default database service is determined through configuration.
        Dim db As Database = GetDatabase()
	
		Select Case commandType
		
			Case CommandType.StoredProcedure
				Dim sqlCommand As String = Me.SchemaStoredProcedureWithSeparator & "daab_AddComputedTest"
				dbCommand = db.GetStoredProcCommand(sqlCommand)
				
				db.AddParameter(dbCommand, "RecordID", DbType.Int32, 0, ParameterDirection.Output, True, 0, 0, "RecordID", DataRowVersion.Default, Convert.DBNull)
				CreateParameters(db, dbCommand)

			Case CommandType.Text
                Me.Query.ClearAll()
				Me.Where.WhereClauseReset()
				For Each item As SchemaItem In Me.SchemaEntries
                    If Not item.IsComputed Then
                        If (item.IsAutoKey AndAlso Me.IdentityInsert) OrElse Not item.IsAutoKey Then
                            Me.Query.AddInsertColumn(item)
                        End If
                    End If
				Next
				dbCommand = Me.Query.GetInsertCommandWrapper()

				dbCommand.Parameters.Clear()
				If Me.IdentityInsert Then
					db.AddInParameter(dbCommand, "RecordID", DbType.Int32, "RecordID", DataRowVersion.Default)
				Else
					db.AddParameter(dbCommand, "RecordID", DbType.Int32, 0, ParameterDirection.Output, True, 0, 0, "RecordID", DataRowVersion.Default, Convert.DBNull)
				End If

				CreateParameters(db, dbCommand)

			Case Else
				Throw New ArgumentException("Invalid CommandType", "commandType")
				
		End Select

        Return dbCommand

    End Function

    Protected Overrides Function GetUpdateCommand(commandType As CommandType) As DbCommand
	
		Dim dbCommand As DbCommand

        ' Create the Database object, using the default database service. The
        ' default database service is determined through configuration.
        Dim db As Database = GetDatabase()
	
		Select Case commandType
		
			Case CommandType.StoredProcedure
				Dim sqlCommand As String = Me.SchemaStoredProcedureWithSeparator & "daab_UpdateComputedTest"
				dbCommand = db.GetStoredProcCommand(sqlCommand)
		
				db.AddInParameter(dbCommand, "RecordID", DbType.Int32, "RecordID", DataRowVersion.Current)
				CreateParameters(db, dbCommand)

			Case CommandType.Text
                Me.Query.ClearAll()
				For Each item As SchemaItem In Me.SchemaEntries
					If Not (item.IsAutoKey OrElse item.IsComputed)
						Me.Query.AddUpdateColumn(item)
					End If
				Next

				Me.Where.WhereClauseReset()
				Me.Where.RecordID.Operator = WhereParameter.Operand.Equal
				dbCommand = Me.Query.GetUpdateCommandWrapper()

				dbCommand.Parameters.Clear()
				CreateParameters(db, dbCommand)
				db.AddInParameter(dbCommand, "RecordID", DbType.Int32, "RecordID", DataRowVersion.Current)

			Case Else
				Throw New ArgumentException("Invalid CommandType", "commandType")
				
		End Select

        Return dbCommand

    End Function

    Protected Overrides Function GetDeleteCommand(commandType As CommandType) As DbCommand
	
		Dim dbCommand As DbCommand

        ' Create the Database object, using the default database service. The
        ' default database service is determined through configuration.
        Dim db As Database = GetDatabase()
	
		Select Case commandType
		
			Case CommandType.StoredProcedure
				Dim sqlCommand As String = Me.SchemaStoredProcedureWithSeparator & "daab_DeleteComputedTest"
				dbCommand = db.GetStoredProcCommand(sqlCommand)
		
				' Add primary key parameters
				db.AddInParameter(dbCommand, "RecordID", DbType.Int32, "RecordID", DataRowVersion.Current)

			Case CommandType.Text
                Me.Query.ClearAll()
				Me.Where.WhereClauseReset()
				Me.Where.RecordID.Operator = WhereParameter.Operand.Equal
				dbCommand = Me.Query.GetDeleteCommandWrapper()

				dbCommand.Parameters.Clear()
				db.AddInParameter(dbCommand, "RecordID", DbType.Int32, "RecordID", DataRowVersion.Current)

			Case Else
				Throw New ArgumentException("Invalid CommandType", "commandType")
				
		End Select

        Return dbCommand

    End Function

    Private Sub CreateParameters(ByVal db As Database, ByVal dbCommand As DbCommand)
		
		db.AddInParameter(dbCommand, "Integer1", DbType.Int32, "Integer1", DataRowVersion.Current)
		db.AddInParameter(dbCommand, "Integer2", DbType.Int32, "Integer2", DataRowVersion.Current)
		db.AddInParameter(dbCommand, "String1", DbType.AnsiString, "String1", DataRowVersion.Current)
		db.AddInParameter(dbCommand, "String2", DbType.AnsiString, "String2", DataRowVersion.Current)
		db.AddParameter(dbCommand, "Computed1", DbType.Int32, ParameterDirection.Output, "Computed1", DataRowVersion.Current, Nothing)
		db.AddParameter(dbCommand, "Computed2", DbType.AnsiString, 11, ParameterDirection.Output, true, 0, 0, "Computed2", DataRowVersion.Current, Nothing)

    End Sub

#Region " Properties "

	Public Overridable Property RecordID() As Integer
        Get
			Return Me.GetInteger(ComputedTestSchema.RecordID.FieldName)
      End Get
        Set(ByVal Value As Integer)
			Me.SetInteger(ComputedTestSchema.RecordID.FieldName, Value)
      End Set
    End Property

	Public Overridable Property Integer1() As Integer
        Get
			Return Me.GetInteger(ComputedTestSchema.Integer1.FieldName)
      End Get
        Set(ByVal Value As Integer)
			Me.SetInteger(ComputedTestSchema.Integer1.FieldName, Value)
      End Set
    End Property

	Public Overridable Property Integer2() As Integer
        Get
			Return Me.GetInteger(ComputedTestSchema.Integer2.FieldName)
      End Get
        Set(ByVal Value As Integer)
			Me.SetInteger(ComputedTestSchema.Integer2.FieldName, Value)
      End Set
    End Property

	Public Overridable Property String1() As String
        Get
			Return Me.GetString(ComputedTestSchema.String1.FieldName)
      End Get
        Set(ByVal Value As String)
			Me.SetString(ComputedTestSchema.String1.FieldName, Value)
      End Set
    End Property

	Public Overridable Property String2() As String
        Get
			Return Me.GetString(ComputedTestSchema.String2.FieldName)
      End Get
        Set(ByVal Value As String)
			Me.SetString(ComputedTestSchema.String2.FieldName, Value)
      End Set
    End Property

	Public Overridable Property Computed1() As Integer
        Get
			Return Me.GetInteger(ComputedTestSchema.Computed1.FieldName)
      End Get
        Set(ByVal Value As Integer)
			Me.SetInteger(ComputedTestSchema.Computed1.FieldName, Value)
      End Set
    End Property

	Public Overridable Property Computed2() As String
        Get
			Return Me.GetString(ComputedTestSchema.Computed2.FieldName)
      End Get
        Set(ByVal Value As String)
			Me.SetString(ComputedTestSchema.Computed2.FieldName, Value)
      End Set
    End Property

    Public Overrides ReadOnly Property TableName() As String
        Get
            Return "ComputedTest"
        End Get
    End Property

#End Region

#Region " String Properties "
		Public Overridable Property s_RecordID As String
			Get
				If Me.IsColumnNull(ComputedTestSchema.RecordID.FieldName) Then
					Return String.Empty
				Else
					Return MyBase.GetIntegerAsString(ComputedTestSchema.RecordID.FieldName)
				End If
			End Get
			Set(ByVal Value As String)
				If String.Empty = value Then
					Me.SetColumnNull(ComputedTestSchema.RecordID.FieldName)
				Else
					Me.RecordID = MyBase.SetIntegerAsString(ComputedTestSchema.RecordID.FieldName, Value)
				End If
			End Set
		End Property

		Public Overridable Property s_Integer1 As String
			Get
				If Me.IsColumnNull(ComputedTestSchema.Integer1.FieldName) Then
					Return String.Empty
				Else
					Return MyBase.GetIntegerAsString(ComputedTestSchema.Integer1.FieldName)
				End If
			End Get
			Set(ByVal Value As String)
				If String.Empty = value Then
					Me.SetColumnNull(ComputedTestSchema.Integer1.FieldName)
				Else
					Me.Integer1 = MyBase.SetIntegerAsString(ComputedTestSchema.Integer1.FieldName, Value)
				End If
			End Set
		End Property

		Public Overridable Property s_Integer2 As String
			Get
				If Me.IsColumnNull(ComputedTestSchema.Integer2.FieldName) Then
					Return String.Empty
				Else
					Return MyBase.GetIntegerAsString(ComputedTestSchema.Integer2.FieldName)
				End If
			End Get
			Set(ByVal Value As String)
				If String.Empty = value Then
					Me.SetColumnNull(ComputedTestSchema.Integer2.FieldName)
				Else
					Me.Integer2 = MyBase.SetIntegerAsString(ComputedTestSchema.Integer2.FieldName, Value)
				End If
			End Set
		End Property

		Public Overridable Property s_String1 As String
			Get
				If Me.IsColumnNull(ComputedTestSchema.String1.FieldName) Then
					Return String.Empty
				Else
					Return MyBase.GetStringAsString(ComputedTestSchema.String1.FieldName)
				End If
			End Get
			Set(ByVal Value As String)
				If String.Empty = value Then
					Me.SetColumnNull(ComputedTestSchema.String1.FieldName)
				Else
					Me.String1 = MyBase.SetStringAsString(ComputedTestSchema.String1.FieldName, Value)
				End If
			End Set
		End Property

		Public Overridable Property s_String2 As String
			Get
				If Me.IsColumnNull(ComputedTestSchema.String2.FieldName) Then
					Return String.Empty
				Else
					Return MyBase.GetStringAsString(ComputedTestSchema.String2.FieldName)
				End If
			End Get
			Set(ByVal Value As String)
				If String.Empty = value Then
					Me.SetColumnNull(ComputedTestSchema.String2.FieldName)
				Else
					Me.String2 = MyBase.SetStringAsString(ComputedTestSchema.String2.FieldName, Value)
				End If
			End Set
		End Property

		Public Overridable ReadOnly Property s_Computed1 As String
			Get
				If Me.IsColumnNull(ComputedTestSchema.Computed1.FieldName) Then
					Return String.Empty
				Else
					Return MyBase.GetIntegerAsString(ComputedTestSchema.Computed1.FieldName)
				End If
			End Get
		End Property

		Public Overridable ReadOnly Property s_Computed2 As String
			Get
				If Me.IsColumnNull(ComputedTestSchema.Computed2.FieldName) Then
					Return String.Empty
				Else
					Return MyBase.GetStringAsString(ComputedTestSchema.Computed2.FieldName)
				End If
			End Get
		End Property


#End Region

#Region " Where Clause "
    Public Class WhereClause

        Public Sub New(ByVal entity As EasyObject)
            Me._entity = entity
        End Sub
		
		Public ReadOnly Property TearOff As TearOffWhereParameter
			Get
				If _tearOff Is Nothing Then
					_tearOff = new TearOffWhereParameter(Me)
				End If

				Return _tearOff
			End Get
		End Property

		#Region " TearOff's "
		Public class TearOffWhereParameter
		
			Private _clause as WhereClause
			
			Public Sub New(ByVal clause As WhereClause)
				Me._clause = clause
			End Sub

			Public ReadOnly Property RecordID() As WhereParameter
				Get
					Dim wp As WhereParameter = New WhereParameter(ComputedTestSchema.RecordID)
                    Dim query As NCI.EasyObjects.DynamicQuery = Me._clause._entity.Query
                    query.AddWhereParameter(wp)
					Return wp
				End Get
			End Property

			Public ReadOnly Property Integer1() As WhereParameter
				Get
					Dim wp As WhereParameter = New WhereParameter(ComputedTestSchema.Integer1)
                    Dim query As NCI.EasyObjects.DynamicQuery = Me._clause._entity.Query
                    query.AddWhereParameter(wp)
					Return wp
				End Get
			End Property

			Public ReadOnly Property Integer2() As WhereParameter
				Get
					Dim wp As WhereParameter = New WhereParameter(ComputedTestSchema.Integer2)
                    Dim query As NCI.EasyObjects.DynamicQuery = Me._clause._entity.Query
                    query.AddWhereParameter(wp)
					Return wp
				End Get
			End Property

			Public ReadOnly Property String1() As WhereParameter
				Get
					Dim wp As WhereParameter = New WhereParameter(ComputedTestSchema.String1)
                    Dim query As NCI.EasyObjects.DynamicQuery = Me._clause._entity.Query
                    query.AddWhereParameter(wp)
					Return wp
				End Get
			End Property

			Public ReadOnly Property String2() As WhereParameter
				Get
					Dim wp As WhereParameter = New WhereParameter(ComputedTestSchema.String2)
                    Dim query As NCI.EasyObjects.DynamicQuery = Me._clause._entity.Query
                    query.AddWhereParameter(wp)
					Return wp
				End Get
			End Property

			Public ReadOnly Property Computed1() As WhereParameter
				Get
					Dim wp As WhereParameter = New WhereParameter(ComputedTestSchema.Computed1)
                    Dim query As NCI.EasyObjects.DynamicQuery = Me._clause._entity.Query
                    query.AddWhereParameter(wp)
					Return wp
				End Get
			End Property

			Public ReadOnly Property Computed2() As WhereParameter
				Get
					Dim wp As WhereParameter = New WhereParameter(ComputedTestSchema.Computed2)
                    Dim query As NCI.EasyObjects.DynamicQuery = Me._clause._entity.Query
                    query.AddWhereParameter(wp)
					Return wp
				End Get
			End Property

	End Class
	
	#End Region	

		Public ReadOnly Property RecordID() As WhereParameter 
			Get
				If _RecordID_W Is Nothing Then
					_RecordID_W = TearOff.RecordID
				End If
				Return _RecordID_W
			End Get
		End Property

		Public ReadOnly Property Integer1() As WhereParameter 
			Get
				If _Integer1_W Is Nothing Then
					_Integer1_W = TearOff.Integer1
				End If
				Return _Integer1_W
			End Get
		End Property

		Public ReadOnly Property Integer2() As WhereParameter 
			Get
				If _Integer2_W Is Nothing Then
					_Integer2_W = TearOff.Integer2
				End If
				Return _Integer2_W
			End Get
		End Property

		Public ReadOnly Property String1() As WhereParameter 
			Get
				If _String1_W Is Nothing Then
					_String1_W = TearOff.String1
				End If
				Return _String1_W
			End Get
		End Property

		Public ReadOnly Property String2() As WhereParameter 
			Get
				If _String2_W Is Nothing Then
					_String2_W = TearOff.String2
				End If
				Return _String2_W
			End Get
		End Property

		Public ReadOnly Property Computed1() As WhereParameter 
			Get
				If _Computed1_W Is Nothing Then
					_Computed1_W = TearOff.Computed1
				End If
				Return _Computed1_W
			End Get
		End Property

		Public ReadOnly Property Computed2() As WhereParameter 
			Get
				If _Computed2_W Is Nothing Then
					_Computed2_W = TearOff.Computed2
				End If
				Return _Computed2_W
			End Get
		End Property

		Private _RecordID_W As WhereParameter = Nothing
		Private _Integer1_W As WhereParameter = Nothing
		Private _Integer2_W As WhereParameter = Nothing
		Private _String1_W As WhereParameter = Nothing
		Private _String2_W As WhereParameter = Nothing
		Private _Computed1_W As WhereParameter = Nothing
		Private _Computed2_W As WhereParameter = Nothing

		Public Sub WhereClauseReset()

		_RecordID_W = Nothing
		_Integer1_W = Nothing
		_Integer2_W = Nothing
		_String1_W = Nothing
		_String2_W = Nothing
		_Computed1_W = Nothing
		_Computed2_W = Nothing
			Me._entity.Query.FlushWhereParameters()

		End Sub
	
		Private _entity As EasyObject
		Private _tearOff As TearOffWhereParameter
    End Class	

	Public ReadOnly Property Where() As WhereClause
		Get
			If _whereClause Is Nothing Then
				_whereClause = New WhereClause(Me)
			End If
	
			Return _whereClause
		End Get
	End Property
	
	Private _whereClause As WhereClause = Nothing	
#End Region	

#Region " Aggregate Clause "
    Public Class AggregateClause

        Public Sub New(ByVal entity As EasyObject)
            Me._entity = entity
        End Sub
		
		Public ReadOnly Property TearOff As TearOffAggregateParameter
			Get
				If _tearOff Is Nothing Then
					_tearOff = new TearOffAggregateParameter(Me)
				End If

				Return _tearOff
			End Get
		End Property

		#Region " TearOff's "
		Public class TearOffAggregateParameter
		
			Private _clause as AggregateClause
			
			Public Sub New(ByVal clause As AggregateClause)
				Me._clause = clause
			End Sub

			Public ReadOnly Property RecordID() As AggregateParameter
				Get
					Dim ap As AggregateParameter = New AggregateParameter(ComputedTestSchema.RecordID)
                    Dim query As NCI.EasyObjects.DynamicQuery = Me._clause._entity.Query
                    query.AddAggregateParameter(ap)
					Return ap
				End Get
			End Property

			Public ReadOnly Property Integer1() As AggregateParameter
				Get
					Dim ap As AggregateParameter = New AggregateParameter(ComputedTestSchema.Integer1)
                    Dim query As NCI.EasyObjects.DynamicQuery = Me._clause._entity.Query
                    query.AddAggregateParameter(ap)
					Return ap
				End Get
			End Property

			Public ReadOnly Property Integer2() As AggregateParameter
				Get
					Dim ap As AggregateParameter = New AggregateParameter(ComputedTestSchema.Integer2)
                    Dim query As NCI.EasyObjects.DynamicQuery = Me._clause._entity.Query
                    query.AddAggregateParameter(ap)
					Return ap
				End Get
			End Property

			Public ReadOnly Property String1() As AggregateParameter
				Get
					Dim ap As AggregateParameter = New AggregateParameter(ComputedTestSchema.String1)
                    Dim query As NCI.EasyObjects.DynamicQuery = Me._clause._entity.Query
                    query.AddAggregateParameter(ap)
					Return ap
				End Get
			End Property

			Public ReadOnly Property String2() As AggregateParameter
				Get
					Dim ap As AggregateParameter = New AggregateParameter(ComputedTestSchema.String2)
                    Dim query As NCI.EasyObjects.DynamicQuery = Me._clause._entity.Query
                    query.AddAggregateParameter(ap)
					Return ap
				End Get
			End Property

			Public ReadOnly Property Computed1() As AggregateParameter
				Get
					Dim ap As AggregateParameter = New AggregateParameter(ComputedTestSchema.Computed1)
                    Dim query As NCI.EasyObjects.DynamicQuery = Me._clause._entity.Query
                    query.AddAggregateParameter(ap)
					Return ap
				End Get
			End Property

			Public ReadOnly Property Computed2() As AggregateParameter
				Get
					Dim ap As AggregateParameter = New AggregateParameter(ComputedTestSchema.Computed2)
                    Dim query As NCI.EasyObjects.DynamicQuery = Me._clause._entity.Query
                    query.AddAggregateParameter(ap)
					Return ap
				End Get
			End Property

	End Class
	
	#End Region	

		Public ReadOnly Property RecordID() As AggregateParameter 
			Get
				If _RecordID_W Is Nothing Then
					_RecordID_W = TearOff.RecordID
				End If
				Return _RecordID_W
			End Get
		End Property

		Public ReadOnly Property Integer1() As AggregateParameter 
			Get
				If _Integer1_W Is Nothing Then
					_Integer1_W = TearOff.Integer1
				End If
				Return _Integer1_W
			End Get
		End Property

		Public ReadOnly Property Integer2() As AggregateParameter 
			Get
				If _Integer2_W Is Nothing Then
					_Integer2_W = TearOff.Integer2
				End If
				Return _Integer2_W
			End Get
		End Property

		Public ReadOnly Property String1() As AggregateParameter 
			Get
				If _String1_W Is Nothing Then
					_String1_W = TearOff.String1
				End If
				Return _String1_W
			End Get
		End Property

		Public ReadOnly Property String2() As AggregateParameter 
			Get
				If _String2_W Is Nothing Then
					_String2_W = TearOff.String2
				End If
				Return _String2_W
			End Get
		End Property

		Public ReadOnly Property Computed1() As AggregateParameter 
			Get
				If _Computed1_W Is Nothing Then
					_Computed1_W = TearOff.Computed1
				End If
				Return _Computed1_W
			End Get
		End Property

		Public ReadOnly Property Computed2() As AggregateParameter 
			Get
				If _Computed2_W Is Nothing Then
					_Computed2_W = TearOff.Computed2
				End If
				Return _Computed2_W
			End Get
		End Property

		Private _RecordID_W As AggregateParameter = Nothing
		Private _Integer1_W As AggregateParameter = Nothing
		Private _Integer2_W As AggregateParameter = Nothing
		Private _String1_W As AggregateParameter = Nothing
		Private _String2_W As AggregateParameter = Nothing
		Private _Computed1_W As AggregateParameter = Nothing
		Private _Computed2_W As AggregateParameter = Nothing

		Public Sub AggregateClauseReset()

		_RecordID_W = Nothing
		_Integer1_W = Nothing
		_Integer2_W = Nothing
		_String1_W = Nothing
		_String2_W = Nothing
		_Computed1_W = Nothing
		_Computed2_W = Nothing
			Me._entity.Query.FlushAggregateParameters()

		End Sub
	
		Private _entity As EasyObject
		Private _tearOff As TearOffAggregateParameter
    End Class	

	Public ReadOnly Property Aggregate() As AggregateClause
		Get
			If _aggregateClause Is Nothing Then
				_aggregateClause = New AggregateClause(Me)
			End If
	
			Return _aggregateClause
		End Get
	End Property
	
	Private _aggregateClause As AggregateClause = Nothing	
#End Region	

End Class

End NameSpace


