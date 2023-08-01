using FluentMigrator;

namespace IssueTracker.DataAccess.Migrations
{
    [Migration(202307281124)]
    public class AddNewColumnsToIssuesAndSprints : Migration
    {
        public override void Up()
        {
            Create.Column("SprintId")
               .OnTable("Issues").AsInt64().Nullable();

            Create.Column("Active")
               .OnTable("Sprints").AsBoolean();
        }

        public override void Down()
        {
            Delete.Column("SprintId")
               .FromTable("Issues");

            Delete.Column("Active")
              .FromTable("Sprints");
        }
    }
}
