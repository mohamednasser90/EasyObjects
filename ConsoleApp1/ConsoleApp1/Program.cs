using System;
using System.Data.SqlClient;
using Dapper;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Northwind;Integrated Security=True;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"
SELECT [RegionID]
      ,[RegionDescription]
  FROM [Northwind].[dbo].[Region]";

                var list = conn.Query<Region>(sql);

                Console.WriteLine("ID\tDescription");
                foreach (var l in list)
                {
                    Console.WriteLine("{0}\t{1}", l.RegionID, l.RegionDescription);
                }
            }

            Console.WriteLine("Press <Enter> to continue...");
            Console.ReadLine();
        }
    }
}