using FluentMigrator;

namespace IssueTracker.DataAccess.Migrations
{
    [Migration(202307111812)]
    public class ChangeUpdatedAtToNullable : Migration
    {
        public override void Up()
        {
            Alter.Table("Issues")
            .AlterColumn("UpdatedAt")
            .AsDateTime()   
            .Nullable();

            Alter.Table("Projects")
            .AlterColumn("UpdatedAt")
            .AsDateTime()
            .Nullable();
        }

        public override void Down()
        {
            Alter.Table("Issues")
            .AlterColumn("UpdatedAt")
            .AsDateTime()    
            .NotNullable();

            Alter.Table("Projects")
            .AlterColumn("UpdatedAt")
            .AsDateTime()
            .NotNullable();
        }
    }
}
