using System;

namespace Sparrow.CommonLibrary.Data.Entity
{
    /// <summary>
    /// 键和值
    /// </summary>
    [Serializable]
    public struct ItemValue
    {
        /// <summary>
        /// Item名称
        /// </summary>
        private string _item;

        /// <summary>
        /// Item名称
        /// </summary>
        public string Item
        {
            get { return _item; }
            set { _item = value; }
        }

        /// <summary>
        /// Item的值
        /// </summary>
        private object _value;

        /// <summary>
        /// Item的值
        /// </summary>
        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="item">字段</param>
        /// <param name="value">值</param>
        public ItemValue(string item, object value)
        {
            _item = item;
            _value = value;
        }

        public override string ToString()
        {
            return string.Concat("[", _item, ":", _value, "]");
        }
    }
    /// <summary>
    /// 键和值
    /// </summary>
    /// <typeparam name="TItem">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    [Serializable]
    public struct ItemValue<TItem, TValue>
    {
        /// <summary>
        /// Item名称
        /// </summary>
        private TItem _item;

        /// <summary>
        /// Item名称
        /// </summary>
        public TItem Item
        {
            get { return _item; }
            set { _item = value; }
        }

        /// <summary>
        /// Item的值
        /// </summary>
        private TValue _value;

        /// <summary>
        /// Item的值
        /// </summary>
        public TValue Value
        {
            get { return _value; }
            set { _value = value; }
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="item">字段</param>
        /// <param name="value">值</param>
        public ItemValue(TItem item, TValue value)
        {
            _item = item;
            _value = value;
        }

        public override string ToString()
        {
            return string.Concat("[", _item, ":", _value, "]");
        }
    }
}
