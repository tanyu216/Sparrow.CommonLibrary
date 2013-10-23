using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparrow.CommonLibrary.Data.Mapper.Metadata
{
    /// <summary>
    /// 适用于自增长标识（序列）的成员字段扩展
    /// </summary>
    public class MetaDbIdentityInfo : IMetaFieldExtend
    {
        private readonly string _name;

        /// <summary>
        /// 自增长标识（序列）名称
        /// </summary>
        public string Name { get { return _name; } }

        public IMetaFieldInfo FieldInfo
        {
            get { throw new NotImplementedException(); }
        }

        public MetaDbIdentityInfo(string name)
        {
            _name = name;
        }
    }
}
