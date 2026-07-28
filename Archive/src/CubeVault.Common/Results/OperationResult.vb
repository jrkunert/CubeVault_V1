Option Strict On
Option Explicit On
Option Infer On

Namespace CubeVault.Common.Results
    Public Class OperationResult
        Public Property Status As Enums.OperationStatus
        Public Property Message As String
        Public ReadOnly Property Succeeded As Boolean
            Get
                Return Status = Enums.OperationStatus.Success
            End Get
        End Property
    End Class
End Namespace
