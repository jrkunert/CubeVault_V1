Option Strict On
Option Explicit On
Option Infer On
Imports System.IO
Imports System.Threading
Imports System.Threading.Tasks
Namespace CubeVault.Common.Services
    Public Class AsyncFileService
        Implements Abstractions.IAsyncFileService
        Public Async Function ReadAllTextAsync(path As String, cancellationToken As CancellationToken) As Task(Of String) Implements Abstractions.IAsyncFileService.ReadAllTextAsync
            Using sr As New StreamReader(path)
                cancellationToken.ThrowIfCancellationRequested()
                Return Await sr.ReadToEndAsync().ConfigureAwait(False)
            End Using
        End Function
        Public Async Function WriteAllTextAsync(path As String, contents As String, cancellationToken As CancellationToken) As Task Implements Abstractions.IAsyncFileService.WriteAllTextAsync
            cancellationToken.ThrowIfCancellationRequested()
            Using sw As New StreamWriter(path, False)
                Await sw.WriteAsync(contents).ConfigureAwait(False)
            End Using
        End Function
    End Class
End Namespace
