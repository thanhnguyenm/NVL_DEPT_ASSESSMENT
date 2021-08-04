CREATE TABLE [dbo].[Users]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
	[OrgUserCode] NVARCHAR(5) NOT NULL,
    [FullName] NVARCHAR(1000) NOT NULL,
	[OrgUserName] NVARCHAR(1000) NOT NULL,
	[DepartmentCode] INT NOT NULL DEFAULT 0,
	[JobTitle] NVARCHAR(1000) NULL,
	[Gender] VARCHAR(1) NULL,
	[Email] NVARCHAR(500) NOT NULL,
	[JobLevel] INT,
	[DateOfBirth] DATETIME,
	[CompanyCode] NVARCHAR(50) NULL,
	[LocationCode] NVARCHAR(1000) NULL,
	[PhoneNumber] NVARCHAR(20) NULL,
	[IsManager] NVARCHAR(5) NULL,
	[Deleted] BIT NULL DEFAULT 0, 
    [CreatedBy] INT NULL, 
    [CreatedDate] DATETIME NULL, 
    [ModifiedBy] INT NULL, 
    [ModifiedDate] DATETIME NULL
)

Go


CREATE INDEX IX_Users_1
ON [Users] ([OrgUserCode]);
Go

CREATE INDEX IX_Users_2
ON [Users] ([JobLevel]);
Go

CREATE INDEX IX_Users_3
ON [Users] ([DepartmentCode]);
Go

CREATE INDEX IX_Users_4
ON [Users] ([Email]);
Go

CREATE INDEX IX_Users_5
ON [Users] ([Deleted]);
Go
