Option Strict On
Option Explicit On
Option Infer On

Namespace CubeVault.Common.Abstractions
    Public Interface IFileEnumerator
        Function EnumerateFiles(path As String, searchPattern As String) As IEnumerable(Of String)
    End Interface
End Namespace
