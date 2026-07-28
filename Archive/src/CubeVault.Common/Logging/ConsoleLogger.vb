Option Strict On
Option Explicit On
Option Infer On
Namespace CubeVault.Common.Logging
    Public Class ConsoleLogger
        Implements ILogger
        Public Sub LogError(message As String) Implements ILogger.LogError
            Console.Error.WriteLine(message)
        End Sub
        Public Sub LogInformation(message As String) Implements ILogger.LogInformation
            Console.WriteLine(message)
        End Sub
    End Class
End Namespace
