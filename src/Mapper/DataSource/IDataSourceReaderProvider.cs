using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparrow.CommonLibrary.Mapper.DataSource
{
    public interface IDataSourceReaderProvider
    {
        bool IsSupported(object source);
        IDataSourceReader<TDestination> Create<TDestination>(object source);
    }
}
