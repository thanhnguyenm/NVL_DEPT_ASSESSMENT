CREATE TABLE [dbo].[PeriodQuetions]
(
	[Id] INT NOT NULL PRIMARY KEY  Identity(1, 1),
	[AssessmentPeriodId] INT NOT NULL,
	[AssessmentQuestionId] INT NOT NULL,
	[Deleted] BIT NULL DEFAULT 0, 
    [CreatedBy] INT NULL, 
    [CreatedDate] DATETIME NULL, 
    [ModifiedBy] INT NULL, 
    [ModifiedDate] DATETIME NULL
)

GO
ALTER TABLE [dbo].[PeriodQuetions]
ADD CONSTRAINT FK_PeriodQuetions_Period
FOREIGN KEY ([AssessmentPeriodId]) REFERENCES [dbo].[AssessmentPeriods](Id);

GO
ALTER TABLE [dbo].[PeriodQuetions]
ADD CONSTRAINT FK_PeriodQuetions_Question
FOREIGN KEY ([AssessmentQuestionId]) REFERENCES [dbo].[AssessmentQuestions](Id);

Go


CREATE INDEX IX_PeriodQuetions_1
ON [PeriodQuetions] ([Deleted]);
Go