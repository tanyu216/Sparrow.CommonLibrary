using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Sparrow.CommonLibrary.Logging;

namespace Sparrow.CommonLibrary.Test.Logging
{
    [TestFixture]
    public class LogTest
    {
        [Test]
        public void Test1()
        {
            Log.GetLog().Info("test" + DateTime.Now.ToString());
            Log.GetLog().Info("test2" + DateTime.Now.ToString(), new { id = 1, name = "name" });
            Log.GetLog().Info("test3" + DateTime.Now.ToString(), new ExtendProperties(new { id = 1, name = "name" }, false, false, false, false, null));
            Log.Flush();
        }
    }
}
