using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConsoleApp1
{
    /// <summary>
    /// 
    /// </summary>
    public class RegionQuery : QueryBase<Region>
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
                    _columns = typeof(RegionQuery.ColumnNames).GetMembers(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly).Select(i => i.Name);
                }
                return _columns;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public RegionQuery(string connectionString) : base(connectionString)
        {
            WhereParameters = new List<WhereParameterBase>();
            UpsertParameters = new List<ValueParameter>();
            SelectColumns = new List<string>();
            Schema = "dbo";
            TableName = "Region";
            PrimaryKeyColumns = new List<string> { "RegionID" };
            OrderByParameters = new List<OrderByParameter>();
            HasIdentity = false;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct ColumnNames
        {
            /// <summary>
            /// 
            /// </summary>
            public static string RegionID = "RegionID";
            /// <summary>
            /// 
            /// </summary>
            public static string RegionDescription = "RegionDescription";
        }
    }
}