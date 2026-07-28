Option Strict On
Option Explicit On
Option Infer On
Imports System.Web.Script.Serialization

Namespace CubeVault.Common.Serialization
    Public Class SystemTextJsonSerializer
        Implements Abstractions.IJsonSerializer
        Private ReadOnly _serializer As New JavaScriptSerializer()
        Public Function Serialize(Of T)(value As T) As String Implements Abstractions.IJsonSerializer.Serialize
            Return _serializer.Serialize(value)
        End Function
    End Class
End Namespace
