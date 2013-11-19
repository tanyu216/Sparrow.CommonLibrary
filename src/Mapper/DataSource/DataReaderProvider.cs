using Sparrow.CommonLibrary.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparrow.CommonLibrary.Mapper.DataSource
{
    public class DataReaderProvider : IDataSourceReaderProvider
    {
        public bool IsSupported(object source)
        {
            return source is IDataReader;
        }

        public IDataSourceReader<TDestination> Create<TDestination>(object source)
        {
            return new DataReaderConverter<TDestination>((IDataReader)source);
        }

        private class DataReaderConverter<TDestination> : IDataSourceReader<TDestination>
        {
            private readonly IDataReader _reader;

            public DataReaderConverter(IDataReader dataSource)
            {
                _reader = dataSource;
            }

            public int Count
            {
                get { return -1; }
            }

            public int[] Ordinal(string[] fields)
            {
                int[] array = new int[fields.Length];
                for (var i = fields.Length - 1; i > -1; i--)
                {
                    try
                    {
                        array[i] = _reader.GetOrdinal(fields[i]);
                    }
                    catch
                    {
                        array[i] = -1;
                    }
                }
                return array;
            }

            public object[] Read(int[] ordinal)
            {
                if (!_reader.Read())
                    return null;

                var data = new object[ordinal.Length];
                for (var i = ordinal.Length - 1; i > -1; i--)
                    data[i] = _reader[ordinal[i]];

                return data;
            }
        }

    }
}
