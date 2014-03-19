using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Retrying
{
    /// <summary>
    /// 递增重试策略
    /// <para>重试间隔示例（秒）:2,4,6,8,10</para>
    /// </summary>
    public class IncrementalInterval : RetryStrategy
    {
        private readonly TimeSpan interval;

        /// <summary>
        /// 重试初始化
        /// </summary>
        public IncrementalInterval()
            : this(DefaultMaxRetryCount, DefaultRetryIncrementalInterval, DefaultFirstFastRetry)
        {
        }

        /// <summary>
        /// 重试初始化
        /// </summary>
        /// <param name="retryCount">最大重试次数</param>
        public IncrementalInterval(int retryCount)
            : this(retryCount, DefaultRetryIncrementalInterval, DefaultFirstFastRetry)
        {
        }

        /// <summary>
        /// 重试初始化
        /// </summary>
        /// <param name="interval">重试时间间隔</param>
        public IncrementalInterval(TimeSpan interval)
            : this(DefaultMaxRetryCount, interval, DefaultFirstFastRetry)
        {
        }

        /// <summary>
        /// 重试初始化
        /// </summary>
        /// <param name="firstFastRetry">第一次异常时立即重试</param>
        public IncrementalInterval(bool firstFastRetry)
            : this(DefaultMaxRetryCount, DefaultRetryIncrementalInterval, firstFastRetry)
        {
        }

        /// <summary>
        /// 重试初始化
        /// </summary>
        /// <param name="retryCount">最大重试次数</param>
        /// <param name="interval">重试时间间隔</param>
        public IncrementalInterval(int retryCount, TimeSpan interval)
            : this(retryCount, interval, DefaultFirstFastRetry)
        {
        }

        /// <summary>
        /// 重试初始化
        /// </summary>
        /// <param name="retryCount">最大重试次数</param>
        /// <param name="interval">重试时间间隔</param>
        /// <param name="firstFastRetry">第一次异常时立即重试</param>
        public IncrementalInterval(int retryCount, TimeSpan interval, bool firstFastRetry)
            : this(retryCount, interval, firstFastRetry, null)
        {
        }

        /// <summary>
        /// 重试初始化
        /// </summary>
        /// <param name="retryCount">最大重试次数</param>
        /// <param name="interval">重试时间间隔</param>
        /// <param name="firstFastRetry">第一次异常时立即重试</param>
        /// <param name="name">重试策略名称</param>
        public IncrementalInterval(int retryCount, TimeSpan interval, bool firstFastRetry, string name)
            : base(name, firstFastRetry, retryCount)
        {
            this.interval = interval;
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
            if (retryCount < MaxRetryCount)
            {
                delay = TimeSpan.FromMilliseconds(this.interval.TotalMilliseconds * retryCount + this.interval.TotalMilliseconds);
                return true;
            }
            //
            delay = TimeSpan.Zero;
            return false;
        }
    }
}
