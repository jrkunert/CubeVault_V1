Option Strict On
Option Explicit On
Option Infer On
Namespace CubeVault.Common.Abstractions
    Public Interface IPathService
        Function Normalize(path As String) As String
    End Interface
End Namespace
