Option Strict On
Option Explicit On
Option Infer On
Namespace CubeVault.Common.Abstractions
    Public Interface IJsonSerializer
        Function Serialize(Of T)(value As T) As String
    End Interface
End Namespace
