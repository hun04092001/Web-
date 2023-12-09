using SV20T1080012.DataLayers;
using SV20T1080012.DataLayers.SQLServer;
using SV20T1080012.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1080012.BusinessLayers
{
    public static class CommonDataService
    {
        private static readonly ICommonDAL<Customer> customerDB;
        private static readonly ICommonDAL<Supplier> supplierDB;
        private static readonly ICommonDAL<Category> categoryDB;
        private static readonly ICommonDAL<Shipper> shipperDB;
        private static readonly ICommonDAL<Employee> employeeDB;
        private static readonly ICommonDAL<Province> provinceDB;
        private static readonly ICommonDAL<Product> productDB;

        /// <summary>
        /// Ctor
        /// </summary>
        static CommonDataService()
        {
            string connectionString = "server=DESKTOP-O4C41DD\\SQLEXPRESS;user id=sa; password=123;database=LiteCormmerceDB;TrustServerCertificate=True";
            customerDB = new DataLayers.SQLServer.CustomerDAL(connectionString);
            supplierDB = new DataLayers.SQLServer.SupplierDAL(connectionString);
            categoryDB = new DataLayers.SQLServer.CategoryDAL(connectionString);
            shipperDB  = new DataLayers.SQLServer.ShipperDAL(connectionString);
            employeeDB = new DataLayers.SQLServer.EmployeeDAL(connectionString);
            provinceDB = new DataLayers.SQLServer.ProvinceDAL(connectionString);
            productDB = new DataLayers.SQLServer.ProductDAl(connectionString);
        }
        public static List<Customer> ListOfCustommers( out int rowCount,
                                                       int page = 1,
                                                       int pageSize = 0,
                                                       string searchValue = "")
        {
            rowCount = customerDB.Count(searchValue);
            return customerDB.List(page, pageSize,searchValue).ToList();
        }
        public static List<Supplier> ListOfSuppliers(
                                                       out int rowCount,
                                                       int page = 1,
                                                       int pageSize = 0,
                                                       string searchValue = ""
                                                    )
        {
            rowCount = supplierDB.Count(searchValue);
            return supplierDB.List(page, pageSize, searchValue).ToList();
        }
        public static List<Category> ListOfCategorys(
                                                      out int rowCount,
                                                      int page = 1,
                                                      int pageSize = 0,
                                                      string searchValue = ""
                                                   )
        {
            rowCount = categoryDB.Count(searchValue);
            return categoryDB.List(page, pageSize, searchValue).ToList();
        }
        public static List<Shipper> ListOfShippers(
                                                      out int rowCount,
                                                      int page = 1,
                                                      int pageSize = 0,
                                                      string searchValue = ""
                                                   )
        {
            rowCount = shipperDB.Count(searchValue);
            return shipperDB.List(page, pageSize, searchValue).ToList();
        }
        public static List<Employee> ListOfEmployees(
                                                      out int rowCount,
                                                      int page = 1,
                                                      int pageSize = 0,
                                                      string searchValue = ""
                                                   )
        {
            rowCount = employeeDB.Count(searchValue);
            return employeeDB.List(page, pageSize, searchValue).ToList();
        }
        public static List<Province> ListOfProvinces()
        {
            return provinceDB.List().ToList();
        }
        public static List<Product> ListOfProducts(out int rowCount,
                                                      int page = 1,
                                                      int pageSize = 0,
                                                      string searchValue = "")
        {
            rowCount = productDB.Count(searchValue);
            return productDB.List(page, pageSize, searchValue).ToList();
        }

        public static Customer? GetCustomer(int id)
        {
            return customerDB.Get(id);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddCustomer(Customer data)
        {
            return customerDB.Add(data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateCustomer(Customer data)
        {
            return customerDB.Update(data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteCustomer(int id)
        {
            return customerDB.Delete(id);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool InUsedCustomer(int id)
        {
            return customerDB.InUsed(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Supplier? GetSupplier(int id)
        {
            return supplierDB.Get(id);
        }
        /// <summary>
        /// Thêm nhà cung cấp
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddSupplier(Supplier data)
        {
            return supplierDB.Add(data);
        }
        /// <summary>
        /// cập nhật thông tin nhà cung cấp
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateSupplier(Supplier data)
        {
            return supplierDB.Update(data);
        }
        /// <summary>
        /// Xóa nhà cung cấp
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteSupplier(int id)
        {
            return supplierDB.Delete(id);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool InUsedSupplier(int id)
        {
            return supplierDB.InUsed(id);
        }

        // Loại hàng
        public static Category? GetCategory(int id)
        {
            return categoryDB.Get(id);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddCategory(Category data)
        {
            return categoryDB.Add(data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateCategory(Category data)
        {
            return categoryDB.Update(data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteCategory(int id)
        {
            return categoryDB.Delete(id);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool InUsedCategory(int id)
        {
            return categoryDB.InUsed(id);
        }
        // Shipper
        public static Shipper? GetShipper(int id)
        {
            return shipperDB.Get(id);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddShipper(Shipper data)
        {
            return shipperDB.Add(data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateShipper(Shipper data)
        {
            return shipperDB.Update(data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteShipper(int id)
        {
            return shipperDB.Delete(id);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool InUsedShipper(int id)
        {
            return shipperDB.InUsed(id);
        }
        // Employee

        public static Employee? GetEmployee(int id)
        {
            return employeeDB.Get(id);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddEmployee(Employee data)
        {
            return employeeDB.Add(data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateEmployee(Employee data)
        {
            return employeeDB.Update(data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteEmployee(int id)
        {
            return employeeDB.Delete(id);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool InUsedEmployee(int id)
        {
            return employeeDB.InUsed(id);
        }

        public static Product? GetProduct(int id)
        {
            return productDB.Get(id);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddProduct(Product data)
        {
            return productDB.Add(data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateProduct(Product data)
        {
            return productDB.Update(data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteProduct(int id)
        {
            return productDB.Delete(id);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool InUsedProduct(int id)
        {
            return productDB.InUsed(id);
        }


    }
}
