using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparrow.CommonLibrary.Mapper.DataSource
{
    public interface IDataSourceReader
    {
        /// <summary>
        /// 获取数据中数据集合的数量
        /// </summary>
        int Count { get; }
        /// <summary>
        /// fields成员映射到数据的下标，如果fields中的成员不被数据源包含，则下标为-1。
        /// </summary>
        /// <param name="fields">数据成员</param>
        /// <returns>依据fields的顺序，返回各个成员在数据源中的下标</returns>
        int[] Ordinal(string[] fields);
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <returns>依据ordinal的顺序返回data，没有数据和元素用null标记。</returns>
        object[] Read();
    }
}
