using System.Collections.Generic;
using Sparrow.CommonLibrary.Mapper.Metadata;
using Sparrow.CommonLibrary.Database;

namespace Sparrow.CommonLibrary.Entity
{
    /// <summary>
    /// 实体信息阐述
    /// </summary>
    public interface IEntityExplain : IEntityFieldAccessor, IEntity, IMetaInfo
    {
        /// <summary>
        /// 获取或设置成员字段的值
        /// </summary>
        /// <param name="field">成员字段</param>
        /// <returns>当field不存在时返回空。</returns>
        object this[string field] { get; set; }

    }
}
