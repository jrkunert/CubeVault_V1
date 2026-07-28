Option Strict On
Option Explicit On
Option Infer On
Imports System.IO
Namespace CubeVault.Common.Utilities
    Public NotInheritable Class AtomicFileWriter
        Private Sub New()
        End Sub
        Public Shared Sub WriteAllText(path As String, contents As String)
            Dim temp = path & ".tmp"
            File.WriteAllText(temp, contents)
            If File.Exists(path) Then
                File.Replace(temp, path, Nothing)
            Else
                File.Move(temp, path)
            End If
        End Sub
    End Class
End Namespace
