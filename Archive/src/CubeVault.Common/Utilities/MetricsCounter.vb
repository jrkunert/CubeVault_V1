Option Strict On
Option Explicit On
Option Infer On
Namespace CubeVault.Common.Utilities
    Public Class MetricsCounter
        Public Property Name As String
        Public Property Value As Long
        Public Sub Increment()
            Value += 1
        End Sub
    End Class
End Namespace
