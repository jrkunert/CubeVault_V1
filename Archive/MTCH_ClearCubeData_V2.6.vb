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

'====================================================================
' Revision History
'====================================================================
'
' 2.0.0  Initial Clear Data Utility
' 2.1.0  Statistics
' 2.2.0  Validation
' 2.3.0  Buffered Logging
' 2.4.0  CSV Export
' 2.4.1  Export Summary
' 2.5    Shared File Utilities
' 2.5.1  CSV Reader
' 2.5.2  Read One CSV Record
' 2.5.3  Proper CSV Reader
' 2.5.4  CSV Column Constants
' 2.5.5  Build RestoreRecord
' 2.5.6  Build the Member Script (Preview Only)
' 2.5.7  Preview Restore
' 2.5.8  Read Every Record (Preview Mode)
' 2.6    Restore One Record
'
'====================================================================



Namespace OneStream.BusinessRule.Finance.MTCH_ClearCubeData

	Public Class MainClass

		Private Const BR_NAME As String = "MTCH_ClearCubeData"
		Private Const BR_VERSION As String = "2.6"
		Private Const OS_VERSION As String = "9.2.1.18605"
		Private Const LOG_SEPARATOR As String = _
    "============================================================"
		
		Private Const EXPORT_PROGRESS_INTERVAL As Integer = 10000

		Private Const DEFAULT_MAXCELLS As Integer = 500000
		Private Const DEFAULT_BATCHSIZE As Integer = 10000
		Private Const ENABLE_FILE_LOGGING As Boolean = True

		Private Shared _logFile As String = Nothing
		Private Shared _log As New StringBuilder()
		Private Shared _exportLineNumber As Integer = 0
		Private Shared _currentFunction As String = "Unknown"


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

							Case "CLEARDATA"

								ClearData(si, api, args)

							Case "STATISTICS"

								Statistics(si, api, args)

							Case "EXPLAIN"

								Explain(si, api, args)								
								
							Case "VALIDATE"

								Validate(si, api, args)
								
							Case "EXPORT"

								Export(si, api, args)
								
							Case "RESTORE"

								ExecuteRestore(si, api, args)
								
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

'====================================================================
'====================================================================

#Region "Main Processing"

		Private Shared Sub ClearData( _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi, _
				ByVal args As FinanceRulesArgs)

			ExecuteClear(si, api, args, False)

		End Sub

		Private Shared Sub Statistics( _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi, _
				ByVal args As FinanceRulesArgs)

			ExecuteStatistics(si, api, args)

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

		Private Shared Sub ExecuteClear( _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi, _
				ByVal args As FinanceRulesArgs, _
				ByVal previewMode As Boolean)

			Dim sw As Stopwatch = Stopwatch.StartNew()

			Try

			Dim maxCells As Integer
			Dim batchSize As Integer
			Dim origins As New List(Of String)

			ReadParameters(api, args, maxCells, batchSize, origins)

				Log(si, "=========================================")
				Log(si, BR_NAME & "  Version " & BR_VERSION)
				Log(si, "Scenario : " & api.Pov.Scenario.Name)
				Log(si, "Time     : " & api.Pov.Time.Name)
				Log(si, "Entity   : " & api.Pov.Entity.Name)
				Log(si, "Cons     : " & api.Pov.Cons.Name)
				Log(si, "View     : " & api.Pov.View.Name)
				
				'Log(si, "Preview  : " & previewMode.ToString)
				If args.CustomCalculateArgs.NameValuePairs IsNot Nothing Then

					Log(si, "Parameters")

					For Each kvp As KeyValuePair(Of String,String) _
						In args.CustomCalculateArgs.NameValuePairs

						Log(si, "   " & kvp.Key & " = " & kvp.Value)

					Next

				End If
				Log(si, "BatchSize : " & batchSize.ToString("#,##0"))
				Log(si, "=========================================")

		'	Dim totalProcessed As Integer = 0

			Log(si, "Origins : " & origins.Count.ToString())

			Dim totalProcessed As Integer = 0

			For Each originName As String In origins

				Dim filterScript As String = BuildFilterScript(api, originName)

				Log(si, "Origin : " & originName)
				Log(si, "Filter : " & filterScript)

				totalProcessed += ClearStoredDataFromFilter( _
					si, _
					api, _
					filterScript, _
					previewMode, _
					maxCells)

			Next

			Log(si, "=========================================")
			Log(si, "Total Cells Cleared : " & totalProcessed.ToString("#,##0"))
			Log(si, "Completed Successfully")
			Log(si, "Elapsed : " & sw.Elapsed.ToString())

			FlushLog(si)

			Catch ex As Exception
				FlushLog(si)
				Throw ErrorHandler.LogWrite(si, _
					New XFException(si, ex))

			End Try

		End Sub
		
		Private Shared Sub ExecuteExplain( _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi, _
				ByVal args As FinanceRulesArgs)

			Try

			InitializeLog()

				Dim maxCells As Integer
				Dim batchSize As Integer
				Dim origins As New List(Of String)

				ReadParameters(api, args, maxCells, batchSize, origins)

				Log(si, "")
				Log(si, "=========================================")
				Log(si, "MTCH_ClearCubeData - Explain")
				Log(si, "=========================================")

				Log(si, "Scenario : " & api.Pov.Scenario.Name)
				Log(si, "Time     : " & api.Pov.Time.Name)
				Log(si, "Entity   : " & api.Pov.Entity.Name)
				Log(si, "Cons     : " & api.Pov.Cons.Name)
				Log(si, "View     : " & api.Pov.View.Name)

				Log(si, "")

				Log(si, "MaxCells : " & maxCells.ToString("#,##0"))
				Log(si, "BatchSize: " & batchSize.ToString("#,##0"))

				Log(si, "")
				Log(si, "Origins")

				For Each origin As String In origins

					Log(si, "   " & origin)

				Next

				Log(si, "")
				Log(si, "Generated Filter(s)")
				Log(si, "--------------------")

				For Each origin As String In origins

					Dim filterScript As String =
						BuildFilterScript(api, origin)

					Log(si, "")
					Log(si, "Origin : " & origin)
					Log(si, filterScript)

					ShowStatisticsFromFilter( _
						si,
						api,
						filterScript)

				Next

				Log(si, "")
				Log(si, "Explain Complete")

			Catch ex As Exception
				FlushLog(si)
				Throw ErrorHandler.LogWrite(si,
					New XFException(si, ex))

			End Try

		End Sub		

		Private Shared Sub ExecuteValidate( _
			ByVal si As SessionInfo, _
			ByVal api As FinanceRulesApi, _
			ByVal args As FinanceRulesArgs)

			Try

				InitializeLog()

				Dim maxCells As Integer
				Dim batchSize As Integer
				Dim origins As New List(Of String)

				ReadParameters(api, args, maxCells, batchSize, origins)

				Log(si, "")
				Log(si, "=========================================")
				Log(si, "VALIDATION REPORT")
				Log(si, "=========================================")

				Log(si, "Version    : " & BR_VERSION)
				Log(si, "Scenario   : " & api.Pov.Scenario.Name)
				Log(si, "Time       : " & api.Pov.Time.Name)
				Log(si, "Entity     : " & api.Pov.Entity.Name)
				Log(si, "Cons       : " & api.Pov.Cons.Name)
				Log(si, "View       : " & api.Pov.View.Name)

				Log(si, "")
				Log(si, "MaxCells   : " & maxCells.ToString("#,##0"))
				Log(si, "BatchSize  : " & batchSize.ToString("#,##0"))

				Log(si, "")
				Log(si, "Origins")
				Log(si, "-------")

				For Each origin As String In origins

					Log(si, "   " & origin)

				Next

				Log(si, "")

				For Each origin As String In origins

					ValidateOrigin( _
						si, _
						api, _
						origin)

				Next

				Log(si, "")
				Log(si, "Validation PASSED")

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

				Dim maxCells As Integer
				Dim batchSize As Integer
				Dim origins As New List(Of String)

				ReadParameters(api, args, maxCells, batchSize, origins)

				Log(si, "=========================================")
				Log(si, BR_NAME & " Statistics")
				Log(si, "Scenario : " & api.Pov.Scenario.Name)
				Log(si, "Time     : " & api.Pov.Time.Name)
				Log(si, "Entity   : " & api.Pov.Entity.Name)
				Log(si, "=========================================")

				For Each originName As String In origins

					Dim filterScript As String = BuildFilterScript(api, originName)

					Log(si, "")
					Log(si, "Origin : " & originName)

					ShowStatisticsFromFilter( _
						si, _
						api, _
						filterScript)

				Next

				Log(si, "")
				Log(si, "Statistics Complete")

			Catch ex As Exception
				FlushLog(si)
				Throw ErrorHandler.LogWrite(si, _
					New XFException(si, ex))

			End Try

		End Sub

		Private Shared Sub ReadParameters( _
			ByVal api As FinanceRulesApi, _
			ByVal args As FinanceRulesArgs, _
			ByRef maxCells As Integer, _
			ByRef batchSize As Integer, _
			ByRef origins As List(Of String))

		   ' Defaults
			maxCells = DEFAULT_MAXCELLS
			batchSize = DEFAULT_BATCHSIZE

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

#End Region ' "Main Processing"

'====================================================================
'====================================================================

#Region "Filter Builder"

		Private Shared Function BuildFilterScript( _
				ByVal api As FinanceRulesApi, _
				ByVal originName As String) As String

			Dim parts As New List(Of String)

		' Exact POV dimensions
		AddExactIfPresent(parts, "S", api.Pov.Scenario.Name)
		AddExactIfPresent(parts, "T", api.Pov.Time.Name)
		AddExactIfPresent(parts, "C", api.Pov.Cons.Name)
		AddExactIfPresent(parts, "V", api.Pov.View.Name)
		AddExactIfPresent(parts, "O", originName)

		' Expanded hierarchy dimensions
		AddExpandedIfPresent(parts, "E", GetMemberOrDefault(api.Pov.Entity.Name))

		AddExpandedIfPresent(parts, "A", GetMemberOrDefault(api.Pov.Account.Name))
		AddExpandedIfPresent(parts, "F", GetMemberOrDefault(api.Pov.Flow.Name))
		AddExpandedIfPresent(parts, "I", GetMemberOrDefault(api.Pov.IC.Name))

		AddExpandedIfPresent(parts, "U1", GetMemberOrDefault(api.Pov.UD1.Name))
		AddExpandedIfPresent(parts, "U2", GetMemberOrDefault(api.Pov.UD2.Name))
		AddExpandedIfPresent(parts, "U3", GetMemberOrDefault(api.Pov.UD3.Name))
		AddExpandedIfPresent(parts, "U4", GetMemberOrDefault(api.Pov.UD4.Name))
		AddExpandedIfPresent(parts, "U5", GetMemberOrDefault(api.Pov.UD5.Name))
		AddExpandedIfPresent(parts, "U6", GetMemberOrDefault(api.Pov.UD6.Name))
		AddExpandedIfPresent(parts, "U7", GetMemberOrDefault(api.Pov.UD7.Name))
		AddExpandedIfPresent(parts, "U8", GetMemberOrDefault(api.Pov.UD8.Name))

		Return BuildFilterMembersScript(parts)

		End Function

		Private Shared Function GetMemberOrDefault( _
			ByVal memberName As String) As String

			If String.IsNullOrWhiteSpace(memberName) Then
				Return Nothing
			End If

			Return memberName.Trim()

		End Function

#End Region '"Filter Builder"

'====================================================================
'====================================================================

#Region "Data Clear Engine"

		Private Shared Sub ValidateOrigin( _
			ByVal si As SessionInfo, _
			ByVal api As FinanceRulesApi, _
			ByVal originName As String)

			Dim filterScript As String =
				BuildFilterScript(api, originName)

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

		Private Shared Sub ShowStatisticsFromFilter( _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi, _
				ByVal filterScript As String)

			Dim sourceBuffer As DataBuffer =
				GetSourceBuffer(api, filterScript)

			Log(si, "Filter Script:")
			Log(si, filterScript)


			If sourceBuffer Is Nothing Then
				Log(si, "No DataBuffer returned.")
				Exit Sub
			End If

			If sourceBuffer.DataBufferCells Is Nothing Then
				Log(si, "No DataBufferCells collection.")
				Exit Sub
			End If

			Log(si, _
				"Stored Cells : " &
				sourceBuffer.DataBufferCells.Count.ToString("#,##0"))

		End Sub

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

		Private Shared Function ClearStoredDataFromFilter( _
			ByVal si As SessionInfo, _
			ByVal api As FinanceRulesApi, _
			ByVal filterScript As String, _
			ByVal previewMode As Boolean, _
			ByVal maxCells As Integer) As Integer

			Try

				Dim sourceBuffer As DataBuffer =
					GetSourceBuffer(api, filterScript)

				If sourceBuffer Is Nothing Then
					Log(si, "No DataBuffer returned.")
					Return 0
				End If

				If sourceBuffer.DataBufferCells Is Nothing Then
					Log(si, "DataBuffer contains no cell collection.")
					Return 0
				End If

				If sourceBuffer.DataBufferCells.Count = 0 Then
					Log(si, "No stored data found.")
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

					Log(si, "Preview Mode - No data written.")

					Return 0

				End If

				Dim resultBuffer As New DataBuffer()

				Dim processed As Integer = 0

				For Each sourceCell As DataBufferCell _
					In sourceBuffer.DataBufferCells.Values

					Dim clearCell As New DataBufferCell(sourceCell)

					clearCell.CellAmount = 0D

					clearCell.CellStatus = _
						DataCellStatus.CreateDataCellStatus(True, False)

					resultBuffer.SetCell(si, clearCell)

					processed += 1

					If processed Mod 10000 = 0 Then

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

#End Region '"Data Clear Engine"

'====================================================================
'====================================================================

#Region "Helper Methods"

		Private Shared Function BuildFilterMembersScript( _
				ByVal parts As List(Of String)) As String

			Dim sb As New StringBuilder()

			sb.Append("FilterMembers(")
			sb.Append(String.Join(", ", parts.ToArray()))
			sb.Append(")")

			Return sb.ToString()

		End Function

		Private Shared Sub AddExactIfPresent( _
				ByVal parts As List(Of String), _
				ByVal dimPrefix As String, _
				ByVal memberName As String)

			If String.IsNullOrWhiteSpace(memberName) Then Exit Sub

			parts.Add(dimPrefix & "#" & memberName)

		End Sub

		Private Shared Sub AddExpandedIfPresent( _
				ByVal parts As List(Of String), _
				ByVal dimPrefix As String, _
				ByVal memberName As String)

			If String.IsNullOrWhiteSpace(memberName) Then Exit Sub

			parts.Add(GetExpandedMemberScript(dimPrefix, memberName))

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

#End Region '"Helper Methods

'====================================================================
'====================================================================

#Region "Helper Classes"

		Private Class ExportContext

			Public Property ExportFile As String

			Public Property ExportTime As DateTime

			Public Property LineNumber As Integer

			Public Property Writer As StreamWriter
			
			Public Property Summary As ExportSummary

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

		Private Class RestoreContext

			Public CsvFile As String

			Public LineNumber As Integer

			Public Summary As RestoreSummary
			
			Public Buffer As DataBuffer

		End Class

		Private Class RestoreSummary

			Public StartTime As DateTime

			Public EndTime As DateTime

			Public CsvFile As String

			Public TotalRows As Integer

			Public RowsRestored As Integer

			Public RowsSkipped As Integer

			Public ErrorCount As Integer

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

#End Region '"Helper Classes"

'====================================================================
'====================================================================

#Region "File Utilities"

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

		Private Shared Function CreateExportFile( _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi) As String

			Return Path.Combine( _
				GetExportFolder(si, api), _
				"Export_" & _
				DateTime.Now.ToString("yyyyMMdd_HHmmss") & _
				".csv")

		End Function
		
		Private Shared Function GetLatestExportFile( _
			ByVal si As SessionInfo, _
			ByVal api As FinanceRulesApi) As String

			Dim exportFolder As String =
				GetExportFolder(si, api)
				
			Log(si, "")
			Log(si, "Looking for exports in:")
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

			Return latestFile

		End Function

#End Region  '"File Utilities"

'====================================================================
'====================================================================

#Region "Export"

		Private Shared Sub Export( _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi, _
				ByVal args As FinanceRulesArgs)

			ExecuteExport( _
				si, _
				api, _
				args)

		End Sub

		Private Shared Sub ExecuteExport( _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi, _
				ByVal args As FinanceRulesArgs)

			Try

				InitializeLog()

				Dim maxCells As Integer
				Dim batchSize As Integer
				Dim origins As New List(Of String)

				ReadParameters(api, args, maxCells, batchSize, origins)
				
				Dim ctx As New ExportContext()
				
				ctx.Summary = New ExportSummary()
				ctx.Summary.StartTime = DateTime.Now
				ctx.Summary.TotalOrigins = origins.Count
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

				ctx.Writer = New StreamWriter( _
					ctx.ExportFile, _
					False, _
					System.Text.Encoding.UTF8)
					
				Log(si, "CSV Writer Opened")

				WriteCsvHeader(ctx)
				
				Log(si, "Header Written")

				Log(si, "Export File : " & ctx.ExportFile)

				For Each originName As String In origins

					ExportOrigin( _
						ctx, _
						si, _
						api, _
						originName)

				Next

				ctx.Writer.Flush()
				ctx.Writer.Close()

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
	
		Private Shared Function CsvValue( _
				ByVal value As String) As String

			If String.IsNullOrEmpty(value) Then
				Return """"""
			End If

			Return """" &
				   value.Replace("""", """""") &
					""""
		End Function

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

		Private Shared Sub ExportOrigin( _
				ByVal ctx As ExportContext, _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi, _
				ByVal originName As String)

			Dim filterScript As String =
				BuildFilterScript(api, originName)

			Log(si, "")
			Log(si, "Origin : " & originName)
			Log(si, "Filter : " & filterScript)

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

			If sourceBuffer Is Nothing Then Exit Sub

			If sourceBuffer.DataBufferCells Is Nothing Then Exit Sub

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

		Private Shared Sub LogExportSummary( _
				ByVal si As SessionInfo, _
				ByVal ctx As ExportContext)

			Log(si, "")
			Log(si, LOG_SEPARATOR)
			Log(si, BR_NAME & " Export Summary")
			Log(si, LOG_SEPARATOR)

			Log(si, "Export File : " & ctx.Summary.ExportFile)
			
			Log(si, "Scenario    : " & ctx.Summary.Scenario)

			Log(si, "Time        : " & ctx.Summary.Time)

			Log(si, "Entity      : " & ctx.Summary.Entity)

			Log(si, "Cube        : " & ctx.Summary.Cube)

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

#End Region  '"Export"

'====================================================================
'====================================================================

#Region "Restore"

		Private Shared Sub ExecuteRestore( _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi, _
				ByVal args As FinanceRulesArgs)

			Try

				InitializeLog()

				Dim ctx As New RestoreContext()
				
				ctx.Buffer = New DataBuffer()

				ctx.Summary = New RestoreSummary()

				ctx.Summary.StartTime = DateTime.Now
				
				ctx.CsvFile = GetLatestExportFile(si, api)

				ctx.Summary.CsvFile = ctx.CsvFile

				Log(si, "")
				Log(si, LOG_SEPARATOR)
				Log(si, BR_NAME & " Version " & BR_VERSION)
				Log(si, "Restore Started")
				Log(si, "")
				Log(si, "Restore File")
				
				Log(si, ctx.CsvFile)
				
				ReadCsv(ctx, si, api)
				
				Log(si, LOG_SEPARATOR)

				FlushLog(si)

			Catch ex As Exception

				FlushLog(si)

				Throw ErrorHandler.LogWrite( _
					si, _
					New XFException(si, ex))

			End Try

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

				Log(si, "")
				Log(si, "CSV Header")
				Log(si, LOG_SEPARATOR)

				Log(si, "Columns Found : " &
					header.Length.ToString())
					
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
						
					rec.MemberScript =
						BuildMemberScript(rec)

					ctx.Summary.TotalRows += 1
					
					If (ctx.Summary.TotalRows Mod 1000) = 0 Then

						Log(si, _
							"Read " &
							ctx.Summary.TotalRows.ToString("#,##0") &
							" records...")

					End If

					If ctx.Summary.TotalRows = 1 Then

						Log(si, "")
						Log(si, "Preview Restore")
						Log(si, LOG_SEPARATOR)

						Log(si, rec.MemberScript)

						Log(si, "")

						Log(si, "Amount = " &
							rec.Amount.ToString())
							
						Log(si, "")

					End If

					If ctx.Summary.TotalRows = 1 Then

						AddRestoreRecordToBuffer(_
							ctx, _
							si, _
							api, _
							rec)

						Exit While

					End If


				End While

				Log(si, "")
				Log(si, "Total Records Read : " &
					ctx.Summary.TotalRows.ToString("#,##0"))

			End Using

		End Sub

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

		Private Shared Function BuildMemberScript( _
				ByVal rec As RestoreRecord) As String

			Dim sb As New System.Text.StringBuilder()

			sb.Append("Cb#Finance")

			sb.Append(":E#").Append(rec.Entity)

			sb.Append(":C#").Append(rec.Cons)

			sb.Append(":S#").Append(rec.Scenario)

			sb.Append(":T#").Append(rec.Time)

			sb.Append(":V#").Append(rec.View)

			sb.Append("A#").Append(rec.Account)

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


		Private Shared Sub AddRestoreRecordToBuffer( _
				ByVal ctx As RestoreContext, _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi, _
				ByVal rec As RestoreRecord)

			Log(si, "")
			Log(si, "Restoring")

			Log(si, rec.MemberScript)

			Log(si, "Amount = " &
				rec.Amount.ToString())

		End Sub


		Private Shared Function ReadCsvHeader( _
				ByVal sr As StreamReader) As String()

			Dim header As String = sr.ReadLine()

			If String.IsNullOrWhiteSpace(header) Then

				Throw New Exception( _
					"CSV header is missing.")

			End If

			Return header.Split(","c)

		End Function
	
#End Region  ' "Restore"

'====================================================================
'====================================================================

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

'====================================================================
'====================================================================

#Region "Logging"

	'====================================================================
	' WRITE LOG FILE
	'====================================================================
	
		Private Shared Sub InitializeLog()

			_log.Clear()

			_logFile = Nothing

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


		Private Shared Sub FlushLog( _
				ByVal si As SessionInfo)

			If Not ENABLE_FILE_LOGGING Then Exit Sub

			Try

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

				If String.IsNullOrWhiteSpace(_logFile) Then

					_logFile =
						Path.Combine(
							logDir,
							BR_NAME &
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

#End Region '"Logging"

	End Class

End Namespace