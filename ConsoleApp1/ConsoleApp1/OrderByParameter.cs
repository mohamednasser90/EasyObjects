using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    /// <summary>
    /// 
    /// </summary>
    public class OrderByParameter
    {
        /// <summary>
        /// 
        /// </summary>
        public SortDirection Direction { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SortColumnName { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public enum SortDirection
    {
        /// <summary>
        /// 
        /// </summary>
        Ascending,
        /// <summary>
        /// 
        /// </summary>
        Descending
    }
}
