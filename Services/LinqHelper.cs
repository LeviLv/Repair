using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Repair
{
    public static class LinqHelper
    {
        public static IEnumerable<TSource> WhereIf<TSource>(
            this IEnumerable<TSource> source,
            bool b,
            Func<TSource, bool> predicate)
        {
            return b ? source.Where(predicate) : source;
        }
        
        public static IEnumerable<TSource> WhereIfNotNull<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, bool> predicate)
        {
            return predicate != null ? source.Where(predicate) : source;
        } 
        
        
        public static IQueryable<TSource> WhereIfNotNull<TSource>(
            this IQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate)
        {
            return predicate != null ? source.Where(predicate) : source;
        } 
    }
}