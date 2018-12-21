using System;
using System.ComponentModel;

namespace Utilities.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="Object"/> class.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Determines if the object is null
        /// </summary>
        /// <param name="Object">The object to check</param>
        /// <returns>True if it is null, false otherwise</returns>
        public static Boolean IsNull(this Object value)
        {
            return value == null;
        }

        /// <summary>
        /// Determines if the object is not null
        /// </summary>
        /// <param name="Object">The object to check</param>
        /// <returns>False if it is null, true otherwise</returns>
        public static Boolean IsNotNull(this Object value)
        {
            return value != null;
        }

        /// <summary>
        /// Determines if the object is null or DBNull
        /// </summary>
        /// <param name="Object">The object to check</param>
        /// <returns>True if it is null/DBNull, false otherwise</returns>
        public static Boolean IsNullOrDBNull(this Object value)
        {
            return value == null || Convert.IsDBNull(value);
        }

        /// <summary>
        /// Determines if the object is not null or DBNull
        /// </summary>
        /// <param name="Object">The object to check</param>
        /// <returns>False if it is null/DBNull, true otherwise</returns>
        public static Boolean IsNotNullOrNotDBNull(this Object value)
        {
            return value != null && !Convert.IsDBNull(value);
        }

        /// <summary>
        /// Attempts to convert the object to another type and returns the value
        /// </summary>
        /// <typeparam name="T">Type to convert from</typeparam>
        /// <typeparam name="R">Return type</typeparam>
        /// <param name="value">Object to convert</param>
        /// <param name="defaultValue">Default value to return if there is an issue or it can't be converted</param>
        /// <returns>The object converted to the other type or the default value if there is an error or can't be converted</returns>
        public static R ConvertTo<T, R>(this T value, R defaultValue = default(R))
        {
            try
            {
                return (R)value.ConvertTo(typeof(R), defaultValue);
            }
            catch 
            { 
            }
            return defaultValue;
        }

        /// <summary>
        /// Attempts to convert the object to another type and returns the value
        /// </summary>
        /// <typeparam name="T">Type to convert from</typeparam>
        /// <param name="resultType">Result type</param>
        /// <param name="value">Object to convert</param>
        /// <param name="defaultValue">Default value to return if there is an issue or it can't be converted</param>
        /// <returns>The object converted to the other type or the default value if there is an error or can't be converted</returns>
        internal static Object ConvertTo<T>(this T value, Type resultType, Object defaultValue = null)
        {
            try
            {
                if (value.IsNullOrDBNull())
                {
                    return defaultValue;
                }
                if ((value as string).IsNotNull())
                {
                    String objectValue = value as String;
                    if (resultType.IsEnum)
                    {
                        return System.Enum.Parse(resultType, objectValue, true);
                    }
                    if (String.IsNullOrEmpty(objectValue))
                    {
                        return defaultValue;
                    }
                }
                if ((value as IConvertible).IsNotNull())
                {
                    return Convert.ChangeType(value, resultType);
                }
                if (resultType.IsAssignableFrom(value.GetType()))
                {
                    return value;
                }
                TypeConverter Converter = TypeDescriptor.GetConverter(value.GetType());
                if (Converter.CanConvertTo(resultType))
                {
                    return Converter.ConvertTo(value, resultType);
                }
                if ((value as String).IsNotNull())
                {
                    return value.ToString().ConvertTo<String>(resultType, defaultValue);
                }
            }
            catch 
            { 
            }
            return defaultValue;
        }

        /// <summary>
        /// Converts specified value to nullable value.
        /// </summary>
        /// <param name="value">The result value.</param>
        /// <returns></returns>
        public static T? ConvertTo<T>(this Object value) where T : struct
        {
            if (value.IsNullOrDBNull())
            {
                return null;
            }
            else
            {
                if (!(value is T))
                {
                    try
                    {
                        value = Convert.ChangeType(value, typeof(T));
                    }
                    catch (Exception e)
                    {
                        throw new ArgumentException("Value is not a valid type.", "value", e);
                    }
                }

                return new T?((T)value);
            }
        }
    }
}
