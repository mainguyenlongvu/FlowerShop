using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookShop.Data.Contexts;
using BookShop.Data.Dtos;
using BookShop.Data.Models;
using Newtonsoft.Json;

namespace BookShop.Controllers
{
    public class AdminOrderController : AdminBaseController
    {
        private readonly AppDbContext _dbContext;

        public AdminOrderController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var session = HttpContext.Session;
            List<Category_requests> category_request = null!;

            category_request = _dbContext.Category_Requests
                    .Where(x => x.is_approved == 0 && x.is_deleted_by_admin == false)
                    .OrderByDescending(x => x.created_at)
                    .ToList();

            if (category_request != null)
            {
                var jsonNotification = JsonConvert.SerializeObject(category_request);
                session.SetString("notification", jsonNotification);
            }

            var orders = _dbContext.Orders.OrderByDescending(x=>x.order_date).ToList();
            return View(orders);
        }
    }
}
