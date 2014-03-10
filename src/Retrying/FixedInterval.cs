using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Retrying
{
    /// <summary>
    /// 定期重试策略
    /// <para>重试间隔示例（秒）:2,2,2,2,2</para>
    /// </summary>
    public class FixedInterval : RetryStrategy
    {
        private readonly TimeSpan interval;

        /// <summary>
        /// 重试初始化
        /// </summary>
        public FixedInterval()
            : this(DefaultMaxRetryCount, DefaultRetryFixedInterval, DefaultFirstFastRetry)
        {
        }

        /// <summary>
        /// 重试初始化
        /// </summary>
        /// <param name="retryCount">最大重试次数</param>
        public FixedInterval(int retryCount)
            : this(retryCount, DefaultRetryFixedInterval, DefaultFirstFastRetry)
        {
        }

        /// <summary>
        /// 重试初始化
        /// </summary>
        /// <param name="interval">重试时间间隔</param>
        public FixedInterval(TimeSpan interval)
            : this(DefaultMaxRetryCount, interval, DefaultFirstFastRetry)
        {
        }

        /// <summary>
        /// 重试初始化
        /// </summary>
        /// <param name="firstFastRetry">第一次是否重试</param>
        public FixedInterval(bool firstFastRetry)
            : this(DefaultMaxRetryCount, DefaultRetryFixedInterval, firstFastRetry)
        {
        }

        /// <summary>
        /// 重试初始化
        /// </summary>
        /// <param name="retryCount">最大重试次数</param>
        /// <param name="interval">重试时间间隔</param>
        public FixedInterval(int retryCount, TimeSpan interval)
            : this(retryCount, interval, DefaultFirstFastRetry)
        {
        }

        /// <summary>
        /// 重试初始化
        /// </summary>
        /// <param name="retryCount">最大重试次数</param>
        /// <param name="interval">重试时间间隔</param>
        /// <param name="firstFastRetry">第一次是否重试</param>
        public FixedInterval(int retryCount, TimeSpan interval, bool firstFastRetry)
            : this(retryCount, interval, firstFastRetry, null)
        {
        }

        /// <summary>
        /// 重试初始化
        /// </summary>
        /// <param name="retryCount">最大重试次数</param>
        /// <param name="interval">重试时间间隔</param>
        /// <param name="firstFastRetry">第一次是否重试</param>
        /// <param name="name">重试策略名称</param>
        public FixedInterval(int retryCount, TimeSpan interval, bool firstFastRetry, string name)
            : base(name, firstFastRetry, retryCount)
        {
            this.interval = interval;
        }

        /// <summary>
        /// 检测是否应该继续执行重试策略，以及返回重试间隔时间。
        /// </summary>
        /// <param name="retryCount"></param>
        /// <param name="lastException"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        protected override bool ShouldRetry(int retryCount, Exception lastException, out TimeSpan delay)
        {
            if (retryCount < MaxRetryCount)
            {
                delay = this.interval;
                return true;
            }
            //
            delay = TimeSpan.Zero;
            return false;
        }
    }
}
