using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FoodApp.Models;
using FoodApp.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.IO;
using FoodApp.SignalHubs;

namespace FoodApp.Controllers
{
    [Authorize]
    public class RecipesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Recipes
        [AllowAnonymous]
        public ActionResult Index(int? tagId)
        {
            var recipes = new List<Recipe>();

            if(tagId != null)
            {
                recipes = db.Recipes.Where(r => r.Tags.Any(t => t.Id == tagId)).ToList();
            }
            else
            {
                recipes = db.Recipes.ToList();
            }

            var allTags = db.Tags.ToList();
            ViewBag.allTags = allTags;

            foreach (var recipe in recipes)
            {
                if (recipe.Description.Length > 150)
                {
                    recipe.Description = recipe.Description.Substring(0, 150);
                }
            }

            return View(recipes);
        }

        // GET: My Recipes
        [Authorize]
        public ActionResult MyRecipes()
        {
           var currentUser = User.Identity.Name;
           var recipes = db.Recipes.Where(r => r.Author == currentUser).ToList();
           
            return View("MyRecipes", recipes);
        }


        

        // GET: Recipes/Details/5
        [AllowAnonymous]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recipe recipe = db.Recipes.Find(id);
            if (recipe == null)
            {
                return HttpNotFound();
            }
            return View(recipe);
        }

        // GET: Recipes/Create
        [Authorize]
        public ActionResult Create()
        {
            RecipeViewModel rvm = new RecipeViewModel();
            var tags = db.Tags.ToList();
            rvm.Recipe = new Recipe();
            rvm.Recipe.Author = User.Identity.Name;
            rvm.AllTags = tags.Select(m => new SelectListItem { Text = m.Name, Value = m.Id.ToString() });

            return View(rvm);
        }

        // POST: Recipes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create(RecipeViewModel rvm)
        {
            var selectedTags = rvm.TagIds;
            if (selectedTags !=null && selectedTags.Length > 0)
            {
                rvm.Recipe.Tags = db.Tags.Where(m => selectedTags.Contains(m.Id)).ToList();
            }
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

            // If the request contains picture file
            if (Request.Files.Count > 0)
            {
                rvm.Recipe.Picture = savedImageName();
            }
            else
            {
                rvm.Recipe.Picture = getDefaultPictureName();
            }

            if (ModelState.IsValid)
            {
                db.Recipes.Add(rvm.Recipe);
                db.SaveChanges();
                signalClientsAboutNewRecipe(rvm.Recipe);
                return RedirectToAction("Index");
            }

            return View(rvm);
        }

        private void signalClientsAboutNewRecipe(Recipe recipe)
        {
            RecipeHub hub = new RecipeHub();
            hub.InformClientAboutNewRecipe(recipe);
        }

        // GET: Recipes/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Find Recipe
            Recipe recipe = db.Recipes.Find(id);
            // Check if Author is Owner of the Recipe
            if (!isOwnerOfRecipe(recipe.Author)) { return RedirectToAction("Login", "Account"); }

            RecipeViewModel rvm = new RecipeViewModel();
            var tags = db.Tags.ToList();
            rvm.Recipe = recipe;
            rvm.Recipe.Author = User.Identity.Name;
            rvm.AllTags = tags.Select(m => new SelectListItem { Text = m.Name, Value = m.Id.ToString() });

            List<int> selectedTags = new List<int>();
            foreach(var tag in rvm.Recipe.Tags)
            {
                selectedTags.Add(tag.Id);
            }

            ViewBag.recipeSelectedTagsIds = selectedTags;

            if (recipe == null)
            {
                return HttpNotFound();
            }
            return View(rvm);
        }

        // POST: Recipes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit(RecipeViewModel rvm)
        {

            //Get original ninja from context
            Recipe recipe = db.Recipes.Find(rvm.Recipe.Id);
            db.Entry(recipe).CurrentValues.SetValues(rvm.Recipe);
            recipe.Tags.Clear();

            var selectedTags = rvm.TagIds;
            if (selectedTags != null &&  selectedTags.Length != rvm.Recipe.Tags.Count)
            {
                foreach (var item in selectedTags)
                {
                    Tag tag = db.Tags.Find(item);
                    recipe.Tags.Add(tag);
                }
            }

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

            // If the request contains picture file
            if (Request.Files.Count > 0)
            {
                recipe.Picture = savedImageName(recipe.Picture);
            }

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Recipes/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recipe recipe = db.Recipes.Find(id);
            if (recipe == null)
            {
                return HttpNotFound();
            }

            // Check if Author is Owner of the Recipe
            if (!isOwnerOfRecipe(recipe.Author)) { return RedirectToAction("Login", "Account"); }

            return View(recipe);
        }

        // POST: Recipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            Recipe recipe = db.Recipes.Find(id);
            db.Recipes.Remove(recipe);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // TAG FUN TIME
        // POST
        [HttpPost]
        [Authorize]
        public ActionResult AddTag(RecipeViewModel rvm)
        {
            Tag tempTag = new Tag();
            tempTag.Name = rvm.Tag.Name;
            db.Tags.Add(tempTag);
            db.SaveChanges();
            return RedirectToAction("Create", "Recipes");
        }


        // ======================================================================
        // PRIVATE FUNCTIONS

        // SAVE IMAGE
        private string savedImageName(string originalPicture = "")
        {
            var file = Request.Files[0];
            string pictureName = "";

            var pictureType = file.ContentType;

            if (!pictureType.Contains("image"))
            {
                var notImage = true;
                return originalPicture;
            }
            else
            {
                var image = true;
            }

            if (file != null && file.ContentLength > 0)
            {
                pictureName = Path.GetFileName(file.FileName);
                var pathToFile = Path.Combine(Server.MapPath("~/Images/"), pictureName);
                file.SaveAs(pathToFile);
            }

            if(pictureName == "") { pictureName = "default.png"; }

            return "~/Images/" + pictureName;
        }
        

        private string getDefaultPictureName()
        {
            return "~/Images/default.png";
        }

        // Is Owner of the recipe
        private bool isOwnerOfRecipe(string authorName)
        {
            return User.Identity.Name == authorName;
        }

    }

    }


