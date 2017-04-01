using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    /// <summary>
    /// 
    /// </summary>
    public enum WhereOperand
    {
        /// <summary>
        /// 
        /// </summary>
        Equal,
        /// <summary>
        /// 
        /// </summary>
        NotEqual,
        /// <summary>
        /// 
        /// </summary>
        IsNull,
        /// <summary>
        /// 
        /// </summary>
        IsNotNull,
        /// <summary>
        /// 
        /// </summary>
        Between
    }
    /// <summary>
    /// 
    /// </summary>
    public abstract class WhereParameterBase : IQueryParameter, IWhereParameter
    {
        /// <summary>
        /// 
        /// </summary>
        public WhereOperand Operand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class WhereParameter : WhereParameterBase, IValueParameter
    {
        /// <summary>
        /// 
        /// </summary>
        public dynamic Value { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class ValueParameter : IValueParameter
    {
        /// <summary>
        /// 
        /// </summary>
        public dynamic Value { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class BetweenWhereParameter : WhereParameterBase
    {
        /// <summary>
        /// 
        /// </summary>
        public object StartValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public object EndValue { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IQueryParameter
    {
        /// <summary>
        /// 
        /// </summary>
        string Name { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IValueParameter : IQueryParameter
    {
        /// <summary>
        /// 
        /// </summary>
        dynamic Value { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IWhereParameter
    {
        /// <summary>
        /// 
        /// </summary>
        WhereOperand Operand { get; set; }
    }
}
