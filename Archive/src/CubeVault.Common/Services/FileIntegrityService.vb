Option Strict On
Option Explicit On
Option Infer On
Namespace CubeVault.Common.Services
    Public Class FileIntegrityService
        Public Function Verify(expectedHash As String, actualHash As String) As Integrity.FileIntegrityResult
            Return New Integrity.FileIntegrityResult With {
                .IsValid = String.Equals(expectedHash, actualHash, StringComparison.OrdinalIgnoreCase),
                .ExpectedHash = expectedHash,
                .ActualHash = actualHash
            }
        End Function
    End Class
End Namespace
