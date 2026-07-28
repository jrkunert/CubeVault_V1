Option Strict On
Option Explicit On
Option Infer On

Namespace CubeVault.Common.Guards
    Public NotInheritable Class Guard
        Private Sub New()
        End Sub

        Public Shared Sub AgainstNull(Of T As Class)(value As T, parameterName As String)
            If value Is Nothing Then Throw New ArgumentNullException(parameterName)
        End Sub

        Public Shared Sub AgainstNullOrWhiteSpace(value As String, parameterName As String)
            If String.IsNullOrWhiteSpace(value) Then Throw New ArgumentException("Value cannot be null or whitespace.", parameterName)
        End Sub
    End Class
End Namespace
