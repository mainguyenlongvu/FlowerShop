namespace BookShop.Data.Dtos
{
    public class ProductDto
    {
        public int product_id { get; set; }
        public int category_id { get; set; }
        public string? cateogry_name { get; set; }
        public string? product_name { get; set; }
        public string? author { get; set; }
        public string? product_description { get; set; }
        public decimal price { get; set; }
        public int quantity { get; set; }
        public string? image_url { get; set; }
        public IFormFile? file { get; set; }
    }
}
