Option Strict On
Option Explicit On
Option Infer On
Imports System.IO
Namespace CubeVault.Common.Abstractions
    Public Interface ICompressionService
        Function Compress(input As Stream, output As Stream) As Long
        Function Decompress(input As Stream, output As Stream) As Long
    End Interface
End Namespace
