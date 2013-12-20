using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Mapper.DataSource
{
    public interface IDataSourceReaderProvider
    {
        bool IsSupported(object dataSource);
        IDataSourceReader CreateReader(object dataSource);
    }
}
