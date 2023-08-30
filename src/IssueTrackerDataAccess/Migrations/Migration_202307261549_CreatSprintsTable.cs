using FluentMigrator;

namespace IssueTracker.DataAccess.Migrations
{
    [Migration(202307261549)]
    public class Migration_202307261549_CreatSprintsTable : Migration
    {
        public override void Up()
        {
            Create.Table("Sprints")
            .WithColumn("Id").AsInt64().PrimaryKey().Identity()
            .WithColumn("Name").AsString()
            .WithColumn("Description").AsString()
            .WithColumn("StartDate").AsDateTime()
            .WithColumn("EndDate").AsDateTime()
            .WithColumn("CreatedAt").AsDateTime()
            .WithColumn("UpdatedAt").AsDateTime().Nullable();
        }

        public override void Down()
        {
            Delete.Table("Sprints");
        }
    }
}
