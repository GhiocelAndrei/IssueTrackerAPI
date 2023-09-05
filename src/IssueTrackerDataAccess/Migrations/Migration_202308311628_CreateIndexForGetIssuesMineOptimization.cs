using FluentMigrator;

namespace IssueTracker.DataAccess.Migrations
{
    [Migration(202308311631)]
    public class Migration_202308311628_CreateIndexForGetIssuesMineOptimization : Migration
    {
        public override void Up()
        {
            Execute.Sql(@"
            CREATE NONCLUSTERED INDEX Ix_IssuesForUser
            ON Issues (ProjectId, AssigneeId, IsDeleted)
            INCLUDE (Title, CreatedAt, UpdatedAt, ExternalId)
            WHERE IsDeleted = 0;
            ");
        }

        public override void Down()
        {
            Execute.Sql(@"
            DROP INDEX Ix_IssuesForUser ON Issues;
            ");
        }
    }
}
