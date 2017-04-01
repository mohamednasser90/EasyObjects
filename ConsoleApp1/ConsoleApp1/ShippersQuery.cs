using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConsoleApp1
{
    public class ShippersQuery : QueryBase<Shippers>
    {
        private static IEnumerable<string> _columns;

        /// <summary>
        /// All column names defined in the ColumnNames struct. The field name is assumed to match the string value, which in turn is assumed to match the db column name.
        /// </summary>
        public override IEnumerable<string> Columns
        {
            get
            {
                if (_columns == null)
                {
                    _columns = typeof(ShippersQuery.ColumnNames).GetMembers(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly).Select(i => i.Name);
                }
                return _columns;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ShippersQuery(string connectionString) : base(connectionString)
        {
            WhereParameters = new List<WhereParameterBase>();
            UpsertParameters = new List<ValueParameter>();
            SelectColumns = new List<string>();
            Schema = "dbo";
            TableName = "Shippers";
            PrimaryKeyColumns = new List<string> { "ShipperID" };
            OrderByParameters = new List<OrderByParameter>();
            HasIdentity = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct ColumnNames
        {
            /// <summary>
            /// 
            /// </summary>
            public static string ShipperID = "ShipperID";
            /// <summary>
            /// 
            /// </summary>
            public static string CompanyName = "CompanyName";
            /// <summary>
            /// 
            /// </summary>
            public static string Phone = "Phone";
        }
    }
}
