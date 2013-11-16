using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sparrow.CommonLibrary.Mapper.Metadata;

namespace Sparrow.CommonLibrary.Entity
{
    public class DbMetaInfo : MetaInfo
    {
        private DbIncrementMetaPropertyInfo _increment;
        private string[] _keys;

        /// <summary>
        /// 自动增长序列
        /// </summary>
        public DbIncrementMetaPropertyInfo Increment
        {
            get
            {
                Reorganize();
                return _increment;
            }
        }

        /// <summary>
        /// 表中主键的数量
        /// </summary>
        public int KeyCount { get { return _keys.Length; } }

        public DbMetaInfo(string tableName, Type entityType)
            : base(tableName, entityType)
        {
        }

        /// <summary>
        /// 验证是否为主键主字段
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public bool IsKey(string columnName)
        {
            Reorganize();
            return _keys.Any(x => x == columnName);
        }

        /// <summary>
        /// 获取所有的主键字段
        /// </summary>
        /// <returns></returns>
        public string[] GetKeys()
        {
            Reorganize();
            var keys = new string[_keys.Length];
            _keys.CopyTo(keys, 0);
            return keys;
        }

        private object _syncObj = new object();
        private bool isReorganize;
        private void Reorganize()
        {
            if (!isReorganize)
            {
                lock (_syncObj)
                {
                    if (!isReorganize)
                    {
                        return;
                    }
                    isReorganize = true;

                    _increment = (DbIncrementMetaPropertyInfo)GetProperties().FirstOrDefault(x => x is DbIncrementMetaPropertyInfo);
                    
                    var keyList = new List<string>();
                    foreach (var property in GetProperties())
                        if (property is DbMetaPropertyInfo && ((DbMetaPropertyInfo)property).IsKey)
                            keyList.Add(((DbMetaPropertyInfo)property).PropertyName);
                    _keys = keyList.ToArray();
                }
            }
        }
    }
}
