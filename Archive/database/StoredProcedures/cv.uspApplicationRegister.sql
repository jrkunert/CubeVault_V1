/******************************************************************************
    CubeVault
    Procedure : cv.uspApplicationRegister
    Version   : 0.1.0
    Purpose   : Registers a OneStream application.
******************************************************************************/

CREATE OR ALTER PROCEDURE cv.uspApplicationRegister
(
      @ApplicationId UNIQUEIDENTIFIER
    , @Name          NVARCHAR(100)
    , @Description   NVARCHAR(500) = NULL
    , @CreatedBy     NVARCHAR(128)
)
AS
BEGIN

    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    --------------------------------------------------------------------------
    -- Validation
    --------------------------------------------------------------------------

    IF @ApplicationId IS NULL
        THROW 50001, 'ApplicationId is required.', 1;

    IF NULLIF(LTRIM(RTRIM(@Name)), '') IS NULL
        THROW 50002, 'Application name is required.', 1;

    IF EXISTS
    (
        SELECT 1
        FROM cv.Application
        WHERE Name = @Name
    )
        THROW 50003, 'Application already exists.', 1;

    --------------------------------------------------------------------------
    -- Insert
    --------------------------------------------------------------------------

    INSERT INTO cv.Application
    (
          ApplicationId
        , Name
        , Description
        , CreatedUtc
        , CreatedBy
        , IsActive
    )
    VALUES
    (
          @ApplicationId
        , LTRIM(RTRIM(@Name))
        , @Description
        , SYSUTCDATETIME()
        , @CreatedBy
        , 1
    );

END
GO