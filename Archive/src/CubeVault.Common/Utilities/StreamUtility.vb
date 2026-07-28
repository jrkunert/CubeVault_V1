Option Strict On
Option Explicit On
Option Infer On
Imports System.IO
Namespace CubeVault.Common.Utilities
    Public NotInheritable Class StreamUtility
        Private Sub New()
        End Sub
        Public Shared Function Copy(input As Stream, output As Stream) As Long
            input.CopyTo(output)
            Return output.Position
        End Function
    End Class
End Namespace
