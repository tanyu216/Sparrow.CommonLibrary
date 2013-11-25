using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Logging.Filter
{
    public class LogFilter : ILogFilter
    {
        public string Name
        {
            get;
            set;
        }

        public string[] Categories
        {
            get;
            set;
        }

        public LogLevel LogLevel
        {
            get;
            set;
        }

        public IList<LogEntry> Filter(IList<LogEntry> logs)
        {
            if (logs == null)
                return new List<LogEntry>(0);
            return logs.Where(x => ((int)LogLevel & (int)x.Level) > 0 && (Categories == null || Categories.Length == 0 || x.Categories.Any(cty => Categories.Contains(cty)))).ToList();
        }
    }
}
