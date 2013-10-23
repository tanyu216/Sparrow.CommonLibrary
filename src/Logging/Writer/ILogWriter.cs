using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Logging.Writer
{
    public interface ILogWriter
    {
        string FilterName { get; set; }
        void AddParameter(string name, string value);
        void Write(IList<LogEntry> logs);
    }
}
