namespace FoodApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Authorchangedtostring : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Recipes", "Author_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Recipes", new[] { "Author_Id" });
            AddColumn("dbo.Recipes", "Author", c => c.String(nullable: false));
            DropColumn("dbo.Recipes", "Author_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Recipes", "Author_Id", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.Recipes", "Author");
            CreateIndex("dbo.Recipes", "Author_Id");
            AddForeignKey("dbo.Recipes", "Author_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
    }
}
