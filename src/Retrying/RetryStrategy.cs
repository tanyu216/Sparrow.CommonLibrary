using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sparrow.CommonLibrary.Retrying
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

        /// <summary>
        /// 重试初始化
        /// </summary>
        /// <param name="name">重试策略名称</param>
        /// <param name="firstFastRetry">第一次异常时立即重试</param>
        protected RetryStrategy(string name, bool firstFastRetry)
            : this(name, firstFastRetry, DefaultMaxRetryCount)
        {
        }

        /// <summary>
        /// 重试初始化
        /// </summary>
        /// <param name="name">重试策略名称</param>
        /// <param name="firstFastRetry">第一次异常时立即重试</param>
        /// <param name="maxRetryCount">重试最大次数</param>
        protected RetryStrategy(string name, bool firstFastRetry, int maxRetryCount)
        {
            this.Name = name;
            this.FirstFastRetry = firstFastRetry;
            this.MaxRetryCount = maxRetryCount;
        }

        /// <summary>
        /// 执行无返回值的方法
        /// </summary>
        /// <param name="action">重试委托</param>
        public void DoExecute(Action action)
        {
            DoExecute(() => { action(); return default(object); });
        }

        /// <summary>
        /// 执行有返回值的方法
        /// </summary>
        /// <typeparam name="TResult">返回值类型</typeparam>
        /// <param name="action">重试委托</param>
        /// <returns>返回结果</returns>
        public TResult DoExecute<TResult>(Func<TResult> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("func");
            }
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
                    {
                        throw;
                    }
                }

                if (delay.TotalMilliseconds < 0)
                {
                    delay = TimeSpan.Zero;
                }

                if (!this.Retrying(retryCount, lastException, delay))
                {
                    throw lastException;
                }

                if (retryCount > 1 || !this.FirstFastRetry)
                {
                    Thread.Sleep(delay);
                }

            } while (true);
        }

        /// <summary>
        /// 检测是否应该继续执行重试策略，以及返回重试间隔时间。
        /// </summary>
        /// <param name="retryCount">重试次数</param>
        /// <param name="lastException">引发重试的异常</param>
        /// <param name="delay">重试时的延迟</param>
        /// <returns>返回true表示继续重试，否则表示停止重试结束操作。</returns>
        protected abstract bool ShouldRetry(int retryCount, Exception lastException, out TimeSpan delay);

        /// <summary>
        /// 触发OnRetrying事件。
        /// </summary>
        /// <param name="retryCount">重试次数</param>
        /// <param name="lastException">引发重试的异常</param>
        /// <param name="delay">重试时的延迟</param>
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
