Option Strict On
Option Explicit On
Option Infer On

Imports CubeVault.Domain.Application

Namespace CubeVault.Application.Repositories

    ''' <summary>
    ''' Repository used to manage registered applications.
    ''' </summary>
    Public Interface IApplicationRepository

        Function GetById(
            applicationId As Guid) As Application

        Function GetByName(
            name As String) As Application

        Function Exists(
            name As String) As Boolean

        Sub Register(
            application As Application)

        Sub Update(
            application As Application)

        Sub Delete(
            applicationId As Guid)

    End Interface

End Namespace