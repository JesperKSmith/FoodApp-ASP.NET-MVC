namespace FoodApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RecipeUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Recipes", "Directions", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Recipes", "Directions");
        }
    }
}
