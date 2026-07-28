Option Strict On
Option Explicit On
Option Infer On
Imports System.IO
Namespace CubeVault.Common.Utilities
Public NotInheritable Class DirectorySynchronizer
Private Sub New()
End Sub
Public Shared Sub CopyFiles(source As String,destination As String)
Directory.CreateDirectory(destination)
For Each f In Directory.GetFiles(source)
File.Copy(f,Path.Combine(destination,Path.GetFileName(f)),True)
Next
End Sub
End Class
End Namespace
