Option Strict On
Option Explicit On
Option Infer On
Imports System.IO
Namespace CubeVault.Common.Services
Public Class FileChangeDetector
Implements Abstractions.IFileChangeDetector
Public Function HasChanged(path As String,lastWriteUtc As DateTime) As Boolean Implements Abstractions.IFileChangeDetector.HasChanged
If Not File.Exists(path) Then Return False
Return File.GetLastWriteTimeUtc(path)>lastWriteUtc
End Function
End Class
End Namespace
