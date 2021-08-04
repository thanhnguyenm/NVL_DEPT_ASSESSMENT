CREATE TABLE [dbo].[Permissions]
(
	[Id] INT NOT NULL PRIMARY KEY Identity(1,1),
	[Email] NVARCHAR(1000) UNIQUE
)

Go


CREATE INDEX IX_Permission_1
ON [Permissions] ([Email]);
Go
