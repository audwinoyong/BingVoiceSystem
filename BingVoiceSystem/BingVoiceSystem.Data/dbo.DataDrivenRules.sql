CREATE TABLE [dbo].[DataDrivenRules]
 (
    [RuleID]       INT NOT NULL,
	[DataID]	   INT NOT NULL,
    [Question]     VARCHAR (1000) NOT NULL,
    [Answer]       VARCHAR (MAX)  NOT NULL,
    [ApprovedBy]   NVARCHAR (128) NOT NULL,
    [LastEditedBy] NVARCHAR (128) NULL,
    PRIMARY KEY CLUSTERED ([DataID] ASC)
);
