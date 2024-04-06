using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using BookShop.Common;
using BookShop.Data.Contexts;
using BookShop.Data.Dtos;
using BookShop.Data.Models;
using Newtonsoft.Json;

namespace BookShop.Controllers
{
    public class AdminUserController : AdminBaseController
    {
        private readonly AppDbContext _dbContext;

        public AdminUserController(AppDbContext dbContext)
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

            var users = _dbContext.Users.OrderByDescending(x=>x.user_id).ToList();
            return View(users);
        }
        [HttpPost]
        public IActionResult Create(RegisterDto userData)
        {
            try
            {
                if (userData.password != userData.confirmPassword)
                {
                    TempData["Message"] = "Mật khẩu xác nhận không đúng";
                    TempData["Success"] = false;
                    return RedirectToAction("Index");
                }
                var usr = _dbContext.Users.Where(x => x.username == userData.username || x.email == userData.email);
                if (usr.Count() > 0)
                {
                    TempData["Message"] = "Tài khoản đã tồn tại";
                    TempData["Success"] = false;
                    return RedirectToAction("Index");
                }
                var user = new Users();
                user.username = userData.username;
                user.role = "User";
                user.password = CreateMD5.MD5Hash(userData.password!);
                user.email = userData.email;
                user.address = userData.address;
                user.phone = userData.phone;
                user.name = userData.name;
                user.image_url = "";
                _dbContext.Users.Add(user);
                _dbContext.SaveChanges();
                TempData["Message"] = "Thêm mới người dùng thành công";
                TempData["Success"] = true;
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                TempData["Message"] = "Lỗi tạo tài khoản";
                TempData["Success"] = false;
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public IActionResult Edit(RegisterDto userData)
        {
            try
            {
                if (userData.password != userData.confirmPassword)
                {
                    TempData["Message"] = "Mật khẩu xác nhận không đúng";
                    TempData["Success"] = false;
                    return RedirectToAction("Index");
                }
                var user = _dbContext.Users.SingleOrDefault(x => x.user_id == userData.user_id);
                if (user == null)
                {
                    TempData["Message"] = "Tài khoản không tồn tại";
                    TempData["Success"] = false;
                    return View(userData);
                }
                if(user.email != userData.email)
                {
                    var usr = _dbContext.Users.Where(x => x.email == userData.email);
                    if (usr.Count() > 0)
                    {
                        TempData["Message"] = "Tài khoản đã tồn tại";
                        TempData["Success"] = false;
                        return RedirectToAction("Index");
                    }
                }
                if (user.username != userData.username)
                {
                    var usr = _dbContext.Users.Where(x => x.username == userData.username);
                    if (usr.Count() > 0)
                    {
                        TempData["Message"] = "Tài khoản đã tồn tại";
                        TempData["Success"] = false;
                        return RedirectToAction("Index");
                    }
                }
                user.role = userData.role;
                if(userData.password != null)
                {
                    user.password = CreateMD5.MD5Hash(userData.password!);
                }
                user.email = userData.email;
                user.address = userData.address;
                user.phone = userData.phone;
                user.name = userData.name;
                _dbContext.SaveChanges();
                TempData["Message"] = "Chỉnh sửa thông tin người dùng thành công";
                TempData["Success"] = true;
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                TempData["Message"] = "Lỗi";
                TempData["Success"] = false;
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public IActionResult Delete(int userId)
        {
            try
            {
                var user = _dbContext.Users.Find(userId);
                if (user != null)
                {
                    _dbContext.Users.Remove(user);
                    _dbContext.SaveChanges();
                    TempData["Message"] = "Xóa tài khoản người dùng thành công";
                    TempData["Success"] = true;
                }
                else
                {
                    TempData["Message"] = "Xóa tài khoản người dùng không thành công";
                    TempData["Success"] = false;
                }
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                TempData["Message"] = "Lỗi";
                TempData["Success"] = false;
                return RedirectToAction("Index");
            }
        }
    }
}
