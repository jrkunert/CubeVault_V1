Option Strict On
Option Explicit On
Option Infer On
Namespace CubeVault.Common.Logging
    Public Class StructuredLogger
        Implements ILogger
        Private ReadOnly _inner As ILogger
        Public Sub New(inner As ILogger)
            _inner = inner
        End Sub
        Public Sub LogInformation(message As String) Implements ILogger.LogInformation
            _inner.LogInformation("[INFO] " & message)
        End Sub
        Public Sub LogError(message As String) Implements ILogger.LogError
            _inner.LogError("[ERROR] " & message)
        End Sub
    End Class
End Namespace
