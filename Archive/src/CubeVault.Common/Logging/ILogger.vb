Option Strict On
Option Explicit On
Option Infer On
Namespace CubeVault.Common.Logging
    Public Interface ILogger
        Sub LogInformation(message As String)
        Sub LogError(message As String)
    End Interface
End Namespace
