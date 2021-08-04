CREATE TABLE [dbo].[PeriodSelectedDepartments]
(
	[Id] INT NOT NULL PRIMARY KEY  Identity(1, 1),
    [AssessmentPeriodId] INT NOT NULL,
    [DepartmentFrom] INT NOT NULL,
    [UserId]  INT NOT NULL,
    [DepartmentTo] INT NOT NULL,
    [Finished] BIT NOT NULL DEFAULT 0,
	[Deleted] BIT NULL DEFAULT 0, 
    [CreatedBy] INT NULL, 
    [CreatedDate] DATETIME NULL, 
    [ModifiedBy] INT NULL, 
    [ModifiedDate] DATETIME NULL
)

GO
ALTER TABLE [dbo].[PeriodSelectedDepartments]
ADD CONSTRAINT FK_PeriodSelectedDepartments_Period
FOREIGN KEY ([AssessmentPeriodId]) REFERENCES [dbo].[AssessmentPeriods](Id);

GO
ALTER TABLE [dbo].[PeriodSelectedDepartments]
ADD CONSTRAINT UC_PeriodSelectedDepartments UNIQUE ([AssessmentPeriodId],[DepartmentFrom],[UserId],[DepartmentTo]);

Go


CREATE INDEX IX_PeriodSelectedDepartments_1
ON [PeriodSelectedDepartments] ([DepartmentFrom]);
Go

CREATE INDEX IX_PeriodSelectedDepartments_2
ON [PeriodSelectedDepartments] ([DepartmentFrom]);
Go

CREATE INDEX IX_PeriodSelectedDepartments_3
ON [PeriodSelectedDepartments] ([UserId]);
Go

CREATE INDEX IX_PeriodSelectedDepartments_4
ON [PeriodSelectedDepartments] ([Finished]);
Go
