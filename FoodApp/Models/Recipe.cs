using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace FoodApp.Models
{
    public class Recipe
    {

        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Must be between 3 and 50 characters")]
        public string Title { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 20, ErrorMessage = "Must be between 50 and 500 characters")]
        public string Description { get; set; }

        [Required]
        public string Ingredients { get; set; }

        [Required]
        public string Author { get; set; }

        public virtual ICollection<Tag> Tags { get; set; }
        public string Picture { get; set; }
    }
}