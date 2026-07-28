Option Strict On
Option Explicit On
Option Infer On
Namespace CubeVault.Common.Validation
    Public NotInheritable Class ConfigurationValidator
        Private Sub New()
        End Sub
        Public Shared Sub RequireValue(key As String, value As String)
            If String.IsNullOrWhiteSpace(value) Then
                Throw New ArgumentException("Missing configuration value: " & key, NameOf(value))
            End If
        End Sub
    End Class
End Namespace
