Option Strict On
Option Explicit On
Option Infer On
Imports System.Runtime.CompilerServices
Namespace CubeVault.Common.Extensions
    Public Module DateTimeExtensions
        <Extension>
        Public Function ToIso8601(value As DateTime) As String
            Return value.ToUniversalTime().ToString("o")
        End Function
    End Module
End Namespace
