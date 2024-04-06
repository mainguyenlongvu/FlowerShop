using System.ComponentModel.DataAnnotations;

namespace BookShop.Data.Models
{
    public class Orders
    {
        [Key]
        public int order_id { get; set; }
        public int user_id { get; set; }
        public string? phone { get; set; }
        public string? email { get; set; }
        public string? address { get; set; }
        public DateTime? order_date { get; set; }
        public decimal total_price { get; set; }
        public string? status { get; set; }
        public string? note { get; set; }
        public Users? User { get; set; }
        public List<Order_items>? order_Items { get; set; }
    }
}
