using System;
using System.IO;

namespace Utilities.IO
{
    /// <summary>
    /// A helper class for Directory operations.
    /// </summary>
    public static class DirectoryHelper
    {
        /// <summary>
        ///  Copies System.IO.DirectoryInfo's contents to a new path.
        /// </summary>
        /// <param name="source">The path of source directory.</param>
        /// <param name="destination">The path to which to copy the directory.</param>
        /// <param name="overwriteFiles">Determines if the files must be overwritten.</param>
        public static void Copy(string source, string destination, Boolean overwriteFiles = true)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }

            foreach (String path in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(path.Replace(source, destination));
            }

            foreach (String newPath in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(source, destination), overwriteFiles);
            }
        }

        /// <summary>
        /// Creates a new directory if it does not exist.
        /// </summary>
        /// <param name="directoryName">Directory to create</param>
        public static void CreateIfNotExists(string directoryName)
        {
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
        }
    }
}