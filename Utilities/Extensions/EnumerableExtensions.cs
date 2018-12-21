using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities.Extensions
{
    /// <summary>
    /// Extension methods for Enumerables.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Determines if <see cref="System.Collections.IEnumerable" /> is null or empty.
        /// </summary>
        public static bool IsNullOrEmpty(this IEnumerable source)
        {
            bool isNullOrEmpty = false;
            if (source == null)
            {
                isNullOrEmpty = true;
            }
            else
            {
                IEnumerator enumerator = source.GetEnumerator();
                enumerator.Reset();
                isNullOrEmpty = enumerator.MoveNext();
            }

            return isNullOrEmpty;
        }

        /// <summary>
        /// Determines if <see cref="System.Collections.Generic.IEnumerable{T}" /> is null or empty.
        /// </summary>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || source.Any() == false;
        }

        /// <summary>
        /// Determines if <see cref="System.Collections.IEnumerable" /> is not null and not empty.
        /// </summary>
        public static bool IsNotNullAndIsNotEmpty(this IEnumerable source)
        {
            return IsNullOrEmpty(source) == false;
        }

        /// <summary>
        /// Determines if <see cref="System.Collections.Generic.IEnumerable{T}" /> is not null and not empty.
        /// </summary>
        public static bool IsNotNullAndIsNotEmpty<T>(this IEnumerable<T> source)
        {
            return IsNullOrEmpty(source) == false;
        }

        /// <summary>
        ///  Filters a <see cref="System.Collections.Generic.IEnumerable{T}" /> by given predicate if given condition is true.
        /// </summary>
        public static IEnumerable<T> WhereIf<T>(this ICollection<T> source, bool condition, Func<T, bool> predicate) where T : class
        {
            return condition ? source.Where(predicate) : source;
        }

        /// <summary>
        ///  Filters a <see cref="System.Collections.Generic.IEnumerable{T}" /> by ifPredicate if given condition is true otherwise by elsePredicate.
        /// </summary>
        public static IEnumerable<T> WhereIfElse<T>(this ICollection<T> source, bool condition, Func<T, bool> ifPredicate, Func<T, bool> elsePredicate) where T : class
        {
            return source.Where(condition ? ifPredicate : elsePredicate);
        }
    }
}
