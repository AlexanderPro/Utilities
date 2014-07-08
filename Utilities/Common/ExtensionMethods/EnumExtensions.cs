using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Utilities.Common.ExtensionMethods
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets the description of enum value.
        /// </summary>
        /// <param name="value">The instance of the <see cref="Enum"/> structure.</param>
        /// <returns>The string description of enum value.</returns>
        public static String GetDescription(this Enum value)
        {
            var attribute = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false).SingleOrDefault() as DescriptionAttribute;
            var description = attribute == null ? String.Empty : attribute.Description;
            return description;
        }
    }
}
