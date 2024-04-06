using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookShop.Data.Contexts;
using BookShop.Data.Dtos;
using BookShop.Data.Models;
using Newtonsoft.Json;

namespace BookShop.Controllers
{
    public class OwnerProductController : OwnerBaseController
    {
        private readonly AppDbContext _dbContext;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly string _imageContentFolder;

        public OwnerProductController(AppDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            _dbContext = dbContext;
            this.webHostEnvironment = webHostEnvironment;
            _imageContentFolder = Path.Combine(webHostEnvironment.WebRootPath, "imgProduct");
        }
        public IActionResult Index()
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

            var products = from a in _dbContext.Products.Include(x => x.Category)
                           where a.user_id == user_id
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
        [HttpPost]
        public IActionResult Create(ProductDto productData)
        {
            var user_id = int.Parse(HttpContext.Session.GetString("user_id")!);
            var product = _dbContext.Products.SingleOrDefault(x => x.name == productData.product_name);
            if (product != null)
            {
                TempData["Message"] = "Sản phẩm đã tồn tại";
                TempData["Success"] = false;
                return RedirectToAction("Index");
            }
            try
            {
                var p = new Products();
                p.user_id = user_id;
                p.name = productData.product_name;
                p.description = productData.product_description;
                p.category_id = productData.category_id;
                p.price = productData.price;
                p.quantity = productData.quantity;
                p.author = productData.author;
                p.image_url = UploadedFile(productData.file!);
                _dbContext.Products.Add(p);
                _dbContext.SaveChanges();
                TempData["Message"] = "Thêm mới sản phẩm thành công";
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
        public IActionResult Edit(ProductDto productData)
        {
            var p = _dbContext.Products.SingleOrDefault(x => x.product_id == productData.product_id);
            if (p == null)
            {
                TempData["Message"] = "Sản phẩm không tồn tại";
                TempData["Success"] = false;
                return RedirectToAction("Index");
            }
            try
            {
                p.name = productData.product_name;
                p.description = productData.product_description;
                p.category_id = productData.category_id;
                p.price = productData.price;
                p.quantity = productData.quantity;
                p.author = productData.author;
                if (productData.file != null)
                {
                    if (p.image_url != "/imgProduct/" && p.image_url != null)
                    {
                        var n = p.image_url!.Remove(0, 12);
                        DeleteImage(n);
                    }
                    p.image_url = UploadedFile(productData.file!);
                }
                _dbContext.SaveChanges();
                TempData["Message"] = "Chỉnh sửa thông tin sản phẩm thành công";
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
        public IActionResult Delete(int productId)
        {
            try
            {
                var product = _dbContext.Products.Find(productId);
                if (product != null)
                {
                    if (product.image_url != "/imgProduct/" && product.image_url != null)
                    {
                        var n = product.image_url!.Remove(0, 12);
                        DeleteImage(n);
                    }
                    _dbContext.Products.Remove(product);
                    _dbContext.SaveChanges();
                    TempData["Message"] = "Xóa sản phẩm thành công";
                    TempData["Success"] = true;
                }
                else
                {
                    TempData["Message"] = "Xóa sản phẩm không thành công";
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
            return "/imgProduct/" + uniqueFileName!;
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
