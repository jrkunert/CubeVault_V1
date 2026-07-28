Option Strict On
Option Explicit On
Option Infer On
Namespace CubeVault.Common.Abstractions
    Public Interface IChecksumService
        Function Compute(path As String) As Models.FileChecksum
    End Interface
End Namespace
