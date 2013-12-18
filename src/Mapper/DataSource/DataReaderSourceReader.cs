using Sparrow.CommonLibrary.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparrow.CommonLibrary.Mapper.DataSource
{
    public class DataReaderSourceReader : IDataSourceReader
    {
        private readonly IDataReader _reader;
        private int[] ordinal;

        public DataReaderSourceReader(IDataReader dataSource)
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
            return (ordinal = array).ToArray();
        }

        public object[] Read()
        {
            if (!_reader.Read())
                return null;

            var data = new object[ordinal.Length];
            for (var i = ordinal.Length - 1; i > -1; i--)
            {
                if (ordinal[i] < 0)
                    continue;
                data[i] = _reader[ordinal[i]];
            }

            return data;
        }
    }
}

