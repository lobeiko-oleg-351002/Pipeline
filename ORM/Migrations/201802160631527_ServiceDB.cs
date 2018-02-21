namespace ORM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ServiceDB : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.User", "signInDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
            DropColumn("dbo.User", "createRights");
            DropColumn("dbo.User", "changeRights");
        }
        
        public override void Down()
        {
            AddColumn("dbo.User", "changeRights", c => c.Boolean(nullable: false));
            AddColumn("dbo.User", "createRights", c => c.Boolean(nullable: false));
            DropColumn("dbo.User", "signInDate");
        }
    }
}
