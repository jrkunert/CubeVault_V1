Option Strict On
Option Explicit On
Option Infer On
Namespace CubeVault.Common.Abstractions
    Public Interface IFileReader
        Function ReadAllText(path As String) As String
    End Interface
End Namespace
