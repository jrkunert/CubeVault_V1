Option Strict On
Option Explicit On
Option Infer On
Imports System.IO
Namespace CubeVault.Common.Services
    Public Class FileMetadataService
        Implements Abstractions.IFileMetadataService
        Public Function GetMetadata(path As String) As Models.FileMetadata Implements Abstractions.IFileMetadataService.GetMetadata
            Dim fi As New FileInfo(path)
            Return New Models.FileMetadata With {
                .FullName = fi.FullName,
                .Length = fi.Length,
                .LastWriteUtc = fi.LastWriteTimeUtc
            }
        End Function
    End Class
End Namespace
