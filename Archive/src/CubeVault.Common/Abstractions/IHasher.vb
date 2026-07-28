Option Strict On
Option Explicit On
Option Infer On

Namespace CubeVault.Common.Abstractions
    Public Interface IHasher
        Function ComputeSha256(value As String) As String
    End Interface
End Namespace
