using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sparrow.CommonLibrary.Data.Database;
using Sparrow.CommonLibrary.Data.SqlBuilder;

namespace Sparrow.CommonLibrary.Data.Query
{
    public class FieldExpression : Expression
    {
        public string FieldName { get;private set; }

        public FieldExpression(string fieldName)
        {
            FieldName = fieldName;
        }

        public override string Build(ISqlBuilder builder, ParameterCollection output)
        {
            return builder.BuildField(FieldName);
        }
    }
}
