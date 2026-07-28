Option Strict On
Option Explicit On
Option Infer On

Namespace CubeVault.Domain.Application

    ''' <summary>
    ''' Represents a registered OneStream application.
    ''' </summary>
    Public NotInheritable Class Application

        Public Sub New(
            applicationId As Guid,
            name As String,
            description As String,
            createdBy As String)

            If applicationId = Guid.Empty Then
                Throw New ArgumentException(
                    "ApplicationId cannot be empty.",
                    NameOf(applicationId))
            End If

            If String.IsNullOrWhiteSpace(name) Then
                Throw New ArgumentException(
                    "Application name is required.",
                    NameOf(name))
            End If

            Me.ApplicationId = applicationId
            Me.Name = name.Trim()
            Me.Description = If(description, String.Empty).Trim()
            Me.CreatedBy = createdBy

            Me.CreatedUtc = DateTime.UtcNow
            Me.IsActive = True

        End Sub

        Public ReadOnly Property ApplicationId As Guid

        Public Property Name As String

        Public Property Description As String

        Public Property IsActive As Boolean

        Public Property CreatedUtc As DateTime

        Public Property CreatedBy As String

        Public Property ModifiedUtc As DateTime?

        Public Property ModifiedBy As String

    End Class

End Namespace