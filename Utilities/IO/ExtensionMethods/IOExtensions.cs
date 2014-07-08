using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Utilities.IO.ExtensionMethods
{
    public static class IOExtensions
    {
        /// <summary>
        ///  Copies System.IO.DirectoryInfo's contents to a new path.
        /// </summary>
        /// <param name="source">The instance of the <see cref="System.IO.DirectoryInfo"/> class.</param>
        /// <param name="destinationPath">The name and path to which to copy this directory.</param>
        /// <param name="overwriteFiles">Determines if the files must be overwritten.</param>
        public static void Copy(this DirectoryInfo source, String destinationPath, Boolean overwriteFiles)
        {
            if (destinationPath == null) throw new ArgumentNullException("destinationPath");

            foreach (String path in Directory.GetDirectories(source.FullName, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(path.Replace(source.FullName, destinationPath));
            }

            foreach (String newPath in Directory.GetFiles(source.FullName, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(source.FullName, destinationPath), overwriteFiles);
            }
        }

        /// <summary>
        /// Copies System.IO.DirectoryInfo's contents to a new path.
        /// </summary>
        /// <param name="source">The instance of the <see cref="System.IO.DirectoryInfo"/> class.</param>
        /// <param name="destinationPath">The name and path to which to copy this directory.</param>
        public static void Copy(this DirectoryInfo source, String destinationPath)
        {
            Copy(source, destinationPath, true);
        }
    }
}
