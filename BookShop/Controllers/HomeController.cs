using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookShop.Data.Contexts;
using BookShop.Data.Dtos;
using BookShop.Data.Models;
using Newtonsoft.Json;

namespace BookShop.Controllers
{
    public class HomeController : Controller
    {

        private readonly AppDbContext _dbContext;
        public HomeController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var session = HttpContext.Session;
            var role = HttpContext.Session.GetString("role");
            List<Category_requests> category_request = null!;

            if (role != null)
            {

                if (role == "Admin")
                {
                    category_request = _dbContext.Category_Requests
                        .Where(x => x.is_approved == 0 && x.is_deleted_by_admin == false)
                        .OrderByDescending(x => x.created_at)
                        .ToList();
                }
                else if (role == "Store Owner")
                {
                    var user_id = int.Parse(HttpContext.Session.GetString("user_id")!);
                    category_request = _dbContext.Category_Requests
                        .Where(x => x.is_deleted_by_owner == false && x.requester_id == user_id && (x.is_approved == -1 || x.is_approved == 1))
                        .OrderByDescending(x => x.created_at)
                        .ToList();
                }

                if (category_request != null)
                {
                    var jsonNotification = JsonConvert.SerializeObject(category_request);
                    session.SetString("notification", jsonNotification);
                }
            }


            var products = from a in _dbContext.Products.Include(x => x.Category)
                           select new { a };
            if (products != null)
            {
                var lstProducts = products.OrderByDescending(x => x.a.product_id).Select(x => new ProductDto()
                {
                    product_id = x.a.product_id,
                    product_name = x.a.name,
                    product_description = x.a.description,
                    author = x.a.author,
                    category_id = x.a.category_id,
                    cateogry_name = x.a.Category!.name,
                    price = x.a.price,
                    quantity = x.a.quantity,
                    image_url = x.a.image_url
                }).ToList();
                ViewBag.Categories = _dbContext.Categories.ToList();
                return View(lstProducts);
            }
            return View();
        }
        public IActionResult FilterProducts(int category_id)
        {
            // Logic filter danh sách sản phẩm theo category
            var filteredProducts = from a in _dbContext.Products.Include(x => x.Category)
                                   select new { a };

            if (category_id != 0)
            {
                filteredProducts = filteredProducts.Where(x => x.a.category_id == category_id);
            }
            if (filteredProducts != null)
            {
                var lstProducts = filteredProducts.Select(x => new ProductDto()
                {
                    product_id = x.a.product_id,
                    product_name = x.a.name,
                    product_description = x.a.description,
                    author = x.a.author,
                    category_id = x.a.category_id,
                    cateogry_name = x.a.Category!.name,
                    price = x.a.price,
                    quantity = x.a.quantity,
                    image_url = x.a.image_url,
                    
                }).ToList();
                return PartialView("ProductList", lstProducts);
            }
            return PartialView("ProductList", null);
        }
        public IActionResult Search(string keyword)
        {
            var filteredProducts = from a in _dbContext.Products.Include(x => x.Category)
                                   select new { a };

            filteredProducts = filteredProducts.Where(x => x.a.name!.Contains(keyword));
            if (filteredProducts != null)
            {
                var lstProducts = filteredProducts.Select(x => new ProductDto()
                {
                    product_id = x.a.product_id,
                    product_name = x.a.name,
                    product_description = x.a.description,
                    author = x.a.author,
                    category_id = x.a.category_id,
                    cateogry_name = x.a.Category!.name,
                    price = x.a.price,
                    quantity = x.a.quantity,
                    image_url = x.a.image_url
                }).ToList();
                return View(lstProducts);
            }
            return View(filteredProducts);
        }
    }
}