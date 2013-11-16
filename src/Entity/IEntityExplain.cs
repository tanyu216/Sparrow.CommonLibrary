using Sparrow.CommonLibrary.Mapper;
using Sparrow.CommonLibrary.Mapper.Metadata;

namespace Sparrow.CommonLibrary.Entity
{
    /// <summary>
    /// 实体信息阐述
    /// </summary>
    public interface IEntityExplain : IEntityFieldAccessor, IEntity, IMappingTrigger
    {
        /// <summary>
        /// 实体映射对象
        /// </summary>
        IMapper Mapper { get; }

        /// <summary>
        /// 表名
        /// </summary>
        string TableName { get; }

        /// <summary>
        /// 自动增长序列
        /// </summary>
        DbIncrementMetaPropertyInfo Increment { get; }

        /// <summary>
        /// 验证是否为主键主字段
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        bool IsKey(string columnName);

        /// <summary>
        /// 获取所有主键
        /// </summary>
        /// <returns></returns>
        string[] GetKeys();
    }

    /// <summary>
    /// 实体信息阐述
    /// </summary>
    public interface IEntityExplain<T>
    {
        /// <summary>
        /// 实体映射对象
        /// </summary>
        new IMapper<T> Mapper { get; }

    }
}
