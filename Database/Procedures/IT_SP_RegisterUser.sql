CREATE OR ALTER PROCEDURE IT_SP_RegisterUser
(
      @FirstName        NVARCHAR(100),
      @LastName         NVARCHAR(100) = NULL,
      @Email            NVARCHAR(200),
      @PhoneNumber      NVARCHAR(20) = NULL,
      @PasswordHash     NVARCHAR(MAX)
)
AS
BEGIN

    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY

        BEGIN TRANSACTION;

        ------------------------------------------------------------
        -- Email Validation
        ------------------------------------------------------------

        IF EXISTS
        (
            SELECT 1
            FROM tbl_users
            WHERE Email = @Email
            AND IsDeleted = 0
        )
        BEGIN

            THROW 50001, 'Email already exists.', 1;

        END

        ------------------------------------------------------------
        -- Generate User Code
        ------------------------------------------------------------

        DECLARE @Sequence INT;

        SET @Sequence = NEXT VALUE FOR seq_user_code;

        DECLARE @UserCode NVARCHAR(50);

        SET @UserCode =
            CONCAT
            (
                'USR',
                RIGHT('000000' + CAST(@Sequence AS VARCHAR(6)), 6)
            );

        ------------------------------------------------------------
        -- Insert User
        ------------------------------------------------------------

        INSERT INTO tbl_users
        (
            UserCode,
            FirstName,
            LastName,
            Email,
            PhoneNumber,
            PasswordHash
        )
        VALUES
        (
            @UserCode,
            @FirstName,
            @LastName,
            @Email,
            @PhoneNumber,
            @PasswordHash
        );

        ------------------------------------------------------------
        -- Get UserId
        ------------------------------------------------------------

        DECLARE @UserId INT;

        SET @UserId = SCOPE_IDENTITY();

        ------------------------------------------------------------
        -- Get Default Role
        ------------------------------------------------------------

        DECLARE @RoleId INT;

        SELECT
            @RoleId = RoleId
        FROM
            tbl_role
        WHERE
            RoleCode = 'OWNER'
            AND IsDeleted = 0
            AND IsActive = 1;

        IF @RoleId IS NULL
        BEGIN

            THROW 50002, 'Default OWNER role not found.', 1;

        END

        ------------------------------------------------------------
        -- Assign Role
        ------------------------------------------------------------

        INSERT INTO tbl_user_role
        (
            UserId,
            RoleId
        )
        VALUES
        (
            @UserId,
            @RoleId
        );

        ------------------------------------------------------------
        -- Commit
        ------------------------------------------------------------

        COMMIT TRANSACTION;

        ------------------------------------------------------------
        -- Return Success
        ------------------------------------------------------------

        SELECT
            CAST(1 AS BIT) AS IsSuccess,
            @UserId AS UserId,
            @UserCode AS UserCode,
            'User registered successfully.' AS Message;

    END TRY

    BEGIN CATCH

        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        SELECT
            CAST(0 AS BIT) AS IsSuccess,
            ERROR_NUMBER() AS ErrorNumber,
            ERROR_MESSAGE() AS Message;

    END CATCH

END
GO