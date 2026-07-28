Option Strict On
Option Explicit On
Option Infer On
Imports System.IO

Namespace CubeVault.Common.Services
    Public Class FileOperations
        Implements Abstractions.IFileOperations

        Public Sub Copy(sourcePath As String, destinationPath As String, overwrite As Boolean) Implements Abstractions.IFileOperations.Copy
            File.Copy(sourcePath, destinationPath, overwrite)
        End Sub

        Public Sub Move(sourcePath As String, destinationPath As String) Implements Abstractions.IFileOperations.Move
            File.Move(sourcePath, destinationPath)
        End Sub

        Public Sub Delete(path As String) Implements Abstractions.IFileOperations.Delete
            If File.Exists(path) Then File.Delete(path)
        End Sub
    End Class
End Namespace
