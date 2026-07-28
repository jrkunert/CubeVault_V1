Option Strict On
Option Explicit On
Option Infer On
Imports System.Runtime.CompilerServices

Namespace CubeVault.Common.Extensions
    Public Module StringExtensions
        <Extension>
        Public Function IsNullOrWhiteSpace(value As String) As Boolean
            Return String.IsNullOrWhiteSpace(value)
        End Function
    End Module
End Namespace
