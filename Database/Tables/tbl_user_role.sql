IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'tbl_user_role')
BEGIN

    CREATE TABLE tbl_user_role
    (
        UserRoleId INT IDENTITY(1,1) PRIMARY KEY,

        UserId INT NOT NULL,

        RoleId INT NOT NULL,

        IsPrimaryRole BIT NOT NULL
            CONSTRAINT DF_tbl_user_role_IsPrimaryRole DEFAULT(1),

        CreatedBy NVARCHAR(100) NOT NULL
            CONSTRAINT DF_tbl_user_role_CreatedBy DEFAULT('SYSTEM'),

        CreatedDate DATETIME NOT NULL
            CONSTRAINT DF_tbl_user_role_CreatedDate DEFAULT(GETDATE()),

        UpdatedBy NVARCHAR(100) NULL,

        UpdatedDate DATETIME NULL,

        IsDeleted BIT NOT NULL
            CONSTRAINT DF_tbl_user_role_IsDeleted DEFAULT(0),

        CONSTRAINT FK_tbl_user_role_User
            FOREIGN KEY(UserId)
            REFERENCES tbl_users(UserId),

        CONSTRAINT FK_tbl_user_role_Role
            FOREIGN KEY(RoleId)
            REFERENCES tbl_role(RoleId)

    );

END
GO