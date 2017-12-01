using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FoodApp.Models
{
    public class Tag
    {
        [Required]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Must be between 3 and 30 characters")]
        public string Name { get; set; }

        public int Id { get; set; }
        
        public virtual ICollection<Recipe> Recipes { get; set; }
        
}
}


