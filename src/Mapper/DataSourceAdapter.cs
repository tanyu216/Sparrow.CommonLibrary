using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sparrow.CommonLibrary.Entity;
using Sparrow.CommonLibrary.Mapper.DataSource;

namespace Sparrow.CommonLibrary.Mapper
{
    /// <summary>
    /// 数据适配器，依据<see cref="IDataSourceReader"/>提供对数据源的支持，实现数据到实体对象的转换。
    /// </summary>
    /// <remarks>所有的方法和属性均保证多线程安全</remarks>
    public class DataSourceAdapter
    {
        public static readonly DataSourceAdapter Instance;

        private readonly IList<IDataSourceReaderProvider> Providers;

        static DataSourceAdapter()
        {
            Instance = new DataSourceAdapter();
        }

        private DataSourceAdapter()
        {
            Providers = new List<IDataSourceReaderProvider>();
            Providers.Add(new DataReaderProvider());
            Providers.Add(new DataTableProvider());
        }

        public void AddConverterProvider(IDataSourceReaderProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            lock (Providers)
            {
                Providers.Insert(0, provider);
            }
        }

        public IDataSourceReader<T> CreateReader<T>(object source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            var providers = Providers.ToArray();

            foreach (var reader in Providers)
            {
                if (reader.IsSupported(source))
                    return reader.Create<T>(source);
            }

            throw new NotSupportedException(string.Format("不受支持的数据源:{0}。", source.GetType().FullName));
        }

        private IPropertyAccessor<T>[] GetPropertyAccessors<T>(IMapper<T> mapper, IDataSourceReader<T> reader, out int[] ordinal)
        {
            var fields = mapper.MetaInfo.GetFieldNames();
            var index = reader.Ordinal(fields);

            if (index == null || index.Length != fields.Length)
                throw new MapperException(string.Format("{0}.IndexOf返回的数据与参数fields不一致。", reader.GetType().FullName));

            var accessorList = new List<IPropertyAccessor<T>>(fields.Length);
            var ordinalList = new List<int>(fields.Length);
            for (var i = fields.Length - 1; i > -1; i--)
            {
                if (index[i] >= 0)
                {
                    accessorList.Add(mapper[i]);
                    ordinalList.Add(index[i]);
                }
            }

            ordinal = ordinalList.ToArray();
            return accessorList.ToArray();
        }

        private void SetValue<T>(T output, object[] data, IPropertyAccessor<T>[] accessors)
        {
            if (output is IMappingTrigger)
                ((IMappingTrigger)output).Begin();

            for (int i = accessors.Length - 1; i > -1; i--)
                accessors[i].SetValue(output, data[i]);

            if (output is IMappingTrigger)
                ((IMappingTrigger)output).End();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ReadSingle<T>(IMapper<T> mapper, object dataSource)
        {
            if (mapper == null)
                throw new ArgumentNullException("mapper");

            int[] ordinal;
            var reader = CreateReader<T>(dataSource);
            var accessors = GetPropertyAccessors<T>(mapper, reader, out ordinal);

            var data = reader.Read(ordinal);
            if (data == null)
                return default(T);

            T obj = mapper.Create();
            SetValue<T>(obj, data, accessors);
            return obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> ReadList<T>(IMapper<T> mapper, object dataSource)
        {
            if (mapper == null)
                throw new ArgumentNullException("mapper");

            int[] ordinal;
            var reader = CreateReader<T>(dataSource);
            var accessors = GetPropertyAccessors<T>(mapper, reader, out ordinal);

            var count = reader.Count;
            var list = count > -1 ? new List<T>(count) : new List<T>(8);

            object[] data;
            while (null != (data = reader.Read(ordinal)))
            {
                T obj = mapper.Create();
                SetValue<T>(obj, data, accessors);
                list.Add(obj);
            }
            return list;
        }
    }
}
