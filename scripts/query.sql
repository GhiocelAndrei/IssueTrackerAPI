-- Clean Up Database
USE IssueTrackerDb;

DELETE FROM Issues;
DELETE FROM Sprints;
DELETE FROM Projects;
DELETE FROM Users;

-- Load Database with Records

-- Load Projects Table

DECLARE @CounterProjects INT = 0;
WHILE @CounterProjects < 100000
BEGIN
    INSERT INTO Projects(Name, Code, IssueSequence, CreatedAt, IsDeleted)
    VALUES (CONCAT('Project_', @CounterProjects), CONCAT('PROJ_', @CounterProjects), 0, GETDATE(), 0);
    SET @CounterProjects = @CounterProjects + 1;
END

-- Load Users Table

DECLARE @CounterUsers INT = 0;
WHILE @CounterUsers < 10000
BEGIN
    INSERT INTO Users(Name, Email, Role, IsDeleted)
    VALUES (CONCAT('User_', @CounterUsers), CONCAT('User_', @CounterUsers, '@issuetracker.com'), 'User', 0);
    SET @CounterUsers = @CounterUsers + 1;
END

-- Load Sprints Table

DECLARE @CounterSprints INT = 0;
WHILE @CounterSprints < 1000
BEGIN
    INSERT INTO Sprints(Name, Active, Description, StartDate, EndDate, CreatedAt, IsDeleted)
    VALUES (CONCAT('Sprint_', @CounterSprints), 1, 'Sprint Description', DATEADD(DAY, -RAND()*10, GETDATE()), DATEADD(DAY, RAND()*10, GETDATE()), GETDATE(), 0);
    SET @CounterSprints = @CounterSprints + 1;
END

-- Load Issues Table

DECLARE @CounterIssues INT = 0;
DECLARE @CurrentProjectId INT, @CurrentReporterId INT, @CurrentAssigneeId INT, @CurrentSprintId INT;

SELECT @CurrentProjectId = MIN(Id) FROM Projects;
SELECT @CurrentReporterId = MIN(Id) FROM Users;
SELECT @CurrentAssigneeId = MIN(Id) FROM Users;
SELECT @CurrentSprintId = MIN(Id) FROM Sprints;

WHILE @CounterIssues < 1000000
BEGIN

    IF @CounterIssues % 10 = 0 AND @CounterIssues != 0
    BEGIN
        SET @CurrentProjectId = @CurrentProjectId + 1;
    END

    IF @CounterIssues % 100 = 0 AND @CounterIssues != 0
    BEGIN
        SET @CurrentReporterId = @CurrentReporterId + 1;
        SET @CurrentAssigneeId = @CurrentAssigneeId + 1;
    END
    
    IF @CounterIssues % 1000 = 0 AND @CounterIssues != 0
    BEGIN
        SET @CurrentSprintId = @CurrentSprintId + 1;
    END

    INSERT INTO Issues(ExternalId, ProjectId, Title, Description, Priority, SprintId, ReporterId, AssigneeId, CreatedAt, IsDeleted)
    VALUES 
    (
        NEWID(),
        @CurrentProjectId,
        CONCAT('Issue_', @CounterIssues),
        'Issue description',
        0,
        @CurrentSprintId
        @CurrentReporterId,
        @CurrentAssigneeId,
        GETDATE(),
        0
    );

    SET @CounterIssues = @CounterIssues + 1;
END


