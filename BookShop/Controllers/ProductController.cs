using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookShop.Data.Contexts;
using BookShop.Data.Dtos;
using BookShop.Data.Models;
using Newtonsoft.Json;
using System;

namespace BookShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _dbContext;

        public ProductController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Show(int productId)
        {
            var session = HttpContext.Session;
            var role = HttpContext.Session.GetString("role");
            List<Category_requests> category_request = null!;

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

            var product = _dbContext.Products.Include(x=>x.Category).Where(x=>x.product_id == productId).FirstOrDefault();
            if (product != null)
            {
                var p = new ProductDto();
                p.product_id = product.product_id;
                p.product_name = product.name;
                p.product_description = product.description;
                p.author = product.author;
                p.category_id = product.category_id;
                p.cateogry_name = product.Category!.name;
                p.price = product.price;
                p.quantity = product.quantity;
                p.image_url = product.image_url;
                return View(p);
            }
            return View(product);
        }
    }
}
