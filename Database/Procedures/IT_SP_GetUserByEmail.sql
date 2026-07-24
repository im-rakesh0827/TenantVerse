CREATE OR ALTER PROCEDURE IT_SP_GetUserByEmail
(
    @Email NVARCHAR(255)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        U.UserId,
        U.UserCode,
        U.FirstName,
        U.LastName,
        U.Email,
        U.PasswordHash,
        U.IsActive,
        U.IsDeleted,
        r.RoleName
    FROM tbl_users U
    JOIN tbl_user_role ur 
        ON ur.UserId = u.UserId
    JOIN tbl_role r 
        ON r.RoleId = ur.RoleId
    WHERE U.Email = @Email;
END
GO


