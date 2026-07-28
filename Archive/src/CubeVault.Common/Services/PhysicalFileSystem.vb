Option Strict On
Option Explicit On
Option Infer On
Namespace CubeVault.Common.Services
    Public Class PhysicalFileSystem
        Implements Abstractions.IFileSystem
        Public Function FileExists(path As String) As Boolean Implements Abstractions.IFileSystem.FileExists
            Return IO.File.Exists(path)
        End Function
    End Class
End Namespace
