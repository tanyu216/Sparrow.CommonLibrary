using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparrow.CommonLibrary.Mapper.Converter
{
    public static class DataConverter
    {
        private static readonly IList<IConverterProvider> Providers;

        static DataConverter()
        {
            Providers = new List<IConverterProvider>();
            Providers.Add(new DataReaderConverterProvider());
            Providers.Add(new DataTableConverterProvider());
        }

        public static void AddConverterProvider(IConverterProvider provider)
        {
            if (provider == null) throw new ArgumentNullException("provider");

            lock (Providers)
            {
                Providers.Insert(0, provider);
            }
        }

        public static IDataConverter<TDestination> CreateConverter<TDestination>(IMapper<TDestination> mapper, object source)
        {
            if (source == null) throw new ArgumentNullException("source");
            for (var i = 0; i < Providers.Count; i++)
            {
                if (Providers[i].IsSupported(source))
                    return Providers[i].Create<TDestination>(mapper, source);
            }
            throw new NotSupportedException(string.Format("不受支持的数据源:{0}。", source.GetType().FullName));
        }
    }
}
