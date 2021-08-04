CREATE TABLE [dbo].[Departments]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[ShortCode] NVARCHAR(1000) NULL,
	[Code] NVARCHAR(1000) NULL,
	[Name] NVARCHAR(1000) NULL,
	[Type] NVARCHAR(5) NULL,
	[DivCode] NVARCHAR(1000) NULL,
	[DivName] NVARCHAR(1000) NULL,
	[EmailHead] NVARCHAR(1000) NULL,
	[Deleted] BIT NULL DEFAULT 0, 
    [CreatedBy] INT NULL, 
    [CreatedDate] DATETIME NULL, 
    [ModifiedBy] INT NULL, 
    [ModifiedDate] DATETIME NULL
)
Go


CREATE INDEX IX_Departments_1
ON [Departments] (ShortCode);
Go

CREATE INDEX IX_Departments_2
ON [Departments] (Code);
Go

CREATE INDEX IX_Departments_3
ON [Departments] ([Type]);
Go

CREATE INDEX IX_Departments_4
ON [Departments] ([EmailHead]);
Go

CREATE INDEX IX_Departments_5
ON [Departments] ([Deleted]);
Go