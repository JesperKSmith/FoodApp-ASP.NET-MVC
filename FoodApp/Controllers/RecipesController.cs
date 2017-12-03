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

namespace FoodApp.Controllers
{
    [Authorize]
    public class RecipesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Recipes
        [AllowAnonymous]
        public ActionResult Index()
        {
            var recipes = db.Recipes.ToList();
            return View(recipes);
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
        public ActionResult Create(RecipeViewModel rvm)
        {
            var selectedTags = rvm.TagIds;
            rvm.Recipe.Tags = db.Tags.Where(m => selectedTags.Contains(m.Id)).ToList();
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
                return RedirectToAction("Index");
            }

            return RedirectToAction("Details", "Recipes");
        }

        // GET: Recipes/Edit/5
        public ActionResult Edit(int? id)
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

        // POST: Recipes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,title,description")] Recipe recipe)
        {
            if (ModelState.IsValid)
            {
                db.Entry(recipe).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(recipe);
        }

        // GET: Recipes/Delete/5
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
            return View(recipe);
        }

        // POST: Recipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
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
        private string savedImageName()
        {
            var file = Request.Files[0];
            string pictureName = "";

            var pictureType = file.ContentType;

            if (!pictureType.Contains("image"))
            {
                var notImage = true;
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

            return "~/Images/" + pictureName;
        }

        private string getDefaultPictureName()
        {
            return "~/Images/default.png";
        }

    }

    }


