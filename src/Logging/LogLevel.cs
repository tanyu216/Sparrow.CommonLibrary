namespace Sparrow.CommonLibrary.Logging
{
    /// <summary>
    /// 日志级别
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// 调试级
        /// </summary>
        Debug = 1,
        /// <summary>
        /// 警告级
        /// </summary>
        Warning = 8,
        /// <summary>
        /// 跟踪级
        /// </summary>
        Trace = 64,
        /// <summary>
        /// 消息级
        /// </summary>
        Info = 128,
        /// <summary>
        /// 错误级
        /// </summary>
        Error = 1024
    }
}
