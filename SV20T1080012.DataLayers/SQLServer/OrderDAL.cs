using Azure;
using Dapper;
using Microsoft.Data.SqlClient;
using SV20T1080012.DomainModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1080012.DataLayers.SQLServer
{
    /// <summary>
    /// 
    /// </summary>
    /// <summary>
    /// 
    /// </summary>
    public class OrderDAL : _BaseDAL, IOrderDAL
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        public OrderDAL(string connectionString) : base(connectionString)
        {
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public int Add(Order data, IEnumerable<OrderDetail> details)
        {
            int orderID = 0;
            //Tạo đơn hàng mới trong CSDL
            using (var connection = OpenConnection())
            {
                var sqlAddOrder = @"if exists(select * from Orders where OrderID = @OrderID)
                                select -1
                            else
                                begin
                                  INSERT INTO Orders(CustomerID, OrderTime, EmployeeID, AcceptTime, ShipperID, ShippedTime, FinishedTime, Status,DeliveryAddress)
                                         VALUES(@CustomerID, @OrderTime, @EmployeeID, @AcceptTime, @ShipperID, @ShippedTime, @FinishedTime, @Status,@DeliveryAddress);
                                         SELECT @@identity;  
                                end";
                var parameters = new
                {
                    OrderID = data.OrderID,
                    CustomerID = data.CustomerID,
                    OrderTime = data.OrderTime,
                    EmployeeID  = data.EmployeeID,
                    AcceptTime = data.AcceptTime,
                    ShipperID = data.ShipperID,
                    ShippedTime = data.ShippedTime,
                    FinishedTime = data.FinishedTime,
                    Status  = data.Status,
                    DeliveryAddress = data.DeliveryAddress



                };
                orderID = connection.ExecuteScalar<int>(sql: sqlAddOrder, param: parameters, commandType: CommandType.Text);
                //Bổ sung chi tiết cho đơn hàng có mã là orderID
                
                var sqlAddOrderDetail = @"INSERT INTO OrderDetails(OrderID, ProductID, Quantity, SalePrice) " +
                                         "VALUES(@OrderID, @ProductID, @Quantity, @SalePrice)";
                foreach (var item in details)
                {
                    var orderDetailsparameters = new
                    {
                        orderID = orderID,
                        productID = item.ProductID,
                        quantity = item.Quantity,
                        salePrice = item.SalePrice,
                    };
                    connection.Execute(sqlAddOrderDetail, orderDetailsparameters);
                }

                connection.Close();
            };
            return orderID;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public int Count(int status = 0, string searchValue = "")
        {
            int count = 0;
           
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT  COUNT(*)
                                    FROM    Orders as o
                                            LEFT JOIN Customers AS c ON o.CustomerID = c.CustomerID
                                            LEFT JOIN Employees AS e ON o.EmployeeID = e.EmployeeID
                                            LEFT JOIN Shippers AS s ON o.ShipperID = s.ShipperID
                                    WHERE   (@Status = 0 OR o.Status = @Status)
                                        AND (@SearchValue = N'' OR c.CustomerName LIKE @SearchValue OR s.ShipperName LIKE @SearchValue)";
                var parameters = new {
                                        Status = status,
                                        SearchValue = searchValue
                                        
                                    };
                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return count;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public bool Delete(int orderID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"DELETE FROM OrderDetails WHERE OrderID = @OrderID;
                            DELETE FROM Orders WHERE OrderID = @OrderID;";
                var parameters = new { OrderID = orderID };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;


        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="productID"></param>
        /// <returns></returns>
        public bool DeleteDetail(int orderID, int productID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"DELETE FROM OrderDetails WHERE OrderID = @OrderID AND ProductID = @ProductID";
                var parameters = new { OrderID = orderID,
                                       ProductID = productID
                                     };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public Order Get(int orderID)
        {
            Order? data = null;

            using (var connection = OpenConnection())
            {
                var sql = @"SELECT  o.*,  
                                            c.CustomerName,
                                            c.ContactName as CustomerContactName,
                                            c.Address as CustomerAddress,
                                            c.Email as CustomerEmail,
                                             e.FullName as EmployeeFullName,
                                            s.ShipperName,
                                            s.Phone as ShipperPhone
                                    FROM    Orders as o
                                            LEFT JOIN Customers AS c ON o.CustomerID = c.CustomerID
                                            LEFT JOIN Employees AS e ON o.EmployeeID = e.EmployeeID
                                            LEFT JOIN Shippers AS s ON o.ShipperID = s.ShipperID
                                    WHERE   o.OrderID = @OrderID";
                var parameters = new { OrderID = orderID };
                data = connection.QueryFirstOrDefault<Order>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="productID"></param>
        /// <returns></returns>
        public OrderDetail GetDetail(int orderID, int productID)
        {
            OrderDetail? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT	od.*, p.ProductName, p.Unit, p.Photo		
                                    FROM	OrderDetails AS od
		                                    JOIN Products AS p ON od.ProductID = p.ProductID
                                    WHERE	od.OrderID = @OrderID AND od.ProductID = @ProductID";
                var parameters = new { OrderID = orderID,
                                       ProductID = productID };
                data = connection.QueryFirstOrDefault<OrderDetail>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="status"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public IList<Order> List(int page = 1, int pageSize = 0, int status = 0, string searchValue = "")
        {

           var data = new List<Order>();
            if (searchValue != "")
                searchValue = "%" + searchValue + "%";

            using (var connection = OpenConnection())
            {
                var sql = @"SELECT  *
                                    FROM     (
                                            SELECT  o.*,
                                                    c.CustomerName,
                                                    c.ContactName as CustomerContactName,
                                                    c.Address as CustomerAddress,
                                                    c.Email as CustomerEmail,
                                                    e.FullName as EmployeeFullName,
                                                    s.ShipperName,
                                                    s.Phone as ShipperPhone,
                                                    ROW_NUMBER() OVER(ORDER BY o.OrderID DESC) AS RowNumber
                                            FROM    Orders as o
                                                    LEFT JOIN Customers AS c ON o.CustomerID = c.CustomerID
                                                    LEFT JOIN Employees AS e ON o.EmployeeID = e.EmployeeID
                                                    LEFT JOIN Shippers AS s ON o.ShipperID = s.ShipperID
                                            WHERE   (@Status = 0 OR o.Status = @Status)
                                                AND (@SearchValue = N'' OR c.CustomerName LIKE @SearchValue OR s.ShipperName LIKE @SearchValue)
                                            ) AS t
                                    WHERE (@PageSize = 0) OR (t.RowNumber BETWEEN(@Page -1)*@PageSize + 1 AND @Page*@PageSize)
                                    ORDER BY t.RowNumber";
                var parameters = new
                {
                    page = page,
                    pageSize = pageSize,
                    Status = status,
                    searchValue = searchValue
                };
                data = (connection.Query<Order>(sql: sql, param: parameters, commandType: CommandType.Text)).ToList();
                connection.Close();
            }
            if (data == null)
                data = new List<Order>();
            return data;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public IList<OrderDetail> ListDetails(int orderID)
        {
            var data = new List<OrderDetail>();
            using (var connection = OpenConnection())
            {

                var sql = @"SELECT	od.*, p.ProductName, p.Unit, p.Photo		
                                    FROM	OrderDetails AS od
		                                    JOIN Products AS p ON od.ProductID = p.ProductID
                                    WHERE	od.OrderID = @OrderID";
                var parameters = new
                {
                    orderID = orderID
                    
                };
                data = (connection.Query<OrderDetail>(sql: sql, param: parameters, commandType: CommandType.Text)).ToList();
                connection.Close();

            }
            return data;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>

        public int SaveDetail(int orderID, int productID, int quantity, decimal salePrice)
        {
            int result = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"
                       -- Kiểm tra sự tồn tại của productId trong đơn hàng orderId
                            IF EXISTS (SELECT 1 FROM OrderDetails WHERE OrderID = @orderID AND ProductID = @productID)
                            BEGIN
                                -- Nếu tồn tại, cập nhật số lượng bằng tổng của số lượng hiện tại và @quantity
                                UPDATE OrderDetails
                                SET Quantity = @quantity,
                                    SalePrice = @salePrice
                                WHERE OrderID = @orderID AND ProductID = @productID;
                            END
                            ELSE
                            BEGIN
                                -- Nếu không tồn tại, thực hiện truy vấn INSERT mới
                                INSERT INTO OrderDetails (OrderID, ProductID, Quantity, SalePrice)
                                VALUES (@orderID, @productID, @quantity, @salePrice);
                            END;

                            -- Trả về số dòng được ảnh hưởng
                            SELECT @@ROWCOUNT AS AffectedRows;";

                var parameters = new
                {
                    orderID = orderID,
                    productID = productID,
                    quantity = quantity,
                    salePrice = salePrice
                };

                result = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Update(Order data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"UPDATE Orders
                                    SET     CustomerID = @CustomerID,
                                            OrderTime = @OrderTime,
                                            EmployeeID = @EmployeeID,
                                            AcceptTime = @AcceptTime,
                                            ShipperID = @ShipperID,
                                            ShippedTime = @ShippedTime,
                                            FinishedTime = @FinishedTime,
                                            Status = @Status
                                    WHERE   OrderID = @OrderID";
                var parameters = new
                {
                    OrderID = data.OrderID,
                    CustomerID = data.CustomerID ,
                    Ordertime = data.OrderTime,
                    EmployeeID = data.EmployeeID,
                    AcceptTime = data.AcceptTime,
                    ShipperID = data.ShipperID,
                    ShippedTime = data.ShippedTime,
                    FinishedTime = data.FinishedTime,
                    Status = data.Status

                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
            }
            return result;
        }
    } 
}
