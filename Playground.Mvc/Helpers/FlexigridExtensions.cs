using System;
using System.Linq;
using System.Linq.Expressions;

namespace Playground.Mvc.Helpers
{
    // ReSharper disable once IdentifierTypo
    public static class FlexigridExtensions
    {
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName, bool asc)
        {
            var type = typeof(T);
            string methodName = asc ? "OrderBy" : "OrderByDescending";
            var property = type.GetProperty(propertyName);
            var parameter = Expression.Parameter(type, "p");
            // ReSharper disable once AssignNullToNotNullAttribute
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);
            // ReSharper disable once RedundantExplicitArrayCreation
            MethodCallExpression resultExp = Expression.Call(typeof(Queryable), methodName, new Type[] { type, property.PropertyType }, source.Expression, Expression.Quote(orderByExp));
            return source.Provider.CreateQuery<T>(resultExp);
        }

        public static IQueryable<T> Like<T>(this IQueryable<T> source, string propertyName, string keyword)
        {
            var type = typeof(T);
            var property = type.GetProperty(propertyName);
            var parameter = Expression.Parameter(type, "p");
            // ReSharper disable once AssignNullToNotNullAttribute
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var constant = Expression.Constant(keyword);

            var like = typeof(string).GetMethod("Contains",
                // ReSharper disable once RedundantExplicitArrayCreation
                new Type[] { typeof(string) });
            // ReSharper disable once AssignNullToNotNullAttribute
            MethodCallExpression methodExp = Expression.Call(propertyAccess, like, constant);
            Expression<Func<T, bool>> lambda =
                  Expression.Lambda<Func<T, bool>>(methodExp, parameter);
            return source.Where(lambda);
        }
    }
}