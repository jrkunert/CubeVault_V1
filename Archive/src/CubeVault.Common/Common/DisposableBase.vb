Option Strict On
Option Explicit On
Option Infer On
Namespace CubeVault.Common.Common
    Public MustInherit Class DisposableBase
        Implements IDisposable
        Private _disposed As Boolean
        Protected Overridable Sub Dispose(disposing As Boolean)
            _disposed=True
        End Sub
        Public Sub Dispose() Implements IDisposable.Dispose
            If Not _disposed Then
                Dispose(True)
                GC.SuppressFinalize(Me)
            End If
        End Sub
    End Class
End Namespace
