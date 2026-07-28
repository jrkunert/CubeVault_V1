Option Strict On
Option Explicit On
Option Infer On
Namespace CubeVault.Common.Logging
    Public Class NullLogger
        Implements ILogger
        Public Sub LogError(message As String) Implements ILogger.LogError
        End Sub
        Public Sub LogInformation(message As String) Implements ILogger.LogInformation
        End Sub
    End Class
End Namespace
