using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Web
{
    public class HttpClientAttribute : Attribute
    {
        public string Url { get; set; }
    }
}
