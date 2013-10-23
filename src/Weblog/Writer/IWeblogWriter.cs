using System.Collections.Generic;

namespace Sparrow.CommonLibrary.Weblog.Writer
{
    public interface IWeblogWriter
    {
        void AddParameter(string name, string value);
        string GetParameter(string name);
        void Write(WeblogEntryCollection weblogEntry);
    }
}
