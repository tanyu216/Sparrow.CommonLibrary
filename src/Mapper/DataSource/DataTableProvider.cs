using Sparrow.CommonLibrary.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparrow.CommonLibrary.Mapper.DataSource
{
    public class DataTableProvider : IDataSourceReaderProvider
    {
        public bool IsSupported(object source)
        {
            return source is DataTable;
        }

        public IDataSourceReader<TDestination> Create<TDestination>(object source)
        {
            return new DataTableConverter<TDestination>((DataTable)source);
        }

        private class DataTableConverter<TDestination> : IDataSourceReader<TDestination>
        {
            private readonly DataTable _dataSource;
            private int _index = -1;

            public DataTableConverter(DataTable dataSource)
            {
                _dataSource = dataSource;
            }

            public int Count
            {
                get { return _dataSource.Rows.Count; }
            }

            public int[] Ordinal(string[] fields)
            {
                int[] array = new int[fields.Length];
                for (var i = fields.Length - 1; i > -1; i--)
                {
                    var column = _dataSource.Columns[fields[i]];
                    array[i] = column == null ? -1 : column.Ordinal;
                }
                return array;
            }

            public object[] Read(int[] ordinal)
            {
                _index++;
                if (_index >= _dataSource.Rows.Count)
                    return null;

                var dataRow = _dataSource.Rows[_index];

                var data = new object[ordinal.Length];
                for (var i = ordinal.Length - 1; i > -1; i--)
                    data[i] = dataRow[ordinal[i]];

                return data;
            }
        }
    }
}
