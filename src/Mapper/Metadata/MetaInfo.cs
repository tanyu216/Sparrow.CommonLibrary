using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Mapper.Metadata
{
    public class MetaInfo : IMetaInfo, IEnumerable<IMetaPropertyInfo>
    {
        private readonly string _name;
        private readonly Type _entityType;
        private readonly List<IMetaPropertyInfo> propertyList;
        private bool isReadonly;
        private string[] propertyNameList;
        private IDictionary<string, int> propertyIndex;

        public MetaInfo(string name, Type entityType)
        {
            if (entityType == null)
                throw new ArgumentNullException("entityType");

            _name = name;
            _entityType = entityType;
            propertyList = new List<IMetaPropertyInfo>();
        }

        /// <summary>
        /// 获取propertyList是一个线程访问安全的集合
        /// </summary>
        /// <returns></returns>
        protected List<IMetaPropertyInfo> SafeList()
        {
            List<IMetaPropertyInfo> list;
            if (isReadonly)
            {
                list = propertyList;
            }
            else
            {
                lock (propertyList)
                {
                    list = propertyList.ToList();
                }
            }
            return list;
        }

        protected bool IsReadonly
        {
            get { return isReadonly; }
        }

        #region IMetaInfo

        public string Name
        {
            get { return _name; }
        }

        public Type EntityType
        {
            get { return _entityType; }
        }

        public int PropertyCount
        {
            get { return propertyList.Count; }
        }

        public IMetaPropertyInfo this[int index]
        {
            get
            {
                if (index < 0 || index >= propertyList.Count)
                    return null;
                return propertyList[index];
            }
        }

        public IMetaPropertyInfo this[string propertyName]
        {
            get
            {
                var index = IndexOf(propertyName);
                if (index < 0)
                    return null;
                return propertyList[index];
            }
        }

        public IMetaPropertyInfo this[System.Reflection.PropertyInfo propertyInfo]
        {
            get
            {
                foreach (var info in SafeList())
                    if (info.PropertyInfo == propertyInfo)
                        return info;

                return null;
            }
        }

        public int IndexOf(string propertyName)
        {
            if (!isReadonly)
            {
                var list = SafeList();
                for (var i = list.Count - 1; i > -1; i--)
                    if (list[i].PropertyName == propertyName)
                        return i;
                return -1;
            }
            else
            {
                Reorganize();

                int index;
                if (propertyIndex.TryGetValue(propertyName, out index))
                    return index;
                return -1;
            }
        }

        public string[] GetPropertyNames()
        {
            if (!isReadonly)
            {
                return SafeList().Select(x => x.PropertyName).ToArray();
            }
            else
            {
                Reorganize();

                string[] destArray = new string[propertyNameList.Length];
                propertyNameList.CopyTo(destArray, 0);
                return destArray;
            }
        }

        public IMetaPropertyInfo[] GetProperties()
        {
            return SafeList().ToArray();
        }

        public void AddPropertyInfo(IMetaPropertyInfo metaPropertyInfo)
        {
            if (isReadonly)
                throw new MapperException("只读状态下的MetaInfo无法添加属性。");

            if (metaPropertyInfo == null)
                throw new ArgumentNullException("metaPropertyInfo");

            if (metaPropertyInfo.MetaInfo != this)
                throw new ArgumentException("metaPropertyInfo不隶属于不前的MetaInfo");

            if (propertyList.Any(x => x.PropertyName == metaPropertyInfo.PropertyName || x.PropertyInfo == metaPropertyInfo.PropertyInfo))
                throw new ArgumentNullException("属性不能重复添加。");

            propertyList.Add(metaPropertyInfo);
        }

        public void RemovePropertyInfo(IMetaPropertyInfo metaPropertyInfo)
        {
            if (isReadonly)
                throw new MapperException("只读状态下的MetaInfo无法添加属性。");

            if (metaPropertyInfo == null)
                throw new ArgumentNullException("metaPropertyInfo");

            propertyList.Remove(metaPropertyInfo);
        }

        private bool isReorganize;
        private void Reorganize()
        {
            if (!isReorganize)
            {
                lock (propertyList)
                {
                    if (!isReorganize)
                    {
                        return;
                    }
                    isReorganize = true;

                    propertyIndex = new Dictionary<string, int>(propertyList.Count);
                    for (var i = propertyList.Count - 1; i > -1; i--)
                        propertyIndex.Add(propertyList[i].PropertyName, i);

                    propertyNameList = new string[propertyList.Count];
                    for (var i = propertyList.Count - 1; i > -1; i--)
                        propertyNameList[i] = propertyList[i].PropertyName;

                }
            }
        }

        public void MakeReadonly()
        {
            isReadonly = true;
        }

        #endregion

        #region IEnumerable<IMetaPropertyInfo>

        public IEnumerator<IMetaPropertyInfo> GetEnumerator()
        {
            return SafeList().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return SafeList().GetEnumerator();
        }

        #endregion

    }
}
