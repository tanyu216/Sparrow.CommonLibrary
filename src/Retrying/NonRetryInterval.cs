using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Retrying
{
    public class NonRetryInterval : RetryStrategy
    {
        public NonRetryInterval()
            : base(null, false, 0)
        {
        }

        protected override bool ShouldRetry(int retryCount, Exception lastException, out TimeSpan delay)
        {
            delay = TimeSpan.Zero;
            return false;
        }
    }
}
