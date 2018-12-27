using System.IO;

namespace Utilities.IO
{
    /// <summary>
    /// A helper class for File operations.
    /// </summary>
    public static class FileUtils
    {
        /// <summary>
        /// Checks and deletes given file if it does exists.
        /// </summary>
        /// <param name="fileName">Path of the file</param>
        public static void DeleteIfExists(string fileName)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }
    }
}
