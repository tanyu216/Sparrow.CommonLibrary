using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Logging.Filter
{
    public interface ILogFilter
    {
        string Name { get; set; }
        string[] Categories { get; set; }
        LogLevel[] LogLevel { get; set; }
        IList<LogEntry> Filter(IList<LogEntry> logs);
    }
}
