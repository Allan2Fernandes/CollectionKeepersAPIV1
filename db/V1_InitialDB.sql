Use CollectionsDB;

GO

CREATE TABLE tbl_User(
    fld_UserID INT PRIMARY KEY IDENTITY,
    fld_Username NVARCHAR(30),
    fld_Password NVARCHAR(30),
    fld_Email NVARCHAR(50)
)

CREATE TABLE tbl_Collection(
    fld_CollectionID INT PRIMARY KEY IDENTITY,
    fld_UserID INT,
    fld_CollectionName NVARCHAR(30),
    fld_CollectionDescription NVARCHAR(100),
    fld_CollectionThumbnail NVARCHAR(500),
    fld_IsPrivate BIT,
    FOREIGN KEY (fld_UserID) REFERENCES tbl_User(fld_UserID)
)

CREATE TABLE tbl_CollectionEntry(
    fld_CollectionEntryID INT PRIMARY KEY IDENTITY
)

CREATE TABLE tbl_Attributes(
    fld_AttributeID INT PRIMARY KEY IDENTITY,
    fld_CollectionID INT,
    fld_AttributeName NVARCHAR(30),
    FOREIGN KEY (fld_CollectionID) REFERENCES tbl_Collection(fld_CollectionID)
)

CREATE TABLE tbl_AttributeValue(
    fld_AttributeValueID INT PRIMARY KEY IDENTITY,
    fld_AttributeID INT,
    fld_Value NVARCHAR(100),
    fld_CollectionEntryID INT,
    FOREIGN KEY (fld_CollectionEntryID) REFERENCES tbl_CollectionEntry (fld_CollectionEntryID),
    FOREIGN KEY (fld_AttributeID) REFERENCES tbl_Attributes(fld_AttributeID)
)

--Scaffold-DbContext "Server=10.176.88.54;database=CollectionsDB;user id=sa;password=EasvEasv123!;trusted_connection=true;TrustServerCertificate=True;integrated security=false;" -Provider Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models