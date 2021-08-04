CREATE TABLE [dbo].[DepartmentMatrix]
(
	[Id] INT NOT NULL PRIMARY KEY  Identity(1, 1), 
    [DepartmentFrom] NVARCHAR(1000) NOT NULL,
	[DepartmentTo] NVARCHAR(1000) NOT NULL,
	[Interact] INT NULL, 
    [Note] NVARCHAR(1000) NULL,
    [Deleted] BIT NULL DEFAULT 0, 
    [CreatedBy] INT NULL, 
    [CreatedDate] DATETIME NULL, 
    [ModifiedBy] INT NULL, 
    [ModifiedDate] DATETIME NULL
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'1,2,3',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'DepartmentMatrix',
    @level2type = N'COLUMN',
    @level2name = N'Interact'

GO
ALTER TABLE [dbo].[DepartmentMatrix]
ADD CONSTRAINT UC_DepartmentMatrix UNIQUE ([DepartmentFrom],[DepartmentTo]);
Go


CREATE INDEX IX_DepartmentMatrix_1
ON [DepartmentMatrix] (DepartmentFrom);
Go

CREATE INDEX IX_DepartmentMatrix_2
ON [DepartmentMatrix] (DepartmentTo);
Go

CREATE INDEX IX_DepartmentMatrix_3
ON [DepartmentMatrix] ([Interact]);
Go

CREATE INDEX IX_DepartmentMatrix_4
ON [DepartmentMatrix] ([Deleted]);
Go