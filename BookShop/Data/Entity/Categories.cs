using System.ComponentModel.DataAnnotations;

namespace BookShop.Data.Models
{
    public class Categories
    {
        [Key]
        public int category_id { get; set; }
        public string? name { get; set; }
        public List<Products>? products { get; set; }
    }
}
