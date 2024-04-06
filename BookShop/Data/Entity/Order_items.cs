using System.ComponentModel.DataAnnotations;

namespace BookShop.Data.Models
{
    public class Order_items
    {
        [Key]
        public int order_item_id { get; set; }
        public int order_id { get; set; }
        public int product_id { get; set; }
        public int quantity { get; set; }
        public decimal price { get; set; }
        public Orders? Order { get; set; }
        public Products? Product { get; set; }
    }
}
