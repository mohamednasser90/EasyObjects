using EasyObjects.Console.BLL;
using System;
using System.Configuration;

namespace EasyObjects.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            DemoEmployees();
            DemoCustomers();
            DemoProducts();
            DemoProductsView();

            System.Console.WriteLine("\nPress <Enter> to continue...");
            System.Console.ReadLine();
        }

        private static void PrintBanner(string method)
        {
            System.Console.WriteLine("*************************************************");
            System.Console.WriteLine($" Method: {method}");
            System.Console.WriteLine("*************************************************");
        }

        private static void DemoProductsView()
        {
            // Note: no need to set the DefaultCommmandType, custom queries are always run as inline SQL
            AlphabeticalListOfProducts view = new AlphabeticalListOfProducts
            {
                ConnectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString
            };

            view.Where.ProductName.Value = "M";
            view.Where.ProductName.Operator = NCI.EasyObjects.WhereParameter.Operand.StartsWith;

            PrintBanner(System.Reflection.MethodBase.GetCurrentMethod().Name);
            if (view.Query.Load())
            {
                System.Console.WriteLine($"Generated query:\n{view.Query.LastQuery}");
                do
                {
                    System.Console.WriteLine($"{view.s_ProductID}, {view.s_ProductName}\t\t{view.UnitPrice:C}");
                } while (view.MoveNext());
            }
            else
            {
                System.Console.WriteLine(view.ErrorMessage);
            }
        }

        private static void DemoCustomers()
        {
            Customers cust = new Customers
            {
                DefaultCommandType = System.Data.CommandType.Text,
                ConnectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString
            };

            PrintBanner(System.Reflection.MethodBase.GetCurrentMethod().Name);
            if (cust.LoadByPrimaryKey("CACTU"))
            {
                do
                {
                    System.Console.WriteLine($"{cust.s_CustomerID} {cust.s_CompanyName}, {cust.s_ContactName}");
                } while (cust.MoveNext());
            }
            else
            {
                System.Console.WriteLine(cust.ErrorMessage);
            }
        }

        private static void DemoProducts()
        {
            Products prod = new Products
            {
                DefaultCommandType = System.Data.CommandType.Text,
                ConnectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString
            };

            // Call AddNew() to add a new row to the EasyObject. You must fill in all 
            // required fields or an error will result when you call Save().
            prod.AddNew();

            // We're going to provide our own IDENTITY column for this record
            prod.IdentityInsert = true;

            // Note the use of the 's_' fields, which take strings as arguments. If this object
            // were being loaded from TextBox objects on a WinForm, you don't have to worry about
            // the datatype because this is handled for you in EasyObjects
            prod.s_ProductID = "78";
            prod.s_ProductName = "EasyObjects";
            prod.s_Discontinued = "True";
            prod.s_QuantityPerUnit = "10";
            prod.s_ReorderLevel = "100";
            prod.s_UnitPrice = "49.95";
            prod.s_UnitsInStock = "200";

            // Save the changes
            prod.Save();

            // Display the updated EasyObject
            PrintBanner(System.Reflection.MethodBase.GetCurrentMethod().Name);
            System.Console.WriteLine($"{prod.s_ProductID} {prod.s_ProductName}, {prod.s_UnitsInStock}");

            // Delete the new addition
            prod.MarkAsDeleted();
            prod.Save();
        }

        private static void DemoEmployees()
        {
            // Note: no need to set the DefaultCommmandType, custom queries are always run as inline SQL
            Employees employees = new Employees
            {
                ConnectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString
            };

            // Limit the columns returned by the SELECT query
            employees.Query.AddResultColumn(EmployeesSchema.LastName);
            employees.Query.AddResultColumn(EmployeesSchema.FirstName);
            employees.Query.AddResultColumn(EmployeesSchema.City);
            employees.Query.AddResultColumn(EmployeesSchema.Region);

            // Note that we can inject a custom field to the
            // dynamic query as well
            employees.Query.AddResultColumn($"{EmployeesSchema.FirstName.FieldName} + ' ' + {EmployeesSchema.LastName.FieldName} AS FullName");

            // Add an ORDER BY clause
            employees.Query.AddOrderBy(EmployeesSchema.LastName);

            // Add a NOLOCK to prevent any locks (optional)
            employees.Query.UseNoLock = true;

            // Add a WHERE clause
            employees.Where.Region.Value = "WA";

            PrintBanner(System.Reflection.MethodBase.GetCurrentMethod().Name);
            if (employees.Query.Load())
            {
                System.Console.WriteLine($"Generated query:\n{employees.Query.LastQuery}");
                do
                {
                    System.Console.WriteLine($"{employees.s_LastName}, {employees.s_FirstName}\t{employees.s_City} {employees.s_Region}");
                } while (employees.MoveNext());
            }
            else
            {
                System.Console.WriteLine(employees.ErrorMessage);
            }
        }
    }
}
