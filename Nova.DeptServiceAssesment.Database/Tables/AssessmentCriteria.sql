CREATE TABLE [dbo].[AssessmentCriteria]
(
	[Id] INT NOT NULL PRIMARY KEY Identity(1, 1), 
    [CriteriaName] NVARCHAR(1000) NOT NULL, 
    [Deleted] BIT NULL DEFAULT 0, 
    [CreatedBy] INT NULL, 
    [CreatedDate] DATETIME NULL, 
    [ModifiedBy] INT NULL, 
    [ModifiedDate] DATETIME NULL
)
Go

CREATE INDEX IX_AssessmentCriteria_1
ON [AssessmentCriteria] ([CriteriaName]);
Go

CREATE INDEX IX_AssessmentCriteria_2
ON [AssessmentCriteria] ([Deleted]);
Go