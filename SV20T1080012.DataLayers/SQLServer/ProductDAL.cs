using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SV20T1080012.DomainModels;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace SV20T1080012.DataLayers.SQLServer
{
    public class ProductDAl : _BaseDAL, ICommonDAL<Product>

    {
        public ProductDAl(string connectionString) : base(connectionString)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>

        public int Add(Product data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Products where ProductID = @ProductID)
                                select -1
                            else
                                begin
                                    INSERT INTO Products(ProductName,CategoryID,ProductDescription,SupplierID,Unit,Price,Photo)
                                    VALUES(@ProductName,@CategoryID,@ProductDescription,@SupplierID,@Unit,@Price,@Photo);
                                    select @@identity;
                                end";
                var parameters = new
                {
                    ProductName = data.ProductName ?? "",
                    CategoryID = data.CategoryID ,
                    SupplierID = data.SupplierID ,
                    ProductDescription = data.ProductDescription ?? "",
                    Unit = data.Unit ?? "",
                    Price = data.Price ?? "",
          
                    Photo = data.Photo ?? ""

                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return id;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public int Count(string searchValue = "")
        {
            int count = 0;
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";
            using (var connection = OpenConnection())
            {
                var sql = @"select count(*) from Products
                    where (@searchValue = N'') or (ProductName like @searchValue)";
                var parameters = new { searchValue = searchValue };
                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }

            return count;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"DELETE FROM Products WHERE ProductID = @ProductID
                                        AND NOT EXISTS(SELECT * FROM ProductAttributes WHERE ProductID = @ProductID)
                                        AND NOT EXISTS(SELECT * FROM ProductPhotos WHERE ProductID = @ProductID)
                                        AND NOT EXISTS(SELECT * FROM OrderDetails WHERE ProductID = @ProductID)";
                var parameters = new { ProductID = id };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Product? Get(int id)
        {
            Product? data = null;
            using (var connection = OpenConnection())
            {
                var sql = "select * from Products where ProductID = @ProductID";
                var parameters = new { ProductID = id };
                data = connection.QueryFirstOrDefault<Product>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool InUsed(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT CASE WHEN EXISTS(
                                                    (SELECT * FROM ProductAttributes WHERE ProductID = @ProductID)
                                                AND (SELECT * FROM ProductPhotos WHERE ProductID = @ProductID)
                                                AND (SELECT * FROM OrderDetails WHERE ProductID = @ProductID)
                                    )
                                                THEN 1 ELSE 0 END";
                var parameters = new { ProductID = id };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IList<Product> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Product> data;
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";
            using (var connection = OpenConnection())
            {
                var sql = @"with cte as
                (
                    select ProductID,ProductName , ProductDescription, SupplierID, CategoryID, Unit, Price, Photo,IsSelling,
                    ROW_NUMBER() over (order by ProductName,SupplierID,CategoryID ) as RowNumber
                    from Products
                    where (@searchValue = N'') or (ProductName like @searchValue)
                               
                )

                select * from cte
                where (@pageSize = 0)
                    or (RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)
                    order by RowNumber";
                var parameters = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue
                };
                data = connection.Query<Product>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();

                connection.Close();
            }
            return data;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool Update(Product data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if not exists(select * from Products where ProductID <> @ProductID )
                                begin
                                    UPDATE Products
                                    SET ProductName = @ProductName, 
                                        SupplierID  = @SupplierID, 
                                        CategoryID  = @CategoryID, 
                                        Unit        = @Unit, 
                                        Price       = @Price, 
                                        Photo       = @Photo
                                    WHERE ProductID = @ProductID"";
                                end";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    ProductName = data.ProductName ?? "",
                    SupplierID = data.SupplierID ,
                    CategoryID = data.CategoryID ,
                    Unit = data.Unit ?? "",
                    Price = data.Price ?? "",
                    photo = data.Photo ?? ""

                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
            }
            return result;
        }

       
    }
}
