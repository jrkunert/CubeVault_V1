Option Strict On
Option Explicit On
Option Infer On

Namespace CubeVault.Common.Exceptions
    Public Class CubeVaultException
        Inherits Exception
        Public Sub New(message As String)
            MyBase.New(message)
        End Sub
    End Class
End Namespace
