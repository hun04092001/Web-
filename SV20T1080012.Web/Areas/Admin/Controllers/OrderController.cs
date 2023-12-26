using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.CodeAnalysis;
using SV20T1080012.BusinessLayers;
using SV20T1080012.DomainModels;
using SV20T1080012.Web;
using SV20T1080012.Web.AppCodes;
using SV20T1080012.Web.Models;
using System.Drawing.Printing;
namespace SV20T1080012.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    [Authorize(Roles = $"{WebUserRoles.Administrator}")]// chuyển đến đăng nhập
    [Area("Admin")]

    public class OrderController : Controller
    {
        
        private const string Order_Search = "Order_Search";
        public const int Page_Size = 10; // Tạo một biến hằng để đồng bộ thuộc tính cho trang web.
        private const string SHOPPING_CART = "ShoppingCart";
        private const string ERROR_MESSAGE = "ErrorMessage";
        private const string Product_Search = "Product_Search";

        /// <summary>
        /// tìm kiếm phân trang
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var input = ApplicationContext.GetSessionData<PaginationSearchOrderInput>(Order_Search);
            if (input == null)
            {
                input = new PaginationSearchOrderInput()
                {
                    Page = 1,
                    Status = 0,
                    PageSize = Page_Size,
                    SearchValue = ""
                    
                };
            }

            return View(input);
        }

        /// <summary>
        /// Hàm trả về danh sách tìm kiếm
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public IActionResult Search(PaginationSearchOrderInput input)
        {

            int rowCount = 0;
            var data = OrderService.ListOrders(input.Page, input.PageSize,input.Status,input.SearchValue, out rowCount);
            var model = new PaginationSearchOrder()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
                
            };
            ApplicationContext.SetSessionData(Order_Search, input);//lưu lại điều kiện tìm kiếm

            //string errorMessage = Convert.ToString(TempData["ErrorMessage"]);
            //ViewBag.ErrorMessage = errorMessage;
            //string deletedMessage = Convert.ToString(TempData["DeletedMessage"]);
            //ViewBag.DeletedMessage = deletedMessage;
            //string savedMessage = Convert.ToString(TempData["SavedMessage"]);
            //ViewBag.SavedMessage = savedMessage;

            return View(model);
        }
        /// <summary>
        /// Giao diện trang chi tiết đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Details(int id = 0)
        {
            //DONE: Code chức năng lấy và hiển thị thông tin của đơn hàng và chi tiết của đơn hàng
            if (id < 0)
            {
                return RedirectToAction("Index");
               
            }
            // lấy thông tin của một đơn hàng và chi tiết đơn hàng đó theo mã đơn hàng
            var order = OrderService.GetOrder(id);
            var orderDetails = OrderService.ListOrderDetails(id);

            var data = new OrderDetailModels()
            {
                Order = order,
                OrderDetails = orderDetails
            };
            ViewBag.ErrorMessage = TempData[ERROR_MESSAGE] ?? "";
            return View(data);
        }
        /// <summary>
        /// Giao diện cập nhật thông tin chi tiết đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <param name="productId"></param>
        /// <returns></returns>        
        [HttpGet]
        public ActionResult EditDetail(int orderID = 0, int productID = 0)
        {
            //TODO: Code chức năng để lấy chi tiết đơn hàng cần edit
            if (orderID < 0)
            {
                return RedirectToAction("Index");
            }
            if (productID < 0)
            {
                return RedirectToAction("Details", new { id = orderID });
            }
            OrderDetail orderDetail = OrderService.GetOrderDetail(orderID, productID);
            if (orderDetail == null)
            {
                return RedirectToAction("Index");
            }
            return View(orderDetail);
        }
        public IActionResult CreateDetail(int id = 0)
        {
            var model = new OrderDetail()
            {
                OrderID = id,
                ProductID = 0
            };
            return View(model);
        }

        /// <summary>
        /// Cập nhật chi tiết đơn hàng (trong giỏ hàng)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="productId"></param>
        /// <param name="quantity">số lượng</param>
        /// <param name="salePrice">Đơn giá</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UpdateDetail(OrderDetail data)
        {
            // mã đặt hàng
            if (data.ProductID <= 0)
            {
                TempData[ERROR_MESSAGE] = "mã đặt hàng không tồn tại";
                return RedirectToAction("Details", new { id = data.OrderID });
            }
            // Số lượng
            if (data.Quantity < 1)
            {
                TempData[ERROR_MESSAGE] = "Số lượng không tồn tại";
                return RedirectToAction("Details", new { id = data.OrderID });
            }

            // Đơn giá
            if (data.SalePrice < 1)
            {
                TempData[ERROR_MESSAGE] = "Đơn giá không tồn tại";
                return RedirectToAction("Details", new { id = data.OrderID });
            }

            // Cập nhật chi tiết 1 đơn hàng nếu kiểm tra đúng hết
            OrderService.SaveOrderDetail(data.OrderID, data.ProductID, data.Quantity, data.SalePrice);
            return RedirectToAction("Details", new { id = data.OrderID });
        }
        /// <summary>
        /// Xóa 1 mặt hàng khỏi giỏ hàng
        /// </summary>
        /// <param name="id"></param>
        /// <param name="productID"></param>
        /// <returns></returns>     
        //[HttpGet]
        //[Route("DeleteDetail/{orderID:int}/{productID:int}")]
        public IActionResult DeleteDetail(int orderID = 0, int productID = 0)
        {
           
            //TODO: Code chức năng xóa 1 chi tiết trong đơn hàng
            if (orderID < 0)
            {
                return RedirectToAction("Index");
            }
            if (productID < 0)
            {
                return RedirectToAction("Details", new { id = orderID });
            }
            bool Delete = OrderService.DeleteOrderDetail(orderID, productID);
            if (!Delete)
            {
                return RedirectToAction("Details", new { id = orderID });
            }
            return RedirectToAction("Details", new {id = orderID});

        }
        /// <summary>
        /// Xóa đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Delete(int id = 0)
        {
            //TODO: Code chức năng để xóa đơn hàng (nếu được phép xóa)

            if (id < 0)
            {
                return RedirectToAction("Index");
            }
            Order model = OrderService.GetOrder(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            // Xoá đơn hàng ở trạng thái vừa tạo, bị huỷ hoặc bị từ chối
            if (model.Status == OrderStatus.INIT
                || model.Status == OrderStatus.CANCEL
                || model.Status == OrderStatus.REJECTED)
            {
                OrderService.DeleteOrder(id);
                return RedirectToAction("Index");
            }
            return RedirectToAction("Details", new { OrderID = id });
            
        }
        /// <summary>
        /// chấp nhận đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Accept(int id = 0)
        {
            //TODO: Duyệt chấp nhận đơn hàng

            if (id < 0)
            {
                return RedirectToAction("Index");
            }
            Order data = OrderService.GetOrder(id);
            if (data == null)
            {
                return RedirectToAction("Index");
            }
            bool isAccepted = OrderService.AcceptOrder(id);
            if (!isAccepted)
            {
                return RedirectToAction("Details", new { id = data.OrderID });
            }

            return RedirectToAction("Details", new { id = id});
        }
        /// <summary>
        /// Xác nhận chuyển đơn hàng cho shipper
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Shipping(int id = 0, int shipperID = 0, int countProducts = 0)
        {
            if (id < 0)
            {
                return RedirectToAction("Index");
            }
            if (Request.Method == "GET")
            {
                ViewBag.OrderID = id;
                ViewBag.CountProducts = countProducts;
                return View();
            }

            Order data = OrderService.GetOrder(id);
            if (data == null)
            {
                return RedirectToAction("Index");
            }
            if (shipperID <= 0)
            {
                TempData[ERROR_MESSAGE] = "Bạn phải chọn đơn vị vận chuyển";
                return RedirectToAction("Details", new { id = id });
            }
            if (countProducts <= 0)
            {
                TempData[ERROR_MESSAGE] = "Không có mặt hàng nào để chuyển giao";
                return RedirectToAction("Details", new { id = id });
            }
            bool shipped = OrderService.ShipOrder(id, shipperID);
            if (!shipped)
            {
                TempData[ERROR_MESSAGE] =
                    $"Xác nhận chuyển đơn hàng cho người giao hàng thất bại vì trạng thái đơn hàng hiện tại là: {data.StatusDescription}";
                return RedirectToAction("Details", new { id = data.OrderID });
            }
            return RedirectToAction("Details", new { id = id });
        }
        /// <summary>
        /// Hoàn tất đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Finish(int id = 0)
        {
            //TODO: Code chức năng ghi nhận hoàn tất đơn hàng (nếu được phép)
            if (id < 0)
            {
                return RedirectToAction("Index");
            }

            Order data = OrderService.GetOrder(id);
            if (data == null)
            {
                return RedirectToAction("Index");
            }
            bool finish = OrderService.FinishOrder(id);
            if (!finish)
            {
                TempData[ERROR_MESSAGE] =
                    $"toàn tất đơn hàng thất bại";
                return RedirectToAction("Details", new { id = data.OrderID });
            }

            return RedirectToAction("Details", new { id = id });
        }
        /// <summary>
        /// Hủy bỏ đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Cancel(int id = 0)
        {
            //TODO: Code chức năng hủy đơn hàng(nếu được phép)
            if (id < 0)
            {
                return RedirectToAction("Index");
            }

            Order data = OrderService.GetOrder(id);
            if (data == null)
            {
                return RedirectToAction("Index");
            }
            bool Cancel = OrderService.CancelOrder(id);
            if (!Cancel)
            {
                TempData[ERROR_MESSAGE] =
                    $"Hủy đơn hàng thất bại";
                return RedirectToAction("Details", new { id = data.OrderID });
            }

            return RedirectToAction("Details", new { id = id });
        }
        /// <summary>
        /// Từ chối đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Reject(int id = 0)
        {
            //TODO: Code chức năng từ chối đơn hàng (nếu được phép)
            if (id < 0)
            {
                return RedirectToAction("Index");
            }

            Order data = OrderService.GetOrder(id);
            if (data == null)
            {
                return RedirectToAction("Index");
            }
            bool Reject = OrderService.RejectOrder(id);
            if (!Reject)
            {
                TempData[ERROR_MESSAGE] =
                    $"Từ chối đơn hàng thất bại";
                return RedirectToAction("Details", new { id = data.OrderID });
            }

            return RedirectToAction("Details", new { id = data.OrderID });
        }
        /// <summary>
        /// Sử dụng 1 biến session để lưu tạm giỏ hàng (danh sách các chi tiết của đơn hàng) trong quá trình xử lý.
        /// Hàm này lấy giỏ hàng hiện đang có trong session (nếu chưa có thì tạo mới giỏ hàng rỗng)
        /// </summary>
        /// <returns></returns>
        private List<OrderDetail> GetShoppingCart()
        {
            List<OrderDetail> shoppingCart = ApplicationContext.GetSessionData<List<OrderDetail>>(SHOPPING_CART);
            if (shoppingCart == null)
            {
                shoppingCart = new List<OrderDetail>();
                ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            }
            return shoppingCart;
        }
        /// <summary>
        /// Giao diện trang tạo đơn hàng
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            ViewBag.ErrorMessage = TempData[ERROR_MESSAGE] ?? "";
            return View(GetShoppingCart());
        }

        /// <summary>
        /// Tìm kiếm mặt hàng để bổ sung vào giỏ hàng
        /// </summary>        
        /// <returns></returns>
        [HttpPost]
        public IActionResult SearchProducts(int page = 1, string searchValue = "")
        {
            var input = ApplicationContext.GetSessionData<PaginationSearchProductInput>(Product_Search);
            if (input == null)
            {
                input = new PaginationSearchProductInput()
                {
                    Page = 1,
                    PageSize = Page_Size,
                    SearchValue = "",
                    CategoryID = 0,
                    SupplierID = 0,
                    MinPrice = 0,
                    MaxPrice = 0,
                };
            }
            int rowCount = 0;
            var data = ProductDataService.ListProducts(page, Page_Size, searchValue, 0, 0, 0, 0, out rowCount);
            ViewBag.Page = page;
            return View(data);
        }
        /// <summary>
        /// Bổ sung thêm hàng vào giỏ hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddToCart(OrderDetail data)
        {
            if (data == null)
            {
                TempData[ERROR_MESSAGE] = "Dữ liệu không hợp lệ";
                return RedirectToAction("Create");
            }
            if (data.SalePrice <= 0 || data.Quantity <= 0)
            {
                TempData[ERROR_MESSAGE] = "Giá bán và số lượng không hợp lệ";
                return RedirectToAction("Create");
            }

            List<OrderDetail> shoppingCart = GetShoppingCart();
            var existsProduct = shoppingCart.FirstOrDefault(m => m.ProductID == data.ProductID);

            if (existsProduct == null) //Nếu mặt hàng cần được bổ sung chưa có trong giỏ hàng thì bổ sung vào giỏ
            {
                shoppingCart.Add(data);
            }
            else //Trường hợp mặt hàng cần bổ sung đã có thì tăng số lượng và thay đổi đơn giá
            {
                existsProduct.Quantity += data.Quantity;
                existsProduct.SalePrice = data.SalePrice;
            }
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return RedirectToAction("Create");
        }
        /// <summary>
        /// Xóa 1 mặt hàng khỏi giỏ hàng
        /// </summary>        
        /// <returns></returns>
        public ActionResult RemoveFromCart(int id = 0)
        {
            List<OrderDetail> shoppingCart = GetShoppingCart();
            int index = shoppingCart.FindIndex(m => m.ProductID == id);
            if (index >= 0)
                shoppingCart.RemoveAt(index);
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return RedirectToAction("Create");
        }
        /// <summary>
        /// Xóa toàn bộ dữ liệu trong giỏ hàng
        /// </summary>
        /// <returns></returns>
        public ActionResult ClearCart()
        {
            List<OrderDetail> shoppingCart = GetShoppingCart();
            shoppingCart.Clear();
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return RedirectToAction("Create");
        }
        /// <summary>
        /// Khởi tạo đơn hàng và chuyển đến trang Details sau khi khởi tạo xong để tiếp tục quá trình xử lý đơn hàng
        /// </summary>        
        /// <returns></returns>
        [HttpPost]
        public IActionResult Init(int customerID = 0, int employeeID = 0)
        {
            List<OrderDetail> shoppingCart = GetShoppingCart();
            if (shoppingCart == null || shoppingCart.Count == 0)
            {
                TempData[ERROR_MESSAGE] = "Không thể tạo đơn hàng với giỏ hàng trống";
                return RedirectToAction("Create");
            }

            if (customerID == 0 || employeeID == 0)
            {
                TempData[ERROR_MESSAGE] = "Vui lòng chọn khách hàng và nhân viên phụ trách";
                return RedirectToAction("Create");
            }

            int orderID = OrderService.InitOrder(customerID, employeeID, DateTime.Now, shoppingCart);

            HttpContext.Session.Remove("SHOPPING_CART");
            return RedirectToAction("Details", new { id = orderID });
        }
    }
}
