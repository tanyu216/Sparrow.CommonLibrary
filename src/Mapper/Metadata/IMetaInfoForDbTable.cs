using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparrow.CommonLibrary.Mapper.Metadata
{
    /// <summary>
    /// 数据库特有的源数据
    /// </summary>
    public interface IMetaInfoForDbTable : IMetaInfo
    {
        /// <summary>
        /// 自动增长标识
        /// </summary>
        IdentityMetaFieldExtend Identity { get; }
    }
}
