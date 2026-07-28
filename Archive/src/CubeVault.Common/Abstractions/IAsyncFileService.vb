Option Strict On
Option Explicit On
Option Infer On
Imports System.Threading
Imports System.Threading.Tasks
Namespace CubeVault.Common.Abstractions
    Public Interface IAsyncFileService
        Function ReadAllTextAsync(path As String, cancellationToken As CancellationToken) As Task(Of String)
        Function WriteAllTextAsync(path As String, contents As String, cancellationToken As CancellationToken) As Task
    End Interface
End Namespace
