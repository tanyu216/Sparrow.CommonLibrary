using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparrow.CommonLibrary.Mapper.Metadata
{
    /// <summary>
    /// 自动增长标识，应用于数据库的成员字段扩展。
    /// </summary>
    public class IncrementFieldExtend : IMetaFieldExtend
    {
        private readonly string _name;
        public string Name { get { return _name; } }

        private readonly int _startVal;
        public int StartVal { get { return _startVal; } }

        private readonly IMetaFieldInfo _field;
        public IMetaFieldInfo FieldInfo
        {
            get { return _field; }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="name"></param>
        public IncrementFieldExtend(IMetaFieldInfo field, string name)
            : this(field, name, 1)
        {
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="name"></param>
        public IncrementFieldExtend(IMetaFieldInfo field, string name, int startVal)
        {
            if (field == null)
                throw new ArgumentNullException("field");
            _field = field;
            _name = name;
            _startVal = startVal;
        }
    }
}
