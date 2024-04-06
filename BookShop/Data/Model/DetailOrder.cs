using BookShop.Data.Model.Cart;

namespace BookShop.Data.Model.Cart
{
    public class DetailOrder
    {
        public int MaKH { get; set; }
        public string? DiaChiGiao { get; set; }
        public string? SDT { get; set; }
        public string? email { get; set; }
        public string? note { get; set; }
        public decimal TongTien { get; set; }
        public List<CartItem>? CartItems { get; set; }
    }
}
