using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Frankstein.DOP
{
    public static class ObjectProxyHelper
    {
        #region public helper methods to get methodnames

        public static string[] GetMethodNames<T>(params Expression<Action<T>>[] expressions)
        {
            if (expressions == null)
                return Enumerable.Empty<string>().ToArray();
            return expressions.Select(expression => expression.ExtractMethod()).ToArray();
        }

        private static string ExtractMethod<T>(this Expression<Action<T>> expression)
        {
            var methodCall = expression.Body as MethodCallExpression;
            if (methodCall == null)
            {
                throw new ArgumentException("expression");
            }
            var method = methodCall.Method;
            return method.Name;
        }

        public static string[] GetMethodNamesFromPropertyInfo(bool getters = true, bool setters = true, params PropertyInfo[] propertyInfos)
        {
            var methodsProperties = new List<string>();
            foreach (var propertyName in propertyInfos.Select(propertyInfo => propertyInfo.Name))
            {
                if (getters) methodsProperties.Add("get_" + propertyName);
                if (setters) methodsProperties.Add("set_" + propertyName);
            }
            return methodsProperties.ToArray();
        }

        /// <summary>
        /// Utility helper method that returns an array of method names 
        /// from a Lambda Expression that denotes 
        /// a Typed Property Definition (get_Property, set_Property)
        /// Usage:  var getters = DynamicDecoratorHelper
        ///       .GetMethodNamesFromLambdaProperty(true, false, t => t.Counter)
        ///        .ToList();
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="getters"></param>
        /// <param name="setters"></param>
        /// <param name="expressions"></param>
        /// <returns></returns>
        public static string[] GetMethodNamesFromLambdaProperty<T>(bool getters = true, bool setters = true, params Expression<Func<T, object>>[] expressions) where T : class
        {
            var methodsProperties = new List<string>();
            foreach (var propertyName in expressions.Select(expression => expression.ExtractPropertyName()))
            {
                if (getters) methodsProperties.Add("get_" + propertyName);
                if (setters) methodsProperties.Add("set_" + propertyName);
            }
            return methodsProperties.ToArray();
        }

        internal static string ExtractPropertyName<T>(this Expression<Func<T, object>> expression)
        {
            var memberExpression = expression.ToMemberExpression();
            var member = memberExpression.Member;
            return member.Name;
        }

        private static MemberExpression ToMemberExpression(this LambdaExpression expression)
        {
            MemberExpression memberExpression;
            if (expression.Body is UnaryExpression)
            {
                var unary = (UnaryExpression)expression.Body;
                memberExpression = (MemberExpression)unary.Operand;
            }
            else memberExpression = expression.Body as MemberExpression;

            if (memberExpression == null)
            {
                throw new ArgumentException("expression");
            }

            return memberExpression;
        }

        #endregion

        #region helper methods

        public static bool Implements<TType>(this Type derived)
        {
            return Implements(typeof(TType), derived);
        }

        public static bool Implements(this Type derived, Type baseType)
        {
            return derived.IsAssignableFrom(baseType);
        }

        #endregion

        public static PropertyInfo[] GetAllProperties(this Type type)
        {
            List<Type> typeList = new List<Type> { type };

            if (type.IsInterface)
            {
                typeList.AddRange(type.GetInterfaces());
            }

            return typeList.SelectMany(interfaceType => interfaceType.GetProperties()).ToArray();
        }
    }
}