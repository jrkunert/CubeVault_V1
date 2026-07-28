Option Strict On
Option Explicit On
Option Infer On
Imports System.IO
Namespace CubeVault.Common.Services
    Public Class FileLockService
        Implements Abstractions.IFileLockService
        Public Function OpenExclusive(path As String, mode As FileMode, access As FileAccess) As FileStream Implements Abstractions.IFileLockService.OpenExclusive
            Return New FileStream(path, mode, access, FileShare.None)
        End Function
    End Class
End Namespace
