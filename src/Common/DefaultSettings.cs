using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary
{
    /// <summary>
    /// 全局默认配置
    /// </summary>
    public static class DefaultSettings
    {
        /// <summary>
        /// app.config/web.config 配置节点sectionGroup名。
        /// </summary>
        public static string ConfigurationName { get; set; }

        static DefaultSettings()
        {
            ConfigurationName = "sparrow.CommonLibrary";
        }

    }
}
