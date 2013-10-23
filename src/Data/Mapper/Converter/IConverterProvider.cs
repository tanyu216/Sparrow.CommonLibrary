using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparrow.CommonLibrary.Data.Mapper.Converter
{
    public interface IConverterProvider
    {
        bool IsSupported(object source);
        IDataConverter<TDestination> Create<TDestination>(IMapper<TDestination> mapper, object source);
    }
}
