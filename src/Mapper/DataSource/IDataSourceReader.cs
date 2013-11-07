using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparrow.CommonLibrary.Mapper.DataSourceReader
{
    public interface IDataSourceReader<TDestination>
    {
        /// <summary>
        /// 获取数据中数据集合的数量
        /// </summary>
        int Count { get; }
        /// <summary>
        /// 获取fields成员映射到数据的下标，如果fields中的成员不被数据源包含，则下标为-1。
        /// </summary>
        /// <param name="fields">数据成员</param>
        /// <returns>依据fields的顺序，返回各个成员在数据源中的下标</returns>
        int[] Ordinal(string[] fields);
        /// <summary>
        /// 读取指定下标的数据
        /// </summary>
        /// <param name="ordinal">通过IndexOf获得的一组有效（不包含-1）下标</param>
        /// <returns>依据ordinal的顺序返回data</returns>
        object[] Read(int[] ordinal);
    }
}
