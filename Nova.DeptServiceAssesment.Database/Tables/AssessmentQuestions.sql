CREATE TABLE [dbo].[AssessmentQuestions]
(
	[Id] INT NOT NULL PRIMARY KEY  Identity(1, 1),
    [CriteriaId] INT NOT NULL,
    [Content] NVARCHAR(MAX) NOT NULL,
	[Deleted] BIT NULL DEFAULT 0, 
    [CreatedBy] INT NULL, 
    [CreatedDate] DATETIME NULL, 
    [ModifiedBy] INT NULL, 
    [ModifiedDate] DATETIME NULL
)
GO

ALTER TABLE [dbo].[AssessmentQuestions]
ADD CONSTRAINT FK_AssessmentQuestions_AssessmentCriteria
FOREIGN KEY ([CriteriaId]) REFERENCES [dbo].[AssessmentCriteria](Id);
Go


CREATE INDEX IX_AssessmentQuestions_1
ON [AssessmentPeriods] ([Deleted]);
Go
