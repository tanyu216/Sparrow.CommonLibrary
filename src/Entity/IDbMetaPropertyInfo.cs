using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Entity
{
    public interface IDbMetaPropertyInfo
    {
        string ColumnName { get; }

        bool IsKey { get; }
    }
}
