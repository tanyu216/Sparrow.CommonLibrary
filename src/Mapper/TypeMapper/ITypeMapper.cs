using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Mapper.TypeMapper
{
    public interface ITypeMapper
    {
        Type DesctinationType { get; }
        object Cast(object value);
    }

    public interface ITypeMapper<TDestination> : ITypeMapper
    {
        TDestination Cast(object value);
    }
}
