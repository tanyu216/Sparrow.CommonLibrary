using Sparrow.CommonLibrary.Database;
using Sparrow.CommonLibrary.Database.SqlBuilder;
using Sparrow.CommonLibrary.Mapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Query
{
    public static class DatabaseHelperExtessions
    {
        public static Queryable<T> CreateQueryable<T>(this DatabaseHelper database)
        {
            return new Queryable<T>(database);
        }

    }
}
