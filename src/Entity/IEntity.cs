using System;
using System.Collections.Generic;
using Sparrow.CommonLibrary.Database;

namespace Sparrow.CommonLibrary.Entity
{
    /// <summary>
    /// 数据实体映射接口
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// 实体状态
        /// </summary>
        DataState OperationState { get; set; }

        /// <summary>
        /// 实体类型
        /// </summary>
        Type EntityType { get; }

        /// <summary>
        /// 验证成员字段是否过赋值操作
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        bool IsSetted(int index);

        /// <summary>
        /// 检测任意一个成员字段是否有赋值操作
        /// </summary>
        /// <returns></returns>
        bool AnySetted();

        /// <summary>
        /// 将实体状态置为数据导入中
        /// </summary>
        void Importing();

        /// <summary>
        /// 将实体状态置为数据导入结束
        /// </summary>
        void Imported();
    }

}
