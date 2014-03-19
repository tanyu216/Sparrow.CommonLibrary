using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Retrying
{
    /// <summary>
    /// 无重试规则
    /// </summary>
    public class NonRetryInterval : RetryStrategy
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public NonRetryInterval()
            : base(null, false, 0)
        {
        }
        /// <summary>
        /// 检测是否应该继续执行重试策略，以及返回重试间隔时间。
        /// </summary>
        /// <param name="retryCount">重试次数</param>
        /// <param name="lastException">引发重试的异常</param>
        /// <param name="delay">重试时的延迟</param>
        /// <returns>返回true表示继续重试，否则表示停止重试结束操作。</returns>
        protected override bool ShouldRetry(int retryCount, Exception lastException, out TimeSpan delay)
        {
            delay = TimeSpan.Zero;
            return false;
        }
    }
}
