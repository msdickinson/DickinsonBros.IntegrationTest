CREATE TABLE [IntegrationTest].[TestResult]
(
	[TestResultId] BIGINT NOT NULL IDENTITY(1,1) CONSTRAINT Data_PK PRIMARY KEY,
    [TestGroup] nvarchar(255) NOT NULL, 
    [TestName] nvarchar(255) NOT NULL, 
    [Pass] bit NOT NULL,
    [Duration] Time NOT NULL,
    [CorrelationId] nvarchar(255) NOT NULL,
    [TestRunId] nvarchar(255) NOT NULL,
    [DateTime] DATETIME2 NOT NULL, 
)