using FluentMigrator;

namespace IssueTracker.DataAccess.Migrations
{
    [Migration(202308011742)]
    public class Migration_202308011742_AddColumnsForExternalIdAndProjectCode : Migration
    {
        public override void Up()
        {
            Alter.Table("Issues")
                 .AddColumn("ExternalId").AsString().Nullable();

            Alter.Table("Projects")
                 .AddColumn("Code").AsString().Nullable()
                 .AddColumn("IssueSequence").AsInt64().NotNullable().WithDefaultValue(0L);
        }

        public override void Down()
        {
            Delete.Column("ExternalId").FromTable("Issues");

            Delete.Column("Code").FromTable("Projects");
            Delete.Column("IssueSequence").FromTable("Projects");
        }
    }
}
