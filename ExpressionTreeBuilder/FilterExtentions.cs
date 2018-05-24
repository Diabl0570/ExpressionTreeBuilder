using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ExpressionTreeBuilder
{
    public static class FilterExtentions
    {
        private static readonly MethodInfo ContainsMethod = typeof(string).GetMethod("Contains");
        private static readonly MethodInfo ListContainsMethod = typeof(List<string>).GetMethod("Contains");
        private static readonly MethodInfo StartsWithMethod = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
        private static readonly MethodInfo EndsWithMethod = typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });

        public static IQueryable<T> Filter<T>(this IQueryable<T> dbSet, FilterModel Filters)
        {
            if (dbSet == null)
            {
                throw new ArgumentException("The dataset set can not be null");
            }
            ParameterExpression pe = Expression.Parameter(typeof(T), "x");

            if (Filters != null && Filters.Filters != null && Filters.Filters.Count > 0)
            {
                Expression<Func<T, bool>> where = null;
                Expression body = BuildExpressionTree<T>(Filters, pe);

                Expression expr = Expression.Call(typeof(Queryable), "Where", new Type[] { typeof(T) }, Expression.Constant(dbSet), body);
                dbSet = dbSet.Provider.CreateQuery<T>(expr);
            }
            return dbSet;
        }
        private static Expression BuildExpressionTree<T>(FilterModel FilterParams, ParameterExpression parameter)
        {
            Expression body = null;
            foreach (FilterParamModel filter in FilterParams.Filters)
            {
                string[] properties = filter.Property.Split('.');
                Expression newBody = BuildExpression(parameter, filter.Value, filter.CompareMethod, properties);
                if (body == null)
                {
                    body = newBody;
                }
                else
                {
                    body = GlueBodies(FilterParams.CompareMethod, parameter, body, newBody);
                    body = Expression.Lambda<Func<T, bool>>(body, parameter);
                }
            }
            if (FilterParams.ChildFilters != null)
            {
                Expression childBody = BuildExpressionTree<T>(FilterParams.ChildFilters, parameter);
                body = GlueBodies(FilterParams.ChildCompareMethod, parameter, body, childBody);
                body = Expression.Lambda<Func<T, bool>>(body, parameter);
            }

            return body;
        }

        public static Expression BuildExpression(Expression parameter, dynamic value, PropertyValueCompareEnum compareMethod, params string[] properties)
        {
            Expression resultExpression = null;
            Expression childParameter, navigationPropertyPredicate;
            Type childType = null;

            if (properties.Count() > 1)
            {
                //build expression
                parameter = Expression.Property(parameter, properties[0]);
                var isCollection = typeof(IEnumerable).IsAssignableFrom(parameter.Type);
                //if it's a collection we later need to use the predicate in the methodexpressioncall
                if (isCollection)
                {
                    childType = parameter.Type.GetGenericArguments()[0];
                    childParameter = Expression.Parameter(childType, childType.Name);
                }
                else
                {
                    childParameter = parameter;
                }
                //skip current property and get navigation property expression recursivly
                var innerProperties = properties.Skip(1).ToArray();
                navigationPropertyPredicate = BuildExpression(childParameter, value, compareMethod, innerProperties);
                if (isCollection)
                {
                    //build methodexpressioncall
                    var anyMethod = typeof(Enumerable).GetMethods().Single(m => m.Name == "Any" && m.GetParameters().Length == 2);
                    anyMethod = anyMethod.MakeGenericMethod(childType);
                    navigationPropertyPredicate = Expression.Call(anyMethod, parameter, navigationPropertyPredicate);
                    resultExpression = MakeLambda(parameter, navigationPropertyPredicate);
                }
                else
                {
                    resultExpression = navigationPropertyPredicate;
                }
            }
            else
            {
                //Formerly from ACLAttribute
                var childProperty = parameter.Type.GetProperty(properties[0]);
                if (childProperty == null)
                {
                    throw new ArgumentException("That property is'nt a member of this class");
                }
                var left = Expression.Property(parameter, childProperty);
                var right = Expression.Constant(value, value.GetType());
                navigationPropertyPredicate = GetCompareMethod(compareMethod, left, right);
                resultExpression = MakeLambda(parameter, navigationPropertyPredicate);
            }
            return resultExpression;
        }
        public static Expression MakeLambda(Expression parameter, Expression predicate)
        {
            var resultParameterVisitor = new ParameterVisitor();
            resultParameterVisitor.Visit(parameter);
            var resultParameter = resultParameterVisitor.Parameter;
            return Expression.Lambda(predicate, (ParameterExpression)resultParameter);
        }
        private class ParameterVisitor : ExpressionVisitor
        {
            public Expression Parameter
            {
                get;
                private set;
            }
            protected override Expression VisitParameter(ParameterExpression node)
            {
                Parameter = node;
                return node;
            }
        }

        public static BinaryExpression GetCompareMethod(PropertyValueCompareEnum compareMethod, Expression left, ConstantExpression right)
        {
            BinaryExpression body;
            switch (compareMethod)
            {
                case PropertyValueCompareEnum.Equal:
                    body = Expression.Equal(left, right);
                    break;
                case PropertyValueCompareEnum.NotEqual:
                    body = Expression.NotEqual(left, right);
                    break;
                case PropertyValueCompareEnum.GreaterThan:
                    body = Expression.GreaterThan(left, right);
                    break;
                case PropertyValueCompareEnum.GreaterThanOrEqual:
                    body = Expression.GreaterThanOrEqual(left, right);
                    break;
                case PropertyValueCompareEnum.LessThan:
                    body = Expression.LessThan(left, right);
                    break;
                case PropertyValueCompareEnum.LessThanOrEqual:
                    body = Expression.LessThanOrEqual(left, right);
                    break;
                case PropertyValueCompareEnum.ListContains:
                    body = Expression.Equal(Expression.Call(left, ListContainsMethod, right), Expression.Constant(true));
                    break;
                case PropertyValueCompareEnum.Contains:
                    body = Expression.Equal(Expression.Call(left, ContainsMethod, right), Expression.Constant(true));
                    break;
                case PropertyValueCompareEnum.StartsWith:
                    body = Expression.Equal(Expression.Call(left, StartsWithMethod, right), Expression.Constant(true));
                    break;
                case PropertyValueCompareEnum.EndsWith:
                    body = Expression.Equal(Expression.Call(left, EndsWithMethod, right), Expression.Constant(true));
                    break;
                default:
                    body = Expression.Equal(left, right);
                    break;
            }
            return body;
        }

        public static BinaryExpression GlueBodies(PropertiesCompareEnum compareMethod, ParameterExpression parameter, Expression left, Expression right)
        {
            BinaryExpression body;
            switch (compareMethod.ToString())
            {
                case "AND":
                    body = Expression.AndAlso(Expression.Invoke(left, parameter), Expression.Invoke(right, parameter));
                    break;
                case "OR":
                    body = Expression.Or(Expression.Invoke(left, parameter), Expression.Invoke(right, parameter));
                    break;
                default:
                    body = Expression.AndAlso(Expression.Invoke(left, parameter), Expression.Invoke(right, parameter));
                    break;
            }

            return body;
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, "OrderBy");
        }
        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, "OrderByDescending");
        }
        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, "ThenBy");
        }
        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, "ThenByDescending");
        }
        static IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> source, string property, string methodName)
        {
            string[] props = property.Split('.');
            Type type = typeof(T);
            ParameterExpression arg = Expression.Parameter(type, "x");
            Expression expr = arg;
            foreach (string prop in props)
            {
                // use reflection (not ComponentModel) to mirror LINQ
                PropertyInfo pi = type.GetProperty(prop);
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }
            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);

            object result = typeof(Queryable).GetMethods().Single(
                    method => method.Name == methodName
                            && method.IsGenericMethodDefinition
                            && method.GetGenericArguments().Length ==
2
                            && method.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(T), type)
                    .Invoke(null, new object[] { source, lambda });
            return (IOrderedQueryable<T>)result;
        }

    }
}
