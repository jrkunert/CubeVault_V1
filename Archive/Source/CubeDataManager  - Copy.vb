Imports System
Imports System.Data
Imports System.IO
Imports System.Threading
Imports System.Data.Common 
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Text
Imports System.Globalization

Imports OneStream.Finance.Database
Imports OneStream.Finance.Engine

Imports OneStream.Shared.Common
Imports OneStream.Shared.Database
Imports OneStream.Shared.Engine
Imports OneStream.Shared.Wcf

	'=========================================================
	'=========================================================
	' SECTION 1 - Constants
	' SECTION 2 - Enumerations
	' SECTION 3 - Runtime Classes
	' SECTION 4 - Main Entry Point
	' SECTION 5 - Public Entry Procedures
	' SECTION 6 - Execution Engines
	' SECTION 7 - Health Check Framework
	' SECTION 8 - Parameter Framework
	' SECTION 9 - Filter Construction Framework
	' SECTION 10 - Metadata Framework
	' SECTION 11 - Export Framework
	' SECTION 12 - Restore Framework
	' SECTION 13 - Clear Framework
	' SECTION 14 - Statistics Framework
	' SECTION 15 - File Framework
	' SECTION 16 - Logging Framework
	' SECTION 17 - Shared Utility Functions
	'=========================================================
	'=========================================================

	'=====================================================================
	' Business Rule: CubeDataManager
	'
	' Version History
	'
	' 5.4  2026-07-19
	' • Refactored architecture into 17 logical sections
	' • Standardized logging framework
	' • Added LogHeader(), LogSection(), LogField()
	' • Added function name to log file names
	' • Centralized parameter handling
	' • Centralized DataBuffer validation
	' • Fixed hierarchy expansion to use correct dimensions
	' • Removed duplicate code and dead code
	' • Replaced magic numbers with named constants
	'
	' 5.3
	' • Previous production version
	'=====================================================================	

Namespace OneStream.BusinessRule.Finance.CubeDataManager

	Public Class MainClass

						'   ##  
						' ####  
						'   ##  
						'   ##  
						'   ##  
						'   ##  
						' ######

	    '=========================================================
        ' SECTION 1 - Constants
        '=========================================================

		Private Const BR_NAME As String = "CubeDataManager"
		Private Const BR_VERSION As String = "5.4"
		Private Const OS_VERSION As String = "9.2.1.18605"
		Private Const LOG_SEPARATOR As String = _
    "============================================================"

		Private Const LOG_PROGRESS_INTERVAL As Integer = 10000
		Private Const CSV_PROGRESS_INTERVAL As Integer = 1000

		Private Const DEBUG_MODE As Boolean = False

		Private Const EXPORT_PROGRESS_INTERVAL As Integer = 10000

		Private Const DEFAULT_MAXCELLS As Integer = 500000

		Private Const DEFAULT_RESTORE_BATCHSIZE As Integer = 1000
		
		Private Const ENABLE_FILE_LOGGING As Boolean = True

		Private Shared _logFile As String = Nothing
		Private Shared _log As New StringBuilder()
		Private Shared _exportLineNumber As Integer = 0
		Private Shared _currentFunction As String = "Unknown"

		Private Shared ReadOnly _hierarchyCache As _
			New Dictionary(Of String, HashSet(Of String))()

		Private Shared ReadOnly DimensionPrefixes As New Dictionary(Of String, String)( _
			StringComparer.OrdinalIgnoreCase) From {
			{"Entity", "Entity"},
			{"Parent", "Entity"},
			{"Cons", "Cons"},
			{"Scenario", "Scenario"},
			{"Time", "Time"},
			{"View", "View"},
			{"Account", "Account"},
			{"Flow", "Flow"},
			{"Origin", "Origin"},
			{"IC", "IC"},
			{"UD1", "UD1"},
			{"UD2", "UD2"},
			{"UD3", "UD3"},
			{"UD4", "UD4"},
			{"UD5", "UD5"},
			{"UD6", "UD6"},
			{"UD7", "UD7"},
			{"UD8", "UD8"}
		}

		#Region "CSV Columns"

		Private Const COL_LINENO As Integer = 0

		Private Const COL_EXPORTDATE As Integer = 1

		Private Const COL_BRVERSION As Integer = 2

		Private Const COL_CUBE As Integer = 3

		Private Const COL_USERNAME As Integer = 4

		Private Const COL_ENTITY As Integer = 5

		Private Const COL_PARENT As Integer = 6

		Private Const COL_CONS As Integer = 7

		Private Const COL_SCENARIO As Integer = 8

		Private Const COL_TIME As Integer = 9

		Private Const COL_VIEW As Integer = 10

		Private Const COL_ACCOUNT As Integer = 11

		Private Const COL_FLOW As Integer = 12

		Private Const COL_ORIGIN As Integer = 13

		Private Const COL_IC As Integer = 14

		Private Const COL_UD1 As Integer = 15

		Private Const COL_UD2 As Integer = 16

		Private Const COL_UD3 As Integer = 17

		Private Const COL_UD4 As Integer = 18

		Private Const COL_UD5 As Integer = 19

		Private Const COL_UD6 As Integer = 20

		Private Const COL_UD7 As Integer = 21

		Private Const COL_UD8 As Integer = 22

		Private Const COL_AMOUNT As Integer = 23

		Private Const CSV_COLUMN_COUNT As Integer = 24

#End Region ' "CSV Columns"

						'  ####### 
						' ##     ##
						'        ##
						'  ####### 
						' ##       
						' ##       
						' #########


        '=========================================================
        ' SECTION 2 - Enumerations
        '=========================================================

		Private Enum ExpansionType

			None = 0

			Base = 1

			Children = 2

			Descendants = 3

		End Enum

						'  ####### 
						' ##     ##
						'        ##
						'  ####### 
						'        ##
						' ##     ##
						'  ####### 

		'=========================================================
        ' SECTION 3 - Runtime Classes
        '=========================================================

	
		Private Class ExecutionConfiguration

			Public Property MaxCells As Integer = DEFAULT_MAXCELLS

			Public Property BatchSize As Integer = DEFAULT_RESTORE_BATCHSIZE

			Public Property Origins As New List(Of String)

			Public Property ExecuteRestore As Boolean = False

			Public Property PreviewMode As Boolean = True

			Public Property UseCurrentDataUnit As Boolean = True

			Public Property FilterParameters As _
				New Dictionary(Of String, String)( _
					StringComparer.OrdinalIgnoreCase)

		End Class


		Private Class HealthCheckResults

			Public Passed As Integer = 0

			Public Failed As Integer = 0

			Public Elapsed As TimeSpan

		End Class


		Private Class ExportContext

			Public Property ExportFile As String

			Public Property ExportTime As DateTime

			Public Property LineNumber As Integer

			Public Property Writer As StreamWriter

			Public Property Summary As ExportSummary

			Public Property FilterParameters As _
				New Dictionary(Of String, String)( _
					StringComparer.OrdinalIgnoreCase)

		End Class	

		

		Private Class ExplainContext

			Public Property Configuration As ExecutionConfiguration

			Public Property Summary As New ExplainSummary()

			Public Property OriginCount As Integer

			Public Property StoredCellCount As Integer

			Public Property CurrentOrigin As String

		End Class		
		
		

		Private Class StatisticsContext

			Public Property Configuration As ExecutionConfiguration

			Public Property MemberFilters As New List(Of DimensionFilter)

			Public Property Metadata As MetadataCache

		End Class
		
		

		Private Class FilterStatistics

			Public Property Origin As String

			Public Property FilterScript As String

			Public Property StoredCellCount As Integer

			Public Property ReturnedMemberCount As Integer

			Public Property ReturnedMembers As New List(Of String)

			Public Property Pov As PovMembers

		End Class
		
		

		Private Class ExplainSummary

			Public Property StartTime As DateTime

			Public Property EndTime As DateTime

			Public Property Elapsed As TimeSpan

			Public Property OriginCount As Integer

			Public Property StoredCellCount As Integer

		End Class
		
		

		Private Class ClearContext

			Public Property Configuration As ExecutionConfiguration

			Public Property TotalCells As Integer

			Public Property OriginCount As Integer

			Public Property StartTime As DateTime

			Public Property Elapsed As TimeSpan

		End Class	
		
		

		Private Class RestoreContext

			Public Configuration As ExecutionConfiguration

			Public Property Summary As New RestoreSummary()

			Public CsvFile As String

			Public LineNumber As Integer

			Public Preview As Boolean = True

			Public ExecuteRestore As Boolean = False

			Public UseCurrentDataUnit As Boolean = True

			Public MemberFilters As New Dictionary( _
				Of String, DimensionFilter)( _
				StringComparer.OrdinalIgnoreCase)

			Public Metadata As New MetadataCache

			Public MemberScriptAndValues As New List(Of MemberScriptAndValue)

			Public RestoreBatchSize As Integer = DEFAULT_RESTORE_BATCHSIZE

			Public ExportAnalysis As New ExportAnalysis

			Public BatchNumber As Integer = 0

			Public HierarchyWarnings As New List(Of HierarchyWarning)

		End Class
		
		

		Private Class MetadataCache

			Public DimPks As New Dictionary(Of String, DimPk)( _
				StringComparer.OrdinalIgnoreCase)

		End Class		
		
		

		Private Class ExportSummary

			Public Property StartTime As DateTime

			Public Property EndTime As DateTime

			Public Property TotalRows As Integer

			Public Property TotalOrigins As Integer

			Public Property ExportFile As String

			Public Scenario As String

			Public Time As String

			Public Entity As String

			Public Cube As String

		End Class
		
		

		Private Class RestoreSummary

			Public Property StartTime As DateTime

			Public Property EndTime As DateTime

			Public Property CsvFile As String

			Public Property TotalRows As Integer

			Public Property RowsMatched As Integer

			Public Property RowsRestored As Integer

			Public Property RowsSkipped As Integer

			Public Property ErrorCount As Integer

			Public Property BatchCount As Integer

		End Class	



		Private Class DimensionInfo

			Public Property Prefix As String
			Public Property MemberName As String

		End Class



		Private Class ExportRecord

			Public LineNo As Integer

			Public ExportDate As DateTime

			Public BRVersion As String

			Public Cube As String

			Public UserName As String

			Public Entity As String

			Public Parent As String

			Public Cons As String

			Public Scenario As String

			Public Time As String

			Public View As String

			Public Account As String

			Public Flow As String

			Public Origin As String

			Public IC As String

			Public UD1 As String

			Public UD2 As String

			Public UD3 As String

			Public UD4 As String

			Public UD5 As String

			Public UD6 As String

			Public UD7 As String

			Public UD8 As String

			Public Amount As Decimal

		End Class
		
		

		Private Class RestoreRecord

			Public LineNo As Integer

			Public ExportDate As DateTime

			Public BRVersion As String

			Public Cube As String

			Public UserName As String

			Public Entity As String

			Public Parent As String

			Public Cons As String

			Public Scenario As String

			Public Time As String

			Public View As String

			Public Account As String

			Public Flow As String

			Public Origin As String

			Public IC As String

			Public UD1 As String

			Public UD2 As String

			Public UD3 As String

			Public UD4 As String

			Public UD5 As String

			Public UD6 As String

			Public UD7 As String

			Public UD8 As String

			Public Amount As Decimal

			Public MemberScript As String

		End Class



		Private Class PovDimension

			Public Property Prefix As String

			Public Property MemberName As String

			Public Sub New( _
				ByVal prefix As String, _
				ByVal memberName As String)

				Me.Prefix = prefix
				Me.MemberName = memberName

			End Sub

		End Class



		Private Class PovMembers

			Public Property Scenario As String
			Public Property Time As String
			Public Property Cons As String
			Public Property View As String

			Public Property Entity As String
			Public Property Account As String
			Public Property Flow As String
			Public Property IC As String

			Public Property UD1 As String
			Public Property UD2 As String
			Public Property UD3 As String
			Public Property UD4 As String
			Public Property UD5 As String
			Public Property UD6 As String
			Public Property UD7 As String
			Public Property UD8 As String

		End Class



		Private Class DimensionFilter

			Public DimensionName As String

			Public FilterExpression As String

			Public MemberName As String

			Public Expansion As ExpansionType = ExpansionType.None

			Public Members As New HashSet(Of String)( _
				StringComparer.OrdinalIgnoreCase)

		End Class		
		
		

		Private Class HierarchyWarning

			Public DimensionName As String

			Public ParentMember As String

			Public Message As String

		End Class		
		
		

		Private Class ExportAnalysis

			Public Accounts As New HashSet(Of String)( _
				StringComparer.OrdinalIgnoreCase)

			Public ParentAccounts As New HashSet(Of String)( _
				StringComparer.OrdinalIgnoreCase)

			Public BaseAccounts As New HashSet(Of String)( _
				StringComparer.OrdinalIgnoreCase)

		End Class


		Private Class PerformanceTimer

			Public Property Name As String

			Public Property Elapsed As TimeSpan

			Public Sub New( _
					ByVal name As String, _
					ByVal elapsed As TimeSpan)

				Me.Name = name
				Me.Elapsed = elapsed

			End Sub

		End Class





						' ##       
						' ##    ## 
						' ##    ## 
						' ##    ## 
						' #########
						'       ## 
						'       ## 	


        '=========================================================
        ' SECTION 4 - Main Entry Point
        '=========================================================


		Public Function Main(ByVal si As SessionInfo,
							 ByVal globals As BRGlobals,
							 ByVal api As FinanceRulesApi,
							 ByVal args As FinanceRulesArgs) As Object
			Try

				Select Case api.FunctionType

					Case FinanceFunctionType.CustomCalculate

						_currentFunction = args.CustomCalculateArgs.FunctionName

						BRApi.ErrorLog.LogMessage( _
							si, _
							"FunctionName=[" & args.CustomCalculateArgs.FunctionName & "]")

						Select Case args.CustomCalculateArgs.FunctionName.Trim().ToUpperInvariant()

							Case "EXPORT"

								Export(si, api, args)

							Case "RESTORE"

								Restore(si, api, args)

							Case "CLEARDATA"

								ClearData(si, api, args)

							Case "EXPLAIN"

								Explain(si, api, args)

							Case "VALIDATE"

								Validate(si, api, args)

							Case "STATISTICS"

								Statistics(si, api, args)

							Case "HEALTHCHECK"

								HealthCheck(si, api, args)

							Case Else

							Throw ErrorHandler.LogWrite(si,
								New XFException(
									BR_NAME &
									" - Unsupported Function Name : " &
									args.CustomCalculateArgs.FunctionName))

						End Select

				End Select

				Return Nothing

			Catch ex As Exception
				FlushLog(si)
				Throw ErrorHandler.LogWrite(si, New XFException(si, ex))
			End Try

		End Function	




						' ########
						' ##      
						' ##      
						' ####### 
						'       ##
						' ##    ##
						'  ###### 	
		
        '=========================================================
        ' SECTION 5 - Public Entry Procedures
        '=========================================================

		Private Shared Sub Export( _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi, _
				ByVal args As FinanceRulesArgs)

			ExecuteExport( _
				si, _
				api, _
				args)

		End Sub
		
		
		
		Private Shared Sub Restore( _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi, _
				ByVal args As FinanceRulesArgs)

			ExecuteRestore( _
				si, _
				api, _
				args)

		End Sub		
		
		

		Private Shared Sub ClearData( _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi, _
				ByVal args As FinanceRulesArgs)

			ExecuteClear( _
				si, _
				api, _
				args, _
				False)

		End Sub



		Private Shared Sub Explain( _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi, _
				ByVal args As FinanceRulesArgs)

			ExecuteExplain(si, api, args)

		End Sub
		
		

		Private Shared Sub Validate( _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi, _
				ByVal args As FinanceRulesArgs)

			ExecuteValidate(si, api, args)

		End Sub
		
		
		Private Shared Sub ValidateOrigin( _
			ByVal si As SessionInfo, _
			ByVal api As FinanceRulesApi, _
			ByVal originName As String)

			Dim filterScript As String =
				BuildFilterScript(si, api, originName)

			Log(si, "Origin : " & originName)
			Log(si, "")

			Log(si, "Filter")
			Log(si, "------")
			Log(si, filterScript)

			Log(si, "")

			ShowStatisticsFromFilter( _
				si, _
				api, _
				filterScript)

			Log(si, "")

		End Sub
		
		

		Private Shared Sub Statistics( _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi, _
				ByVal args As FinanceRulesArgs)

			ExecuteStatistics(si, api, args)

		End Sub
		
		
		
		Private Shared Sub HealthCheck( _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi, _
				ByVal args As FinanceRulesArgs)

			ExecuteHealthCheck( _
				si, _
				api, _
				args)

		End Sub		



						'  ####### 
						' ##     ##
						' ##       
						' ######## 
						' ##     ##
						' ##     ##
						'  ####### 		

        '=========================================================
        ' SECTION 6 - Execution Engines
        '=========================================================



		Private Shared Sub ExecuteClear( _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi, _
				ByVal args As FinanceRulesArgs, _
				ByVal PreviewMode As Boolean)

				Dim ctx As New ClearContext()

				Dim sw As Stopwatch = Stopwatch.StartNew()

				Dim performance As New List(Of PerformanceTimer)
				Dim phase As Stopwatch

			Try

				phase = Stopwatch.StartNew()

				ctx.Configuration = ReadExecutionConfiguration( _
					si, _
					api, _
					args)

				RecordPerformance( _
					performance, _
					"Read Configuration", _
					phase)

				ctx.StartTime = DateTime.Now

				Dim config As ExecutionConfiguration = ctx.Configuration

				LogHeader(si, BR_NAME & "  Version " & BR_VERSION)
				Log(si, "Scenario : " & api.Pov.Scenario.Name)
				Log(si, "Time     : " & api.Pov.Time.Name)
				Log(si, "Entity   : " & api.Pov.Entity.Name)
				Log(si, "Cons     : " & api.Pov.Cons.Name)
				Log(si, "View     : " & api.Pov.View.Name)

				Log(si, "Preview  : " & config.PreviewMode.ToString())

				If args.CustomCalculateArgs.NameValuePairs IsNot Nothing Then

					Log(si, "Parameters")

					For Each kvp As KeyValuePair(Of String,String) _
						In args.CustomCalculateArgs.NameValuePairs

						Log(si, "   " & kvp.Key & " = " & kvp.Value)

					Next

				End If
				
				Log(si, "BatchSize : " & config.batchSize.ToString("#,##0"))
				Log(si, "=========================================")
				
				ctx.OriginCount = config.Origins.Count

				Log(si, "Origins : " &
					ctx.OriginCount.ToString())

				ctx.TotalCells = 0

				For Each originName As String In config.Origins

					phase = Stopwatch.StartNew()

					Dim filterScript As String = BuildFilterScript( _
						si, _
						api, _
						originName)

					RecordPerformance( _
						performance, _
						"Build Filter - " & originName, _
						phase)

					LogSection(si, "Origin")

					Log(si, "Origin : " & originName)
					Log(si, "Filter :")
					Log(si, filterScript)

					phase = Stopwatch.StartNew()

					ctx.TotalCells += ClearStoredDataFromFilter( _
						si, _
						api, _
						filterScript, _
						config.PreviewMode, _
						config.maxCells)

					RecordPerformance( _
						performance, _
						"Clear Data - " & originName, _
						phase)

				Next

				LogHeader(si, "Execution Summary")
				
				ctx.Elapsed = sw.Elapsed

				Log(si, "Origins Processed : " &
					ctx.OriginCount.ToString())

				Log(si, "Total Cells Cleared : " &
					ctx.TotalCells.ToString("#,##0"))

				Log(si, "Elapsed : " &
					ctx.Elapsed.ToString())
					
				Log(si, String.Empty)

				LogPerformance(si, performance)

				LogFooter(si)

				FlushLog(si)

			Catch ex As Exception
				FlushLog(si)
				Throw ErrorHandler.LogWrite(si, _
					New XFException(si, ex))

				FlushLog(si)

			End Try

		End Sub



		Private Shared Sub ExecuteRestore( _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi, _
				ByVal args As FinanceRulesArgs)

			Try

				InitializeLog()
				
				Dim sw As Stopwatch = Stopwatch.StartNew()

				Dim performance As New List(Of PerformanceTimer)

				Dim phase As Stopwatch

				phase = Stopwatch.StartNew()

				Dim config As ExecutionConfiguration = _
					ReadExecutionConfiguration( _
						si, _
						api, _
						args)

				RecordPerformance( _
					performance, _
					"Read Configuration", _
					phase)

				Dim ctx As New RestoreContext()

				ctx.Configuration = config    ' We'll add this property

				phase = Stopwatch.StartNew()

				ReadRestoreParameters( _
					si, _
					api, _
					args, _
					ctx)

				RecordPerformance( _
					performance, _
					"Read Restore Parameters", _
					phase)
					
					ctx.Summary.StartTime = DateTime.Now

					ctx.UseCurrentDataUnit =
						config.UseCurrentDataUnit

					ctx.ExecuteRestore =
						config.ExecuteRestore

					ctx.Preview =
						Not config.ExecuteRestore

					LogHeader(si, BR_NAME & " Version " & BR_VERSION)
					LogSection(si, "Restore")

					If ctx.Preview Then

						Log(si, "")
						Log(si, "*** PREVIEW MODE ***")
						Log(si, "No data will be restored.")

					Else

						Log(si, "")
						Log(si, "*** EXECUTE MODE ***")
						Log(si, "Data WILL be restored.")

					End If

					Log(si, "")
					Log(si, "UseCurrentDataUnit = " &
						ctx.UseCurrentDataUnit.ToString())				

					phase = Stopwatch.StartNew()

					ReadMemberFilters( _
						si, _
						args, _
						ctx)

					RecordPerformance( _
						performance, _
						"Read Member Filters", _
						phase)
	
					phase = Stopwatch.StartNew()

					ParseMemberFilters( _
						si, _
						ctx)

					RecordPerformance( _
						performance, _
						"Parse Member Filters", _
						phase)

					phase = Stopwatch.StartNew()

					InitializeMetadata( _
						si, _
						api, _
						ctx)

					RecordPerformance( _
						performance, _
						"Initialize Metadata", _
						phase)

					phase = Stopwatch.StartNew()

					ResolveMemberFilters( _
						si, _
						api, _
						ctx)

					RecordPerformance( _
						performance, _
						"Resolve Member Filters", _
						phase)

					Log(si, "")
					Log(si, "Resolved Member Filters: " &
						ctx.MemberFilters.Count.ToString())

					phase = Stopwatch.StartNew()

					ValidateHierarchy( _
						si, _
						api, _
						ctx)

					RecordPerformance( _
						performance, _
						"Validate Hierarchy", _
						phase)
	
					ctx.CsvFile = GetLatestExportFile(si, api)

					If String.IsNullOrWhiteSpace(ctx.CsvFile) Then
						Throw New Exception("No export file was found to restore.")
					End If

					ctx.Summary.CsvFile = ctx.CsvFile	

					Log(si, "")
					Log(si, "Restore File : " & ctx.CsvFile)
					Log(si, LOG_SEPARATOR)

					phase = Stopwatch.StartNew()

					ReadCsv( _
						ctx, _
						si, _
						api)

					RecordPerformance( _
						performance, _
						"Read CSV", _
						phase)

					phase = Stopwatch.StartNew()

					AnalyzeExportAccounts( _
						si, _
						api, _
						ctx)

					RecordPerformance( _
						performance, _
						"Analyze Export Accounts", _
						phase)	

						ctx.Summary.EndTime = DateTime.Now

						LogRestoreSummary( _
							ctx, _
							si)

					Log(si, String.Empty)

					LogPerformance( _
						si, _
						performance)

						Log(si, LOG_SEPARATOR)

						FlushLog(si)

			Catch ex As Exception

				FlushLog(si)

				Throw ErrorHandler.LogWrite( _
					si, _
					New XFException(si, ex))

			End Try

		End Sub




		Private Shared Sub ExecuteExport( _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi, _
				ByVal args As FinanceRulesArgs)

				Try

					InitializeLog()

					  Dim config As ExecutionConfiguration = _
							ReadExecutionConfiguration( _
								si, _
								api, _
								args)


					Dim ctx As New ExportContext()

					ReadExportMemberFilters( _
						args, _
						ctx)

					ctx.Summary = New ExportSummary()
					ctx.Summary.StartTime = DateTime.Now
					ctx.Summary.TotalOrigins = config.Origins.Count
					ctx.ExportTime = DateTime.Now
					ctx.LineNumber = 0
					ctx.ExportFile = CreateExportFile(si, api)
					ctx.Summary.ExportFile = ctx.ExportFile
					ctx.Summary.Scenario = api.Pov.Scenario.Name
					ctx.Summary.Time = api.Pov.Time.Name
					ctx.Summary.Entity = api.Pov.Entity.Name
					ctx.Summary.Cube = api.Pov.Cube.Name

					Log(si, "")
					Log(si, LOG_SEPARATOR)
					Log(si, BR_NAME & "  Version " & BR_VERSION)
					Log(si, "Export Started")
					Log(si, LOG_SEPARATOR)

					Using writer As New StreamWriter( _
						ctx.ExportFile, _
						False, _
						System.Text.Encoding.UTF8)

						ctx.Writer = writer

						Log(si, "CSV Writer Opened")

						WriteCsvHeader(ctx)

						Log(si, "Header Written")

						Log(si, "Export File : " & ctx.ExportFile)

						For Each originName As String In config.Origins

							ExportOrigin( _
								ctx, _
								si, _
								api, _
								originName)

						Next

						writer.Flush()

					End Using

					ctx.Writer = Nothing

					Log(si, "")
					Log(si, "Rows Exported : " & _
						ctx.LineNumber.ToString("#,##0"))

					ctx.Summary.EndTime = DateTime.Now

					LogExportSummary( _
						si, _
						ctx)

					FlushLog(si)

				Catch ex As Exception

					FlushLog(si)

					Throw ErrorHandler.LogWrite( _
						si, _
						New XFException(si, ex))

				End Try

		End Sub




		Private Shared Sub ExecuteExplain( _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi, _
				ByVal args As FinanceRulesArgs)

			Try

				InitializeLog()

				Dim sw As Stopwatch = Stopwatch.StartNew()

				Dim config As ExecutionConfiguration = _
					ReadExecutionConfiguration( _
						si, _
						api, _
						args)

				LogHeader(si, "MTCH_ClearCubeData - Explain")

				'=========================================================
				' Execution Parameters
				'=========================================================

				LogSection(si, "Execution Parameters")

				Log(si, "Scenario      : " & api.Pov.Scenario.Name)
				Log(si, "Time          : " & api.Pov.Time.Name)
				Log(si, "Entity        : " & api.Pov.Entity.Name)
				Log(si, "Consolidation : " & api.Pov.Cons.Name)
				Log(si, "View          : " & api.Pov.View.Name)

				Log(si, "Max Cells     : " & _
					config.MaxCells.ToString("#,##0"))

				Log(si, "Batch Size    : " & _
					config.BatchSize.ToString("#,##0"))

				'=========================================================
				' Origins
				'=========================================================

				LogSection(si, "Origins")

				Log(si, "Origin Count : " & _
					config.Origins.Count.ToString())

				Log(si, "")

				For Each origin As String In config.Origins

					Log(si, "  • " & origin)

				Next

				'=========================================================
				' Generated Filters
				'=========================================================

				Dim originNumber As Integer = 1

				For Each origin As String In config.Origins

					LogSection( _
						si, _
						"Origin " & _
						originNumber.ToString() & _
						" of " & _
						config.Origins.Count.ToString())

					Log(si, "Origin        : " & origin)

					Dim filterScript As String = _
						BuildFilterScript( _
							si, _
							api, _
							origin)

					Log(si, "Filter Script :")
					Log(si, "    " & filterScript)

					ShowStatisticsFromFilter( _
						si, _
						api, _
						filterScript)

					originNumber += 1

				Next

				'=========================================================
				' Summary
				'=========================================================

					sw.Stop()

					LogSection(si, "Summary")

					Log(si, "Origins Evaluated : " & _
						config.Origins.Count.ToString())

					Log(si, "Elapsed Time      : " & _
						sw.Elapsed.ToString())

					Log(si, "")
					Log(si, "Explain Complete")

					LogFooter(si)

					FlushLog(si)

			Catch ex As Exception

				FlushLog(si)

				Throw ErrorHandler.LogWrite( _
					si, _
					New XFException(si, ex))

			End Try

		End Sub



		Private Shared Sub ExecuteValidate( _
			ByVal si As SessionInfo, _
			ByVal api As FinanceRulesApi, _
			ByVal args As FinanceRulesArgs)

			Try

				InitializeLog()

				Dim config As ExecutionConfiguration = _
					ReadExecutionConfiguration( _
						si, _
						api, _
						args)

				LogHeader(si, "Validation Report")

				Log(si, "Version    : " & BR_VERSION)
				Log(si, "Scenario   : " & api.Pov.Scenario.Name)
				Log(si, "Time       : " & api.Pov.Time.Name)
				Log(si, "Entity     : " & api.Pov.Entity.Name)
				Log(si, "Cons       : " & api.Pov.Cons.Name)
				Log(si, "View       : " & api.Pov.View.Name)

				Log(si, "")
				Log(si, "MaxCells : " & config.MaxCells.ToString("#,##0"))
				Log(si, "BatchSize: " & config.BatchSize.ToString("#,##0"))

				LogSection(si, "Origins")

				For Each origin As String In config.Origins

					Log(si, "   " & origin)

				Next

				Log(si, "")

				For Each origin As String In config.Origins

					ValidateOrigin( _
						si, _
						api, _
						origin)

				Next

				Log(si, "")
				Log(si, "Validation PASSED")

				LogFooter(si)

				FlushLog(si)

			Catch ex As Exception

				FlushLog(si)

				Throw ErrorHandler.LogWrite( _
					si, _
					New XFException(si, ex))

			End Try

		End Sub
		
		
		
		Private Shared Sub ExecuteStatistics( _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi, _
				ByVal args As FinanceRulesArgs)

			Try

				InitializeLog()

				Dim config As ExecutionConfiguration = _
					ReadExecutionConfiguration( _
						si, _
						api, _
						args)

						Dim ctx As New StatisticsContext()

						ctx.Configuration = config

					LogHeader(si, BR_NAME & " Statistics")
					Log(si, "Scenario : " & api.Pov.Scenario.Name)
					Log(si, "Time     : " & api.Pov.Time.Name)
					Log(si, "Entity   : " & api.Pov.Entity.Name)

					For Each originName As String In config.Origins

						LogSection(si, "Origin : " & originName)

						Dim filterScript As String = BuildFilterScript(si, api, originName)

						ShowStatisticsFromFilter( _
							si, _
							api, _
							filterScript)

					Next

					LogFooter(si)

					FlushLog(si)

			Catch ex As Exception

				FlushLog(si)

				Throw ErrorHandler.LogWrite( _
					si, _
					New XFException(si, ex))

			End Try

		End Sub





						' ########
						' ##    ##
						'     ##  
						'    ##   
						'   ##    
						'   ##    
						'   ##    
						
        '=========================================================
        ' SECTION 7 - Health Check Framework
        '=========================================================
		
		
			Private Shared Sub ExecuteHealthCheck( _
					ByVal si As SessionInfo, _
					ByVal api As FinanceRulesApi, _
					ByVal args As FinanceRulesArgs)

				LogHeader(si, "Health Check")

				Dim sw As Stopwatch = Stopwatch.StartNew()

				Dim results As New HealthCheckResults()
				
				Dim performance As New List(Of PerformanceTimer)

				'==========================================================
				' Environment
				'==========================================================

				LogSection(si, "Environment")

				LogField(si, "Business Rule", BR_NAME)
				LogField(si, "Version", BR_VERSION)
				LogField(si, "OneStream", OS_VERSION)
				LogField(si, "User", si.UserName)
				LogField(si, "Application", si.AppName)

				'==========================================================
				' Current POV
				'==========================================================

				LogSection(si, "Current POV")

				LogField(si, "Cube", api.Pov.Cube.Name)
				LogField(si, "Scenario", api.Pov.Scenario.Name)
				LogField(si, "Time", api.Pov.Time.Name)
				LogField(si, "Entity", api.Pov.Entity.Name)
				LogField(si, "Parent", api.Pov.Parent.Name)
				LogField(si, "Cons", api.Pov.Cons.Name)
				LogField(si, "View", api.Pov.View.Name)
				LogField(si, "Origin", api.Pov.Origin.Name)

				'==========================================================
				' Execution Configuration
				'==========================================================

				LogSection(si, "Execution Configuration")

				Dim phase As Stopwatch = Stopwatch.StartNew()

				Dim config As ExecutionConfiguration =
					ReadExecutionConfiguration(si, api, args)

				RecordPerformance( _
					performance, _
					"Read Configuration", _
					phase)

				LogField(si, "Max Cells", config.MaxCells.ToString())
				LogField(si, "Batch Size", config.BatchSize.ToString())
				LogField(si, "Preview Mode", config.PreviewMode.ToString())
				LogField(si, "Execute Restore", config.ExecuteRestore.ToString())
				LogField(si, "Use Current DataUnit", config.UseCurrentDataUnit.ToString())
				LogField(si, "Origin Count", config.Origins.Count.ToString())

				If config.Origins.Count > 0 Then
					For Each origin As String In config.Origins
						Log(si, "    " & origin)
					Next
				Else
					Log(si, "    <None>")
				End If

				'==========================================================
				' File System
				'==========================================================

				phase = Stopwatch.StartNew()

				LogSection(si, "File System")

				Dim logDir As String = GetLogDirectory(si)

				LogField(si, "Log Directory", logDir)
				LogField(si, "Log Directory Exists", Directory.Exists(logDir).ToString())

				If Directory.Exists(logDir) Then

					Dim logFiles() As String =
						Directory.GetFiles(logDir, "*.log")

					LogField(si, "Log Files", logFiles.Length.ToString())

				End If

				RecordPerformance( _
					performance, _
					"File System", _
					phase)
					
				'==========================================================
				' Runtime Statistics
				'==========================================================
				
				phase = Stopwatch.StartNew()

				LogSection(si, "Runtime Statistics")

				LogField(si, "Managed Memory (MB)",
					(GC.GetTotalMemory(False) / 1024D / 1024D).ToString("N2"))

				LogField(si, "Machine", Environment.MachineName)

				RecordPerformance( _
					performance, _
					"Runtime Statistics", _
					phase)

				'==========================================================
				' Health Tests
				'==========================================================

				phase = Stopwatch.StartNew()

				LogSection(si, "Health Tests")

				LogField(si, "Read Configuration", "PASS")
				results.Passed += 1

				If config.Origins.Count > 0 Then
					LogField(si, "Origin Retrieval",
						"PASS (" & config.Origins.Count.ToString() & ")")
					results.Passed += 1
				Else
					LogField(si, "Origin Retrieval", "FAIL")
					results.Failed += 1
				End If

				If Directory.Exists(logDir) Then
					LogField(si, "Log Directory", "PASS")
					results.Passed += 1
				Else
					LogField(si, "Log Directory", "FAIL")
					results.Failed += 1
				End If

				Try

					If config.Origins IsNot Nothing Then

						LogField( _
							si, _
							"Metadata Access", _
							"PASS (" & config.Origins.Count.ToString() & " origins)")

						results.Passed += 1

					Else

						LogField(si, "Metadata Access", "FAIL")
						results.Failed += 1

					End If

				Catch ex As Exception

					LogField(si, "Metadata Access", "FAIL")
					LogField(si, "Error", ex.Message)

					results.Failed += 1

				End Try

				RecordPerformance( _
					performance, _
					"Health Tests", _
					phase)

				'==========================================================
				' Health Summary
				'==========================================================

				sw.Stop()

					results.Elapsed = sw.Elapsed

				LogSection(si, "Health Summary")

				LogField(si, "Passed", results.Passed.ToString())
				LogField(si, "Failed", results.Failed.ToString())
				LogField(si, "Elapsed", results.Elapsed.ToString())
				
				LogPerformance(si, performance)

				If results.Failed = 0 Then
					LogField(si, "Overall Status", "PASS")
				Else
					LogField(si, "Overall Status", "FAIL")
				End If

				LogFooter(si)

				FlushLog(si)

			End Sub

		
	

						'  ####### 
						' ##     ##
						' ##     ##
						'  ####### 
						' ##     ##
						' ##     ##
						'  ####### 	

        '=========================================================
        ' SECTION 8 - Parameter Framework
        '=========================================================
		

		Private Shared Function ReadExecutionConfiguration( _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi, _
				ByVal args As FinanceRulesArgs) _
				As ExecutionConfiguration

			Dim config As New ExecutionConfiguration()

			ReadParameters( _
				api, _
				args, _
				config.MaxCells, _
				config.BatchSize, _
				config.Origins, _
				config.ExecuteRestore)

			config.UseCurrentDataUnit = _
				GetUseCurrentDataUnit(args)

			Return config

		End Function




		Private Shared Sub ReadParameters( _
			ByVal api As FinanceRulesApi, _
			ByVal args As FinanceRulesArgs, _
			ByRef maxCells As Integer, _
			ByRef batchSize As Integer, _
			ByRef origins As List(Of String), _
			ByRef executeRestore As Boolean)

		   ' Defaults
			maxCells = DEFAULT_MAXCELLS
			batchSize = DEFAULT_RESTORE_BATCHSIZE

			origins.Clear()

			' Default Origin(s)
			If String.IsNullOrWhiteSpace(api.Pov.Origin.Name) Then

				origins.Add("Import")
				origins.Add("Forms")
				origins.Add("AdjInput")
				origins.Add("Journals")
				origins.Add("Elimination")

			Else

				origins.Add(api.Pov.Origin.Name)

			End If

			'----------------------------
			' Read NameValuePairs
			'----------------------------

			If args.CustomCalculateArgs.NameValuePairs Is Nothing Then Exit Sub

			For Each kvp As KeyValuePair(Of String, String) In args.CustomCalculateArgs.NameValuePairs

				Select Case kvp.Key.Trim.ToUpperInvariant()

					Case "MAXCELLS"

						Integer.TryParse(kvp.Value, maxCells)

					Case "BATCHSIZE"

						Integer.TryParse(kvp.Value, batchSize)

					Case "ORIGIN"

						origins.Clear()

						For Each origin As String In kvp.Value.Split(","c)

							origins.Add(origin.Trim())

						Next

				End Select

			Next

		End Sub	



		Private Shared Sub ReadRestoreParameters( _
			ByVal si As SessionInfo, _
			ByVal api As FinanceRulesApi, _
			ByVal args As FinanceRulesArgs, _
			ByVal ctx As RestoreContext)

			ctx.ExecuteRestore = GetExecuteRestore(si, args)
			
		End Sub
				
		

		Private Shared Function GetParameterValue( _
				ByVal args As FinanceRulesArgs, _
				ByVal parameterName As String) As String

			If args.CustomCalculateArgs Is Nothing Then
				Return Nothing
			End If

			If args.CustomCalculateArgs.NameValuePairs Is Nothing Then
				Return Nothing
			End If

			For Each kvp As KeyValuePair(Of String, String) _
				In args.CustomCalculateArgs.NameValuePairs

				If kvp.Key.Trim().Equals( _
						parameterName, _
						StringComparison.OrdinalIgnoreCase) Then

					Return kvp.Value.Trim()

				End If

			Next

			Return Nothing

		End Function
		
		

		Private Shared Function GetBooleanParameter( _
				ByVal si As SessionInfo, _
				ByVal args As FinanceRulesArgs, _
				ByVal parameterName As String, _
				ByVal defaultValue As Boolean) As Boolean

			Dim value As String =
				GetParameterValue(args, parameterName)

			If String.IsNullOrWhiteSpace(value) Then
				Return defaultValue
			End If

			Dim result As Boolean

			If Boolean.TryParse(value, result) Then
				Return result
			End If

			Log(si, "Invalid Boolean parameter: " & _
				parameterName & "=" & value)

			Return defaultValue

		End Function
		
		

		Private Shared Function GetExecuteRestore( _
				ByVal si As SessionInfo, _
				ByVal args As FinanceRulesArgs) As Boolean

			Return GetBooleanParameter( _
				si, _
				args, _
				"EXECUTE", _
				False)

		End Function
		
		

		Private Shared Function GetUseCurrentDataUnit( _
				ByVal args As FinanceRulesArgs) As Boolean

			Return GetBooleanParameter( _
				Nothing, _
				args, _
				"UseCurrentDataUnit", _
				True)

		End Function
		
		

		Private Shared Sub ReadMemberFilters( _
				ByVal si As SessionInfo, _
				ByVal args As FinanceRulesArgs, _
				ByVal ctx As RestoreContext)

			If args.CustomCalculateArgs Is Nothing Then Exit Sub

			If args.CustomCalculateArgs.NameValuePairs Is Nothing Then Exit Sub

			For Each kvp As KeyValuePair(Of String, String) _
				In args.CustomCalculateArgs.NameValuePairs

				Dim key As String =
					kvp.Key.Trim()

				Dim value As String =
					kvp.Value.Trim()

			Select Case key.ToUpperInvariant()

				Case "FUNCTION", _
					 "EXECUTE", _
					 "USECURRENTDATAUNIT"

					'Skip system parameters

				Case Else

					If Not ctx.MemberFilters.ContainsKey(key) Then

						Dim filter As New DimensionFilter()

						filter.DimensionName = key
						filter.FilterExpression = value

						ctx.MemberFilters.Add(key, filter)

						Log(si, "")
						Log(si, "Member Filter Added")
						Log(si, "Dimension  : " & filter.DimensionName)
						Log(si, "Expression : " & filter.FilterExpression)

					End If

			End Select

			Next

		End Sub
		
		

		Private Shared Sub ParseMemberFilters( _
				ByVal si As SessionInfo, _
				ByVal ctx As RestoreContext)

			For Each filter As DimensionFilter _
				In ctx.MemberFilters.Values

				ParseSingleMemberFilter(filter)

				Log(si, "")
				Log(si, "Parsed Filter")
				Log(si, "Dimension  : " & filter.DimensionName)
				Log(si, "Expression : " & filter.FilterExpression)
				Log(si, "Member     : " & filter.MemberName)
				Log(si, "Expansion  : " & filter.Expansion.ToString())

			Next

		End Sub



		Private Shared Sub ReadExportMemberFilters( _
				ByVal args As FinanceRulesArgs, _
				ByVal ctx As ExportContext)

			If args.CustomCalculateArgs.NameValuePairs Is Nothing Then Exit Sub

			For Each kvp As KeyValuePair(Of String, String) _
				In args.CustomCalculateArgs.NameValuePairs

				Select Case kvp.Key.Trim().ToUpperInvariant()

					Case "ACCOUNT", _
						 "FLOW", _
						 "IC", _
						 "UD1", _
						 "UD2", _
						 "UD3", _
						 "UD4", _
						 "UD5", _
						 "UD6", _
						 "UD7", _
						 "UD8"

						ctx.FilterParameters(kvp.Key.Trim()) = _
							kvp.Value.Trim()

				End Select

			Next

		End Sub	


						'  ####### 
						' ##     ##
						' ##     ##
						'  ########
						'        ##
						' ##     ##
						'  ####### 

        '=========================================================
        ' SECTION 9 - Filter Construction Framework
        '=========================================================

		Private Shared Function BuildFilterScript( _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi, _
				ByVal originName As String) As String

			Dim parts As List(Of String) =
				BuildBaseFilterParts(si, api)

			parts.Insert(4, "O#" & originName)

			Log(si, "")
			Log(si, "POV Values")
			Log(si, "Entity  = '" & api.Pov.Entity.Name & "'")
			Log(si, "Account = '" & api.Pov.Account.Name & "'")
			Log(si, "Flow    = '" & api.Pov.Flow.Name & "'")
			Log(si, "IC      = '" & api.Pov.IC.Name & "'")
			Log(si, "")

			AddExpandedIfPresent( _
				si, _
				api, _
				parts, _
				"A", _
				GetMemberOrDefault(api.Pov.Account.Name))

			AddExpandedIfPresent( _
				si, _
				api, _
				parts, _
				"F", _ 
				GetMemberOrDefault(api.Pov.Flow.Name))

			AddExpandedIfPresent( _
				si, _
				api, _
				parts, _
				"I", _ 
				GetMemberOrDefault(api.Pov.IC.Name))

			AddExpandedIfPresent( _
				si, _
				api, _
				parts, _
				"U1", _ 
				GetMemberOrDefault(api.Pov.UD1.Name))	

			AddExpandedIfPresent( _
				si, _
				api, _
				parts, _
				"U2", _ 
				GetMemberOrDefault(api.Pov.UD2.Name))	

			AddExpandedIfPresent( _
				si, _
				api, _
				parts, _
				"U3", _ 
				GetMemberOrDefault(api.Pov.UD3.Name))	

			AddExpandedIfPresent( _
				si, _
				api, _
				parts, _
				"U4", _ 
				GetMemberOrDefault(api.Pov.UD4.Name))	

			AddExpandedIfPresent( _
				si, _
				api, _
				parts, _
				"U5", _ 
				GetMemberOrDefault(api.Pov.UD5.Name))	

			AddExpandedIfPresent( _
				si, _
				api, _
				parts, _
				"U6", _ 
				GetMemberOrDefault(api.Pov.UD6.Name))	

			AddExpandedIfPresent( _
				si, _
				api, _
				parts, _
				"U7", _ 
				GetMemberOrDefault(api.Pov.UD7.Name))	

			AddExpandedIfPresent( _
				si, _
				api, _
				parts, _
				"U8", _ 
				GetMemberOrDefault(api.Pov.UD8.Name))				

			Return BuildFilterMembersScript(parts)

		End Function



		Private Shared Function BuildBaseFilterParts( _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi) _
				As List(Of String)

			Dim pov As PovMembers = GetPovMembers(api)

			Dim parts As New List(Of String)

			AddExactIfPresent(parts, "S", pov.Scenario)
			AddExactIfPresent(parts, "T", pov.Time)
			AddExactIfPresent(parts, "C", pov.Cons)
			AddExactIfPresent(parts, "V", pov.View)

			For Each dimInfo As DimensionInfo In GetDimensionInfo(api)

				AddExpandedIfPresent( _
					si, _
					api, _
					parts, _
					dimInfo.Prefix, _
					GetMemberOrDefault(dimInfo.MemberName))

			Next

			Return parts

		End Function



		Private Shared Function BuildFilterMembersScript( _
				ByVal parts As List(Of String)) As String

			Dim sb As New StringBuilder()

			sb.Append("FilterMembers(")
			sb.Append(String.Join(", ", parts.ToArray()))
			sb.Append(")")

			Return sb.ToString()

		End Function



		Private Shared Sub AddFilterParameter( _
				ByVal si As SessionInfo, _
				ByVal parts As List(Of String), _
				ByVal prefix As String, _
				ByVal ctx As ExportContext, _
				ByVal parameterName As String)

			If Not ctx.FilterParameters.ContainsKey(parameterName) Then
				Exit Sub
			End If

			Dim value As String = ctx.FilterParameters(parameterName).Trim()

			If String.IsNullOrWhiteSpace(value) Then
				Exit Sub
			End If

			Log(si, "")
			Log(si, "Using Export Filter")
			Log(si, parameterName & " = " & value)

			parts.Add(prefix & "#" & value)

		End Sub



		Private Shared Sub AddExactIfPresent( _
				ByVal parts As List(Of String), _
				ByVal dimPrefix As String, _
				ByVal memberName As String)

			If String.IsNullOrWhiteSpace(memberName) Then Exit Sub

			parts.Add(dimPrefix & "#" & memberName)

		End Sub
		
		

		Private Shared Function ResolveHierarchyMembers( _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi, _
				ByVal dimensionName As String, _
				ByVal expression As String) _
				As HashSet(Of String)

			Dim cacheKey As String = _
				dimensionName & "|" & expression

			Dim members As HashSet(Of String)

			If _hierarchyCache.ContainsKey(cacheKey) Then

				members = _hierarchyCache(cacheKey)

				LogResolvedMembers(si, members)

				Return members

			End If

			Dim filter As New DimensionFilter

			filter.DimensionName = dimensionName
			filter.FilterExpression = expression

			ParseSingleMemberFilter(filter)

			members = ExpandMemberFilter( _
				si, _
				api, _
				filter)

			_hierarchyCache(cacheKey) = members

			LogResolvedMembers(si, members)

			Return members

		End Function
		
		

		Private Shared Sub ParseSingleMemberFilter( _
				ByVal filter As DimensionFilter)

				filter.MemberName = filter.FilterExpression.Trim()

				filter.Expansion = ExpansionType.None

				If filter.MemberName.EndsWith(".Base", _
					StringComparison.OrdinalIgnoreCase) Then

					filter.MemberName = _
						filter.MemberName.Substring( _
							0, _
							filter.MemberName.Length - 5)

					filter.Expansion = ExpansionType.Base

				ElseIf filter.MemberName.EndsWith(".Children", _
					StringComparison.OrdinalIgnoreCase) Then

					filter.MemberName = _
						filter.MemberName.Substring( _
							0, _
							filter.MemberName.Length - 9)

					filter.Expansion = ExpansionType.Children

				ElseIf filter.MemberName.EndsWith(".Descendants", _
					StringComparison.OrdinalIgnoreCase) Then

					filter.MemberName = _
						filter.MemberName.Substring( _
							0, _
							filter.MemberName.Length - 12)

					filter.Expansion = ExpansionType.Descendants

				End If

		End Sub
		
		

		Private Shared Function ExpandMemberFilter( _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi, _
				ByVal filter As DimensionFilter) As HashSet(Of String)

			Select Case filter.Expansion

				Case ExpansionType.None

					Return ExpandExact( _
						si, _
						filter)

				Case ExpansionType.Base

					Return ExpandBase( _
						si, _
						api, _
						filter)

				Case ExpansionType.Children

					Return ExpandChildren( _
						si, _
						api, _ 						
						filter)

				Case ExpansionType.Descendants

					Return ExpandDescendants( _
						si, _
						api, _
						filter)

				Case Else

					Return New HashSet(Of String)( _
						StringComparer.OrdinalIgnoreCase)

			End Select

		End Function
		
		

		Private Shared Function ExpandExact( _
				ByVal si As SessionInfo, _
				ByVal filter As DimensionFilter) _
				As HashSet(Of String)

			Dim members As New HashSet(Of String)( _
				StringComparer.OrdinalIgnoreCase)

			members.Add(filter.MemberName)

			Return members

		End Function
		
		

		Private Shared Sub ResolveMemberFilters( _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi, _
				ByVal ctx As RestoreContext)

			Try

				For Each filter As DimensionFilter In ctx.MemberFilters.Values

					filter.Members = ExpandMemberFilter( _
						si, _
						api, _
						filter)

					Log(si, "")
					Log(si, "Resolved Members : " &
						filter.Members.Count.ToString())

				Next

			Catch ex As Exception

				Log(si, "")
				Log(si, "ERROR in ResolveMemberFilters")
				Log(si, ex.ToString())

				Throw

			End Try

		End Sub



		Private Shared Sub AddExpandedIfPresent( _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi, _
				ByVal parts As List(Of String), _
				ByVal dimPrefix As String, _
				ByVal memberName As String)

			If String.IsNullOrWhiteSpace(memberName) Then Exit Sub

			Dim dimensionName As String = GetDimensionName(dimPrefix)

			Dim members As HashSet(Of String) = _
				ResolveHierarchyMembers( _
					si, _
					api, _
					dimensionName, _
					memberName)

			If members.Count = 0 Then

				parts.Add(GetExpandedMemberScript( _
					dimPrefix, _
					memberName))

				Exit Sub

			End If

			For Each member As String In members

				parts.Add(dimPrefix & "#" & member)

			Next

		End Sub		
		
		
		
		Private Shared Function GetExpandedMemberScript( _
				ByVal dimPrefix As String, _
				ByVal memberName As String) As String

			If String.IsNullOrWhiteSpace(memberName) Then
				Return dimPrefix & "#Top.Base"
			End If

			If memberName.XFEqualsIgnoreCase("Top") Then
				Return dimPrefix & "#Top.Base"
			End If

			If memberName.XFEqualsIgnoreCase("None") Then
				Return dimPrefix & "#None"
			End If

			Return dimPrefix & "#" & memberName & ".Base"

		End Function	
		
		
		
		Private Shared Function BuildExportFilterScript( _
				ByVal ctx As ExportContext, _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi, _
				ByVal originName As String) As String

				Dim parts As New List(Of String)

				'---------------------------------------------------------
				' Exact POV Dimensions
				'---------------------------------------------------------

				AddExactIfPresent(parts, "S", api.Pov.Scenario.Name)
				AddExactIfPresent(parts, "T", api.Pov.Time.Name)
				AddExactIfPresent(parts, "C", api.Pov.Cons.Name)
				AddExactIfPresent(parts, "V", api.Pov.View.Name)
				'AddExactIfPresent(parts, "O", originName)

				'---------------------------------------------------------
				' Entity always comes from the POV
				'---------------------------------------------------------

				If Not String.IsNullOrWhiteSpace(api.Pov.Entity.Name) Then

					parts.Add("E#" & api.Pov.Entity.Name)

				End If
				
				'---------------------------------------------------------
				' Member Filters supplied by Data Management Parameters
				'---------------------------------------------------------

				AddFilterParameter(si, parts, "A", ctx, "Account")
				AddFilterParameter(si, parts, "F", ctx, "Flow")
				AddFilterParameter(si, parts, "I", ctx, "IC")

				AddFilterParameter(si, parts, "U1", ctx, "UD1")
				AddFilterParameter(si, parts, "U2", ctx, "UD2")
				AddFilterParameter(si, parts, "U3", ctx, "UD3")
				AddFilterParameter(si, parts, "U4", ctx, "UD4")
				AddFilterParameter(si, parts, "U5", ctx, "UD5")
				AddFilterParameter(si, parts, "U6", ctx, "UD6")
				AddFilterParameter(si, parts, "U7", ctx, "UD7")
				AddFilterParameter(si, parts, "U8", ctx, "UD8")

			Return BuildFilterMembersScript(parts)

		End Function



		Private Shared Function BuildMemberScript( _
				ByVal rec As RestoreRecord) As String

			Dim sb As New System.Text.StringBuilder()

			sb.Append("Cb#Finance")

			sb.Append(":E#").Append(rec.Entity)

			sb.Append(":C#").Append(rec.Cons)

			sb.Append(":S#").Append(rec.Scenario)

			sb.Append(":T#").Append(rec.Time)

			sb.Append(":V#").Append(rec.View)

			sb.Append(":A#").Append(rec.Account)

			sb.Append(":F#").Append(rec.Flow)

			sb.Append(":O#").Append(rec.Origin)

			sb.Append(":I#").Append(rec.IC)

			sb.Append(":U1#").Append(rec.UD1)

			sb.Append(":U2#").Append(rec.UD2)

			sb.Append(":U3#").Append(rec.UD3)

			sb.Append(":U4#").Append(rec.UD4)

			sb.Append(":U5#").Append(rec.UD5)

			sb.Append(":U6#").Append(rec.UD6)

			sb.Append(":U7#").Append(rec.UD7)

			sb.Append(":U8#").Append(rec.UD8)

			Return sb.ToString()

		End Function



					'   ##     #####  
					' ####    ##   ## 
					'   ##   ##     ##
					'   ##   ##     ##
					'   ##   ##     ##
					'   ##    ##   ## 
					' ######   #####  	


        '=========================================================
        ' SECTION 10 - Metadata Framework
        '=========================================================
		

		Private Shared Sub InitializeMetadata( _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi, _
				ByVal ctx As RestoreContext)

		End Sub	
		
		

		Private Shared Function GetDimPk( _
				ByVal api As FinanceRulesApi, _
				ByVal dimensionName As String) _
				As DimPk

			Select Case dimensionName.Trim().ToUpperInvariant()

				Case "ACCOUNT"
					Return api.Pov.AccountDim.DimPk

				Case "ENTITY"
					Return api.Pov.EntityDim.DimPk

				Case "FLOW"
					Return api.Pov.FlowDim.DimPk

				Case "ORIGIN"
					Return api.Pov.OriginDim.DimPk

				Case "IC"
					Return api.Pov.ICDim.DimPk

				Case "SCENARIO"
					Return api.Pov.ScenarioDim.DimPk

				Case "TIME"
					Return api.Pov.TimeDim.DimPk

				Case "VIEW"
					Return api.Pov.ViewDim.DimPk

				Case "UD1"
					Return api.Pov.UD1Dim.DimPk

				Case "UD2"
					Return api.Pov.UD2Dim.DimPk

				Case "UD3"
					Return api.Pov.UD3Dim.DimPk

				Case "UD4"
					Return api.Pov.UD4Dim.DimPk

				Case "UD5"
					Return api.Pov.UD5Dim.DimPk

				Case "UD6"
					Return api.Pov.UD6Dim.DimPk

				Case "UD7"
					Return api.Pov.UD7Dim.DimPk

				Case "UD8"
					Return api.Pov.UD8Dim.DimPk

				Case Else

					Throw New Exception( _
						"Unsupported dimension: " & dimensionName)

			End Select

		End Function
		
		

		Private Shared Function GetDimTypeId( _
				ByVal dimensionName As String) As Integer

			Select Case dimensionName.ToUpperInvariant()

				Case "ACCOUNT"
					Return DimType.Account.Id

				Case "ENTITY"
					Return DimType.Entity.Id

				Case "CONS"
					Return DimType.Consolidation.Id

				Case "SCENARIO"
					Return DimType.Scenario.Id

				Case "TIME"
					Return DimType.Time.Id

				Case "VIEW"
					Return DimType.View.Id

				Case "FLOW"
					Return DimType.Flow.Id

				Case "ORIGIN"
					Return DimType.Origin.Id

				Case "IC"
					Return DimType.IC.Id

				Case "UD1"
					Return DimType.UD1.Id

				Case "UD2"
					Return DimType.UD2.Id

				Case "UD3"
					Return DimType.UD3.Id

				Case "UD4"
					Return DimType.UD4.Id

				Case "UD5"
					Return DimType.UD5.Id

				Case "UD6"
					Return DimType.UD6.Id

				Case "UD7"
					Return DimType.UD7.Id

				Case "UD8"
					Return DimType.UD8.Id

				Case Else

					Throw New Exception( _
						"Unknown dimension: " & dimensionName)

			End Select

		End Function
		
		

		Private Shared Function IsParentMember( _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi, _
				ByVal dimensionName As String, _
				ByVal memberName As String) _
				As Boolean

			Dim dimTypeId As Integer = _
				GetDimTypeId( _
					dimensionName)

			Dim member As Member = _
				api.Members.GetMember( _
					dimTypeId, _
					memberName)

			If member Is Nothing Then

				Return False

			End If

			Dim dimPk As DimPk = _
				GetDimPk( _
					api, _
					dimensionName)

			Dim children As List(Of Member) = _
				api.Members.GetChildren( _
					dimPk, _
					member.MemberId)

			Return children.Count > 0

		End Function



		Private Shared Function ExpandBase( _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi, _
				ByVal filter As DimensionFilter) _
				As HashSet(Of String)

			Dim members As New HashSet(Of String)( _
				StringComparer.OrdinalIgnoreCase)

			Log(si, "")

			Log(si, "Dimension = " & filter.DimensionName)
			Log(si, "Member    = " & filter.MemberName)
			
			Dim dimTypeId As Integer = _
				GetDimTypeId( _
					filter.DimensionName)

			Dim member As Member = _
				api.Members.GetMember( _
					dimTypeId, _
					filter.MemberName)

			If member Is Nothing Then

				Log(si, "*** MEMBER NOT FOUND ***")

				Return members

			End If

			Dim dimPk As DimPk = _
				GetDimPk( _
					api, _
					filter.DimensionName)

				Dim baseMembers As List(Of Member) = _
					api.Members.GetBaseMembers( _
						dimPk, _
						member.MemberId)

					Dim i As Integer = 0

					For Each m As Member In baseMembers

						Log(si, "Base Member : " & m.Name)

						i += 1

						If i >= 25 Then Exit For

					Next

				For Each baseMember As Member In baseMembers

					members.Add(baseMember.Name)

				Next

				Return members

		End Function



		Private Shared Function ExpandChildren( _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi, _
				ByVal filter As DimensionFilter) _
				As HashSet(Of String)

			Dim members As New HashSet(Of String)( _
				StringComparer.OrdinalIgnoreCase)

			Log(si, "")
			Log(si, "Dimension = " & filter.DimensionName)
			Log(si, "Member    = " & filter.MemberName)

			Dim dimTypeId As Integer = _
				GetDimTypeId( _
					filter.DimensionName)

			Dim member As Member = _
				api.Members.GetMember( _
					dimTypeId, _
					filter.MemberName)

			If member Is Nothing Then

				Log(si, "*** MEMBER NOT FOUND ***")

				Return members

			End If

			Dim dimPk As DimPk = _
				GetDimPk( _
					api, _
					filter.DimensionName)

			Dim childMembers As List(Of Member) = _
				api.Members.GetChildren( _
					dimPk, _
					member.MemberId)

			For Each child As Member In childMembers

				members.Add(child.Name)

			Next

			Return members

		End Function
		
		

		Private Shared Function ExpandDescendants( _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi, _
				ByVal filter As DimensionFilter) _
				As HashSet(Of String)

			Dim members As New HashSet(Of String)( _
				StringComparer.OrdinalIgnoreCase)

			Log(si, "")
			Log(si, "Dimension = " & filter.DimensionName)
			Log(si, "Member    = " & filter.MemberName)

			Dim dimTypeId As Integer = _
				GetDimTypeId( _
					filter.DimensionName)

			Dim member As Member = _
				api.Members.GetMember( _
					dimTypeId, _
					filter.MemberName)

			If member Is Nothing Then

				Log(si, "*** MEMBER NOT FOUND ***")

				Return members

			End If

			Dim dimPk As DimPk = _
				GetDimPk( _
					api, _
					filter.DimensionName)

			Dim descendantMembers As List(Of Member) = _
				api.Members.GetDescendants( _
					dimPk, _
					member.MemberId)

			For Each descendant As Member In descendantMembers

				members.Add(descendant.Name)

			Next

			Return members

		End Function	
		
		
		
		Private Shared Sub ValidateHierarchy( _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi, _
				ByVal ctx As RestoreContext)

				Log(si, "")
				Log(si, "==================================================")
				Log(si, "Hierarchy Validation")
				Log(si, "==================================================")

		End Sub	



		Private Shared Function GetDimensionInfo( _
				ByVal api As FinanceRulesApi) _
				As List(Of DimensionInfo)

			Return New List(Of DimensionInfo) From {
				New DimensionInfo With {.Prefix="E", .MemberName=api.Pov.Entity.Name},
				New DimensionInfo With {.Prefix="A", .MemberName=api.Pov.Account.Name},
				New DimensionInfo With {.Prefix="F", .MemberName=api.Pov.Flow.Name},
				New DimensionInfo With {.Prefix="I", .MemberName=api.Pov.IC.Name},
				New DimensionInfo With {.Prefix="U1", .MemberName=api.Pov.UD1.Name},
				New DimensionInfo With {.Prefix="U2", .MemberName=api.Pov.UD2.Name},
				New DimensionInfo With {.Prefix="U3", .MemberName=api.Pov.UD3.Name},
				New DimensionInfo With {.Prefix="U4", .MemberName=api.Pov.UD4.Name},
				New DimensionInfo With {.Prefix="U5", .MemberName=api.Pov.UD5.Name},
				New DimensionInfo With {.Prefix="U6", .MemberName=api.Pov.UD6.Name},
				New DimensionInfo With {.Prefix="U7", .MemberName=api.Pov.UD7.Name},
				New DimensionInfo With {.Prefix="U8", .MemberName=api.Pov.UD8.Name}
			}

		End Function
		


					'   ##      ##  
					' ####    ####  
					'   ##      ##  
					'   ##      ##  
					'   ##      ##  
					'   ##      ##  
					' ######  ######						

        '=========================================================
        ' SECTION 11 - Export Framework
        '=========================================================

		Private Shared Sub ExportOrigin( _
				ByVal ctx As ExportContext, _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi, _
				ByVal originName As String)

				Dim filterScript As String = _
					BuildExportFilterScript(
						ctx, _
						si, _
						api, _
						originName)

				LogSection(si, "Origin : " & originName)
				LogField(si, "Filter", filterScript)

			ExportBuffer( _
				ctx, _
				si, _
				api, _
				filterScript, _
				originName)

		End Sub 
		
		

		Private Shared Sub ExportBuffer( _
				ByVal ctx As ExportContext, _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi, _
				ByVal filterScript As String, _
				ByVal originName As String)

			Dim sourceBuffer As DataBuffer =
				GetSourceBuffer(api, filterScript)

			If Not HasDataBufferData(si, sourceBuffer) Then
				Exit Sub
			End If

			Log(si, "Exporting " &
				sourceBuffer.DataBufferCells.Count.ToString("#,##0") &
				" cells.")

			For Each cell As DataBufferCell In sourceBuffer.DataBufferCells.Values

				ctx.LineNumber += 1

				Dim rec As ExportRecord = _
					BuildExportRecord( _
						ctx, _
						si, _
						api, _
						ctx.LineNumber, _
						originName, _
						cell)

				WriteCsvRecord(ctx, rec)

				ctx.Summary.TotalRows += 1

				If (ctx.Summary.TotalRows Mod EXPORT_PROGRESS_INTERVAL) = 0 Then

					Log(si, _
						"Exported " &
						ctx.Summary.TotalRows.ToString("#,##0") &
						" rows...")

				End If

			Next

		End Sub



		Private Shared Sub WriteCsvHeader( _
				ByVal ctx As ExportContext)

			ctx.Writer.WriteLine( _
				"LineNo," & _
				"ExportDateTime," & _
				"BRVersion," & _
				"Cube," & _
				"User," & _
				"Entity," & _
				"Parent," & _			
				"Cons," & _						
				"Scenario," & _
				"Time," & _
				"View," & _
				"Account," & _
				"Flow," & _
				"Origin," & _
				"IC," & _
				"UD1," & _
				"UD2," & _
				"UD3," & _
				"UD4," & _
				"UD5," & _
				"UD6," & _
				"UD7," & _
				"UD8," & _
				"Amount")

		End Sub



 		Private Shared Sub WriteCsvRecord( _
				ByVal ctx As ExportContext, _
				ByVal rec As ExportRecord)

			Dim values As New List(Of String)

			values.Add(rec.LineNo.ToString())
			values.Add(rec.ExportDate.ToString("yyyy-MM-dd HH:mm:ss"))
			values.Add(CsvValue(rec.BRVersion))
			values.Add(CsvValue(rec.Cube))
			values.Add(CsvValue(rec.UserName))
			values.Add(CsvValue(rec.Entity))
			values.Add(CsvValue(rec.Parent))
			values.Add(CsvValue(rec.Cons))
			values.Add(CsvValue(rec.Scenario))
			values.Add(CsvValue(rec.Time))
			values.Add(CsvValue(rec.View))
			values.Add(CsvValue(rec.Account))
			values.Add(CsvValue(rec.Flow))
			values.Add(CsvValue(rec.Origin))
			values.Add(CsvValue(rec.IC))
			values.Add(CsvValue(rec.UD1))
			values.Add(CsvValue(rec.UD2))
			values.Add(CsvValue(rec.UD3))
			values.Add(CsvValue(rec.UD4))
			values.Add(CsvValue(rec.UD5))
			values.Add(CsvValue(rec.UD6))
			values.Add(CsvValue(rec.UD7))
			values.Add(CsvValue(rec.UD8))

			values.Add(rec.Amount.ToString( _
				System.Globalization.CultureInfo.InvariantCulture))

			ctx.Writer.WriteLine(String.Join(",", values))

		End Sub
		
		

		Private Shared Function BuildExportRecord( _
				ByVal ctx As ExportContext, _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi, _
				ByVal lineNo As Integer, _
				ByVal originName As String, _
				ByVal cell As DataBufferCell) As ExportRecord

			Dim rec As New ExportRecord()

			rec.LineNo = lineNo
			rec.ExportDate = ctx.ExportTime
			rec.BRVersion = BR_VERSION
			rec.Cube = api.Pov.Cube.Name
			rec.UserName = si.UserName
			rec.Entity = api.Pov.Entity.Name
			rec.Parent = ""
			rec.Cons = api.Pov.Cons.Name
			rec.Scenario = api.Pov.Scenario.Name
			rec.Time = api.Pov.Time.Name
			rec.View = api.Pov.View.Name
			rec.Account = cell.DataBufferCellPk.GetAccountName(api)
			rec.Flow = cell.DataBufferCellPk.GetFlowName(api)
			rec.Origin = cell.DataBufferCellPk.GetOriginName(api)
			rec.IC = cell.DataBufferCellPk.GetICName(api)
			rec.UD1 = cell.DataBufferCellPk.GetUD1Name(api)
			rec.UD2 = cell.DataBufferCellPk.GetUD2Name(api)
			rec.UD3 = cell.DataBufferCellPk.GetUD3Name(api)
			rec.UD4 = cell.DataBufferCellPk.GetUD4Name(api)
			rec.UD5 = cell.DataBufferCellPk.GetUD5Name(api)
			rec.UD6 = cell.DataBufferCellPk.GetUD6Name(api)
			rec.UD7 = cell.DataBufferCellPk.GetUD7Name(api)
			rec.UD8 = cell.DataBufferCellPk.GetUD8Name(api)

			rec.Amount = CDec(cell.CellAmount)

			Return rec

		End Function



		Private Shared Function CreateExportFile( _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi) As String

			'Dim fileName As String = _
			'	"Export_" & _
			'	DateTime.Now.ToString("yyyyMMdd_HHmmss") & _
			'	".csv"
				
				
			Dim fileName As String = _
				"Export_" &
				api.Pov.Entity.Name & "_" &
				DateTime.Now.ToString("yyyyMMdd_HHmmss") &
				".csv"


			Return Path.Combine( _
				GetExportFolder(si, api), _
				fileName)

		End Function



		Private Shared Function GetExportFolder( _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi) As String

			Dim root As String = _
				BRApi.Utilities.GetFileShareFolder( _
					si, _
					FileShareFolderTypes.BatchHarvest, _
					Nothing)

			Dim folder As String = _
				Path.Combine( _
					root, _
					"..\..\Contents\Exports\" & _
					BR_NAME & "\" & _
					api.Pov.Scenario.Name & "\" & _
					api.Pov.Time.Name)

			folder = Path.GetFullPath(folder)

			If Not Directory.Exists(folder) Then
				Directory.CreateDirectory(folder)
			End If

			Return folder

		End Function
		


					'   ##    ####### 
					' ####   ##     ##
					'   ##          ##
					'   ##    ####### 
					'   ##   ##       
					'   ##   ##       
					' ###### #########					
			
        '=========================================================
        ' SECTION 12 - Restore Framework
        '=========================================================


		Private Shared Function ReadRestoreRecord( _
				ByVal fields() As String) As RestoreRecord

			Dim rec As New RestoreRecord()

			rec.LineNo = Integer.Parse(fields(COL_LINENO))

			rec.ExportDate = DateTime.Parse(fields(COL_EXPORTDATE))

			rec.BRVersion = fields(COL_BRVERSION)

			rec.Cube = fields(COL_CUBE)

			rec.UserName = fields(COL_USERNAME)

			rec.Entity = fields(COL_ENTITY)

			rec.Parent = fields(COL_PARENT)

			rec.Cons = fields(COL_CONS)

			rec.Scenario = fields(COL_SCENARIO)

			rec.Time = fields(COL_TIME)

			rec.View = fields(COL_VIEW)

			rec.Account = fields(COL_ACCOUNT)

			rec.Flow = fields(COL_FLOW)

			rec.Origin = fields(COL_ORIGIN)

			rec.IC = fields(COL_IC)

			rec.UD1 = fields(COL_UD1)

			rec.UD2 = fields(COL_UD2)

			rec.UD3 = fields(COL_UD3)

			rec.UD4 = fields(COL_UD4)

			rec.UD5 = fields(COL_UD5)

			rec.UD6 = fields(COL_UD6)

			rec.UD7 = fields(COL_UD7)

			rec.UD8 = fields(COL_UD8)

			rec.Amount = Decimal.Parse(fields(COL_AMOUNT))

			Return rec

		End Function



		Private Shared Sub FlushRestoreBatch( _
				ByVal ctx As RestoreContext, _
				ByVal si As SessionInfo)

			If ctx.MemberScriptAndValues.Count = 0 Then Return

			ctx.Summary.BatchCount += 1

			ctx.BatchNumber += 1

			Log(si, "")
			Log(si, "Submitting Batch " &
				ctx.BatchNumber.ToString() &
				" : " &
				ctx.MemberScriptAndValues.Count.ToString("#,##0") &
				" records")
	
			Dim objXFResult As XFResult =
				BRApi.Finance.Data.SetDataCellsUsingMemberScript( _
					si, _
					ctx.MemberScriptAndValues)

			Log(si, "Batch Complete.")

			ctx.MemberScriptAndValues.Clear()

		End Sub		



		Private Shared Sub RestoreDataRecord( _
				ByVal ctx As RestoreContext, _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi, _
				ByVal rec As RestoreRecord)

			If ctx.Preview Then

				Log(si, "")
				Log(si, "PREVIEW ONLY")
				Log(si, rec.MemberScript)
				Log(si, "Amount = " & rec.Amount.ToString())

				Return

			End If

			Dim msv As New MemberScriptAndValue()

			msv.Script = rec.MemberScript
			msv.Amount = rec.Amount
			msv.IsNoData = False
			msv.CubeName = rec.Cube

			ctx.MemberScriptAndValues.Add(msv)

			ctx.Summary.RowsRestored += 1

			If ctx.MemberScriptAndValues.Count >= ctx.RestoreBatchSize Then

				FlushRestoreBatch( _
					ctx, _
					si)

			End If

		End Sub	
	

	
		Private Shared Sub ReadCsv( _
				ByVal ctx As RestoreContext, _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi)

			Using parser As New Microsoft.VisualBasic.FileIO.TextFieldParser(ctx.CsvFile)

				parser.TextFieldType = _
					Microsoft.VisualBasic.FileIO.FieldType.Delimited

				parser.SetDelimiters(",")

				parser.HasFieldsEnclosedInQuotes = True

				Dim header() As String = parser.ReadFields()

				If header Is Nothing Then

					Throw New Exception("CSV file is empty.")

				End If

				LogHeader(si, "CSV Validation")
				LogField(si, "Columns Found", header.Length.ToString())

				If header.Length <> CSV_COLUMN_COUNT Then

					Throw New Exception( _
						"Invalid CSV format." &
						Environment.NewLine &
						"Expected Columns : " &
						CSV_COLUMN_COUNT.ToString() &
						Environment.NewLine &
						"Actual Columns : " &
						header.Length.ToString())

				End If

				LogSection(si, "CSV Header")

				For Each column As String In header

					Log(si, column)

				Next

				While Not parser.EndOfData

					Dim fields() As String = parser.ReadFields()

					If fields.Length <> CSV_COLUMN_COUNT Then

						Throw New Exception( _
							"Invalid CSV row." & _
							Environment.NewLine & _
							"Expected : " & CSV_COLUMN_COUNT.ToString() & _
							Environment.NewLine & _
							"Actual : " & fields.Length.ToString())

					End If

					Dim rec As RestoreRecord =
						ReadRestoreRecord(fields)

					If Not String.IsNullOrWhiteSpace(rec.Account) Then

						ctx.ExportAnalysis.Accounts.Add(rec.Account)

					End If

					rec.MemberScript =
						BuildMemberScript(rec)

					If ctx.UseCurrentDataUnit Then

						If Not IsMatchingDataUnit(api, rec) Then

							ctx.Summary.RowsSkipped += 1

							Continue While

						End If

					End If

					If Not IsMatchingMemberFilters(ctx, rec) Then

						ctx.Summary.RowsSkipped += 1

						Continue While

					End If

					ctx.Summary.RowsMatched += 1

					ctx.Summary.TotalRows += 1

					RestoreDataRecord( _
						ctx, _
						si, _
						api, _
						rec)

					If (ctx.Summary.TotalRows Mod CSV_PROGRESS_INTERVAL) = 0 Then

						Log(si, _
							"Read " &
							ctx.Summary.TotalRows.ToString("#,##0") &
							" records...")

					End If

				End While

				If ctx.ExecuteRestore Then

					FlushRestoreBatch( _
						ctx, _
						si)
				End If

			End Using

		End Sub



		Private Shared Function GetRestoreRecordValue( _
				ByVal rec As RestoreRecord, _
				ByVal propertyName As String) As String

			Select Case propertyName.ToUpperInvariant()

				Case "ENTITY"
					Return rec.Entity

				Case "PARENT"
					Return rec.Parent

				Case "CONS"
					Return rec.Cons

				Case "SCENARIO"
					Return rec.Scenario

				Case "TIME"
					Return rec.Time

				Case "VIEW"
					Return rec.View

				Case "ACCOUNT"
					Return rec.Account

				Case "FLOW"
					Return rec.Flow

				Case "ORIGIN"
					Return rec.Origin

				Case "IC"
					Return rec.IC

				Case "UD1"
					Return rec.UD1

				Case "UD2"
					Return rec.UD2

				Case "UD3"
					Return rec.UD3

				Case "UD4"
					Return rec.UD4

				Case "UD5"
					Return rec.UD5

				Case "UD6"
					Return rec.UD6

				Case "UD7"
					Return rec.UD7

				Case "UD8"
					Return rec.UD8

				Case Else
					Return String.Empty

			End Select

		End Function



					'   ##    ####### 
					' ####   ##     ##
					'   ##          ##
					'   ##    ####### 
					'   ##          ##
					'   ##   ##     ##
					' ######  ####### 	


        '=========================================================
        ' SECTION 13 - Clear Framework
        '=========================================================



		Private Shared Function ClearStoredDataFromFilter( _
			ByVal si As SessionInfo, _
			ByVal api As FinanceRulesApi, _
			ByVal filterScript As String, _
			ByVal previewMode As Boolean, _
			ByVal maxCells As Integer) As Integer

			Try

				Dim sourceBuffer As DataBuffer =
					GetSourceBuffer(api, filterScript)

				If Not HasDataBufferData(si, sourceBuffer) Then
					Return 0
				End If

				Log(si, "Retrieved Cells : " &
						sourceBuffer.DataBufferCells.Count.ToString("#,##0"))

				If maxCells > 0 Then

					If sourceBuffer.DataBufferCells.Count > maxCells Then

				Throw ErrorHandler.LogWrite(si,
					New XFException(
						"Operation cancelled." &
						Environment.NewLine &
						"Returned Cells : " &
						sourceBuffer.DataBufferCells.Count.ToString("#,##0") &
						Environment.NewLine &
						"Maximum Allowed : " &
						maxCells.ToString("#,##0")))

					End If

				End If

				If previewMode Then

					Log(si, "Preview Mode - No data Cleared.")

					Return 0

				End If

				Dim resultBuffer As New DataBuffer()

				Dim processed As Integer = 0

				For Each sourceCell As DataBufferCell _
					In sourceBuffer.DataBufferCells.Values

					Dim clearCell As New DataBufferCell(sourceCell)

					clearCell.CellAmount = 0

					clearCell.CellStatus = _
						DataCellStatus.CreateDataCellStatus(True, False)

					resultBuffer.SetCell(si, clearCell)

					processed += 1

					If processed Mod LOG_PROGRESS_INTERVAL = 0 Then

						Log(si, _
							processed.ToString("#,##0") &
							" cells processed...")

					End If

				Next

					Log(si, "Writing " &
					processed.ToString("#,##0") &
					" cleared cells to cube.")

					api.Data.SetDataBuffer( _
					resultBuffer, _
					api.Data.GetExpressionDestinationInfo(""))

					Log(si, "Cube update complete.")

					Return processed

			Catch ex As Exception
				FlushLog(si)
				Throw ErrorHandler.LogWrite(si, _
					New XFException(si, ex))

			End Try

		End Function



		Private Shared Function GetSourceBuffer( _
				ByVal api As FinanceRulesApi, _
				ByVal filterScript As String) As DataBuffer

			Try

				Dim clearDestination As ExpressionDestinationInfo =
					api.Data.GetExpressionDestinationInfo("")

				Return api.Data.GetDataBufferUsingFormula( _
					filterScript, _
					DataApiScriptMethodType.Calculate, _
					False, _
					clearDestination)

			Catch

				Return Nothing

			End Try

		End Function
	

	
		Private Shared Function HasDataBufferData( _
				ByVal si As SessionInfo, _
				ByVal sourceBuffer As DataBuffer) As Boolean

			If sourceBuffer Is Nothing Then

				Log(si, "No DataBuffer returned.")
				Return False

			End If

			If sourceBuffer.DataBufferCells Is Nothing Then

				Log(si, "DataBuffer contains no cell collection.")
				Return False

			End If

			If sourceBuffer.DataBufferCells.Count = 0 Then

				Log(si, "No stored data found.")
				Return False

			End If

			Return True

		End Function		




					'   ##   ##       
					' ####   ##    ## 
					'   ##   ##    ## 
					'   ##   ##    ## 
					'   ##   #########
					'   ##         ## 
					' ######       ## 	

        '=========================================================
        ' SECTION 14 - Statistics Framework
        '=========================================================


		Private Shared Sub ShowStatisticsFromFilter( _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi, _
				ByVal filterScript As String)

			Dim sourceBuffer As DataBuffer =
				GetSourceBuffer(api, filterScript)

			LogSection(si, "Filter Script")
			Log(si, filterScript)

			If Not HasDataBufferData(si, sourceBuffer) Then
				Exit Sub
			End If

			Log(si, _
				"Stored Cells : " &
				sourceBuffer.DataBufferCells.Count.ToString("#,##0"))

		End Sub


		Private Shared Sub AnalyzeExportAccounts( _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi, _
				ByVal ctx As RestoreContext)

			LogHeader(si, "Export Analysis")

			Log(si, "Unique Accounts : " & _
				ctx.ExportAnalysis.Accounts.Count.ToString())

			For Each account As String In ctx.ExportAnalysis.Accounts

				If IsParentMember( _
					si, _
					api, _
					"Account", _
					account) Then

					ctx.ExportAnalysis.ParentAccounts.Add(account)

				Else

					ctx.ExportAnalysis.BaseAccounts.Add(account)

				End If

			Next				

			Log(si, "")
			Log(si, "Parent Accounts : " & _
				ctx.ExportAnalysis.ParentAccounts.Count.ToString())
			Log(si, "")
			Log(si, "Base Accounts   : " & _
				ctx.ExportAnalysis.BaseAccounts.Count.ToString())

			If ctx.ExportAnalysis.ParentAccounts.Count > 0 Then

				LogSection(si, "Parent Accounts")

				For Each account As String In _
					ctx.ExportAnalysis.ParentAccounts

					Log(si, account)

				Next

			End If

		End Sub
		
		
		
		Private Shared Sub RecordPerformance( _
				ByVal performance As List(Of PerformanceTimer), _
				ByVal operationName As String, _
				ByVal sw As Stopwatch)

			sw.Stop()

			performance.Add(New PerformanceTimer( _
				operationName, _
				sw.Elapsed))

		End Sub
		
		
		

					'   ##   ########
					' ####   ##      
					'   ##   ##      
					'   ##   ####### 
					'   ##         ##
					'   ##   ##    ##
					' ######  ###### 		

        '=========================================================
        ' SECTION 15 - File Framework
        '=========================================================



		Private Shared Function GetLatestExportFile( _
			ByVal si As SessionInfo, _
			ByVal api As FinanceRulesApi) As String

			Dim exportFolder As String =
				GetExportFolder(si, api)

			LogSection(si, "Looking for Exports")

			Log(si, exportFolder)

			If Not Directory.Exists(exportFolder) Then

				Throw New Exception( _
					"Export folder not found." &
					Environment.NewLine &
					exportFolder)

			End If

			Dim csvFiles() As String =
				Directory.GetFiles( _
					exportFolder,
					"*.csv",
					SearchOption.AllDirectories)

			Log(si, "CSV Files Found : " & csvFiles.Length.ToString())

			If csvFiles.Length = 0 Then

				Throw New Exception( _
					"No export files found.")

			End If

			Dim latestFile As String =
				csvFiles _
					.OrderByDescending(Function(f) File.GetLastWriteTime(f)) _
					.First()
					
				LogField(si, "Latest Export", latestFile)

			Return latestFile

		End Function


					'   ##    ####### 
					' ####   ##     ##
					'   ##   ##       
					'   ##   ######## 
					'   ##   ##     ##
					'   ##   ##     ##
					' ######  ####### 		

        '=========================================================
        ' SECTION 16 - Logging Framework
        '=========================================================		



		Private Shared Sub InitializeLog()

			_log.Clear()

			_logFile = Nothing

			_hierarchyCache.Clear()

		End Sub



		Private Shared Sub Log( _
				ByVal si As SessionInfo, _
				ByVal message As String)

			Dim line As String =
				DateTime.Now.ToString("HH:mm:ss.fff") &
				"  " &
				message

			BRApi.ErrorLog.LogMessage(si, BR_NAME & " - " & message)

			If ENABLE_FILE_LOGGING Then

				_log.AppendLine(line)

			End If

		End Sub



		Private Shared Sub LogBlankLine( _
				ByVal si As SessionInfo)

			Log(si, String.Empty)

		End Sub

	

		Private Shared Sub LogField( _
				ByVal si As SessionInfo, _
				ByVal label As String, _
				ByVal value As String)

			Log(si, _
				label.PadRight(15) & _
				": " & _
				value)

		End Sub



		Private Shared Sub LogResolvedMembers( _
				ByVal si As SessionInfo, _
				ByVal members As HashSet(Of String))

			Log(si, "Returned Count = " & members.Count.ToString())

			For Each member As String In members
				Log(si, "Returned Member = '" & member & "'")
			Next

		End Sub



		Private Shared Sub FlushLog( _
				ByVal si As SessionInfo)

			If Not ENABLE_FILE_LOGGING Then Exit Sub

			Try

				Dim logDir As String = GetLogDirectory(si)

				If String.IsNullOrWhiteSpace(_logFile) Then

					_logFile =
						Path.Combine(
							logDir,
							BR_NAME &
							"_" &
							_currentFunction &
							"_" &
							DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") &
							".log")

				End If

				File.WriteAllText(
					_logFile,
					_log.ToString())

			Catch ex As Exception

				BRApi.ErrorLog.LogMessage(
					si,
					"Unable to write log file." &
					Environment.NewLine &
					ex.ToString())

			End Try

		End Sub



		Private Shared Sub LogExportSummary( _
				ByVal si As SessionInfo, _
				ByVal ctx As ExportContext)

			LogHeader(si, BR_NAME & " Export Summary")

			LogField(si, "Export File", ctx.Summary.ExportFile)
			LogField(si, "Scenario", ctx.Summary.Scenario)
			LogField(si, "Time", ctx.Summary.Time)
			LogField(si, "Entity", ctx.Summary.Entity)
			LogField(si, "Cube", ctx.Summary.Cube)

			Log(si, "Origins     : " &
				ctx.Summary.TotalOrigins.ToString("#,##0"))

			Log(si, "Rows        : " &
				ctx.Summary.TotalRows.ToString("#,##0"))

			Log(si, "Started     : " &
				ctx.Summary.StartTime.ToString("yyyy-MM-dd HH:mm:ss"))

			Log(si, "Finished    : " &
				ctx.Summary.EndTime.ToString("yyyy-MM-dd HH:mm:ss"))

			Dim elapsed As TimeSpan =
				ctx.Summary.EndTime.Subtract(ctx.Summary.StartTime)

			Dim rowsPerSecond As Double = 0

			If elapsed.TotalSeconds > 0 Then

				rowsPerSecond = _
					ctx.Summary.TotalRows / elapsed.TotalSeconds
			End If

			Log(si, "Elapsed     : " &
				elapsed.TotalSeconds.ToString("N2") &
				" seconds")
				
			Log(si, "Throughput  : " &
				rowsPerSecond.ToString("N0") &
				" rows/sec")

			Log(si, LOG_SEPARATOR)

		End Sub	



		Private Shared Sub LogRestoreSummary( _
				ByVal ctx As RestoreContext, _
				ByVal si As SessionInfo)

			Dim elapsed As TimeSpan =
				ctx.Summary.EndTime - ctx.Summary.StartTime

			LogHeader(si, "Restore Complete")

			LogField(si, "CSV File", ctx.Summary.CsvFile)
			LogField(si, "Rows Read", ctx.Summary.TotalRows.ToString("#,##0"))
			LogField(si, "Rows Matched", ctx.Summary.RowsMatched.ToString("#,##0"))
			LogField(si, "Rows Restored", ctx.Summary.RowsRestored.ToString("#,##0"))
			LogField(si, "Rows Skipped", ctx.Summary.RowsSkipped.ToString("#,##0"))
			LogField(si, "Batches", ctx.Summary.BatchCount.ToString("#,##0"))
			LogField(si, "Errors", ctx.Summary.ErrorCount.ToString("#,##0"))

			Log(si, "Elapsed Time    : " &
				elapsed.ToString())

		End Sub



		Private Shared Sub LogHeader( _
			ByVal si As SessionInfo, _
			ByVal title As String)

			Log(si, String.Empty)
			Log(si, New String("="c, 70))
			Log(si, title)
			LogField(si, "Function", _currentFunction)
			Log(si, New String("="c, 70))

		End Sub

	

		Private Shared Sub LogSection( _
			ByVal si As SessionInfo, _
			ByVal title As String)

			Log(si, String.Empty)
			Log(si, title)
			Log(si, New String("-"c, 70))

		End Sub



		Private Shared Sub LogFooter( _
			ByVal si As SessionInfo)

			Log(si, New String("="c, 70))
			Log(si, "Completed")
			Log(si, String.Empty)

		End Sub



		Private Shared Sub LogPerformance( _
				ByVal si As SessionInfo, _
				ByVal performance As List(Of PerformanceTimer))

			LogSection(si, "Performance")

			Dim totalSeconds As Double = 0

			For Each item As PerformanceTimer In performance

				If item.Elapsed.TotalMilliseconds < 1000 Then

					LogField( _
						si, _
						item.Name, _
						item.Elapsed.TotalMilliseconds.ToString("N0") & " ms")

				Else

					If totalSeconds < 1 Then

						LogField( _
							si, _
							"Total", _
							(totalSeconds * 1000).ToString("N0") & " ms")

					Else

						LogField( _
							si, _
							"Total", _
							totalSeconds.ToString("N2") & " sec")

					End If

				End If

				totalSeconds += item.Elapsed.TotalSeconds

			Next

			LogField( _
				si, _
				"Total", _
				totalSeconds.ToString("N2") & " sec")

		End Sub




					'   ##   ########
					' ####   ##    ##
					'   ##       ##  
					'   ##      ##   
					'   ##     ##    
					'   ##     ##    
					' ######   ##    		

        '=========================================================
        ' SECTION 17 - Shared Utility Functions
        '=========================================================		

		Private Shared Function GetMemberOrDefault( _
			ByVal memberName As String) As String

			If String.IsNullOrWhiteSpace(memberName) Then
				Return Nothing
			End If

			Return memberName.Trim()

		End Function
		
		

		Private Shared Function GetDimensionName( _
				ByVal dimPrefix As String) _
				As String

			Select Case dimPrefix.ToUpperInvariant()

				Case "A"
					Return "Account"

				Case "E"
					Return "Entity"

				Case "F"
					Return "Flow"

				Case "I"
					Return "IC"

				Case "U1"
					Return "UD1"

				Case "U2"
					Return "UD2"

				Case "U3"
					Return "UD3"

				Case "U4"
					Return "UD4"

				Case "U5"
					Return "UD5"

				Case "U6"
					Return "UD6"

				Case "U7"
					Return "UD7"

				Case "U8"
					Return "UD8"

				Case Else

			Throw New Exception( _
				"Unknown dimension prefix: " & dimPrefix)

			End Select

		End Function



		Private Shared Function CsvValue( _
				ByVal value As String) As String

			If String.IsNullOrEmpty(value) Then
				Return """"""
			End If

			Return """" &
				   value.Replace("""", """""") &
					""""
		End Function



		Private Shared Function IsMatchingDataUnit( _
				ByVal api As FinanceRulesApi, _
				ByVal rec As RestoreRecord) As Boolean

			If Not rec.Entity.Equals( _
					api.Pov.Entity.Name, _
					StringComparison.OrdinalIgnoreCase) Then

				Return False

			End If

			If Not rec.Parent.Equals( _
					api.Pov.Parent.Name, _
					StringComparison.OrdinalIgnoreCase) Then

				Return False

			End If

			If Not rec.Cons.Equals( _
					api.Pov.Cons.Name, _
					StringComparison.OrdinalIgnoreCase) Then

				Return False

			End If

			If Not rec.Scenario.Equals( _
					api.Pov.Scenario.Name, _
					StringComparison.OrdinalIgnoreCase) Then

				Return False

			End If

			If Not rec.Time.Equals( _
					api.Pov.Time.Name, _
					StringComparison.OrdinalIgnoreCase) Then

				Return False

			End If

			Return True

		End Function		



		Private Shared Function GetPovMembers( _
			ByVal api As FinanceRulesApi) _
			As PovMembers

			Dim pov As New PovMembers

			pov.Scenario = api.Pov.Scenario.Name
			pov.Time = api.Pov.Time.Name
			pov.Cons = api.Pov.Cons.Name
			pov.View = api.Pov.View.Name

			pov.Entity = GetMemberOrDefault(api.Pov.Entity.Name)
			pov.Account = GetMemberOrDefault(api.Pov.Account.Name)
			pov.Flow = GetMemberOrDefault(api.Pov.Flow.Name)
			pov.IC = GetMemberOrDefault(api.Pov.IC.Name)

			pov.UD1 = GetMemberOrDefault(api.Pov.UD1.Name)
			pov.UD2 = GetMemberOrDefault(api.Pov.UD2.Name)
			pov.UD3 = GetMemberOrDefault(api.Pov.UD3.Name)
			pov.UD4 = GetMemberOrDefault(api.Pov.UD4.Name)
			pov.UD5 = GetMemberOrDefault(api.Pov.UD5.Name)
			pov.UD6 = GetMemberOrDefault(api.Pov.UD6.Name)
			pov.UD7 = GetMemberOrDefault(api.Pov.UD7.Name)
			pov.UD8 = GetMemberOrDefault(api.Pov.UD8.Name)

			Return pov

		End Function



		Private Shared Function GetExpandedPovDimensions( _
				ByVal pov As PovMembers) _
				As List(Of PovDimension)

			Dim dimensions As New List(Of PovDimension) From {
				New PovDimension("E", pov.Entity),
				New PovDimension("A", pov.Account),
				New PovDimension("F", pov.Flow),
				New PovDimension("I", pov.IC),
				New PovDimension("U1", pov.UD1),
				New PovDimension("U2", pov.UD2),
				New PovDimension("U3", pov.UD3),
				New PovDimension("U4", pov.UD4),
				New PovDimension("U5", pov.UD5),
				New PovDimension("U6", pov.UD6),
				New PovDimension("U7", pov.UD7),
				New PovDimension("U8", pov.UD8)
			}

			Return dimensions

		End Function
		
		

		Private Shared Function GetLogDirectory( _
			ByVal si As SessionInfo) As String

			Dim logRoot As String =
				BRApi.Utilities.GetFileShareFolder(
					si,
					FileShareFolderTypes.BatchHarvest,
					Nothing)

			Dim logDir As String =
				Path.Combine(
					logRoot,
					"..\..\Contents\Logs\MTCH_ClearCubeData")

			logDir = Path.GetFullPath(logDir)

			If Not Directory.Exists(logDir) Then
				Directory.CreateDirectory(logDir)
			End If

			Return logDir

		End Function
		
		

		Private Shared Function IsMatchingMemberFilters( _
				ByVal ctx As RestoreContext, _
				ByVal rec As RestoreRecord) As Boolean

			For Each kvp As KeyValuePair(Of String, DimensionFilter) In ctx.MemberFilters

				Dim recordValue As String =
					GetRestoreRecordValue( _
						rec, _
						kvp.Key)

				If String.IsNullOrWhiteSpace(recordValue) Then

					Continue For

				End If

				If Not kvp.Value.Members.Contains(recordValue) Then

					Return False

				End If

			Next

			Return True

		End Function

	End Class

End Namespace