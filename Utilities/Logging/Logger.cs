using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Logging
{
   public abstract class Logger : ILogger
   {
      protected Boolean _enabled = true;

      public abstract void Write(ILoggerEntry entry);

      public virtual Boolean Enabled
      {
         get
         {
            return _enabled;
         }
         set
         {
            _enabled = value;
         }
      }
   }
}
