using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sparrow.CommonLibrary.TransientFault
{
    /// <summary>
    /// 重试策略
    /// </summary>
    public abstract class RetryStrategy
    {
        /// <summary>
        /// 默认最大重试次数。
        /// </summary>
        public static readonly int DefaultMaxRetryCount = 5;

        /// <summary>
        /// <see cref="FixedInterval"/>默认重试的时间间隔。
        /// </summary>
        public static readonly TimeSpan DefaultRetryFixedInterval = TimeSpan.FromSeconds(1);

        /// <summary>
        /// <see cref="IncrementalInterval"/>默认重试的时间间隔。
        /// </summary>
        public static readonly TimeSpan DefaultRetryIncrementalInterval = TimeSpan.FromSeconds(1);

        /// <summary>
        /// <see cref="ExponentialBackoffInterval"/>默认重试时间间隔最小指数。
        /// </summary>
        public static readonly TimeSpan DefaultRetryMinBackoff = TimeSpan.FromSeconds(1);

        /// <summary>
        /// <see cref="ExponentialBackoffInterval"/>默认重试时间间隔最大指数。
        /// </summary>
        public static readonly TimeSpan DefaultRetryMaxBackoff = TimeSpan.FromSeconds(30);

        /// <summary>
        /// <see cref="ExponentialBackoffInterval"/>默认计算指数延迟指数的三角。
        /// </summary>
        public static readonly TimeSpan DefaultDeltaBackoff = TimeSpan.FromSeconds(10);

        /// <summary>
        /// 默认第一次异常时立即重试。
        /// </summary>
        public static readonly bool DefaultFirstFastRetry = true;

        /// <summary>
        /// <see cref="RetryStrategy"/>名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 指示第一次异常时是否立即重试
        /// </summary>
        public bool FirstFastRetry { get; set; }

        /// <summary>
        /// 最大重试次数
        /// </summary>
        public int MaxRetryCount { get; private set; }

        /// <summary>
        /// “重试”事件
        /// </summary>
        public event EventHandler<RetryingEventArgs> OnRetrying;

        protected RetryStrategy(string name, bool firstFastRetry)
            : this(name, firstFastRetry, DefaultMaxRetryCount)
        {
        }

        protected RetryStrategy(string name, bool firstFastRetry, int maxRetryCount)
        {
            this.Name = name;
            this.FirstFastRetry = firstFastRetry;
            this.MaxRetryCount = maxRetryCount;
        }

        public void DoExecute(Action action)
        {
            DoExecute(() => { action(); return default(object); });
        }

        public TResult DoExecute<TResult>(Func<TResult> action)
        {
            if (action == null)
                throw new ArgumentNullException("func");
            //
            int retryCount = 0;
            TimeSpan delay = TimeSpan.Zero;
            Exception lastException;
            //
            do
            {
                try
                {
                    return action();
                }
                catch (Exception exception)
                {
                    lastException = exception;
                    //
                    if (!ShouldRetry(retryCount++, lastException, out delay))
                        throw;
                }

                if (delay.TotalMilliseconds < 0)
                    delay = TimeSpan.Zero;

                if (!this.Retrying(retryCount, lastException, delay))
                    throw lastException;

                if (retryCount > 1 || !this.FirstFastRetry)
                    Thread.Sleep(delay);

            } while (true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="retryCount">重试次数</param>
        /// <param name="lastException">最后一次抛出的异常</param>
        /// <param name="delay">重试时的延迟</param>
        /// <returns>返回true表示继续重试，否则表示停止重试结束操作。</returns>
        protected abstract bool ShouldRetry(int retryCount, Exception lastException, out TimeSpan delay);

        /// <summary>
        /// 触发OnRetrying事件。
        /// </summary>
        /// <param name="retryCount"></param>
        /// <param name="lastException"></param>
        /// <param name="delay"></param>
        /// <returns>指示是否继续执行</returns>
        protected virtual bool Retrying(int retryCount, Exception lastException, TimeSpan delay)
        {
            if (OnRetrying != null)
            {
                var args = new RetryingEventArgs(retryCount, lastException, delay) { Retrying = true };
                OnRetrying(this, args);
                return args.Retrying;
            }
            //
            return true;
        }
    }
}
