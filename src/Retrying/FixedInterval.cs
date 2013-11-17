using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Retrying
{
    /// <summary>
    /// 定期重试。
    /// <para>重试间隔示例（秒）:2,2,2,2,2</para>
    /// </summary>
    public class FixedInterval : RetryStrategy
    {
        private readonly TimeSpan interval;

        public FixedInterval()
            : this(DefaultMaxRetryCount, DefaultRetryFixedInterval, DefaultFirstFastRetry)
        {
        }

        public FixedInterval(int retryCount)
            : this(retryCount, DefaultRetryFixedInterval, DefaultFirstFastRetry)
        {
        }

        public FixedInterval(TimeSpan interval)
            : this(DefaultMaxRetryCount, interval, DefaultFirstFastRetry)
        {
        }

        public FixedInterval(bool firstFastRetry)
            : this(DefaultMaxRetryCount, DefaultRetryFixedInterval, firstFastRetry)
        {
        }

        public FixedInterval(int retryCount, TimeSpan interval)
            : this(retryCount, interval, DefaultFirstFastRetry)
        {
        }

        public FixedInterval(int retryCount, TimeSpan interval, bool firstFastRetry)
            : this(retryCount, interval, firstFastRetry, null)
        {
        }

        public FixedInterval(int retryCount, TimeSpan interval, bool firstFastRetry, string name)
            : base(name, firstFastRetry, retryCount)
        {
            this.interval = interval;
        }

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
