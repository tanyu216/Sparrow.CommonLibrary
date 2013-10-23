using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Weblog.Writer
{
    public abstract class WeblogWriterBase : IWeblogWriter
    {
        public WeblogWriterBase()
        {
            _parameters = new Dictionary<string, string>();
        }

        #region implement

        private readonly IDictionary<string, string> _parameters;

        public void AddParameter(string name, string value)
        {
            _parameters[name] = value;
        }

        public string GetParameter(string name)
        {
            string value;
            if (_parameters.TryGetValue(name, out value))
                return value;
            return null;
        }

        public abstract void Write(WeblogEntryCollection weblogEntry);

        #endregion

    }
}
