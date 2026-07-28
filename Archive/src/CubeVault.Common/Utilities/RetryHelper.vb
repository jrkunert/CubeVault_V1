Option Strict On
Option Explicit On
Option Infer On
Namespace CubeVault.Common.Utilities
    Public NotInheritable Class RetryHelper
        Private Sub New()
        End Sub
        Public Shared Sub Execute(action As Action, attempts As Integer)
            If attempts < 1 Then Throw New ArgumentOutOfRangeException(NameOf(attempts))
            Dim last As Exception = Nothing
            For i=1 To attempts
                Try
                    action()
                    Return
                Catch ex As Exception
                    last = ex
                End Try
            Next
            Throw last
        End Sub
    End Class
End Namespace
