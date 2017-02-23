using System;
using System.Data.SqlClient;
using Dapper;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                InitializeDB();

                string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=Northwind;Integrated Security=True;";

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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Press <Enter> to continue...");
            Console.ReadLine();
        }

        private static void InitializeDB()
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=Master;Integrated Security=True;";
            var dbrecreated = false;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                try
                {
                    connection.Execute(@"DROP DATABASE Northwind;");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("database drop  failed - close and reopen VS and try again:" + ex.Message);
                }

                try
                {
                    connection.Execute(@"CREATE DATABASE Northwind;");
                    dbrecreated = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("database create failed - close and reopen VS and try again:" + ex.Message);
                }
            }
            if (!dbrecreated) return;

            connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=Northwind;Integrated Security=True;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                CreateTables(connection);
                CreateDefaultData(connection);
            }
        }

        private static void CreateDefaultData(SqlConnection connection)
        {
            Console.WriteLine("CreateDefaultData: Adding default values");
            connection.Execute(@"
INSERT [dbo].[Region] ([RegionID], [RegionDescription]) VALUES (1, N'Eastern                                           ');
INSERT [dbo].[Region] ([RegionID], [RegionDescription]) VALUES (2, N'Western                                           ');
INSERT [dbo].[Region] ([RegionID], [RegionDescription]) VALUES (3, N'Northern                                          ');
INSERT [dbo].[Region] ([RegionID], [RegionDescription]) VALUES (4, N'Southern                                          ');
");
        }

        private static void CreateTables(SqlConnection connection)
        {
            Console.WriteLine("CreateTables: Adding tables");
            connection.Execute(@"
CREATE TABLE [dbo].[Categories](
	[CategoryID] [int] IDENTITY(1,1) NOT NULL,
	[CategoryName] [nvarchar](15) NOT NULL,
	[Description] [ntext] NULL,
	[Picture] [image] NULL,
 CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED 
(
	[CategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
");
            connection.Execute(@"
CREATE TABLE [dbo].[CustomerCustomerDemo](
	[CustomerID] [nchar](5) NOT NULL,
	[CustomerTypeID] [nchar](10) NOT NULL,
 CONSTRAINT [PK_CustomerCustomerDemo] PRIMARY KEY NONCLUSTERED 
(
	[CustomerID] ASC,
	[CustomerTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
");
            connection.Execute(@"
CREATE TABLE [dbo].[CustomerDemographics](
	[CustomerTypeID] [nchar](10) NOT NULL,
	[CustomerDesc] [ntext] NULL,
 CONSTRAINT [PK_CustomerDemographics] PRIMARY KEY NONCLUSTERED 
(
	[CustomerTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
");
            connection.Execute(@"
CREATE TABLE [dbo].[Customers](
	[CustomerID] [nchar](5) NOT NULL,
	[CompanyName] [nvarchar](40) NOT NULL,
	[ContactName] [nvarchar](30) NULL,
	[ContactTitle] [nvarchar](30) NULL,
	[Address] [nvarchar](60) NULL,
	[City] [nvarchar](15) NULL,
	[Region] [nvarchar](15) NULL,
	[PostalCode] [nvarchar](10) NULL,
	[Country] [nvarchar](15) NULL,
	[Phone] [nvarchar](24) NULL,
	[Fax] [nvarchar](24) NULL,
 CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]");
            connection.Execute(@"
CREATE TABLE [dbo].[Employees](
	[EmployeeID] [int] IDENTITY(1,1) NOT NULL,
	[LastName] [nvarchar](20) NOT NULL,
	[FirstName] [nvarchar](10) NOT NULL,
	[Title] [nvarchar](30) NULL,
	[TitleOfCourtesy] [nvarchar](25) NULL,
	[BirthDate] [datetime] NULL,
	[HireDate] [datetime] NULL,
	[Address] [nvarchar](60) NULL,
	[City] [nvarchar](15) NULL,
	[Region] [nvarchar](15) NULL,
	[PostalCode] [nvarchar](10) NULL,
	[Country] [nvarchar](15) NULL,
	[HomePhone] [nvarchar](24) NULL,
	[Extension] [nvarchar](4) NULL,
	[Photo] [image] NULL,
	[Notes] [ntext] NULL,
	[ReportsTo] [int] NULL,
	[PhotoPath] [nvarchar](255) NULL,
 CONSTRAINT [PK_Employees] PRIMARY KEY CLUSTERED 
(
	[EmployeeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]");
            connection.Execute(@"
CREATE TABLE [dbo].[EmployeeTerritories](
	[EmployeeID] [int] NOT NULL,
	[TerritoryID] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_EmployeeTerritories] PRIMARY KEY NONCLUSTERED 
(
	[EmployeeID] ASC,
	[TerritoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]");
            connection.Execute(@"
CREATE TABLE [dbo].[Order Details](
	[OrderID] [int] NOT NULL,
	[ProductID] [int] NOT NULL,
	[UnitPrice] [money] NOT NULL,
	[Quantity] [smallint] NOT NULL,
	[Discount] [real] NOT NULL,
 CONSTRAINT [PK_Order_Details] PRIMARY KEY CLUSTERED 
(
	[OrderID] ASC,
	[ProductID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]");
            connection.Execute(@"
CREATE TABLE [dbo].[Orders](
	[OrderID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerID] [nchar](5) NULL,
	[EmployeeID] [int] NULL,
	[OrderDate] [datetime] NULL,
	[RequiredDate] [datetime] NULL,
	[ShippedDate] [datetime] NULL,
	[ShipVia] [int] NULL,
	[Freight] [money] NULL,
	[ShipName] [nvarchar](40) NULL,
	[ShipAddress] [nvarchar](60) NULL,
	[ShipCity] [nvarchar](15) NULL,
	[ShipRegion] [nvarchar](15) NULL,
	[ShipPostalCode] [nvarchar](10) NULL,
	[ShipCountry] [nvarchar](15) NULL,
 CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED 
(
	[OrderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]");
            connection.Execute(@"
CREATE TABLE [dbo].[Products](
	[ProductID] [int] IDENTITY(1,1) NOT NULL,
	[ProductName] [nvarchar](40) NOT NULL,
	[SupplierID] [int] NULL,
	[CategoryID] [int] NULL,
	[QuantityPerUnit] [nvarchar](20) NULL,
	[UnitPrice] [money] NULL,
	[UnitsInStock] [smallint] NULL,
	[UnitsOnOrder] [smallint] NULL,
	[ReorderLevel] [smallint] NULL,
	[Discontinued] [bit] NOT NULL,
 CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED 
(
	[ProductID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]");
            connection.Execute(@"
CREATE TABLE [dbo].[Region](
	[RegionID] [int] NOT NULL,
	[RegionDescription] [nchar](50) NOT NULL,
 CONSTRAINT [PK_Region] PRIMARY KEY NONCLUSTERED 
(
	[RegionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]");
            connection.Execute(@"
CREATE TABLE [dbo].[Shippers](
	[ShipperID] [int] IDENTITY(1,1) NOT NULL,
	[CompanyName] [nvarchar](40) NOT NULL,
	[Phone] [nvarchar](24) NULL,
 CONSTRAINT [PK_Shippers] PRIMARY KEY CLUSTERED 
(
	[ShipperID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]");
            connection.Execute(@"
CREATE TABLE [dbo].[Suppliers](
	[SupplierID] [int] IDENTITY(1,1) NOT NULL,
	[CompanyName] [nvarchar](40) NOT NULL,
	[ContactName] [nvarchar](30) NULL,
	[ContactTitle] [nvarchar](30) NULL,
	[Address] [nvarchar](60) NULL,
	[City] [nvarchar](15) NULL,
	[Region] [nvarchar](15) NULL,
	[PostalCode] [nvarchar](10) NULL,
	[Country] [nvarchar](15) NULL,
	[Phone] [nvarchar](24) NULL,
	[Fax] [nvarchar](24) NULL,
	[HomePage] [ntext] NULL,
 CONSTRAINT [PK_Suppliers] PRIMARY KEY CLUSTERED 
(
	[SupplierID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]");
            connection.Execute(@"
CREATE TABLE [dbo].[Territories](
	[TerritoryID] [nvarchar](20) NOT NULL,
	[TerritoryDescription] [nchar](50) NOT NULL,
	[RegionID] [int] NOT NULL,
 CONSTRAINT [PK_Territories] PRIMARY KEY NONCLUSTERED 
(
	[TerritoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]");

            Console.WriteLine("CreateTables: Adding table constraints");
            connection.Execute(@"
ALTER TABLE [dbo].[Order Details] ADD  CONSTRAINT [DF_Order_Details_UnitPrice]  DEFAULT ((0)) FOR [UnitPrice];
ALTER TABLE [dbo].[Order Details] ADD  CONSTRAINT [DF_Order_Details_Quantity]  DEFAULT ((1)) FOR [Quantity];
ALTER TABLE [dbo].[Order Details] ADD  CONSTRAINT [DF_Order_Details_Discount]  DEFAULT ((0)) FOR [Discount];
ALTER TABLE [dbo].[Orders] ADD  CONSTRAINT [DF_Orders_Freight]  DEFAULT ((0)) FOR [Freight];
ALTER TABLE [dbo].[Products] ADD  CONSTRAINT [DF_Products_UnitPrice]  DEFAULT ((0)) FOR [UnitPrice];
ALTER TABLE [dbo].[Products] ADD  CONSTRAINT [DF_Products_UnitsInStock]  DEFAULT ((0)) FOR [UnitsInStock];
ALTER TABLE [dbo].[Products] ADD  CONSTRAINT [DF_Products_UnitsOnOrder]  DEFAULT ((0)) FOR [UnitsOnOrder];
ALTER TABLE [dbo].[Products] ADD  CONSTRAINT [DF_Products_ReorderLevel]  DEFAULT ((0)) FOR [ReorderLevel];
ALTER TABLE [dbo].[Products] ADD  CONSTRAINT [DF_Products_Discontinued]  DEFAULT ((0)) FOR [Discontinued];
");

            Console.WriteLine("CreateTables: Adding foreign keys");
            connection.Execute(@"
ALTER TABLE [dbo].[CustomerCustomerDemo]  WITH CHECK ADD  CONSTRAINT [FK_CustomerCustomerDemo] FOREIGN KEY([CustomerTypeID])
REFERENCES [dbo].[CustomerDemographics] ([CustomerTypeID]);

ALTER TABLE [dbo].[CustomerCustomerDemo] CHECK CONSTRAINT [FK_CustomerCustomerDemo];

ALTER TABLE [dbo].[CustomerCustomerDemo]  WITH CHECK ADD  CONSTRAINT [FK_CustomerCustomerDemo_Customers] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customers] ([CustomerID]);

ALTER TABLE [dbo].[CustomerCustomerDemo] CHECK CONSTRAINT [FK_CustomerCustomerDemo_Customers];

ALTER TABLE [dbo].[Employees]  WITH NOCHECK ADD  CONSTRAINT [FK_Employees_Employees] FOREIGN KEY([ReportsTo])
REFERENCES [dbo].[Employees] ([EmployeeID]);

ALTER TABLE [dbo].[Employees] CHECK CONSTRAINT [FK_Employees_Employees];

ALTER TABLE [dbo].[EmployeeTerritories]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeTerritories_Employees] FOREIGN KEY([EmployeeID])
REFERENCES [dbo].[Employees] ([EmployeeID]);

ALTER TABLE [dbo].[EmployeeTerritories] CHECK CONSTRAINT [FK_EmployeeTerritories_Employees];

ALTER TABLE [dbo].[EmployeeTerritories]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeTerritories_Territories] FOREIGN KEY([TerritoryID])
REFERENCES [dbo].[Territories] ([TerritoryID]);

ALTER TABLE [dbo].[EmployeeTerritories] CHECK CONSTRAINT [FK_EmployeeTerritories_Territories];

ALTER TABLE [dbo].[Order Details]  WITH NOCHECK ADD  CONSTRAINT [FK_Order_Details_Orders] FOREIGN KEY([OrderID])
REFERENCES [dbo].[Orders] ([OrderID]);

ALTER TABLE [dbo].[Order Details] CHECK CONSTRAINT [FK_Order_Details_Orders];

ALTER TABLE [dbo].[Order Details]  WITH NOCHECK ADD  CONSTRAINT [FK_Order_Details_Products] FOREIGN KEY([ProductID])
REFERENCES [dbo].[Products] ([ProductID]);

ALTER TABLE [dbo].[Order Details] CHECK CONSTRAINT [FK_Order_Details_Products];

ALTER TABLE [dbo].[Orders]  WITH NOCHECK ADD  CONSTRAINT [FK_Orders_Customers] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customers] ([CustomerID]);

ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_Customers];

ALTER TABLE [dbo].[Orders]  WITH NOCHECK ADD  CONSTRAINT [FK_Orders_Employees] FOREIGN KEY([EmployeeID])
REFERENCES [dbo].[Employees] ([EmployeeID]);

ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_Employees];

ALTER TABLE [dbo].[Orders]  WITH NOCHECK ADD  CONSTRAINT [FK_Orders_Shippers] FOREIGN KEY([ShipVia])
REFERENCES [dbo].[Shippers] ([ShipperID]);

ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_Shippers];

ALTER TABLE [dbo].[Products]  WITH NOCHECK ADD  CONSTRAINT [FK_Products_Categories] FOREIGN KEY([CategoryID])
REFERENCES [dbo].[Categories] ([CategoryID]);

ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_Categories];

ALTER TABLE [dbo].[Products]  WITH NOCHECK ADD  CONSTRAINT [FK_Products_Suppliers] FOREIGN KEY([SupplierID])
REFERENCES [dbo].[Suppliers] ([SupplierID]);

ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_Suppliers];

ALTER TABLE [dbo].[Territories]  WITH CHECK ADD  CONSTRAINT [FK_Territories_Region] FOREIGN KEY([RegionID])
REFERENCES [dbo].[Region] ([RegionID]);

ALTER TABLE [dbo].[Territories] CHECK CONSTRAINT [FK_Territories_Region];
");

            Console.WriteLine("CreateTables: Adding additional table constraints");
            connection.Execute(@"
ALTER TABLE [dbo].[Employees]  WITH NOCHECK ADD  CONSTRAINT [CK_Birthdate] CHECK  (([BirthDate]<getdate()));
ALTER TABLE [dbo].[Employees] CHECK CONSTRAINT [CK_Birthdate];
ALTER TABLE [dbo].[Order Details]  WITH NOCHECK ADD  CONSTRAINT [CK_Discount] CHECK  (([Discount]>=(0) AND [Discount]<=(1)));
ALTER TABLE [dbo].[Order Details] CHECK CONSTRAINT [CK_Discount];
ALTER TABLE [dbo].[Order Details]  WITH NOCHECK ADD  CONSTRAINT [CK_Quantity] CHECK  (([Quantity]>(0)));
ALTER TABLE [dbo].[Order Details] CHECK CONSTRAINT [CK_Quantity];
ALTER TABLE [dbo].[Order Details]  WITH NOCHECK ADD  CONSTRAINT [CK_UnitPrice] CHECK  (([UnitPrice]>=(0)));
ALTER TABLE [dbo].[Order Details] CHECK CONSTRAINT [CK_UnitPrice];
ALTER TABLE [dbo].[Products]  WITH NOCHECK ADD  CONSTRAINT [CK_Products_UnitPrice] CHECK  (([UnitPrice]>=(0)));
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [CK_Products_UnitPrice];
ALTER TABLE [dbo].[Products]  WITH NOCHECK ADD  CONSTRAINT [CK_ReorderLevel] CHECK  (([ReorderLevel]>=(0)));
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [CK_ReorderLevel];
ALTER TABLE [dbo].[Products]  WITH NOCHECK ADD  CONSTRAINT [CK_UnitsInStock] CHECK  (([UnitsInStock]>=(0)));
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [CK_UnitsInStock];
ALTER TABLE [dbo].[Products]  WITH NOCHECK ADD  CONSTRAINT [CK_UnitsOnOrder] CHECK  (([UnitsOnOrder]>=(0)));
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [CK_UnitsOnOrder];
");
        }
    }
}