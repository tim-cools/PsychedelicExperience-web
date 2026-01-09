using System;
using System.Linq.Expressions;
using System.Reflection;

namespace PsychedelicExperience.Common
{
    public static class Expressionxtensions
    {
        public static PropertyInfo GetPropertyInfo<TSource>(this Expression<Func<TSource, object>> propertyLambda)
        {
            var member = propertyLambda.Body as MemberExpression;
            if (member == null)
            {
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));
            }

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
            {
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));
            }

            return propInfo;
        }

        public static string GetPropertyName<TSource>(this Expression<Func<TSource, object>> propertyLambda)
        {
            var propertyInfo = propertyLambda.GetPropertyInfo();
            return propertyInfo.Name;
        }
    }
}