IF EXISTS (SELECT name FROM [master].dbo.sysdatabases WHERE name = 'BlobShareDataStore')
  ALTER DATABASE [BlobShareDataStore] SET SINGLE_USER WITH ROLLBACK IMMEDIATE 
GO

IF EXISTS (SELECT name FROM [master].dbo.sysdatabases WHERE name = 'BlobShareDataStore')
  DROP DATABASE [BlobShareDataStore]
GO

CREATE DATABASE [BlobShareDataStore]
GO