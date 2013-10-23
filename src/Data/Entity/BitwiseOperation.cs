using System;
using System.Collections.Generic;
using System.Linq;

namespace Sparrow.CommonLibrary.Data.Entity
{
    /// <summary>
    /// 用于记录成员字段是否被修改过。成员字段有一个下标编号，这是一个在编译时确定从零开始的编码。
    /// <remarks>这种做法有利于节省空间，带来性能上的提高(内部采用位运算实现)。依据字段的多少来难写空间的开销（计算方法：(size / 8 + ((size % 8) > 0 ? 1 : 0))），此公式可以计算出成员字段需用多少个字节来做记录。</remarks>
    /// </summary>
    public struct BitwiseOperation
    {
        private const byte ItemSize = 8;

        private readonly byte[] _values;
        private readonly int _size;

        /// <summary>
        /// 初始化一个可以存放大数据量的数组。
        /// </summary>
        /// <param name="size">二进制位大小，size = 8 * 返回值数组长度。因为ushort占用两个字节，所有二进制位大小为8。</param>
        public BitwiseOperation(int size)
        {
            _size = size;
            _values = new byte[(int)Math.Ceiling((double)size / ItemSize)];
        }

        /// <summary>
        /// 计算<paramref name="index"/>在数组中的位置，并返回<paramref name="index"/>对应的比特的值。
        /// </summary>
        /// <param name="index">二进制位</param>
        /// <param name="arrayIndex">数组下标。一个很大的值需要拆分成一个数组保存，如：参数<paramref name="index"/>等于 9 它大于8了，所以这个值会放在数组下标为1的对象中的第0个位置。</param>
        /// <returns>计算出的这个比特位的值</returns>
        private static byte IndexBitValue(int index, out int arrayIndex)
        {
            byte bitVal;
            if (index >= ItemSize)
            {
                arrayIndex = index / ItemSize;
                bitVal = (byte)(1 << (index - (ItemSize << (arrayIndex - 1))));
            }
            else
            {
                arrayIndex = 0;
                bitVal = (byte)(1 << index);
            }
            return bitVal;
        }

        /// <summary>
        /// 任意比特位有为1的值时返回true
        /// </summary>
        /// <returns></returns>
        public bool HasValue()
        {
            return _values.Any(t => t > 0);
        }

        /// <summary>
        /// 比特位上值为 1 的个数
        /// </summary>
        /// <returns></returns>
        public int ValueCount()
        {
            int count = 0;
            for (int i = 0; i < _size; i++)
            {
                if (HasValue(i))
                    count++;
            }
            return count;
        }

        /// <summary>
        /// 获取所有值为1的比特位下标
        /// </summary>
        /// <returns></returns>
        public int[] ValueIndex()
        {
            var list = new List<int>(_size);
            for (int i = 0; i < _size; i++)
            {
                if (HasValue(i))
                    list.Add(i);
            }
            return list.ToArray();
        }

        /// <summary>
        /// 验证指定的比特位是否有值
        /// </summary>
        /// <param name="index">比特位</param>
        /// <returns>返回true表示该比特位有值，false表示该比特位没有值。</returns>
        public bool HasValue(int index)
        {
            int arrayIndex;
            byte bitVal = IndexBitValue(index, out arrayIndex);
            return (_values[arrayIndex] & bitVal) > 0;
        }

        /// <summary>
        /// 在指定的比特位置为值 1
        /// </summary>
        /// <param name="index">比特位位置。</param>
        public void SetValue(int index)
        {
            int arrayIndex;
            byte bitVal = IndexBitValue(index, out arrayIndex);
            _values[arrayIndex] = (byte)(_values[arrayIndex] | bitVal);
        }

        /// <summary>
        /// 将所有的位记录清零
        /// </summary>
        public void Clean()
        {
            for (int i = 0; i < _values.Length; i++)
            {
                _values[i] = 0;
            }
        }
    }
}
