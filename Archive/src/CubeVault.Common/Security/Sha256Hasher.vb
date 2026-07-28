Option Strict On
Option Explicit On
Option Infer On
Imports System.Security.Cryptography
Imports System.Text

Namespace CubeVault.Common.Security
    Public Class Sha256Hasher
        Implements Abstractions.IHasher
        Public Function ComputeSha256(value As String) As String Implements Abstractions.IHasher.ComputeSha256
            Using sha=SHA256.Create()
                Dim bytes=sha.ComputeHash(Encoding.UTF8.GetBytes(If(value,String.Empty)))
                Return BitConverter.ToString(bytes).Replace("-","").ToLowerInvariant()
            End Using
        End Function
    End Class
End Namespace
