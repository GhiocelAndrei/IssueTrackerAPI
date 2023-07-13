using FluentMigrator;
using Microsoft.AspNetCore.Http.HttpResults;

namespace IssueTrackerAPI.Migrations
{
    [Migration(202307121418)]
    public class AddRoleColumnToUsersTable : Migration
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
