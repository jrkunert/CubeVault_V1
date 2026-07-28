Option Strict On
Option Explicit On
Option Infer On
Namespace CubeVault.Common.Abstractions
    Public Interface IFileMetadataService
        Function GetMetadata(path As String) As Models.FileMetadata
    End Interface
End Namespace
