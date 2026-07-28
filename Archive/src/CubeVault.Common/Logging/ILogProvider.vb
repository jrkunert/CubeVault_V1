Option Strict On
Option Explicit On
Option Infer On
Namespace CubeVault.Common.Logging
    Public Interface ILogProvider
        Function CreateLogger(name As String) As ILogger
    End Interface
End Namespace
