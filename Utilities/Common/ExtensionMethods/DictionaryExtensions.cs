using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities.Common.ExtensionMethods
{
    public static class DictionaryExtensions
    {
        public static String ToKeyValueString<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, String keyValueDelimiter = "=", String pairDelimiter = ";", Func<String, String> normalizeKeyFunc = null, Func<String, String> normalizeValueFunc = null)
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

        public static IDictionary<String, String> ToDictionary(this String value, String keyValueDelimiter = "=", String pairDelimiter = ";", Func<String, String> normalizeKeyFunc = null, Func<String, String> normalizeValueFunc = null)
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
