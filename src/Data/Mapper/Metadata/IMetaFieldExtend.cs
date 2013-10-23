using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparrow.CommonLibrary.Data.Mapper.Metadata
{
    /// <summary>
    /// 成员字段扩展
    /// </summary>
    public interface IMetaFieldExtend
    {
        /// <summary>
        /// 隶属的成员字段
        /// </summary>
        IMetaFieldInfo FieldInfo { get; }
    }
}
