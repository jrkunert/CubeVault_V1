Option Strict On
Option Explicit On
Option Infer On

Namespace CubeVault.Common.Results
    Public Class Result(Of T)
        Public Property Success As Boolean
        Public Property Value As T
        Public Property ErrorMessage As String

        Public Shared Function Ok(value As T) As Result(Of T)
            Return New Result(Of T) With {.Success=True,.Value=value}
        End Function

        Public Shared Function Fail(message As String) As Result(Of T)
            Return New Result(Of T) With {.Success=False,.ErrorMessage=message}
        End Function
    End Class
End Namespace
