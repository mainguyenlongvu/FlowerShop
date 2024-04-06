using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookShop.Data.Contexts;
using BookShop.Data.Models;
using Newtonsoft.Json;

namespace BookShop.Controllers
{
    public class OwnerOrderController : OwnerBaseController
    {
        private readonly AppDbContext _dbContext;

        public OwnerOrderController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var user_id = int.Parse(HttpContext.Session.GetString("user_id")!);

            var session = HttpContext.Session;
            List<Category_requests> category_request = null!;

            category_request = _dbContext.Category_Requests
                    .Where(x => x.is_deleted_by_owner == false && x.requester_id == user_id && (x.is_approved == -1 || x.is_approved == 1))
                    .OrderByDescending(x => x.created_at)
                    .ToList();

            if (category_request != null)
            {
                var jsonNotification = JsonConvert.SerializeObject(category_request);
                session.SetString("notification", jsonNotification);
            }

            var query = from a in _dbContext.Orders
                        join b in _dbContext.Order_Items
                        on a.order_id equals b.order_id
                        join c in _dbContext.Products
                        on b.product_id equals c.product_id
                        where c.user_id == user_id
                        select new { a };
            var orders = await query.OrderByDescending(x => x.a.order_date).Select(x=> new Orders
            {
                order_id = x.a.order_id,
                user_id = x.a.user_id,
                phone = x.a.phone,
                email = x.a.email,
                address = x.a.address,
                order_date = x.a.order_date,
                total_price = x.a.total_price,
                status = x.a.status,
                note = x.a.note

            }).ToListAsync();
            return View(orders);
        }
        [HttpPost]
        public IActionResult Edit(int orderId, string status)
        {
            try
            {
                var user_id = int.Parse(HttpContext.Session.GetString("user_id")!);

                var order = _dbContext.Orders.Find(orderId);
                if (order != null)
                {
                    order.status = status;
                    _dbContext.SaveChanges();
                    TempData["Message"] = "Cập nhật trạng thái đơn hàng thành công";
                    TempData["Success"] = true;
                }
                else
                {
                    TempData["Message"] = "Đơn hàng không tồn tại";
                    TempData["Success"] = false;
                }
                return Ok();
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Lỗi";
                TempData["Success"] = false;
                return BadRequest();
            }
        }
        public IActionResult ProfileCustomer(string email)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.email == email);
            if (user == null)
            {
                TempData["Message"] = "Tài khoản không tồn tại";
                TempData["Success"] = false;
                return RedirectToAction("Index");
            }
            return View(user);
        }
    }
}
