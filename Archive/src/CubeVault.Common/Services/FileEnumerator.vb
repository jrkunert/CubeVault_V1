Option Strict On
Option Explicit On
Option Infer On
Imports System.IO

Namespace CubeVault.Common.Services
    Public Class FileEnumerator
        Implements Abstractions.IFileEnumerator

        Public Function EnumerateFiles(path As String, searchPattern As String) As IEnumerable(Of String) Implements Abstractions.IFileEnumerator.EnumerateFiles
            Return Directory.EnumerateFiles(path, searchPattern, SearchOption.TopDirectoryOnly)
        End Function
    End Class
End Namespace
