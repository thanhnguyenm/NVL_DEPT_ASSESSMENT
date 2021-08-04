CREATE TABLE [dbo].[AssessmentPeriods]
(
	[Id] INT NOT NULL PRIMARY KEY  Identity(1, 1),
	[PeriodName] NVARCHAR(1000) NOT NULL,
	[PeriodFrom] Datetime NOT NULL Default GETDATE(),
	[PeriodTo] Datetime NOT NULL Default GETDATE(),
	[Published] BIT DEFAULT 0,
	[Note] NVARCHAR(MAX),
    [Deleted] BIT NULL DEFAULT 0, 
    [CreatedBy] INT NULL, 
    [CreatedDate] DATETIME NULL, 
    [ModifiedBy] INT NULL, 
    [ModifiedDate] DATETIME NULL
)
Go


CREATE INDEX IX_AssessmentPeriods_1
ON [AssessmentPeriods] ([PeriodName]);
Go

CREATE INDEX IX_AssessmentPeriods_2
ON [AssessmentPeriods] ([Deleted]);
Go

CREATE INDEX IX_AssessmentPeriods_3
ON [AssessmentPeriods] ([PeriodFrom], [PeriodTo]);
Go

CREATE INDEX IX_AssessmentPeriods_4
ON [AssessmentPeriods] ([Published]);
Go