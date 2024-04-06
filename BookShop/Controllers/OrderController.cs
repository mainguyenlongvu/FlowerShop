using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookShop.Data.Contexts;
using BookShop.Data.Dtos;
using BookShop.Data.Models;
using System;
using Newtonsoft.Json;

namespace BookShop.Controllers
{
    public class OrderController : BaseController
    {
        private readonly AppDbContext _dbContext;

        public OrderController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult ViewOrders()
        {
            var orders = _dbContext.Orders.OrderByDescending(x=>x.order_date).Where(x=>x.user_id == int.Parse(HttpContext.Session.GetString("user_id")!)).ToList();
            return View(orders);
        }
        public IActionResult ViewOrder(int orderId)
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


            var query = from a in _dbContext.Order_Items.Include(x=>x.Product)
                        where a.order_id == orderId
                        select new { a };
            if(query != null)
            {
                var data = query.Select(x=>new OrderItemsDto()
                {
                    order_item_id = x.a.order_item_id,
                    order_id = orderId,
                    product_id = x.a.product_id,
                    product_name = x.a.Product!.name,
                    price = x.a.price,
                    quantity = x.a.quantity
                }).ToList();
                return Json(data);
            }
            return Json(null);
        }
        public async Task<IActionResult> HuyDon(int orderId)
        {
            try
            {
                var data = await _dbContext.Orders.SingleOrDefaultAsync(x => x.order_id == orderId);
                if (data != null)
                {
                    data.status = "Hủy đơn";
                    await _dbContext.SaveChangesAsync();
                    var order_item = await _dbContext.Order_Items.Where(x => x.order_id == orderId).ToListAsync();
                    if (order_item != null)
                    {
                        foreach(var item in order_item)
                        {
                            var product = await _dbContext.Products.SingleOrDefaultAsync(x => x.product_id == item.product_id);
                            if(product!= null)
                            {
                                product.quantity = item.quantity + product.quantity;
                                await _dbContext.SaveChangesAsync();
                            }
                        }
                    }
                    TempData["Message"] = "Hủy đơn hàng thành công";
                    TempData["Success"] = true;
                    return Json(data);
                }
                TempData["Message"] = "Đơn hàng không tồn tại";
                TempData["Success"] = false;
                return Json(null);
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Lỗi";
                TempData["Success"] = false;
                return RedirectToAction("", "Cart");
            }
            
        }
    }
}
