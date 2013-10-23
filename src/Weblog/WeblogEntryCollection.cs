using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Weblog
{
    /// <summary>
    /// Weblog日志集合
    /// </summary>
    public class WeblogEntryCollection : IEnumerable<WeblogEntry>
    {
        private readonly string _version;
        /// <summary>
        /// Weblog日志版本
        /// </summary>
        public string Version
        {
            get { return _version; }
        }

        private readonly string[] _items;
        /// <summary>
        /// Weblog日志的数据选项
        /// </summary>
        public string[] Items
        {
            get { return _items; }
        }

        private IEnumerable<WeblogEntry> _entrys;

        public WeblogEntryCollection(string version, string[] items, IEnumerable<WeblogEntry> weblogEntry)
        {
            if (string.IsNullOrEmpty(version))
                throw new ArgumentNullException("version");
            if (items == null || items.Length == 0)
                throw new ArgumentNullException("items");
            if (weblogEntry == null)
                throw new ArgumentNullException("entrys");

            this._version = version;
            this._items = new string[items.Length];
            items.CopyTo(_items, 0);
            this._entrys = new List<WeblogEntry>(weblogEntry);
        }

        public IEnumerator<WeblogEntry> GetEnumerator()
        {
            return _entrys.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _entrys.GetEnumerator();
        }
    }
}
