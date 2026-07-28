Option Strict On
Option Explicit On
Option Infer On

Namespace CubeVault.Common.Logging
    Public Class LogEntry
        Public Property TimestampUtc As DateTime
        Public Property Level As String
        Public Property Message As String
    End Class
End Namespace
