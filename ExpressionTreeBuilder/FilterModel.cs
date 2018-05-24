using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionTreeBuilder
{
    public class FilterModel
    {
        /// <summary>
        /// A list of params and values we want to filter 
        /// </summary>
        public List<FilterParamModel> Filters { get; set; }
        /// <summary>
        /// Note this is only used when Filters.Count>1
        /// How should these params be combined, IE param1 == 4 && param2 == 5
        /// </summary>
        public PropertiesCompareEnum CompareMethod { get; set; }
        /// <summary>
        /// If we want to filter other params and want to combine with a different method we need to define child filters
        /// </summary>
        public FilterModel ChildFilters { get; set; }
        /// <summary>
        /// How should we combine the child filters? 
        /// </summary>
        public PropertiesCompareEnum ChildCompareMethod { get; set; }
        ///// <summary>
        ///// Group result set by this property
        ///// </summary>
        //[DataMember]
        //public string GroupBy { get; set; }
    }
}
