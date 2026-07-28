Option Strict On
Option Explicit On
Option Infer On
Imports System.IO
Namespace CubeVault.Common.Services
    Public Class PathService
        Implements Abstractions.IPathService
        Public Function Normalize(path As String) As String Implements Abstractions.IPathService.Normalize
            Return Path.GetFullPath(path)
        End Function
    End Class
End Namespace
