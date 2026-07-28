Option Strict On
Option Explicit On
Option Infer On
Imports System.IO
Namespace CubeVault.Common.Services
    Public Class FileService
        Implements Abstractions.IFileReader, Abstractions.IFileWriter
        Public Function ReadAllText(path As String) As String Implements Abstractions.IFileReader.ReadAllText
            Return File.ReadAllText(path)
        End Function
        Public Sub WriteAllText(path As String, contents As String) Implements Abstractions.IFileWriter.WriteAllText
            File.WriteAllText(path, contents)
        End Sub
    End Class
End Namespace
