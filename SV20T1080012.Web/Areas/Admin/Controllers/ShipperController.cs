using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1080012.BusinessLayers;
using SV20T1080012.DomainModels;
using SV20T1080012.Web.Models;

namespace SV20T1080012.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator}")]// chuyển đến đăng nhập
    [Area("Admin")]
    public class ShipperController : Controller
    {
        private const int PAGE_SIZE = 10;
        public IActionResult Index(int page = 1, string searchValue = "")
        {
            int rowCount = 0;
            var data = CommonDataService.ListOfShippers(out rowCount, page, PAGE_SIZE, searchValue ?? "");
            var model = new PaginationSearchShipper()
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
        // create shipper
        public IActionResult Create()
        {
            var model = new Shipper()
            {
                ShipperID = 0
            };
            ViewBag.Title = "Bổ sung người giao hàng";
            return View(model);
        }
        // Edit shipper
        public IActionResult Edit(int id = 0)
        {
            var model = CommonDataService.GetShipper(id);
            if (model == null)
            {
                return RedirectToAction("Index");

            }
            ViewBag.Title = "Cập nhật người giao hàng";
            return View("Create",model);
        }
        // delete shipper
        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                bool success = CommonDataService.DeleteShipper(id);
                if (!success)
                {
                    TempData["ErrorMessage"] = "Không thể xóa người giao hàng này !";
                }
                else
                {
                    TempData["SuccessMessage"] = "Xóa thành công !";
                }
                return RedirectToAction("Index");
            }

            var model = CommonDataService.GetShipper(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }
        public IActionResult Save(Shipper data)
        {
            ViewBag.Title = data.ShipperID == 0 ? "Bổ sung người giao hàng" : "Cập nhật người giao hàng";

            // kiểm soát dữ liệu đầu vào
            if (string.IsNullOrWhiteSpace(data.ShipperName))
                ModelState.AddModelError(nameof(data.ShipperName), "Tên người giao hàng không được rỗng");

            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Số điện thoại không được rỗng");
            if (!ModelState.IsValid)
            {
                return View("Create", data);
            }
            if (data.ShipperID == 0)
            {
                int shipperId = CommonDataService.AddShipper(data);
                if (shipperId > 0)
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
                    bool success = CommonDataService.UpdateShipper(data);
                    if (!success)
                    {
                        ViewBag.ErrorMessage = "Không bổ sung được dữ liệu";
                    } else
                        TempData["SuccessMessage"] = "Cập nhật thành công !";
                }

                return RedirectToAction("Index");

            }
        }
    }
}
