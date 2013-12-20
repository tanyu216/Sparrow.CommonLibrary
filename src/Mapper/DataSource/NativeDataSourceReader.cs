using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Mapper.DataSource
{
    public static class NativeDataSourceReader
    {
        private static readonly IList<IDataSourceReaderProvider> Providers;

        static NativeDataSourceReader()
        {
            Providers = new List<IDataSourceReaderProvider>();
            Providers.Add(new DataReaderSourceProvider());
            Providers.Add(new DataTableSourceProvider());
        }

        public static void AddProvider(IDataSourceReaderProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            lock (Providers)
            {
                Providers.Add(provider);
            }
        }

        public static IDataSourceReader GetReader(object source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            var providers = Providers.ToArray();

            foreach (var reader in Providers)
            {
                if (reader.IsSupported(source))
                    return reader.CreateReader(source);
            }

            return null;
        }
    }
}
