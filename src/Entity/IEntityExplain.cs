using Sparrow.CommonLibrary.Mapper;
using Sparrow.CommonLibrary.Mapper.Metadata;

namespace Sparrow.CommonLibrary.Entity
{
    /// <summary>
    /// 实体信息阐述
    /// </summary>
    public interface IEntityExplain : IEntityFieldAccessor, IEntity, IDbMetaInfo, IMappingTrigger
    {
        /// <summary>
        /// 实体对象
        /// </summary>
        object EntityData { get; }
    }

    /// <summary>
    /// 实体信息阐述
    /// </summary>
    public interface IEntityExplain<T> : IEntityExplain
    {
        /// <summary>
        /// 实体对象
        /// </summary>
        new T EntityData { get; }
    }
}
