using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Weblog
{
    public class WeblogEntry
    {
        public string[] Data { get; set; }

        public WeblogEntry()
        {
        }

        public WeblogEntry(string[] data)
        {
            this.Data = data;
        }
    }
}
