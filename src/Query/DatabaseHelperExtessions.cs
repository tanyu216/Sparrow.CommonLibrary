using Sparrow.CommonLibrary.Database;
using System;
using System.Collections.Generic;
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
