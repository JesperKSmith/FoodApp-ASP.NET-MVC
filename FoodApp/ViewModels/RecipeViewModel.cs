using FoodApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodApp.ViewModels
{
    public class RecipeViewModel
    {
        public Recipe Recipe { get; set; }
        public IEnumerable<SelectListItem> AllTags { get; set; }
        public int[] TagIds { get; set; }
        public Tag Tag { get; set; }
        public TagViewModel tvm { get; set; }
    }
}
