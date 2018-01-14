namespace FoodApp.Migrations
{
    using FoodApp.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<FoodApp.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(FoodApp.Models.ApplicationDbContext context)
        {

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            string role = "Admin";

            if (!roleManager.RoleExists(role))
            {
                var adminRole = roleManager.Create(new IdentityRole(role));
            }
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
       
           
            //Fetching UserManager
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if (userManager.FindByName("Admin") != null)
            {
                var admin = userManager.FindByName("Admin");
                userManager.Delete(admin);
            }
            //Create User with UserManager
            var user = new ApplicationUser { UserName = "admin@admin.dk", Email = "admin@admin.dk", Id = "1" };
            userManager.Create(user, "Jesper100%");

            //Get User from Database based on userId
            var currentUser = userManager.FindByName("Admin");
            
            //Add a role to a User
            var result = userManager.AddToRole(user.Id, "Admin");
        }
    }
}
