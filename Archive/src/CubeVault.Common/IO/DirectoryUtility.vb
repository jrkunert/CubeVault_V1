Option Strict On
Option Explicit On
Option Infer On
Imports System.IO
Namespace CubeVault.Common.IO
    Public NotInheritable Class DirectoryUtility
        Private Sub New()
        End Sub
        Public Shared Sub EnsureExists(path As String)
            If Not Directory.Exists(path) Then Directory.CreateDirectory(path)
        End Sub
    End Class
End Namespace
