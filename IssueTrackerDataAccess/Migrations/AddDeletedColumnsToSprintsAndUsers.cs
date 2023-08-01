using FluentMigrator;

namespace IssueTracker.DataAccess.Migrations
{
    [Migration(202308011224)]
    public class AddDeletedColumnsToSprintsAndUsers : Migration
    {
        public override void Up()
        {
            Create.Column("IsDeleted")
               .OnTable("Sprints").AsBoolean().NotNullable().WithDefaultValue(false);

            Create.Column("DeletedAt")
               .OnTable("Sprints").AsDateTime().Nullable();

            Create.Column("IsDeleted")
               .OnTable("Users").AsBoolean().NotNullable().WithDefaultValue(false);

            Create.Column("DeletedAt")
               .OnTable("Users").AsDateTime().Nullable();
        }

        public override void Down()
        {
            Delete.Column("IsDeleted")
               .FromTable("Sprints");

            Delete.Column("DeletedAt")
               .FromTable("Sprints");

            Delete.Column("IsDeleted")
               .FromTable("Users");

            Delete.Column("DeletedAt")
               .FromTable("Users");
        }
    }
}
