using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1080012.BusinessLayers;
using SV20T1080012.DomainModels;
using SV20T1080012.Web.Models;

namespace SV20T1080012.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator}")]// chuyển đến đăng nhập
    [Area("Admin")]
    public class CustomerController : Controller
    {
        private const int PAGE_SIZE = 10;
        public IActionResult Index(int page = 1, string searchValue = "")
        {

            int rowCount = 0;
            var data = CommonDataService.ListOfCustommers(out rowCount, page, PAGE_SIZE, searchValue ?? "");
            var model = new PaginationSearchCustomer()
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
        // create khách hàng
        public IActionResult Create()
        {
            var model = new Customer()
            {
                CustomerID = 0
            };
            ViewBag.Title = "Bổ sung khách hàng";
            return View(model);
        }
        // Edit khách hàng
        public IActionResult Edit(int id = 0)
        {
            var model =  CommonDataService.GetCustomer(id);
            if(model == null)
            {
                return RedirectToAction("Index");

            }
            ViewBag.Title = "Cập nhật khách hàng";
            return View("Create", model);
        }
        // delete khách hàng 
        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                bool success = CommonDataService.DeleteCustomer(id);
                if(!success)
                {
                    TempData["ErrorMessage"] = "Không thể xóa khách hàng này !";
                }
                else
                {
                    TempData["SuccessMessage"] = "Xóa thành công !";
                }    
                return RedirectToAction("Index");
            }

            var model = CommonDataService.GetCustomer(id);
            if(model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }
        
        public IActionResult Changepass()
        {
           
            return View();
        }
        public IActionResult Save(Customer data)
        {
            ViewBag.Title = data.CustomerID == 0 ? "Bổ sung khách hàng" : "Cập nhật khách hàng";
            // kiểm soát dữ liệu đầu vào
            if (string.IsNullOrWhiteSpace(data.CustomerName))
                ModelState.AddModelError(nameof(data.CustomerName), "Tên khách hàng không được rỗng");

            if (string.IsNullOrWhiteSpace(data.ContactName))
                ModelState.AddModelError(nameof(data.ContactName), "Tên giao dịch không được rỗng");

            if (string.IsNullOrWhiteSpace(data.Province))
                ModelState.AddModelError(nameof(data.Province), "Tỉnh thành không được rỗng");

            if (string.IsNullOrWhiteSpace(data.Address))
                ModelState.AddModelError(nameof(data.Address), "Địa chỉ không được rỗng");

            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Số điện thoại không được rỗng");

            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "email không được rỗng");

            if(!ModelState.IsValid)
            {
                return View("Create", data);
            }
            // thông báo 
            if (data.CustomerID == 0)
            {
                int customerId = CommonDataService.AddCustomer(data);
                if (customerId > 0)
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
                    CommonDataService.UpdateCustomer(data);
                    TempData["SuccessMessage"] = "Cập nhật thành công !";
                }    
                
                return RedirectToAction("Index");
              
            }
        }
    }
}
