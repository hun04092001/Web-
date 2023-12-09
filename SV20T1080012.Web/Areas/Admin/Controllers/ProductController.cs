using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using SV20T1080012.BusinessLayers;
using SV20T1080012.DomainModels;
using SV20T1080012.Web;
using SV20T1080012.Web.AppCodes;
using SV20T1080012.Web.Models;
using System.Reflection;

namespace LiteCommerce.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    [Authorize(Roles = $"{WebUserRoles.Administrator}")]// chuyển đến đăng nhập
    [Area("Admin")]
    public class ProductController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
         private const int PAGE_SIZE = 6;
        public IActionResult Index(int page = 1, string searchValue = "")
        {
            int rowCount = 0;
            var data = CommonDataService.ListOfProducts(out rowCount, page, PAGE_SIZE, searchValue ?? "");
            var model = new PaginationSearchProduct()
            {
                Page = page,
                PageSize = PAGE_SIZE,
                SearchValue = searchValue ?? "",
                RowCount = rowCount,
                Data = data
            };

            string? errorMessage = Convert.ToString(TempData["ErrorMessage"]);
            ViewBag.ErrorMessage = errorMessage;

            string? successMessage = Convert.ToString(TempData["SuccessMessage"]);
            ViewBag.SuccessMessage = successMessage;

            string? addsuccessMessage = Convert.ToString(TempData["AddSuccessMessage"]);
            ViewBag.AddSuccessMessage = addsuccessMessage;
            return View(model);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            var model = new Product()
            {
                ProductID = 0
            };
            ViewBag.Title = "Bổ sung mặt hàng";
            return View(model);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Edit(int id = 0)
        {
            var model = CommonDataService.GetProduct(id);
            if (model == null)
            {
                return RedirectToAction("Index");

            }
            ViewBag.Title = "Cập nhật mặt hàng";
            return View("Create", model);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                bool success = CommonDataService.DeleteProduct(id);
                if (!success)
                {
                    TempData["ErrorMessage"] = "Không thể xóa mặt hàng này !";
                }
                else
                {
                    TempData["SuccessMessage"] = "Xóa thành công !";
                }
                return RedirectToAction("Index");
            }

            var model = CommonDataService.GetProduct(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="method"></param>
        /// <param name="photoId"></param>
        /// <returns></returns>
        public IActionResult Photo(int id = 0, string method = "add", int photoId = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung ảnh";
                    return View();
                case "edit":
                    ViewBag.Title = "Thay đổi ảnh";
                    return View();
                case "delete":
                    //TODO: Delete photo
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="method"></param>
        /// <param name="attributeId"></param>
        /// <returns></returns>
        public IActionResult Attribute(int id = 0, string method = "add", int attributeId = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung thuộc tính";
                    return View();
                case "edit":
                    ViewBag.Title = "Thay đổi thuộc tính";
                    return View();
                case "delete":
                    //TODO: Delete Attribute
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }
        }
        public IActionResult Save(Product data, IFormFile? uploadPhoto)
        {
            ViewBag.Title = data.ProductID == 0 ? "Bổ sung mặt hàng" : "Cập nhật mặt hàng";
           
            //Xử lý với ảnh
            //Upload ảnh lên (nếu có), sau khi upload xong thì mới lấy tên file ảnh vừa upload
            //để gán cho trường Photo của Employee
            if (uploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
                string filePath = System.IO.Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"images\Product", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                data.Photo = fileName;
            }
            // kiểm soát dữ liệu đầu vào
            if (string.IsNullOrWhiteSpace(data.ProductName))
                ModelState.AddModelError(nameof(data.ProductName), "Tên mặt hàng không được rỗng");

            if (string.IsNullOrWhiteSpace(data.ProductDescription))
            {
                ModelState.AddModelError(nameof(data.ProductDescription), "Mô tả không được trống");
            }

           // if (string.IsNullOrWhiteSpace(data.CategoryID))
              //  ModelState.AddModelError(nameof(data.CategoryID), "Loại hàng không được rỗng");

           // if (string.IsNullOrWhiteSpace(data.SupplierID))
              //  ModelState.AddModelError(nameof(data.SupplierID), "Nhà cung cấp không được rỗng");

            if (string.IsNullOrWhiteSpace(data.Unit))
                ModelState.AddModelError(nameof(data.Unit), "đơn vị tính không được rỗng");

            if (string.IsNullOrWhiteSpace(data.Price))
                ModelState.AddModelError(nameof(data.Price), "giá không được rỗng");

            if (!ModelState.IsValid)
            {
                return View("Create", data);
            }

            if (data.ProductID == 0)
            {
                int productId = CommonDataService.AddProduct(data);
                if (productId > 0)
                {
                    TempData["AddSuccessMessage"] = "Bổ sung thành công !";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.ErrorMessage = "Không bổ sung được dữ liệu";
                    return View("Create", data);
                }
            }
            else
            {
                if (data != null)
                {
                    CommonDataService.UpdateProduct(data);
                    TempData["SuccessMessage"] = "Cập nhật thành công !";
                }

                return RedirectToAction("Index");

            }
        }
        }
}
