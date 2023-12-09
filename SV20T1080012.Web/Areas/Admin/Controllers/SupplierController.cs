using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1080012.BusinessLayers;
using SV20T1080012.DomainModels;
using SV20T1080012.Web.Models;

namespace SV20T1080012.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    ///
    [Authorize(Roles = $"{WebUserRoles.Administrator}")]// chuyển đến đăng nhập
    [Area("Admin")]
    public class SupplierController : Controller
    {
        private const int PAGE_SIZE = 10;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IActionResult Index(int page = 1, string searchValue = "")
        {
            int rowCount = 0;
            var data = CommonDataService.ListOfSuppliers(out rowCount, page, PAGE_SIZE, searchValue ?? "");
            var model = new PaginationSearchSuppliers()
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
        // create nhà cung cấp
        public IActionResult Create()
        {
            var model = new Supplier()
            {
                SupplierID = 0
            };
            ViewBag.Title = "Bổ sung nhà cung cấp";
            return View(model);
        }
        // Edit nhà cung cấp
        public IActionResult Edit(int id  = 0)
        {
            var model = CommonDataService.GetSupplier(id);
            if (model == null)
            {
                return RedirectToAction("Index");

            }
            ViewBag.Title = "Cập nhật nhà cung cấp";
            return View("Create",model);
        }
        // delete nhà cung cấp 
        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                bool success = CommonDataService.DeleteSupplier(id);
                if (!success)
                {
                    TempData["ErrorMessage"] = "Không thể xóa nhà cung cấp này !";
                }
                else
                {
                    TempData["SuccessMessage"] = "Xóa thành công !";
                }
                return RedirectToAction("Index");
            }

            var model = CommonDataService.GetSupplier(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public IActionResult Save(Supplier data)
        {
            ViewBag.Title = data.SupplierID == 0 ? "Bổ sung nhà cung cấp" : "Cập nhật nhà cung cấp";

            // kiểm soát dữ liệu đầu vào
            if (string.IsNullOrWhiteSpace(data.SupplierName))
                ModelState.AddModelError(nameof(data.SupplierName), "Tên khách hàng không được rỗng");

            if (string.IsNullOrWhiteSpace(data.ContactName))
                ModelState.AddModelError(nameof(data.ContactName), "Tên giao dịch không được rỗng");

            if (string.IsNullOrWhiteSpace(data.Provice))
                ModelState.AddModelError(nameof(data.Provice), "Tỉnh thành không được rỗng");

            if (string.IsNullOrWhiteSpace(data.Address))
                ModelState.AddModelError(nameof(data.Address), "Địa chỉ không được rỗng");

            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Số điện thoại không được rỗng");

            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "email không được rỗng");

            if (!ModelState.IsValid)
            {
                return View("Create", data);
            }
            if (data.SupplierID == 0)
            {
                int supplierId = CommonDataService.AddSupplier(data);
                if (supplierId > 0)
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
                    CommonDataService.UpdateSupplier(data);
                    TempData["SuccessMessage"] = "Cập nhật thành công !";
                }

                return RedirectToAction("Index");

            }
        }

    }
}
