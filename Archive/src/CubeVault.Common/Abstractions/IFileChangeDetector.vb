Option Strict On
Option Explicit On
Option Infer On
Namespace CubeVault.Common.Abstractions
Public Interface IFileChangeDetector
Function HasChanged(path As String,lastWriteUtc As DateTime) As Boolean
End Interface
End Namespace
