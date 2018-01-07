using EasyObjects.Console.BLL;
using System.Configuration;

namespace EasyObjects.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Employees employees = new Employees
            {
                DefaultCommandType = System.Data.CommandType.Text,
                ConnectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString
            };
            Customers customers = new Customers
            {
                DefaultCommandType = System.Data.CommandType.Text,
                ConnectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString
            };
            Products products = new Products
            {
                DefaultCommandType = System.Data.CommandType.Text,
                ConnectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString
            };

            if (employees.LoadAll())
            {
                do
                {
                    System.Console.WriteLine($"{employees.s_EmployeeID} {employees.s_LastName}, {employees.s_FirstName}");
                } while (employees.MoveNext());
            }
            else
            {
                System.Console.WriteLine(employees.ErrorMessage);
            }

            if (customers.LoadAll())
            {
                do
                {
                    System.Console.WriteLine($"{customers.s_CustomerID} {customers.s_CompanyName}, {customers.s_ContactName}");
                } while (customers.MoveNext());
            }
            else
            {
                System.Console.WriteLine(customers.ErrorMessage);
            }

            if (products.LoadAll())
            {
                do
                {
                    System.Console.WriteLine($"{products.s_ProductID} {products.s_ProductName}, {products.s_UnitsInStock}");
                } while (products.MoveNext());
            }
            else
            {
                System.Console.WriteLine(products.ErrorMessage);
            }

            System.Console.WriteLine("\nPress <Enter> to continue...");
            System.Console.ReadLine();
        }
    }
}
