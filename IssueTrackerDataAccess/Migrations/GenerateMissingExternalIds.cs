using FluentMigrator;

namespace IssueTracker.DataAccess.Migrations
{
    [Migration(202308111742)]
    public class GenerateMissingExternalIds : ForwardOnlyMigration
    {
        public override void Up()
        {
            Execute.Sql(@"
            UPDATE Projects
            SET Code = 'PROJ' + CAST(Id AS VARCHAR(20)), 
                IssueSequence = 0
            WHERE Code IS NULL OR IssueSequence IS NULL
            ");

            Execute.Sql(@"
            CREATE TABLE #ProjectSequences 
            (
                ProjectId BIGINT PRIMARY KEY,
                IssueSequence BIGINT
            )

            INSERT INTO #ProjectSequences (ProjectId, IssueSequence)
            SELECT Id, IssueSequence
            FROM Projects
            ");

            Execute.Sql(@"
            DECLARE @IssueId BIGINT, @ProjectId BIGINT, @ExternalId NVARCHAR(100), @Seq BIGINT

            DECLARE IssueCursor CURSOR FOR
            SELECT i.Id, i.ProjectId
            FROM Issues i
            WHERE i.ExternalId IS NULL

            OPEN IssueCursor
            FETCH NEXT FROM IssueCursor INTO @IssueId, @ProjectId

            WHILE @@FETCH_STATUS = 0
            BEGIN
                SELECT @Seq = IssueSequence + 1 FROM #ProjectSequences WHERE ProjectId = @ProjectId
    
                SET @ExternalId = 'PROJ' + CAST(@ProjectId AS NVARCHAR(20)) + '-' + CAST(@Seq AS NVARCHAR(20))
                UPDATE Issues SET ExternalId = @ExternalId WHERE Id = @IssueId
    
                UPDATE #ProjectSequences SET IssueSequence = @Seq WHERE ProjectId = @ProjectId
    
                FETCH NEXT FROM IssueCursor INTO @IssueId, @ProjectId
            END

            CLOSE IssueCursor
            DEALLOCATE IssueCursor
            ");

            Execute.Sql(@"
            UPDATE p
            SET p.IssueSequence = ps.IssueSequence
            FROM Projects p
            JOIN #ProjectSequences ps ON p.Id = ps.ProjectId
            ");

            Execute.Sql(@"
                DROP TABLE #ProjectSequences
            ");
        }
    }
}
