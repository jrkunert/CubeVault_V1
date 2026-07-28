Option Strict On
Option Explicit On
Option Infer On

Namespace CubeVault.Common.Abstractions
    Public Interface IFileOperations
        Sub Copy(sourcePath As String, destinationPath As String, overwrite As Boolean)
        Sub Move(sourcePath As String, destinationPath As String)
        Sub Delete(path As String)
    End Interface
End Namespace
