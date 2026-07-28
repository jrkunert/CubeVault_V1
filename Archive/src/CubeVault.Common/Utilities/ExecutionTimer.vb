Option Strict On
Option Explicit On
Option Infer On
Imports System.Diagnostics
Namespace CubeVault.Common.Utilities
    Public NotInheritable Class ExecutionTimer
        Private Sub New()
        End Sub
        Public Shared Function Measure(action As Action) As TimeSpan
            Dim sw = Stopwatch.StartNew()
            action()
            sw.Stop()
            Return sw.Elapsed
        End Function
    End Class
End Namespace
