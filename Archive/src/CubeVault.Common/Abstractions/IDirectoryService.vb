Option Strict On
Option Explicit On
Option Infer On
Namespace CubeVault.Common.Abstractions
    Public Interface IDirectoryService
        Function Exists(path As String) As Boolean
        Sub Create(path As String)
    End Interface
End Namespace
