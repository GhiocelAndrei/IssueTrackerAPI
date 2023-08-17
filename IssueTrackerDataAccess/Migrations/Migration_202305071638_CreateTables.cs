using FluentMigrator;

namespace IssueTracker.DataAccess.Migrations
{
    [Migration(202305071638)]
    public class Migration_202305071638_CreateTables : Migration
    {
        public override void Up()
        {
            CreateProjectsTable();
            CreateUsersTable();
            CreateIssuesTable();
        }

        public override void Down()
        {
            Delete.Table("Issues");
            Delete.Table("Projects");
            Delete.Table("Users");
        }

        void CreateIssuesTable()
        {
            Create.Table("Issues")
           .WithColumn("Id").AsInt64().PrimaryKey().Identity()
           .WithColumn("ProjectId").AsInt64()
           .WithColumn("Title").AsString()
           .WithColumn("Description").AsString()
           .WithColumn("Priority").AsInt32()
           .WithColumn("ReporterId").AsInt64()
           .WithColumn("AssigneeId").AsInt64()
           .WithColumn("CreatedAt").AsDateTime()
           .WithColumn("UpdatedAt").AsDateTime()
           .WithColumn("IsDeleted").AsBoolean()
           .WithColumn("DeletedAt").AsDateTime().Nullable();

            Create.ForeignKey("FK_Issues_Projects")
                .FromTable("Issues").ForeignColumn("ProjectId")
                .ToTable("Projects").PrimaryColumn("Id");

            Create.ForeignKey("FK_Issues_Users_Reporter")
                .FromTable("Issues").ForeignColumn("ReporterId")
                .ToTable("Users").PrimaryColumn("Id");

            Create.ForeignKey("FK_Issues_Users_Assignee")
                .FromTable("Issues").ForeignColumn("AssigneeId")
                .ToTable("Users").PrimaryColumn("Id");
        }

        void CreateProjectsTable()
        {
            Create.Table("Projects")
            .WithColumn("Id").AsInt64().PrimaryKey().Identity()
            .WithColumn("Name").AsString()
            .WithColumn("CreatedAt").AsDateTime()
            .WithColumn("UpdatedAt").AsDateTime()
            .WithColumn("IsDeleted").AsBoolean()
            .WithColumn("DeletedAt").AsDateTime().Nullable();
        }

        void CreateUsersTable()
        {
            Create.Table("Users")
            .WithColumn("Id").AsInt64().PrimaryKey().Identity()
            .WithColumn("Name").AsString()
            .WithColumn("Email").AsString();
        }
    }
}
