using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionTreeBuilder
{
    public class FilterParamModel
    {
        /// <summary>
        /// What property name do we want to filter
        /// </summary>
        public string Property { get; set; }
        /// <summary>
        /// How do we want to compare this property
        /// </summary>
        public PropertyValueCompareEnum CompareMethod { get; set; }
        /// <summary>
        /// What value do we want to compare to
        /// </summary>
        public object Value { get; set; }
    }
}
