Option Strict On
Option Explicit On
Option Infer On
Imports System.Runtime.CompilerServices
Namespace CubeVault.Common.Diagnostics
Public Module ExceptionExtensions
<Extension>
Public Function FullMessage(ex As Exception) As String
Return ex.Message & If(ex.InnerException Is Nothing,""," --> " & ex.InnerException.Message)
End Function
End Module
End Namespace
