Option Strict On
Option Explicit On
Option Infer On
Namespace CubeVault.Common.Abstractions
    Public Interface IFileWriter
        Sub WriteAllText(path As String, contents As String)
    End Interface
End Namespace
