Option Strict On
Option Explicit On
Option Infer On
Imports System.IO
Namespace CubeVault.Common.Services
Public Class ManifestService
Public Function CreateManifest(folder As String) As IEnumerable(Of String)
Return Directory.EnumerateFiles(folder,"*",SearchOption.AllDirectories)
End Function
End Class
End Namespace
