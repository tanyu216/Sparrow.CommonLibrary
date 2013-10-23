using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.TransientFault
{
    /// <summary>
    /// 递增重试
    /// <para>重试间隔示例（秒）:2,4,6,8,10</para>
    /// </summary>
    public class IncrementalInterval : RetryStrategy
    {
        private readonly TimeSpan interval;

        public IncrementalInterval()
            : this(DefaultMaxRetryCount, DefaultRetryIncrementalInterval, DefaultFirstFastRetry)
        {
        }

        public IncrementalInterval(int retryCount)
            : this(retryCount, DefaultRetryIncrementalInterval, DefaultFirstFastRetry)
        {
        }

        public IncrementalInterval(TimeSpan interval)
            : this(DefaultMaxRetryCount, interval, DefaultFirstFastRetry)
        {
        }

        public IncrementalInterval(bool firstFastRetry)
            : this(DefaultMaxRetryCount, DefaultRetryIncrementalInterval, firstFastRetry)
        {
        }

        public IncrementalInterval(int retryCount, TimeSpan interval)
            : this(retryCount, interval, DefaultFirstFastRetry)
        {
        }

        public IncrementalInterval(int retryCount, TimeSpan interval, bool firstFastRetry)
            : this(DefaultMaxRetryCount, interval, firstFastRetry, null)
        {
        }

        public IncrementalInterval(int retryCount, TimeSpan interval, bool firstFastRetry, string name)
            : base(name, firstFastRetry, retryCount)
        {
            this.interval = interval;
        }

        protected override bool ShouldRetry(int retryCount, Exception lastException, out TimeSpan delay)
        {
            if (retryCount < MaxRetryCount)
            {
                delay = TimeSpan.FromMilliseconds(this.interval.TotalMilliseconds * 2 + this.interval.TotalMilliseconds);
                return true;
            }
            //
            delay = TimeSpan.Zero;
            return false;
        }
    }
}
