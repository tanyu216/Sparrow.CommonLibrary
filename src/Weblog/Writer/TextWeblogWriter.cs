using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Sparrow.CommonLibrary.Utility;

namespace Sparrow.CommonLibrary.Weblog.Writer
{
    public class TextWeblogWriter : WeblogWriterBase
    {
        internal static readonly string MaxSizeParamName = "maxsize";
        internal static readonly string FolderParamName = "folder";

        private readonly PathBuilder _pathBuilder;
        public PathBuilder PathBuilder { get { return _pathBuilder; } }

        public TextWeblogWriter()
        {
            _pathBuilder = new PathBuilder();
        }

        public override void Write(WeblogEntryCollection weblogEntry)
        {
            using (var sm = CreateStream(GetPath()))
            {
                using (var sw = new StreamWriter(sm, Encoding.UTF8))
                {
                    if (sm.Length < 1)
                    {
                        OutputHeadToStream(sw, weblogEntry.Items);
                    }
                    //
                    foreach (var entry in weblogEntry)
                        OutputToStream(sw, entry);
                }
            }
        }

        protected virtual string GetPath()
        {
            var filepath = _pathBuilder.Build(GetParameter(FolderParamName), this);
            filepath = _pathBuilder.RebuildPathByFileSize(filepath, GetParameter(MaxSizeParamName));
            //
            if (!Directory.Exists(Path.GetDirectoryName(filepath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filepath));
            }
            return filepath;
        }

        protected virtual Stream CreateStream(string filepath)
        {
            return new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, 1024 * 8);
        }

        protected virtual void OutputHeadToStream(StreamWriter sw, string[] items)
        {
            sw.WriteLine(string.Join("\t", items));
        }

        protected virtual void OutputToStream(StreamWriter sw, WeblogEntry entry)
        {
            var regReplace = new Regex("(\t)|(\n)");
            for (var i = 0; i < entry.Data.Length; i++)
            {
                if (string.IsNullOrEmpty(entry.Data[i]))
                    sw.Write(string.Empty);
                else
                    sw.Write(regReplace.Replace(entry.Data[i], ""));
                //
                if (i + 1 < entry.Data.Length)
                    sw.Write("\t");
            }
            sw.WriteLine();
        }

    }
}
