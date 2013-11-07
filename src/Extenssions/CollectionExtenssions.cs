using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Extenssions
{
    public static class CollectionExtenssions
    {
        /// <summary>
        /// 随机打乱集合的顺序
        /// </summary>
        /// <param name="list"></param>
        public static void Random(this IList list)
        {
            var random = new Random();
            var len = list.Count / 2;
            var max = list.Count;
            for (int i = 0; i <= len && i + 1 < max; i++)
            {
                int rnd = random.Next(i + 1, max);
                object val = list[i];
                list[i] = list[rnd];
                list[rnd] = val;
            }
        }

        /// <summary>
        /// 随机打乱集合的顺序
        /// </summary>
        /// <param name="arrary"></param>
        public static void Random(this object[] arrary)
        {
            var random = new Random();
            var len = arrary.Length / 2;
            var max = arrary.Length;
            for (int i = 0; i <= len && i + 1 < max; i++)
            {
                int rnd = random.Next(i + 1, max);
                object val = arrary[i];
                arrary[i] = arrary[rnd];
                arrary[rnd] = val;
            }
        }
    }
}
