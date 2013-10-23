using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Sparrow.CommonLibrary.Weblog.Collect
{
    /// <summary>
    /// Weblog信息采集器，实现该接口的对象通常都支持单件模式。
    /// </summary>
    public interface ICollecter
    {
        string Name { get; }
        string GetValue(HttpApplication app);
    }

    /// <summary>
    /// 带上下文的Weblog采集器，实现该接口的对象通常不支持单件模式。
    /// </summary>
    interface ICollecterWithContext : ICollecter
    {
        void Begin(HttpApplication app);
        void End(HttpApplication app);
    }
}
