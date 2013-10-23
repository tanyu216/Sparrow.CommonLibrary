using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparrow.CommonLibrary.Data.Mapper.Converter
{
    public interface IDataConverter<TDestination>
    {
        TDestination Next();
        List<TDestination> Cast();
    }
}
