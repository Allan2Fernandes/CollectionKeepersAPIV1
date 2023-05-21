Use CollectionsDB;

GO

ALTER TABLE tbl_AttributeValue
ALTER COLUMN fld_Value NVARCHAR(MAX)


--Scaffold-DbContext "Server=10.176.88.54;database=CollectionsDB;user id=sa;password=EasvEasv123!;trusted_connection=true;TrustServerCertificate=True;integrated security=false;" -Provider Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models