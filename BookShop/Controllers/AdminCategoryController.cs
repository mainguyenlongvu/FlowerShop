using Microsoft.AspNetCore.Mvc;
using BookShop.Data.Contexts;
using BookShop.Data.Models;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Controllers
{
    public class AdminCategoryController : AdminBaseController
    {
        private readonly AppDbContext _dbContext;

        public AdminCategoryController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
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

            var all_category = await _dbContext.Categories
                    .ToListAsync();
            return View(all_category);
        }
        [HttpPost]
        public IActionResult Create(Categories request)
        {
            var category = _dbContext.Categories.SingleOrDefault(x => x.name == request.name);
            if (category != null)
            {
                TempData["Message"] = "Danh mục đã tồn tại";
                TempData["Success"] = false;
                return RedirectToAction("Index");
            }
            try
            {
                var p = new Categories();
                p.name = request.name;
                _dbContext.Categories.Add(p);
                _dbContext.SaveChanges();
                TempData["Message"] = "Thêm mới danh mục thành công";
                TempData["Success"] = true;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Lỗi";
                TempData["Success"] = false;
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public IActionResult Edit(Categories request)
        {
            var p = _dbContext.Categories.SingleOrDefault(x => x.category_id == request.category_id);
            if (p == null)
            {
                TempData["Message"] = "Danh mục không tồn tại";
                TempData["Success"] = false;
                return RedirectToAction("Index");
            }
            try
            {
                p.name = request.name;
                _dbContext.SaveChanges();
                TempData["Message"] = "Chỉnh sửa danh mục thành công";
                TempData["Success"] = true;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Lỗi";
                TempData["Success"] = false;
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public IActionResult Delete(int category_id)
        {
            try
            {
                var category = _dbContext.Categories.Find(category_id);
                if (category != null)
                {

                    _dbContext.Categories.Remove(category);
                    _dbContext.SaveChanges();
                    TempData["Message"] = "Xóa danh mục thành công";
                    TempData["Success"] = true;
                }
                else
                {
                    TempData["Message"] = "Xóa danh mục không thành công";
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
    }
}
