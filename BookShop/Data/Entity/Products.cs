using System.ComponentModel.DataAnnotations;

namespace BookShop.Data.Models
{
    public class Products
    {
        [Key]
        public int product_id { get; set; }
        public int user_id { get; set; }
        public int category_id { get; set; }
        public string? name { get; set; }
        public string? author { get; set; }
        public string? description { get; set; }
        public int quantity { get; set; }
        public decimal price { get; set; }
        public string? image_url { get; set; }
        public Categories? Category { get; set; }
        public List<Order_items>? order_Items { get; set; }
    }
}
