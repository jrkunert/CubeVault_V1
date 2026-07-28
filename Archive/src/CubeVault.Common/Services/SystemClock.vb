Option Strict On
Option Explicit On
Option Infer On

Namespace CubeVault.Common.Services
    Public Class SystemClock
        Implements Abstractions.IClock

        Public ReadOnly Property UtcNow As Date Implements Abstractions.IClock.UtcNow
            Get
                Return Date.UtcNow
            End Get
        End Property
    End Class
End Namespace
