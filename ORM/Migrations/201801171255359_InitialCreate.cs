namespace ORM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AttributeLib",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Event",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 50),
                        description = c.String(maxLength: 50),
                        status_lib_id = c.Int(nullable: false),
                        filepath_lib_id = c.Int(nullable: false),
                        attribute_lib_id = c.Int(nullable: false),
                        receiver_lib_id = c.Int(nullable: false),
                        date = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        sender_id = c.Int(nullable: false),
                        type_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.EventType", t => t.type_id, cascadeDelete: true)
                .ForeignKey("dbo.User", t => t.sender_id, cascadeDelete: true)
                .ForeignKey("dbo.UserLib", t => t.receiver_lib_id)
                .ForeignKey("dbo.StatusLib", t => t.status_lib_id)
                .ForeignKey("dbo.FilepathLib", t => t.filepath_lib_id, cascadeDelete: true)
                .ForeignKey("dbo.AttributeLib", t => t.attribute_lib_id)
                .Index(t => t.status_lib_id)
                .Index(t => t.filepath_lib_id)
                .Index(t => t.attribute_lib_id)
                .Index(t => t.receiver_lib_id)
                .Index(t => t.sender_id)
                .Index(t => t.type_id);
            
            CreateTable(
                "dbo.EventType",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.SelectedEventType",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        entity_id = c.Int(nullable: false),
                        lib_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.EventTypeLib", t => t.lib_id, cascadeDelete: true)
                .ForeignKey("dbo.EventType", t => t.entity_id, cascadeDelete: true)
                .Index(t => t.entity_id)
                .Index(t => t.lib_id);
            
            CreateTable(
                "dbo.EventTypeLib",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        login = c.String(nullable: false, maxLength: 50),
                        password = c.String(nullable: false, maxLength: 50),
                        fullName = c.String(nullable: false, maxLength: 50),
                        group_id = c.Int(nullable: false),
                        createRights = c.Boolean(nullable: false),
                        changeRights = c.Boolean(nullable: false),
                        statusLib_id = c.Int(nullable: false),
                        eventTypeLib_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Group", t => t.group_id, cascadeDelete: true)
                .ForeignKey("dbo.StatusLib", t => t.statusLib_id)
                .ForeignKey("dbo.EventTypeLib", t => t.eventTypeLib_id)
                .Index(t => t.group_id)
                .Index(t => t.statusLib_id)
                .Index(t => t.eventTypeLib_id);
            
            CreateTable(
                "dbo.Group",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.SelectedUser",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        lib_id = c.Int(nullable: false),
                        entity_id = c.Int(nullable: false),
                        isEventAccepted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.UserLib", t => t.lib_id, cascadeDelete: true)
                .ForeignKey("dbo.User", t => t.entity_id, cascadeDelete: true)
                .Index(t => t.lib_id)
                .Index(t => t.entity_id);
            
            CreateTable(
                "dbo.UserLib",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.StatusLib",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.SelectedStatus",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        lib_id = c.Int(nullable: false),
                        entity_id = c.Int(nullable: false),
                        date = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Status", t => t.entity_id, cascadeDelete: true)
                .ForeignKey("dbo.StatusLib", t => t.lib_id, cascadeDelete: true)
                .Index(t => t.lib_id)
                .Index(t => t.entity_id);
            
            CreateTable(
                "dbo.Status",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.FilepathLib",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        folderName = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Filepath",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        path = c.String(nullable: false, maxLength: 50),
                        lib_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.FilepathLib", t => t.lib_id, cascadeDelete: true)
                .Index(t => t.lib_id);
            
            CreateTable(
                "dbo.SelectedAttribute",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        lib_id = c.Int(nullable: false),
                        entity_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Attribute", t => t.entity_id, cascadeDelete: true)
                .ForeignKey("dbo.AttributeLib", t => t.lib_id, cascadeDelete: true)
                .Index(t => t.lib_id)
                .Index(t => t.entity_id);
            
            CreateTable(
                "dbo.Attribute",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SelectedAttribute", "lib_id", "dbo.AttributeLib");
            DropForeignKey("dbo.SelectedAttribute", "entity_id", "dbo.Attribute");
            DropForeignKey("dbo.Event", "attribute_lib_id", "dbo.AttributeLib");
            DropForeignKey("dbo.Filepath", "lib_id", "dbo.FilepathLib");
            DropForeignKey("dbo.Event", "filepath_lib_id", "dbo.FilepathLib");
            DropForeignKey("dbo.SelectedEventType", "entity_id", "dbo.EventType");
            DropForeignKey("dbo.User", "eventTypeLib_id", "dbo.EventTypeLib");
            DropForeignKey("dbo.User", "statusLib_id", "dbo.StatusLib");
            DropForeignKey("dbo.SelectedStatus", "lib_id", "dbo.StatusLib");
            DropForeignKey("dbo.SelectedStatus", "entity_id", "dbo.Status");
            DropForeignKey("dbo.Event", "status_lib_id", "dbo.StatusLib");
            DropForeignKey("dbo.SelectedUser", "entity_id", "dbo.User");
            DropForeignKey("dbo.SelectedUser", "lib_id", "dbo.UserLib");
            DropForeignKey("dbo.Event", "receiver_lib_id", "dbo.UserLib");
            DropForeignKey("dbo.User", "group_id", "dbo.Group");
            DropForeignKey("dbo.Event", "sender_id", "dbo.User");
            DropForeignKey("dbo.SelectedEventType", "lib_id", "dbo.EventTypeLib");
            DropForeignKey("dbo.Event", "type_id", "dbo.EventType");
            DropIndex("dbo.SelectedAttribute", new[] { "entity_id" });
            DropIndex("dbo.SelectedAttribute", new[] { "lib_id" });
            DropIndex("dbo.Filepath", new[] { "lib_id" });
            DropIndex("dbo.SelectedStatus", new[] { "entity_id" });
            DropIndex("dbo.SelectedStatus", new[] { "lib_id" });
            DropIndex("dbo.SelectedUser", new[] { "entity_id" });
            DropIndex("dbo.SelectedUser", new[] { "lib_id" });
            DropIndex("dbo.User", new[] { "eventTypeLib_id" });
            DropIndex("dbo.User", new[] { "statusLib_id" });
            DropIndex("dbo.User", new[] { "group_id" });
            DropIndex("dbo.SelectedEventType", new[] { "lib_id" });
            DropIndex("dbo.SelectedEventType", new[] { "entity_id" });
            DropIndex("dbo.Event", new[] { "type_id" });
            DropIndex("dbo.Event", new[] { "sender_id" });
            DropIndex("dbo.Event", new[] { "receiver_lib_id" });
            DropIndex("dbo.Event", new[] { "attribute_lib_id" });
            DropIndex("dbo.Event", new[] { "filepath_lib_id" });
            DropIndex("dbo.Event", new[] { "status_lib_id" });
            DropTable("dbo.Attribute");
            DropTable("dbo.SelectedAttribute");
            DropTable("dbo.Filepath");
            DropTable("dbo.FilepathLib");
            DropTable("dbo.Status");
            DropTable("dbo.SelectedStatus");
            DropTable("dbo.StatusLib");
            DropTable("dbo.UserLib");
            DropTable("dbo.SelectedUser");
            DropTable("dbo.Group");
            DropTable("dbo.User");
            DropTable("dbo.EventTypeLib");
            DropTable("dbo.SelectedEventType");
            DropTable("dbo.EventType");
            DropTable("dbo.Event");
            DropTable("dbo.AttributeLib");
        }
    }
}
