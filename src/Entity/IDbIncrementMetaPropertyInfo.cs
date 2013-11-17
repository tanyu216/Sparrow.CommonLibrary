using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Entity
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDbIncrementMetaPropertyInfo : IDbMetaPropertyInfo
    {
        string IncrementName { get; }

        int StartVal { get; }
    }
}
