USE [BlobShareDataStore]
GO

CREATE USER [BlobShare] FOR LOGIN [BlobShare]
GO

EXEC sp_addrolemember 'db_owner', 'BlobShare'
GO