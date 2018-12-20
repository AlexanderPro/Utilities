using System;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Utilities.Extensions
{
    /// <summary>
    /// Extension methods for Enum.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets the description of enum value.
        /// </summary>
        /// <param name="value">The instance of the <see cref="Enum"/> structure.</param>
        /// <returns>The string description of enum value.</returns>
        public static String GetDescription(this Enum value)
        {
            var attribute = GetSingleAttributeOrNull<DescriptionAttribute>(value);
            var description = attribute == null ? String.Empty : attribute.Description;
            return description;
        }

        /// <summary>
        /// Gets the display name of enum value.
        /// </summary>
        /// <param name="value">The instance of the <see cref="Enum"/> structure.</param>
        /// <returns>The string display name of enum value.</returns>
        public static String GetDisplayName(this Enum value)
        {
            var attribute = GetSingleAttributeOrNull<DisplayAttribute>(value);
            var name = attribute == null ? String.Empty : attribute.Name;
            return name;
        }

        /// <summary>
        /// Gets the display short name of enum value.
        /// </summary>
        /// <param name="value">The instance of the <see cref="Enum"/> structure.</param>
        /// <returns>The string display short name of enum value.</returns>
        public static String GetDisplayShortName(this Enum value)
        {
            var attribute = GetSingleAttributeOrNull<DisplayAttribute>(value);
            var name = attribute == null ? String.Empty : attribute.ShortName;
            return name;
        }

        /// <summary>
        /// Gets the display description of enum value.
        /// </summary>
        /// <param name="value">The instance of the <see cref="Enum"/> structure.</param>
        /// <returns>The string display description of enum value.</returns>
        public static String GetDisplayDescription(this Enum value)
        {
            var attribute = GetSingleAttributeOrNull<DisplayAttribute>(value);
            var name = attribute == null ? String.Empty : attribute.Description;
            return name;
        }

        /// <summary>
        /// Gets a single attribute of enum value.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute</typeparam>
        /// <param name="value">The instance of the <see cref="Enum"/> structure.</param>
        /// <returns>Returns the attribute object if found. Returns null if not found.</returns>
        internal static TAttribute GetSingleAttributeOrNull<TAttribute>(this Enum value) where TAttribute : Attribute
        {
            var attribute = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(TAttribute), false).SingleOrDefault() as TAttribute;
            return attribute;
        }
    }
}
