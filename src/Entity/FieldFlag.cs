using System;
using System.Collections.Generic;
using System.Linq;

namespace Sparrow.CommonLibrary.Entity
{
    /// <summary>
    /// 用于记录成员字段是否被修改过。成员字段有一个下标编号，这是一个在编译时确定从零开始的编码。
    /// <remarks>这种做法有利于节省空间，带来性能上的提高(内部采用位运算实现)。依据字段的多少来空间的开销（计算方法：(int)Math.Ceiling((double)size / 8)），此公式可以计算出成员字段需用多少个字节来做记录。</remarks>
    /// </summary>
    public struct FieldFlag
    {
        /// <summary>
        /// 数组中的一个对象只存32个比特位。
        /// </summary>
        private const byte ItemSize = 31;

        private readonly int[] _values;
        private readonly int _size;
        /// <summary>
        /// 
        /// </summary>
        public int Size
        {
            get { return _size; }
        }

        /// <summary>
        /// 比特位上值为 1 的个数
        /// </summary>
        /// <returns></returns>
        public int MarkedCount
        {
            get
            {
                int count = 0;
                for (int i = 0; i < _size; i++)
                {
                    if (HasMarked(i))
                        count++;
                }
                return count;
            }
        }

        /// <summary>
        /// 初始化一个可以存放大数据量的数组。
        /// </summary>
        /// <param name="size">二进制位大小，size = 32 * 返回值数组长度。因为int占用4个字节，所有二进制位大小为32。</param>
        public FieldFlag(int size)
        {
            _size = size;
            _values = new int[(int)Math.Ceiling((double)size / ItemSize)];
        }

        /// <summary>
        /// 计算<paramref name="index"/>在数组中的比特位，并返回<paramref name="index"/>对应的比特位。
        /// </summary>
        /// <param name="index">比特位的下标。</param>
        /// <param name="arrayIndex">数组下标。一个很大的值需要拆分成一个数组保存，如：参数<paramref name="index"/>等于 9 它大于8了，所以这个值会放在数组下标为1的byte中的第0个比特位。</param>
        /// <returns>计算出的这个比特位的值</returns>
        private static int BitCompute(int index, out int arrayIndex)
        {
            int bit;
            if (index >= ItemSize)
            {
                arrayIndex = index / ItemSize;
                bit = 1 << (index - (ItemSize << (arrayIndex - 1)));
            }
            else
            {
                arrayIndex = 0;
                bit = 1 << index;
            }
            return bit;
        }

        /// <summary>
        /// 检查指定比特位的标记。
        /// </summary>
        /// <returns>任意比特位已被标记时返回true，false则表示所有比特位均未被标记。</returns>
        public bool HasMarked()
        {
            return _values.Any(t => t > 0);
        }

        /// <summary>
        /// 检查指定比特位的标记。
        /// </summary>
        /// <param name="index">比特位</param>
        /// <returns>指定比特位已被标记时返回true，false则表示未被标记。</returns>
        public bool HasMarked(int index)
        {
            if (index >= _size)
                throw new IndexOutOfRangeException();
            int i;
            int bit = BitCompute(index, out i);
            return (_values[i] & bit) > 0;
        }

        /// <summary>
        /// 获取所有被标记的比特位的位置下标。
        /// </summary>
        /// <returns></returns>
        public int[] IndexArray()
        {
            var list = new List<int>(_size);
            for (int i = 0; i < _size; i++)
            {
                if (HasMarked(i))
                    list.Add(i);
            }
            return list.ToArray();
        }

        /// <summary>
        /// 在指定的比特位，将其值标记为 1。
        /// </summary>
        /// <param name="index">比特位位置。</param>
        public void Mark(int index)
        {
            if (index >= _size)
                throw new IndexOutOfRangeException();
            int i;
            int bit = BitCompute(index, out i);
            _values[i] = _values[i] | bit;
        }

        /// <summary>
        /// 在指定的比特位，将其值标记为 0（取消标记）。
        /// </summary>
        /// <param name="index">比特位位置。</param>
        public void UnMark(int index)
        {
            if (index >= _size)
                throw new IndexOutOfRangeException();
            int i;
            int bit = BitCompute(index, out i);
            if ((_values[i] & bit) == 0)
                return;
            _values[i] = _values[i] ^ ~bit;

        }

        /// <summary>
        /// 清除所有比特位标记。
        /// </summary>
        public void Clean()
        {
            for (int i = _values.Length - 1; i > -1; i--)
            {
                _values[i] = 0;
            }
        }
    }
}
