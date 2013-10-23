using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sparrow.CommonLibrary.Data.Database;
using Sparrow.CommonLibrary.Data.SqlBuilder;

namespace Sparrow.CommonLibrary.Data.Query
{
    public abstract class Expression
    {
        public abstract string Build(ISqlBuilder builder, ParameterCollection output);
    }
}
