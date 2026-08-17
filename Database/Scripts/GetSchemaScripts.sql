SELECT
    c.column_id,
    c.name AS ColumnName,
    t.name AS DataType,
    c.max_length,
    c.precision,
    c.scale,
    c.is_nullable,
    dc.definition AS DefaultValue
FROM sys.columns c
INNER JOIN sys.types t
    ON c.user_type_id = t.user_type_id
LEFT JOIN sys.default_constraints dc
    ON c.default_object_id = dc.object_id
WHERE c.object_id = OBJECT_ID('dbo.tbl_Property')
ORDER BY c.column_id;