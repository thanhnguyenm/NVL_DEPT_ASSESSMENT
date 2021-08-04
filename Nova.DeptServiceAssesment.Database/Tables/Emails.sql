CREATE TABLE [dbo].[Emails]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[Subject] NVARCHAR(MAX) NOT NULL,
	[Body] NVARCHAR(MAX) NOT NULL,
	[To] NVARCHAR(MAX) NOT NULL,
	[Status] NVARCHAR(20),
	[Deleted] BIT NULL DEFAULT 0, 
    [CreatedBy] INT NULL, 
    [CreatedDate] DATETIME NULL, 
    [ModifiedBy] INT NULL, 
    [ModifiedDate] DATETIME NULL
)
Go


CREATE INDEX IX_Emails_1
ON [Emails] ([Status]);
Go

CREATE INDEX IX_Emails_2
ON [Emails] ([Deleted]);
Go