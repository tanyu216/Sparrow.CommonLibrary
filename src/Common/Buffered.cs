using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Timers;

namespace Sparrow.CommonLibrary.Common
{
    /// <summary>
    /// 多线程安全的缓冲区，用于网络传输或文件操作的中间衔接，以提高数据输入输出的效率。
    /// </summary>
    public class Buffered<T> : IDisposable
    {
        private Timer _timer;
        private readonly ConcurrentQueue<T> _queue;

        /// <summary>
        /// Flush事件
        /// </summary>
        public event EventHandler<BufferedFlushEventArgs<T>> OnFlush;

        /// <summary>
        /// 缓冲区启用自动Flush后，每一次自动Flush的时间间隔（时间单位：毫秒，默认：4*1000毫秒）。
        /// </summary>
        public double Interval
        {
            get
            {
                TestDisposed();
                return _timer.Interval;
            }
            set
            {
                TestDisposed();
                _timer.Interval = value;
            }
        }

        /// <summary>
        /// 缓冲区最大值，超出该值时，将会引发缓冲区溢出异常（默认值：131072）。
        /// </summary>
        public int MaxBufer { get; set; }

        /// <summary>
        /// 缓冲区临界值，当该临界值时强制触发Flush事件（默认值：65535），小于 1 则不会触发Flush事件。
        /// </summary>
        public int Threshold { get; set; }

        /// <summary>
        /// 指示当前实例是否正在执行Flush事件。
        /// </summary>
        public bool Flushing { get; private set; }

        private object SyncFlushing = new object();

        /// <summary>
        /// 缓冲区初始化
        /// </summary>
        public Buffered()
            : this(4 * 1000)
        {
        }

        /// <summary>
        /// 缓冲区初始化
        /// </summary>
        /// <param name="autoFlush">设置缓冲区自动Flush</param>
        /// <param name="interval">当缓冲区设置自动Flush后，每一次自动Flush的时间间隔</param>
        public Buffered(double interval)
        {
            MaxBufer = 131072;
            Threshold = 65535;
            _queue = new ConcurrentQueue<T>();
            _timer = new Timer(interval);
            _timer.Elapsed += TimerElapsed;
            _timer.Enabled = true;
            _timer.Start();
        }

        /// <summary>
        /// 定时Flush事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (_queue.Count > 0)
                Flush();
        }

        /// <summary>
        /// 刷新缓冲区
        /// </summary>
        public void Flush()
        {
            TestDisposed();

            int max = _queue.Count;
            if (max < 1)
                return;

            if (Flushing)
                return;
            lock (SyncFlushing)
            {
                if (Flushing)
                    return;
                Flushing = true;
            }

            var list = new List<T>(max);
            for (int i = 0; i < max; i++)
            {
                T item;
                if (_queue.TryDequeue(out item))
                {
                    list.Add(item);
                }
                else if (_queue.Count == 0)
                {
                    break;
                }
            }

            // 触发Flush事件
            if (list.Count > 0 && OnFlush != null)
                OnFlush(this, new BufferedFlushEventArgs<T>(list));

            Flushing = false;
        }

        /// <summary>
        /// 向缓冲区写入一个对象。
        /// </summary>
        /// <param name="item"></param>
        public void Write(T item)
        {
            TestDisposed();
            if (_queue.Count > MaxBufer)
                throw new OutOfMemoryException(string.Format("缓冲区溢出，超出最大值{0}。", MaxBufer));
            _queue.Enqueue(item);
            if (_queue.Count >= Threshold)
                Flush();
        }

        /// <summary>
        /// 向缓冲区写入一组对象集合。
        /// </summary>
        /// <param name="items"></param>
        public void Write(ICollection<T> items)
        {
            TestDisposed();
            if (items == null)
                throw new ArgumentNullException("items");
            if (_queue.Count + items.Count > MaxBufer)
                throw new OutOfMemoryException(string.Format("缓冲区溢出，超出最大值{0}。", MaxBufer));
            foreach (var item in items)
            {
                _queue.Enqueue(item);
            }
            if (_queue.Count >= Threshold)
                Flush();
        }

        #region IDispose
        private bool _disposed;
        private void TestDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName, "缓冲区已经释放所有资源。");
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Flush();
                _timer.Dispose();
                _timer = null;
                _disposed = true;
            }
        }
        #endregion
    }

    public class BufferedFlushEventArgs<T> : EventArgs
    {
        public IList<T> List { get; private set; }

        public BufferedFlushEventArgs(IList<T> list)
        {
            this.List = list;
        }
    }
}
