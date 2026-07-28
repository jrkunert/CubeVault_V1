Option Strict On
Option Explicit On
Option Infer On
Namespace CubeVault.Common.Services
    Public Class EnvironmentProvider
        Implements Abstractions.IEnvironmentProvider
        Public ReadOnly Property MachineName As String Implements Abstractions.IEnvironmentProvider.MachineName
            Get
                Return Environment.MachineName
            End Get
        End Property
    End Class
End Namespace
