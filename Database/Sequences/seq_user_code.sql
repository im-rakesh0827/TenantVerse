IF NOT EXISTS
(
    SELECT *
    FROM sys.sequences
    WHERE name = 'seq_user_code'
)
BEGIN

    CREATE SEQUENCE seq_user_code
        AS INT
        START WITH 1
        INCREMENT BY 1
        MINVALUE 1
        NO MAXVALUE
        CACHE 100;

END
GO