using System;
using System.Runtime.Serialization;

namespace Sparrow.CommonLibrary.Data.Mapper
{
	/// <summary>
	/// 数据对象映射异常
	/// </summary>
	[Serializable]
	public class MapperException : Exception
	{
		/// <summary>
		/// 
		/// </summary>
		public MapperException()
		{
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public MapperException(string message)
			: base(message)
		{
		}
		protected MapperException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public MapperException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
