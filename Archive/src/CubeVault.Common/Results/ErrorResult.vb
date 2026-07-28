Option Strict On
Option Explicit On
Option Infer On
Namespace CubeVault.Common.Results
    Public Class ErrorResult
        Inherits OperationResult
        Public Sub New(message As String)
            Status = Enums.OperationStatus.Failed
            Me.Message = message
        End Sub
    End Class
End Namespace
