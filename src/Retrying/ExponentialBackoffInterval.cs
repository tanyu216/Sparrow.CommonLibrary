using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Retrying
{
    public class ExponentialBackoffInterval : RetryStrategy
    {
        private readonly TimeSpan minBackoff;
        private readonly TimeSpan maxBackoff;
        private readonly TimeSpan deltaBackoff;

        public ExponentialBackoffInterval()
            : this(DefaultMaxRetryCount, DefaultRetryMinBackoff, DefaultRetryMaxBackoff, DefaultDeltaBackoff, DefaultFirstFastRetry)
        {
        }

        public ExponentialBackoffInterval(int retryCount)
            : this(retryCount, DefaultRetryMinBackoff, DefaultRetryMaxBackoff, DefaultDeltaBackoff, DefaultFirstFastRetry)
        {
        }

        public ExponentialBackoffInterval(TimeSpan minInterval, TimeSpan maxInterval, TimeSpan delta)
            : this(DefaultMaxRetryCount, minInterval, maxInterval, delta, DefaultFirstFastRetry)
        {
        }

        public ExponentialBackoffInterval(bool firstFastRetry)
            : this(DefaultMaxRetryCount, DefaultRetryMinBackoff, DefaultRetryMaxBackoff, DefaultDeltaBackoff, firstFastRetry)
        {
        }

        public ExponentialBackoffInterval(int retryCount, TimeSpan minInterval, TimeSpan maxInterval, TimeSpan delta)
            : this(retryCount, minInterval, maxInterval, delta, DefaultFirstFastRetry)
        {
        }

        public ExponentialBackoffInterval(int retryCount, TimeSpan minInterval, TimeSpan maxInterval, TimeSpan delta, bool firstFastRetry)
            : this(retryCount, minInterval, maxInterval, delta, firstFastRetry, null)
        {
        }

        public ExponentialBackoffInterval(int retryCount, TimeSpan minInterval, TimeSpan maxInterval, TimeSpan delta, bool firstFastRetry, string name)
            : base(name, firstFastRetry, retryCount)
        {
            this.minBackoff = minInterval;
            this.maxBackoff = maxInterval;
            this.deltaBackoff = delta;
        }

        protected override bool ShouldRetry(int retryCount, Exception lastException, out TimeSpan delay)
        {
            if (retryCount < MaxRetryCount)
            {
                var random = new Random();

                var delta = (int)((Math.Pow(2.0, retryCount) - 1.0) * random.Next((int)(this.deltaBackoff.TotalMilliseconds * 0.8), (int)(this.deltaBackoff.TotalMilliseconds * 1.2)));
                var interval = (int)Math.Min(checked(this.minBackoff.TotalMilliseconds + delta), this.maxBackoff.TotalMilliseconds);
                delay = TimeSpan.FromMilliseconds(interval);
                return true;
            }
            //
            delay = TimeSpan.Zero;
            return false;
        }
    }
}
