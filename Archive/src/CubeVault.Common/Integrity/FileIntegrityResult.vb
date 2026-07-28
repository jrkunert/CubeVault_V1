Option Strict On
Option Explicit On
Option Infer On
Namespace CubeVault.Common.Integrity
    Public Class FileIntegrityResult
        Public Property IsValid As Boolean
        Public Property ExpectedHash As String
        Public Property ActualHash As String
    End Class
End Namespace
