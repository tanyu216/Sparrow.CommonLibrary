namespace Sparrow.CommonLibrary.Data.Entity
{
    /// <summary>
    /// 数据状态
    /// </summary>
    public enum DataState
    {
        /// <summary>
        /// 数据为新建状态，在执行保存操作时只会插入的方式
        /// </summary>
        New = 0,
        /// <summary>
        /// 数据为修改状态，在执行保存操作时只会修改的方式，此状态下要求对象必须拥有主键值
        /// </summary>
        Modify = 1,
        /// <summary>
        /// 新建或修改状态，在执行保存时会在数据库中判断数据（主键）如果存在则更新数据，否则插入数据。
        /// </summary>
        NewOrModify = 2
    }
}
