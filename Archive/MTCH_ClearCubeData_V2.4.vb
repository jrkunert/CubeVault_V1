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


Namespace OneStream.BusinessRule.Finance.MTCH_ClearCubeData

	Public Class MainClass

		Private Const BR_NAME As String = "MTCH_ClearCubeData"
		Private Const BR_VERSION As String = "2.4.0"
		Private Const OS_VERSION As String = "9.2.1.18605"
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

						Select Case args.CustomCalculateArgs.FunctionName.ToUpperInvariant

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

				Log(si, "Origins Found : " & origins.Count.ToString())

				For Each o As String In origins
					Log(si, "Origin = " & o)
				Next

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

#End Region

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

#End Region

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

#End Region

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
#End Region


		Private Class ExportContext

			Public Property ExportFile As String

			Public Property ExportTime As DateTime

			Public Property LineNumber As Integer

			Public Property Writer As StreamWriter

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

				ctx.ExportTime = DateTime.Now
				ctx.LineNumber = 0
				ctx.ExportFile = CreateExportFile(si, api)
				
				Log(si, "Export File = " & ctx.ExportFile)

				Log(si, "")
				Log(si, "=======================================")
				Log(si, "EXPORT")
				Log(si, "=======================================")

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

				FlushLog(si)

			Catch ex As Exception

				FlushLog(si)

				Throw ErrorHandler.LogWrite( _
					si, _
					New XFException(si, ex))

			End Try

		End Sub

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
			
		Private Shared Function GetMemberName( _
				ByVal api As FinanceRulesApi, _
				ByVal dimTypeId As Integer, _
				ByVal memberId As Integer) As String

			If memberId <= 0 Then
				Return ""
			End If

			Try

				Dim member As Member = api.Members.GetMember(dimTypeId, memberId)

				If member Is Nothing Then
					Return ""
				End If

				Return member.Name

			Catch

				Return ""

			End Try

		End Function
	
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

			Next

		End Sub

#End Region


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


#End Region

	End Class

End Namespace