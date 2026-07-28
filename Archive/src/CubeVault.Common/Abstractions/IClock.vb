Option Strict On
Option Explicit On
Option Infer On

Namespace CubeVault.Common.Abstractions
    Public Interface IClock
        ReadOnly Property UtcNow As DateTime
    End Interface
End Namespace
