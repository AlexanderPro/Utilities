using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Utilities.Logging
{
    public class FileLogger : Logger
    {
        #region Properties.Public

        public Encoding Encoding
        {
            get;
            set;
        }

        public String FileName
        {
            get;
            set;
        }

        #endregion

        #region Methods.Public

        public FileLogger(String fileName)
            : this(fileName, Encoding.UTF8)
        {

        }

        public FileLogger(String fileName, Encoding encoding)
        {
            FileName = fileName;
            Encoding = encoding;
        }

        public override void Write(ILoggerEntry entry)
        {
            try
            {
                if (base._enabled)
                {
                    String fileName = ProcessFileName(FileName);
                    String directoryPath = Path.GetDirectoryName(fileName);
                    if (!String.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    using (FileStream stream = new FileStream(fileName, FileMode.Append, FileAccess.Write))
                    {
                        using (TextWriter writer = new StreamWriter(stream, Encoding))
                        {
                            writer.WriteLine(entry.ToString());
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                throw new InvalidOperationException("Unable to write entry!", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unable to write entry!", ex);
            }
        }

        private String ProcessFileName(String fileName)
        {
            fileName = String.Format(fileName, DateTime.Now);
            if (fileName.StartsWith("~"))
            {
                String executablePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                fileName = (System.Web.HttpContext.Current != null) ? System.Web.HttpContext.Current.Server.MapPath(fileName) : (executablePath + fileName.Remove(0, 1));
            }
            return fileName;
        }

        #endregion
    }
}