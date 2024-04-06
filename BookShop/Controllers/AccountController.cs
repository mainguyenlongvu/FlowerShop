using Azure;
using Microsoft.AspNetCore.Mvc;
using BookShop.Common;
using BookShop.Data.Contexts;
using BookShop.Data.Dtos;
using BookShop.Data.Models;

namespace BookShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _imageContentFolder;

        public AccountController(AppDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            _dbContext = dbContext;
            _webHostEnvironment = webHostEnvironment;
            _imageContentFolder = Path.Combine(webHostEnvironment.WebRootPath, "imgProfile");
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginDto loginData)
        {
            var pwt = CreateMD5.MD5Hash(loginData.password!);
            var user = _dbContext.Users.FirstOrDefault(x => x.username == loginData.username && x.password == pwt);
            if (user != null)
            {
                HttpContext.Session.SetString("username", user.username!);
                HttpContext.Session.SetString("role", user.role!);
                HttpContext.Session.SetString("user_id", user.user_id.ToString()!);
                HttpContext.Session.SetString("name", user.name!);
                HttpContext.Session.SetString("email", user.email!);
                HttpContext.Session.SetString("phone", user.phone!);
                HttpContext.Session.SetString("address", user.address!);
                if(user.role == "Admin")
                {
                    TempData["Message"] = "Chào mừng Quản trị viên";
                }
                else if(user.role == "Store Owner")
                {
                    TempData["Message"] = "Chào mừng Chủ cửa hàng";
                }
                else
                {
                    TempData["Message"] = "Chào mừng Khách hàng";
                }
                TempData["Success"] = true;
                return RedirectToAction("Index", "Home");
            }
            TempData["Message"] = "Đăng nhập không thành công";
            TempData["Success"] = false;
            return View(loginData);
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(RegisterDto userData)
        {
            try
            {
                if (userData.password != userData.confirmPassword)
                {
                    TempData["Message"] = "Mật khẩu xác nhận không đúng";
                    TempData["Success"] = false;
                    return View(userData);
                }
                var usr = _dbContext.Users.Where(x => x.username == userData.username || x.email == userData.email);
                if (usr.Count() > 0)
                {
                    TempData["Message"] = "Tài khoản đã tồn tại";
                    TempData["Success"] = false;
                    return View(userData);
                }
                var user = new Users();
                user.username = userData.username;
                user.role = "User";
                user.password = CreateMD5.MD5Hash(userData.password!);
                user.email = userData.email;
                user.address = userData.address;
                user.phone = userData.phone;
                user.name = userData.name;
                if (userData.file != null)
                {
                    user.image_url = UploadedFile(userData.file);
                }
                else
                {
                    user.image_url = "";
                }
                _dbContext.Users.Add(user);
                _dbContext.SaveChanges();
                TempData["Message"] = "Đăng kí thành công";
                TempData["Success"] = true;
                return RedirectToAction("Login", "Account");
            }
            catch (Exception e)
            {
                TempData["Message"] = "Lỗi tạo tài khoản";
                TempData["Success"] = false;
                return View(userData);
            }
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Profile()
        {
            if(HttpContext.Session.GetString("user_id")!= null)
            {

                var user_id = int.Parse(HttpContext.Session.GetString("user_id")!);
                var user = _dbContext.Users.FirstOrDefault(x => x.user_id == user_id);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                return View(user);
            }
            return RedirectToAction("Login", "Account");
        }
        [HttpPost]
        public IActionResult UpdateProfile(RegisterDto userData)
        {
            try
            {
                if (HttpContext.Session.GetString("user_id") != null)
                {

                    var user_id = int.Parse(HttpContext.Session.GetString("user_id")!);
                    var email = HttpContext.Session.GetString("email");

                    if (email != userData.email)
                    {
                        var usr = _dbContext.Users.Where(x => x.email == userData.email);
                        if (usr.Count() > 0)
                        {
                            TempData["Message"] = "Tài khoản đã tồn tại";
                            TempData["Success"] = false;
                            return RedirectToAction("Profile", "Account");
                        }
                    }
                    var user = _dbContext.Users.FirstOrDefault(x => x.user_id == user_id);
                    if (user == null)
                    {
                        return RedirectToAction("Login", "Account");
                    }
                    else if (userData.password != userData.confirmPassword)
                    {
                        TempData["Message"] = "Mật khẩu xác nhận không đúng";
                        TempData["Success"] = false;
                        return RedirectToAction("Profile", "Account");
                    }
                    else if (userData.password != null)
                    {
                        if(user.password!.Length >= 8)
                        {
                            user.password = CreateMD5.MD5Hash(userData.password!);
                        }
                        else
                        {
                            TempData["Message"] = "Mật khẩu phải ít nhất 8 kí tự";
                            TempData["Success"] = false;
                            return RedirectToAction("Profile", "Account");
                        }
                    }
                    user.email = userData.email;
                    user.address = userData.address;
                    user.phone = userData.phone;
                    user.name = userData.name;
                    if (userData.file != null)
                    {
                        if (user.image_url != "/imgProfile/" && user.image_url != null)
                        {
                            var n = user.image_url!.Remove(0, 12);
                            DeleteImage(n);
                        }
                        user.image_url = UploadedFile(userData.file);
                    }
                    _dbContext.SaveChanges();
                    TempData["Message"] = "Cập nhật thông tin thành công";
                    TempData["Success"] = true;
                    return RedirectToAction("Profile", "Account");
                }
            
                return RedirectToAction("Login", "Account");
            }
            catch (Exception e)
            {
                TempData["Message"] = "Lỗi cập nhật thông tin tài khoản";
                TempData["Success"] = false;
                return RedirectToAction("Profile", "Account");
            }
        }


        private string UploadedFile(IFormFile file)
        {
            string uniqueFileName = null!;

            if (file != null)
            {
                string uploadsFolder = _imageContentFolder;
                uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
            }
            return "/imgProfile/" + uniqueFileName!;
        }
        public void DeleteImage(string fileName)
        {
            var filePath = Path.Combine(_imageContentFolder, fileName);
            if (System.IO.File.Exists(filePath))
            {
                Task.Run(() => System.IO.File.Delete(filePath));
            }
        }
    }
}
