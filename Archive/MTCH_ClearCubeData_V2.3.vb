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
		Private Const BR_VERSION As String = "2.3.0"
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

		Private Shared Sub Export( _
			ByVal si As SessionInfo, _
			ByVal api As FinanceRulesApi, _
			ByVal args As FinanceRulesArgs)

			ExecuteExport(si, api, args)

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

		Private Shared Sub ExecuteExport( _
			ByVal si As SessionInfo, _
			ByVal api As FinanceRulesApi, _
			ByVal args As FinanceRulesArgs)

			Try

				InitializeLog()
			
				_exportLineNumber = 0
			
				Dim maxCells As Integer
				Dim batchSize As Integer
				Dim origins As New List(Of String)

				ReadParameters(api, args, maxCells, batchSize, origins)			
			
			Dim exportFile As String =
				CreateExportFile(si, api)

			Using sw As New StreamWriter( _
					exportFile, _
					False, _
					Encoding.UTF8)

				WriteCsvHeader(sw)

				Log(si, "Export File")
				Log(si, exportFile)
				Log(si, "")

				For Each originName As String In origins

					ExportOrigin( _
						si, _
						api, _
						originName, _
						sw)

				Next

			End Using

			Log(si, "Export File")
			Log(si, exportFile)
			Log(si, "")

				Log(si, "")
				Log(si, "====================================")
				Log(si, "Scenario  : " & api.Pov.Scenario.Name)
				Log(si, "Time      : " & api.Pov.Time.Name)
				Log(si, "Entity    : " & api.Pov.Entity.Name)
				Log(si, "MaxCells  : " & maxCells.ToString("#,##0"))
				Log(si, "BatchSize : " & batchSize.ToString("#,##0"))
				Log(si, "")
				Log(si, "EXPORT")
				Log(si, "====================================")

				Log(si, "")
				Log(si, "====================================")
				Log(si, "Export Complete")
				Log(si, "Rows Exported : " & _
					_exportLineNumber.ToString("#,##0"))
				Log(si, "====================================")

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

		Private Shared Sub ExportOrigin( _
			ByVal si As SessionInfo, _
			ByVal api As FinanceRulesApi, _
			ByVal originName As String, _
			ByVal sw As StreamWriter)

			Dim filterScript As String =
				BuildFilterScript(api, originName)

			Log(si, "")
			Log(si, "Origin : " & originName)

			ExportBuffer( _
				si, _
				api, _
				filterScript, _
				originName, _
				sw)

		End Sub

		Private Shared Sub ExportBuffer( _
			ByVal si As SessionInfo, _
			ByVal api As FinanceRulesApi, _
			ByVal filterScript As String, _
			ByVal originName As String, _
			ByVal sw As StreamWriter)
			
			Log(si, "Building DataBuffer...")

			Dim sourceBuffer As DataBuffer =
				GetSourceBuffer(api, filterScript)

			If sourceBuffer Is Nothing Then

				Log(si, "No DataBuffer Returned")

				Exit Sub

			End If

			If sourceBuffer.DataBufferCells Is Nothing Then

				Log(si, "No DataBufferCells")

				Exit Sub

			End If

			Log(si,
				"Exporting " &
				sourceBuffer.DataBufferCells.Count.ToString("#,##0") &
				" cells...")

			For Each cell As DataBufferCell In sourceBuffer.DataBufferCells.Values

				_exportLineNumber += 1

				WriteCsvRow( _
					sw, _
					_exportLineNumber, _
					api, _
					originName, _
					cell)

			Next

			Log(si, "Export Complete")

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

#Region "Export Helpers"

		Private Shared Function GetExportFolder( _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi) As String

			Dim root As String = _
				BRApi.Utilities.GetFileShareFolder( _
					si, _
					FileShareFolderTypes.BatchHarvest, _
					Nothing)

			Dim exportFolder As String = _
				Path.Combine( _
					root, _
					"..\..\Contents\Exports\" & _
					BR_NAME & "\" & _
					api.Pov.Scenario.Name & "\" & _
					api.Pov.Time.Name)

			exportFolder = Path.GetFullPath(exportFolder)

			If Not Directory.Exists(exportFolder) Then
				Directory.CreateDirectory(exportFolder)
			End If

			Return exportFolder

		End Function

		Private Shared Function CreateExportFile( _
				ByVal si As SessionInfo, _
				ByVal api As FinanceRulesApi) As String

			Return Path.Combine( _
				GetExportFolder(si, api), _
				"Export_" &
				DateTime.Now.ToString("yyyyMMdd_HHmmss") &
				".csv")

		End Function

		Private Shared Function GetMemberName( _
				ByVal api As FinanceRulesApi, _
				ByVal dimType As DimType, _
				ByVal memberId As Integer) As String

			If memberId <= 0 Then Return ""

			Return api.Members.GetMember(dimType.Id, memberId).Name

		End Function

		Private Shared Sub WriteCsvHeader( _
				ByVal sw As StreamWriter)

			sw.WriteLine( _
				"LineNo,Scenario,Time,Entity,Parent,Cons,View,Account,Origin,Flow,IC,UD1,UD2,UD3,UD4,UD5,UD6,UD7,UD8,Amount")

		End Sub

		Private Shared Sub WriteCsvRow( _
				ByVal sw As StreamWriter, _
				ByVal lineNumber As Integer, _
				ByVal api As FinanceRulesApi, _
				ByVal originName As String, _
				ByVal cell As DataBufferCell)

			Dim row As New List(Of String)

			row.Add(lineNumber.ToString())
			'row.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
			row.Add(api.Pov.Scenario.Name)
			row.Add(api.Pov.Time.Name)
			row.Add(api.Pov.Entity.Name)
			row.Add(api.Pov.Parent.Name)
			row.Add(api.Pov.Cons.Name)
			row.Add(api.Pov.View.Name)
			row.Add( _
				GetMemberName( _
					api, _
					DimType.Account, _
					cell.DataBufferCellPk.AccountId))
			row.Add(originName)
			'row.Add( _
			'	GetMemberName( _
			'		api, _
			'		DimType.Origin, _
			'		cell.DataBufferCellPk.OriginId))
			row.Add( _
				GetMemberName( _
					api, _
					DimType.Flow, _
					cell.DataBufferCellPk.FlowId))	
			row.Add( _
				GetMemberName( _
					api, _
					DimType.IC, _
					cell.DataBufferCellPk.ICId))
			row.Add( _
				GetMemberName( _
					api, _
					DimType.UD1, _
					cell.DataBufferCellPk.UD1Id))				
			row.Add( _
				GetMemberName( _
					api, _
					DimType.UD2, _
					cell.DataBufferCellPk.UD2Id))			
			row.Add( _
				GetMemberName( _
					api, _
					DimType.UD3, _
					cell.DataBufferCellPk.UD3Id))
			row.Add( _
				GetMemberName( _
					api, _
					DimType.UD4, _
					cell.DataBufferCellPk.UD4Id))
			row.Add( _
				GetMemberName( _
					api, _
					DimType.UD5, _
					cell.DataBufferCellPk.UD5Id))
			row.Add( _
				GetMemberName( _
					api, _
					DimType.UD6, _
					cell.DataBufferCellPk.UD6Id))
			row.Add( _
				GetMemberName( _
					api, _
					DimType.UD7, _
					cell.DataBufferCellPk.UD7Id))
			row.Add( _
				GetMemberName( _
					api, _
					DimType.UD8, _
					cell.DataBufferCellPk.UD8Id))


			row.Add( _
			cell.CellAmount.ToString( _
				System.Globalization.CultureInfo.InvariantCulture))
				
			sw.WriteLine(String.Join(",", row.ToArray()))

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