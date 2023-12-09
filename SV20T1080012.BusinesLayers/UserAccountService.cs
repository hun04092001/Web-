using SV20T1080012.DataLayers;
using SV20T1080012.DomainModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SV20T1080012.DataLayers.SQLServer;

namespace SV20T1080012.BusinessLayers
{
    public static class UserAccountService
    {
        static readonly IUserAccountDAL employeeUserAccountDB;

        static UserAccountService()
        {
            string connectionString = "server=DESKTOP-O4C41DD\\SQLEXPRESS;user id=sa; password=123;database=LiteCormmerceDB;TrustServerCertificate=True";

            employeeUserAccountDB = new DataLayers.SQLServer.EmployeeUserAccountDAL(connectionString);
        }

        public static UserAccount? Authorize(string username, string password, TypeOfAccount typeOfAccount)
        {
            switch (typeOfAccount)
            {
                case TypeOfAccount.Employee:
                    return employeeUserAccountDB.Authorize(username, password);
                default: return null;
            }
        }

       
        public static bool ChangePassword(string username, string password, TypeOfAccount typeOfAccount)
        {
            switch (typeOfAccount)
            {
                case TypeOfAccount.Employee:
                    return employeeUserAccountDB.ChangePassword(username, password);
                default: return false;
            }
        }
    }
    

    public enum TypeOfAccount
    {
        Employee,
        Customer
    }
}