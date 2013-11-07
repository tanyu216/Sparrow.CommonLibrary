using System.Collections.Generic;

namespace Sparrow.CommonLibrary.Entity
{
    public interface IEntityFieldAccessor
    {
        /// <summary>
        /// 获取或设置成员字段的值
        /// </summary>
        /// <param name="field">成员字段</param>
        /// <returns>当field不存在时返回空。</returns>
        object this[string field] { get; set; }

        /// <summary>
        /// 被赋值过的成员字段
        /// </summary>
        IEnumerable<string> GetSettedFields();

        /// <summary>
        /// 获取成员字段的值
        /// </summary>
        /// <param name="fields">成员字段（字段名称）</param>
        /// <returns></returns>
        IEnumerable<ItemValue> GetValues(IEnumerable<string> fields);
    }
}
