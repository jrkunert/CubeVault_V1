Option Strict On
Option Explicit On
Option Infer On

Namespace CubeVault.Common.Utilities
    Public NotInheritable Class VersionUtility
        Private Sub New()
        End Sub

        Public Shared Function Compare(leftVersion As String, rightVersion As String) As Integer
            Return Version.Parse(leftVersion).CompareTo(Version.Parse(rightVersion))
        End Function
    End Class
End Namespace
