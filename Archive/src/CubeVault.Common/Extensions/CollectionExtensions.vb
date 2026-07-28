Option Strict On
Option Explicit On
Option Infer On
Imports System.Runtime.CompilerServices
Namespace CubeVault.Common.Extensions
    Public Module CollectionExtensions
        <Extension>
        Public Function IsNullOrEmpty(Of T)(source As ICollection(Of T)) As Boolean
            Return source Is Nothing OrElse source.Count=0
        End Function
    End Module
End Namespace
