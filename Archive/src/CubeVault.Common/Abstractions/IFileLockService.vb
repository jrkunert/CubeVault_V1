Option Strict On
Option Explicit On
Option Infer On
Imports System.IO
Namespace CubeVault.Common.Abstractions
    Public Interface IFileLockService
        Function OpenExclusive(path As String, mode As FileMode, access As FileAccess) As FileStream
    End Interface
End Namespace
