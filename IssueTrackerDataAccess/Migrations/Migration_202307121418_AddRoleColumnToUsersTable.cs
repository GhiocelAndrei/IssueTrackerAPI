using FluentMigrator;

namespace IssueTracker.DataAccess.Migrations
{
    [Migration(202307121418)]
    public class Migration_202307121418_AddRoleColumnToUsersTable : Migration
    {
        public override void Up()
        {
            Create.Column("Role")
                .OnTable("Users").AsString().WithDefaultValue("User");
        }

        public override void Down()
        {
            Delete.Column("Role")
                .FromTable("Users");
        }
    }
}
