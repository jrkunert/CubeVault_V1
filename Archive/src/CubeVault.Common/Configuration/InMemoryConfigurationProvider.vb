Option Strict On
Option Explicit On
Option Infer On
Imports System.Collections.Generic
Namespace CubeVault.Common.Configuration
    Public Class InMemoryConfigurationProvider
        Implements Abstractions.IConfigurationProvider
        Private ReadOnly _values As IDictionary(Of String,String)
        Public Sub New(values As IDictionary(Of String,String))
            _values=values
        End Sub
        Public Function GetValue(key As String) As String Implements Abstractions.IConfigurationProvider.GetValue
            If _values.ContainsKey(key) Then Return _values(key)
            Return Nothing
        End Function
    End Class
End Namespace
