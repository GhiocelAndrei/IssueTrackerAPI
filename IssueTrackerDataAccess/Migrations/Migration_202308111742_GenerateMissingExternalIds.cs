using FluentMigrator;

namespace IssueTracker.DataAccess.Migrations
{
    [Migration(202308111742)]
    public class Migration_202308111742_GenerateMissingExternalIds : Migration
    {
        public override void Up()
        {
            Execute.Sql(@"
            UPDATE Projects
            SET Code = 'PROJ' + CAST(Id AS VARCHAR(20)), 
                IssueSequence = 0
            WHERE Code IS NULL AND IssueSequence IS NULL;

            DECLARE @IssueId BIGINT, @ProjectId BIGINT, @Seq BIGINT, @ProjectCode NVARCHAR(25);
            
            DECLARE @SeqTable TABLE (Sequence BIGINT);

            DECLARE IssueCursor CURSOR FOR
            SELECT i.Id, i.ProjectId, p.Code
            FROM Issues i
            JOIN Projects p ON p.Id = i.ProjectId
            WHERE i.ExternalId IS NULL;

            OPEN IssueCursor;
            FETCH NEXT FROM IssueCursor INTO @IssueId, @ProjectId, @ProjectCode;

            WHILE @@FETCH_STATUS = 0
            BEGIN
                DELETE FROM @SeqTable;

                UPDATE Projects WITH(HOLDLOCK, ROWLOCK)
                SET IssueSequence = IssueSequence + 1
                OUTPUT inserted.IssueSequence INTO @SeqTable
                WHERE Id = @ProjectId;
    
                SELECT @Seq = Sequence FROM @SeqTable;

                UPDATE Issues SET ExternalId = @ProjectCode + '-' + CAST(@Seq AS NVARCHAR(20)) WHERE Id = @IssueId;

                FETCH NEXT FROM IssueCursor INTO @IssueId, @ProjectId, @ProjectCode;
            END;
            
            CLOSE IssueCursor;
            DEALLOCATE IssueCursor;
            ");
        }

        public override void Down()
        {
           
        }
    }
}
