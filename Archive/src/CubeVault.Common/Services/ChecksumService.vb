Option Strict On
Option Explicit On
Option Infer On
Imports System.IO
Namespace CubeVault.Common.Services
    Public Class ChecksumService
        Implements Abstractions.IChecksumService
        Public Function Compute(path As String) As Models.FileChecksum Implements Abstractions.IChecksumService.Compute
            Dim bytes = File.ReadAllBytes(path)
            Dim hash = New Security.Sha256Hasher().ComputeSha256(System.Text.Encoding.UTF8.GetString(bytes))
            Return New Models.FileChecksum With {.Algorithm="SHA-256", .Value=hash}
        End Function
    End Class
End Namespace
