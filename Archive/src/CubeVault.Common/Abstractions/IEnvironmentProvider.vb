Option Strict On
Option Explicit On
Option Infer On
Namespace CubeVault.Common.Abstractions
    Public Interface IEnvironmentProvider
        ReadOnly Property MachineName As String
    End Interface
End Namespace
