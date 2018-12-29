using System;
using System.Collections.Generic;
using System.Linq;

namespace Utilities.Extensions
{
    /// <summary>
    /// Extension methods for Dictionary.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Gets a value from the dictionary with given key. Returns default value if can not find.
        /// </summary>
        /// <param name="dictionary">Dictionary to check and get</param>
        /// <param name="key">Key to find the value</param>
        /// <param name="defaultValue">Default value</param>
        /// <typeparam name="TKey">Type of the key</typeparam>
        /// <typeparam name="TValue">Type of the value</typeparam>
        /// <returns>Value if found, default if can not found.</returns>
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default(TValue))
        {
            TValue obj;
            return dictionary.TryGetValue(key, out obj) ? obj : defaultValue;
        }

        /// <summary>
        /// Gets a value from the dictionary with given key. Returns default value if can not find.
        /// </summary>
        /// <param name="dictionary">Dictionary to check and get</param>
        /// <param name="key">Key to find the value</param>
        /// <param name="factory">A factory method used to create the value if not found in the dictionary</param>
        /// <typeparam name="TKey">Type of the key</typeparam>
        /// <typeparam name="TValue">Type of the value</typeparam>
        /// <returns>Value if found, default if can not found.</returns>
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> factory)
        {
            TValue obj;
            if (dictionary.TryGetValue(key, out obj)) return obj;
            return dictionary[key] = factory(key);
        }

        /// <summary>
        /// Gets a value from the dictionary with given key. Returns default value if can not find.
        /// </summary>
        /// <param name="dictionary">Dictionary to check and get</param>
        /// <param name="key">Key to find the value</param>
        /// <param name="factory">A factory method used to create the value if not found in the dictionary</param>
        /// <typeparam name="TKey">Type of the key</typeparam>
        /// <typeparam name="TValue">Type of the value</typeparam>
        /// <returns>Value if found, default if can not found.</returns>
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> factory)
        {
            return dictionary.GetOrAdd(key, k => factory());
        }

        /// <summary>
        /// This method is used to try to get a value in a dictionary if it does exists.
        /// </summary>
        /// <typeparam name="T">Type of the value</typeparam>
        /// <param name="dictionary">The collection object</param>
        /// <param name="key">Key</param>
        /// <param name="value">Value of the key (or default value if key not exists)</param>
        /// <returns>True if key does exists in the dictionary</returns>
        internal static bool TryGetValue<T>(this IDictionary<string, object> dictionary, string key, out T value)
        {
            object valueObj;
            if (dictionary.TryGetValue(key, out valueObj) && valueObj is T)
            {
                value = (T)valueObj;
                return true;
            }

            value = default(T);
            return false;
        }

        /// <summary>
        /// Converts a dictionary to a key-value string
        /// </summary>
        public static String ToKeyValueString<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, String pairDelimiter = ";", String keyValueDelimiter = "=", Func<String, String> normalizeKeyFunc = null, Func<String, String> normalizeValueFunc = null)
        {
            var pairs = dictionary.
                        Select(kv =>
                               (normalizeKeyFunc == null ? kv.Key.ToString() : normalizeKeyFunc(kv.Key.ToString())) +
                                keyValueDelimiter +
                               (normalizeValueFunc == null ? kv.Value.ToString() : normalizeValueFunc(kv.Value.ToString()))).
                        ToArray();
            var result = String.Join(pairDelimiter, pairs);
            return result;
        }

        /// <summary>
        /// Converts a key-value string to a dictionary
        /// </summary>
        public static IDictionary<String, String> ToDictionary(this String value, String pairDelimiter = ";", String keyValueDelimiter = "=", Func<String, String> normalizeKeyFunc = null, Func<String, String> normalizeValueFunc = null)
        {
            var dictionary = new Dictionary<String, String>();
            if (String.IsNullOrEmpty(value)) return dictionary;
            var pairs = value.Split(pairDelimiter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (String pair in pairs)
            {
                var index = pair.IndexOf(keyValueDelimiter);
                if (index != -1)
                {
                    var k = pair.Substring(0, index);
                    var v = pair.Substring(index + 1);
                    k = normalizeKeyFunc == null ? k : normalizeKeyFunc(k);
                    v = normalizeValueFunc == null ? v : normalizeValueFunc(v);
                    dictionary.Add(k, v);
                }
            }
            return dictionary;
        }
    }
}
