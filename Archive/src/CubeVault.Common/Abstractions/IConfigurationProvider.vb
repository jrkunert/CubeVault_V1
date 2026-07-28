Option Strict On
Option Explicit On
Option Infer On
Namespace CubeVault.Common.Abstractions
    Public Interface IConfigurationProvider
        Function GetValue(key As String) As String
    End Interface
End Namespace
