using IntranetApi.Enum;
using System.Linq.Expressions;

namespace IntranetApi.Helper
{
    public static class LinqFilterExtensions
    {
        public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> query, string sortColumn, bool isAscending = true) /*where T : class*/
        {
            var parameter = Expression.Parameter(typeof(T), "p");

            string command = isAscending ? "OrderBy" : "OrderByDescending";

            Expression resultExpression = null;

            var property = typeof(T).GetProperty(sortColumn);
            // this is the part p.SortColumn
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);

            // this is the part p =&gt; p.SortColumn
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);

            // finally, call the "OrderBy" / "OrderByDescending" method with the order by lamba expression
            resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { typeof(T), property.PropertyType },
               query.Expression, Expression.Quote(orderByExpression));

            return query.Provider.CreateQuery<T>(resultExpression);
        }

        public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> query, string sortColumn, string sortDirection) /*where T : class*/
        {
            return query.OrderByDynamic(sortColumn, sortDirection.Equals(SortDirection.ASC, StringComparison.OrdinalIgnoreCase));
        }

        public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> query, string sortValue) /*where T : class*/
        {
            if (string.IsNullOrEmpty(sortValue))
                return query;
            var sortParts = sortValue.Contains(',') ? sortValue.Split(',') : sortValue.Split(' ');
            return query.OrderByDynamic(sortParts[0], sortParts[1].Equals(SortDirection.ASC, StringComparison.OrdinalIgnoreCase));
        }
    }

    public static class LinqExtensions
    {
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
        {
            return condition
                ? query.Where(predicate)
                : query;
        }
    }
}