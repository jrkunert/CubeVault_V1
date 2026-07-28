/******************************************************************************
    CubeVault
    File      : cv.Application.sql
    Version   : 0.1.0
    Schema    : cv
    Purpose   : Stores registered OneStream applications.
******************************************************************************/

CREATE TABLE cv.Application
(
    ApplicationId UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_Application
        PRIMARY KEY CLUSTERED,

    Name NVARCHAR(100) NOT NULL,

    Description NVARCHAR(500) NULL,

    IsActive BIT NOT NULL
        CONSTRAINT DF_Application_IsActive
        DEFAULT (1),

    CreatedUtc DATETIME2(3) NOT NULL
        CONSTRAINT DF_Application_CreatedUtc
        DEFAULT (SYSUTCDATETIME()),

    CreatedBy NVARCHAR(128) NOT NULL,

    ModifiedUtc DATETIME2(3) NULL,

    ModifiedBy NVARCHAR(128) NULL,

    RowVersion ROWVERSION NOT NULL,

    CONSTRAINT CK_Application_Name
        CHECK (LEN(LTRIM(RTRIM(Name))) > 0)
);
GO

CREATE UNIQUE INDEX IX_Application_Name
ON cv.Application(Name);
GO

EXEC sys.sp_addextendedproperty
    @name = N'MS_Description',
    @value = N'Registered OneStream applications managed by CubeVault.',
    @level0type = N'SCHEMA',
    @level0name = N'cv',
    @level1type = N'TABLE',
    @level1name = N'Application';
GO