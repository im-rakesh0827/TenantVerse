CREATE INDEX IX_tbl_Unit_PropertyId
ON dbo.tbl_Unit(PropertyId);
GO

CREATE INDEX IX_tbl_Unit_Status
ON dbo.tbl_Unit(Status);
GO

CREATE INDEX IX_tbl_Unit_PropertyId_Status
ON dbo.tbl_Unit(PropertyId, Status);
GO


CREATE UNIQUE INDEX UX_tbl_Unit_PropertyId_UnitNumber
ON dbo.tbl_Unit(PropertyId, UnitNumber)
WHERE IsDeleted = 0;
GO