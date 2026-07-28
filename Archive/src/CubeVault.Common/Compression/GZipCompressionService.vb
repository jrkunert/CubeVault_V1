Option Strict On
Option Explicit On
Option Infer On
Imports System.IO
Imports System.IO.Compression
Namespace CubeVault.Common.Compression
    Public Class GZipCompressionService
        Implements Abstractions.ICompressionService
        Public Function Compress(input As Stream, output As Stream) As Long Implements Abstractions.ICompressionService.Compress
            Using gz As New GZipStream(output, CompressionMode.Compress, True)
                input.CopyTo(gz)
            End Using
            Return output.Position
        End Function
        Public Function Decompress(input As Stream, output As Stream) As Long Implements Abstractions.ICompressionService.Decompress
            Using gz As New GZipStream(input, CompressionMode.Decompress, True)
                gz.CopyTo(output)
            End Using
            Return output.Position
        End Function
    End Class
End Namespace
