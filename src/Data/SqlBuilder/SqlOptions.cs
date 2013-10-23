namespace Sparrow.CommonLibrary.Data.SqlBuilder
{
    /// <summary>
    /// Sql选项
    /// </summary>
    public enum SqlOptions
    {
        /// <summary>
        /// 无选项
        /// </summary>
        None = 0,
        /// <summary>
        /// 不使用锁
        /// </summary>
        /// <remarks>NoLock/RowLock/UpLock/TableLock,只能使用其中一个锁,当参数指定多个锁时,则按以上顺序设置锁</remarks>
        NoLock = 1,
        /// <summary>
        /// 行锁
        /// </summary>
        /// <remarks>NoLock/RowLock/UpLock/TableLock,只能使用其中一个锁,当参数指定多个锁时,则按以上顺序设置锁</remarks>
        RowLock = 4,
        /// <summary>
        /// 共享更新封锁
        /// </summary>
        /// <remarks>NoLock/RowLock/UpLock/TableLock,只能使用其中一个锁,当参数指定多个锁时,则按以上顺序设置锁</remarks>
        UpdateLock = 16,
        /// <summary>
        /// 独占封锁方式（表锁）
        /// </summary>
        /// <remarks>NoLock/RowLock/UpLock/TableLock,只能使用其中一个锁,当参数指定多个锁时,则按以上顺序设置锁</remarks>
        TableLock = 64
    }
}
