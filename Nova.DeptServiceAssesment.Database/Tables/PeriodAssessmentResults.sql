CREATE TABLE [dbo].[PeriodAssessmentResults]
(
	[Id] INT NOT NULL PRIMARY KEY Identity(1,1),
	[PeriodSelectedDepartmentId] INT NOT NULL,
	[PeriodQuestionId] INT NOT NULL,
    [Result]  INT NULL,
    [ResultComment] NVARCHAR(1000),
	[Deleted] BIT NULL DEFAULT 0, 
    [CreatedBy] INT NULL, 
    [CreatedDate] DATETIME NULL, 
    [ModifiedBy] INT NULL, 
    [ModifiedDate] DATETIME NULL
)

GO
ALTER TABLE [dbo].[PeriodAssessmentResults]
ADD CONSTRAINT FK_PeriodAssessmentResult_Period
FOREIGN KEY ([PeriodSelectedDepartmentId]) REFERENCES [dbo].[PeriodSelectedDepartments](Id);

GO
ALTER TABLE [dbo].[PeriodAssessmentResults]
ADD CONSTRAINT FK_PeriodAssessmentResult_Questions
FOREIGN KEY ([PeriodQuestionId]) REFERENCES [dbo].[PeriodQuetions](Id);

Go


CREATE INDEX IX_PeriodAssessmentResults_1
ON [PeriodAssessmentResults] ([Result]);
Go

CREATE INDEX IX_PeriodAssessmentResults_2
ON [PeriodAssessmentResults] ([Deleted]);
Go