using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sparrow.CommonLibrary.Extenssions;

namespace Sparrow.CommonLibrary.Test.Extenssions
{
    [TestFixture]
    public class CollectionExtenssionsTest
    {
        private int[] CreatArray(int length)
        {
            var array = new int[length];
            for (var i = 0; i < length; i++)
            {
                array[i] = i * 10;
            }
            return array;
        }

        private List<int> CreatList(int length)
        {
            var list = new List<int>(length);
            for (var i = 0; i < length; i++)
            {
                list.Add(i * 10);
            }
            return list;
        }

        [Test]
        public void RandomListTest1()
        {
            for (var i = 1; i < 101; i++)
            {
                var array = CreatArray(i);
                array.Random();
            }
        }

        [Test]
        public void RandomListTest2()
        {
            for (var i = 1; i < 101; i++)
            {
                var list = CreatList(i);
                list.Random();
            }
        }

    }
}
