Option Strict On
Option Explicit On
Option Infer On

Namespace CubeVault.Common.Abstractions
    Public Interface IFileSystem
        Function FileExists(path As String) As Boolean
    End Interface
End Namespace
