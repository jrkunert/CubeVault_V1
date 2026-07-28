Option Strict On
Option Explicit On
Option Infer On
Imports System.IO
Namespace CubeVault.Common.Services
    Public Class DirectoryService
        Implements Abstractions.IDirectoryService
        Public Function Exists(path As String) As Boolean Implements Abstractions.IDirectoryService.Exists
            Return Directory.Exists(path)
        End Function
        Public Sub Create(path As String) Implements Abstractions.IDirectoryService.Create
            If Not Directory.Exists(path) Then Directory.CreateDirectory(path)
        End Sub
    End Class
End Namespace
