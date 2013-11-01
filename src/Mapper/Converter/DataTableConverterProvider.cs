using Sparrow.CommonLibrary.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparrow.CommonLibrary.Mapper.Converter
{
    public class DataTableConverterProvider : IConverterProvider
    {
        public bool IsSupported(object source)
        {
            return source is DataTable;
        }

        public IDataConverter<TDestination> Create<TDestination>(IMapper<TDestination> mapper, object source)
        {
            return new DataTableConverter<TDestination>(mapper, source);
        }

        private class DataTableConverter<TDestination> : IDataConverter<TDestination>
        {
            private readonly DataTable _table;
            private readonly IMapper<TDestination> _mapper;
            private readonly IPropertyValue<TDestination>[] _properties;
            private int _index = -1;

            public DataTableConverter(IMapper<TDestination> mapper, object datasource)
            {
                _table = (DataTable)datasource;
                _mapper = mapper;
                _properties = GetProperties();
            }

            public TDestination Next()
            {
                if (_table.Rows.Count > (++_index))
                {
                    return Create(_table.Rows[_index]);
                }
                return default(TDestination);
            }

            public List<TDestination> Cast()
            {
                var list = new List<TDestination>();
                while (_table.Rows.Count > (++_index))
                {
                    list.Add(Create(_table.Rows[_index]));
                }
                return list;
            }

            private TDestination Create(DataRow dataRow)
            {
                var output = _mapper.Create();

                var entity = output as IEntity;
                if (entity != null)
                {
                    entity.OperationState = DataState.Modify;
                    entity.Importing();// 开始导入数据
                }
                for (int i = 0; i < _properties.Length; i++)
                {
                    if (_properties[i] != null)
                        _properties[i].SetValue(output, dataRow[i]);
                }

                if (entity != null)
                    entity.Imported();// 结束导入数据
                return output;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="reader"></param>
            /// <returns>返回的数组中记录取数的下标（reader中数据的下标），如果值为-1表示IPropertyValue的成员字段没有被包含在reader中，所以请注意在从reader中取值之前要对值进行验证是否为-1。</returns>
            /// <remarks></remarks>
            private IPropertyValue<TDestination>[] GetProperties()
            {
                var array = new IPropertyValue<TDestination>[_table.Columns.Count];
                var fields = _mapper.MetaInfo.GetFieldNames();
                // 考虑数据库里面不区分大小写，但.net里面区分。
                for (int i = 0; i < array.Length; i++)
                {
                    string field = _table.Columns[i].ColumnName;
                    // 大部分时候返回的数据字段，与定义的字段大小写基本都是一致的，所以要考虑2/8原则（因为作大小写转换有性能损耗）。
                    field = fields.SingleOrDefault(x => x == field || x.ToLower() == field.ToLower());
                    if (field != null)
                        array[i] = _mapper[field];
                }
                //
                return array;
            }
        }
    }
}
