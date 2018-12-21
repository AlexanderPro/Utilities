using System;
using System.Collections.Generic;
using System.Linq;

namespace Utilities.Extensions
{
    /// <summary>
    /// Extension methods for Collections.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Checks whatever given collection object is null or has no item.
        /// </summary>
        /// <param name="source">Collection</param>
        /// <returns>Returns True if null or empty, otherwise False.</returns>
        public static bool IsNullOrEmpty<T>(this ICollection<T> source)
        {
            return source == null || source.Count <= 0;
        }

        /// <summary>
        /// Splits collection to parts.
        /// </summary>
        /// <param name="source">Collection</param>
        /// <param name="partSize">Part size</param>
        /// <returns>Returns list with parts</returns>
        public static IList<IList<T>> SplitToParts<T>(this ICollection<T> source, int partSize)
        {
            if (source.IsNullOrEmpty())
            {
                return new List<IList<T>>();
            }

            if (partSize <= 0)
            {
                throw new ArgumentOutOfRangeException("partSize");
            }

            var stepCount = (int)Math.Ceiling(source.Count / (decimal)partSize);
            var result = new List<IList<T>>(stepCount);
            Enumerable.Range(0, stepCount).ToList().ForEach(x => result.Add(source.Skip(x * partSize).Take(partSize).ToList()));

            return result;
        }

        /// <summary>
        /// Adds an item to the collection if it's not already in the collection.
        /// </summary>
        /// <param name="source">Collection</param>
        /// <param name="item">Item to check and add</param>
        /// <typeparam name="T">Type of the items in the collection</typeparam>
        /// <returns>Returns True if added,otherwise False.</returns>
        public static bool AddIfNotContains<T>(this ICollection<T> source, T item)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (source.Contains(item))
            {
                return false;
            }

            source.Add(item);
            return true;
        }

        /// <summary>
        /// Splits collection to parts and operates all items in parallel.
        /// </summary>
        public static void ProcessParallel<T>(this ICollection<T> source, int threadCount, Action<IList<T>> action)
        { 
            source.SplitToParts(threadCount).AsParallel().ForAll(action);
        }
    }
}
