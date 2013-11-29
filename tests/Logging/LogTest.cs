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
        public void DebugTest1()
        {
            Log.GetLog().Debug("test Debug" + DateTime.Now.ToString());
            Log.GetLog().Debug("test2 Debug" + DateTime.Now.ToString(), null, new { id = 1, name = "name" });
            Log.GetLog().Debug("test3 Debug" + DateTime.Now.ToString(), null, new ExtendProperties(new { id = 1, name = "name" }, false, false, false, false, null));
            Log.Flush();
        }

        [Test]
        public void DebugTest2()
        {
            for (var i = 0; i < 10000; i++)
            {
                Log.GetLog().Debug("test Debug" + DateTime.Now.ToString());
                Log.GetLog().Debug("test2 Debug" + DateTime.Now.ToString(), null, new { id = 1, name = "name" });
                Log.GetLog().Debug("test3 Debug" + DateTime.Now.ToString(), null, new ExtendProperties(new { id = 1, name = "name" }, false, false, false, false, null));
            }
            Log.Flush();
        }

        [Test]
        public void WarningTest1()
        {
            Log.GetLog().Warning("test Warning" + DateTime.Now.ToString());
            Log.GetLog().Warning("test2 Warning" + DateTime.Now.ToString(), null, new { id = 1, name = "name" });
            Log.GetLog().Warning("test3 Warning" + DateTime.Now.ToString(), null, new ExtendProperties(new { id = 1, name = "name" }, false, false, false, false, null));
            Log.Flush();
        }

        [Test]
        public void InfoTest1()
        {
            Log.GetLog().Info("test" + DateTime.Now.ToString());
            Log.GetLog().Info("test2" + DateTime.Now.ToString(), new { id = 1, name = "name" });
            Log.GetLog().Info("test3" + DateTime.Now.ToString(), new ExtendProperties(new { id = 1, name = "name" }, false, false, false, false, null));
            Log.Flush();
        }

        [Test]
        public void TraceTest1()
        {
            Log.GetLog().Trace("Trace" + DateTime.Now.ToString());
            Log.GetLog().Trace("Trace2" + DateTime.Now.ToString(), new { id = 1, name = "name" });
            Log.GetLog().Trace("Trace3" + DateTime.Now.ToString(), new ExtendProperties(new { id = 1, name = "name" }, false, false, false, false, null));
            Log.Flush();
        }

        [Test]
        public void TraceTest2()
        {
            for (var i = 0; i < 10000; i++)
            {
                Log.GetLog().Trace("Trace" + DateTime.Now.ToString());
                Log.GetLog().Trace("Trace2" + DateTime.Now.ToString(), new { id = 1, name = "name" });
                Log.GetLog().Trace("Trace3" + DateTime.Now.ToString(), new ExtendProperties(new { id = 1, name = "name" }, false, false, false, false, null));
            }
            Log.Flush();
        }

        [Test]
        public void ErrorTest1()
        {
            Log.GetLog().Error("test error" + DateTime.Now.ToString(), null);
            Log.GetLog().Error("test2 error" + DateTime.Now.ToString(), null, new { id = 1, name = "name" });
            Log.GetLog().Error("test3 error" + DateTime.Now.ToString(), null, new ExtendProperties(new { id = 1, name = "name" }, false, false, false, false, null));
            Log.Flush();
        }
    }
}
