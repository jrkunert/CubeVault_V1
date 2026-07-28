Option Strict On
Option Explicit On
Option Infer On

Namespace CubeVault.Common.Utilities
    Public NotInheritable Class PathUtility
        Private Sub New()
        End Sub

        Public Shared Function Combine(ParamArray parts() As String) As String
            Return IO.Path.Combine(parts)
        End Function
    End Class
End Namespace
