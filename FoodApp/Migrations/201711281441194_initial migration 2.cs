namespace FoodApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initialmigration2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 30),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TagRecipes",
                c => new
                    {
                        Tag_Id = c.Int(nullable: false),
                        Recipe_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Tag_Id, t.Recipe_Id })
                .ForeignKey("dbo.Tags", t => t.Tag_Id, cascadeDelete: true)
                .ForeignKey("dbo.Recipes", t => t.Recipe_Id, cascadeDelete: true)
                .Index(t => t.Tag_Id)
                .Index(t => t.Recipe_Id);
            
            AddColumn("dbo.Recipes", "Ingredients", c => c.String(nullable: false));
            AddColumn("dbo.Recipes", "Picture", c => c.String());
            AddColumn("dbo.Recipes", "Author_Id", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Recipes", "Title", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Recipes", "Description", c => c.String(nullable: false, maxLength: 500));
            CreateIndex("dbo.Recipes", "Author_Id");
            AddForeignKey("dbo.Recipes", "Author_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TagRecipes", "Recipe_Id", "dbo.Recipes");
            DropForeignKey("dbo.TagRecipes", "Tag_Id", "dbo.Tags");
            DropForeignKey("dbo.Recipes", "Author_Id", "dbo.AspNetUsers");
            DropIndex("dbo.TagRecipes", new[] { "Recipe_Id" });
            DropIndex("dbo.TagRecipes", new[] { "Tag_Id" });
            DropIndex("dbo.Recipes", new[] { "Author_Id" });
            AlterColumn("dbo.Recipes", "Description", c => c.String());
            AlterColumn("dbo.Recipes", "Title", c => c.String());
            DropColumn("dbo.Recipes", "Author_Id");
            DropColumn("dbo.Recipes", "Picture");
            DropColumn("dbo.Recipes", "Ingredients");
            DropTable("dbo.TagRecipes");
            DropTable("dbo.Tags");
        }
    }
}
