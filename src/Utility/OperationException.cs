using System;
using System.Runtime.Serialization;

namespace Sparrow.CommonLibrary.Utility
{
    /// <summary>
    /// 业务操作异常
    /// </summary>
    public class OperationException : Exception
    {
        /// <summary>
        /// 业务代码、编号
        /// </summary>
        public string Code { get; private set; }

        public OperationException(string code)
        {
            this.Code = code;
        }
        public OperationException(string code, string message)
            : base(message)
        {
            this.Code = code;
        }
        protected OperationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.Code = (string)info.GetValue("Code", typeof(string));
        }
        public OperationException(string code, string message, Exception innerException)
            : base(message, innerException)
        {
            this.Code = code;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Code", this.Code);
        }
    }
}
