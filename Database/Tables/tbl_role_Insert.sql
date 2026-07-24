INSERT INTO tbl_role
(
    RoleCode,
    RoleName,
    Description,
    DisplayOrder
)
VALUES
('ADMIN',   'Administrator', 'System Administrator', 1),
('OWNER',   'Owner',         'Property Owner',       2),
('MANAGER', 'Manager',       'Property Manager',     3),
('OPERATOR','Operator',      'Operations User',      4),
('TENANT',  'Tenant',        'Property Tenant',      5);