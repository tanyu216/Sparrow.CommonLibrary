using System;
using System.Globalization;

namespace Sparrow.CommonLibrary.Utility
{
    /// <summary>
    /// 基于格林威志的Unix时间截
    /// </summary>
    public struct Timestamp : IComparable, IComparable<Timestamp>, IConvertible, IEquatable<Timestamp>, IFormattable
    {
        private readonly long _timestamp;

        private Timestamp(long timestamp)
        {
            _timestamp = timestamp;
        }

        private Timestamp(DateTime datetime)
        {
            _timestamp = datetime.ToUniversalTime().Ticks / TicksM - FirstYear;
        }

        private static readonly long FirstYear = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks / TicksM;
        private const int TicksM = 10000000;

        public static Timestamp Now
        {
            get
            {
                return new Timestamp(DateTime.UtcNow.Ticks / TicksM - FirstYear);
            }
        }

        #region IComparable

        public int CompareTo(object obj)
        {
            if (obj is Timestamp)
                return CompareTo((Timestamp)obj);
            if (obj is DateTime)
                return CompareTo(new Timestamp((DateTime)obj));
            //
            throw new InvalidCastException();
        }

        public int CompareTo(Timestamp other)
        {
            if (_timestamp < other._timestamp)
                return -1;
            if (_timestamp == other._timestamp)
                return 0;

            return 1;
        }

        #endregion

        #region IConvertible

        public TypeCode GetTypeCode()
        {
            return TypeCode.Int64;
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            return Convert.ToBoolean(_timestamp);
        }

        public byte ToByte(IFormatProvider provider)
        {
            return Convert.ToByte(_timestamp);
        }

        public char ToChar(IFormatProvider provider)
        {
            return Convert.ToChar(_timestamp);
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(_timestamp);
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            return Convert.ToDecimal(_timestamp);
        }

        public double ToDouble(IFormatProvider provider)
        {
            return Convert.ToDouble(_timestamp);
        }

        public short ToInt16(IFormatProvider provider)
        {
            return Convert.ToInt16(_timestamp);
        }

        public int ToInt32(IFormatProvider provider)
        {
            return Convert.ToInt32(_timestamp);
        }

        public long ToInt64(IFormatProvider provider)
        {
            return _timestamp;
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            return Convert.ToSByte(_timestamp);
        }

        public float ToSingle(IFormatProvider provider)
        {
            return Convert.ToSingle(_timestamp);
        }

        public string ToString(IFormatProvider provider)
        {
            return _timestamp.ToString(provider);
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return Convert.ChangeType(_timestamp, conversionType, provider);
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            return Convert.ToUInt16(_timestamp);
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(_timestamp);
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt64(_timestamp);
        }

        #endregion

        public bool Equals(Timestamp other)
        {
            return other._timestamp == _timestamp;
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return _timestamp.ToString(format, formatProvider);
        }

        #region override object

        public override string ToString()
        {
            return _timestamp.ToString(CultureInfo.InvariantCulture);
        }

        public override bool Equals(object obj)
        {
            return _timestamp.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _timestamp.GetHashCode();
        }

        #endregion

        #region override operator

        public static TimeSpan operator -(Timestamp t1, Timestamp t2)
        {
            return new TimeSpan(t1._timestamp * TicksM - t2._timestamp * TicksM);
        }

        public static TimeSpan operator -(Timestamp t1, TimeSpan t2)
        {
            return new TimeSpan(t1._timestamp * TicksM - t2.Ticks);
        }

        public static Timestamp operator +(Timestamp t1, TimeSpan t2)
        {
            return new Timestamp(t1._timestamp + t2.Ticks / TicksM);
        }

        public static bool operator !=(Timestamp t1, Timestamp t2)
        {
            return t1._timestamp != t2._timestamp;
        }

        public static bool operator ==(Timestamp t1, Timestamp t2)
        {
            return t1._timestamp == t2._timestamp;
        }

        public static bool operator <(Timestamp t1, Timestamp t2)
        {
            return t1._timestamp < t2._timestamp;
        }

        public static bool operator >(Timestamp t1, Timestamp t2)
        {
            return t1._timestamp < t2._timestamp;
        }

        public static bool operator <=(Timestamp t1, Timestamp t2)
        {
            return t1._timestamp <= t2._timestamp;
        }

        public static bool operator >=(Timestamp t1, Timestamp t2)
        {
            return t1._timestamp <= t2._timestamp;
        }

        public static implicit operator Int64(Timestamp t1)
        {
            return t1._timestamp;
        }

        public static implicit operator DateTime(Timestamp t1)
        {
            return t1.ToDateTime(CultureInfo.InvariantCulture);
        }

        public static implicit operator Timestamp(long timestamp)
        {
            return new Timestamp(timestamp);
        }

        public static implicit operator Timestamp(DateTime t1)
        {
            return new Timestamp(t1);
        }

        #endregion

        #region Parse

        public static Timestamp Parse(string s)
        {
            long val;
            if (long.TryParse(s, out  val))
            {
                return new Timestamp(val);
            }
            DateTime dVal;
            if (DateTime.TryParse(s, out dVal))
            {
                return new Timestamp(dVal);
            }
            throw new InvalidCastException();
        }

        public static Timestamp Parse(string s, IFormatProvider provider, DateTimeStyles style)
        {
            var val = DateTime.Parse(s, provider, style);
            return new Timestamp(val);
        }

        public static Timestamp Parse(string s, IFormatProvider provider, NumberStyles style)
        {
            var val = long.Parse(s, style, provider);
            return new Timestamp(val);
        }

        #endregion

        #region TryParse

        public static bool TryParse(string s, out Timestamp timestamp)
        {
            long val;
            if (long.TryParse(s, out  val))
            {
                timestamp = new Timestamp(val);
                return true;
            }
            DateTime dVal;
            if (DateTime.TryParse(s, out dVal))
            {
                timestamp = new Timestamp(dVal);
                return true;
            }
            timestamp = new Timestamp();
            return false;
        }

        public static bool TryParse(string s, IFormatProvider provider, NumberStyles style, out Timestamp timestamp)
        {
            long val;
            if (long.TryParse(s, style, provider, out  val))
            {
                timestamp = new Timestamp(val);
                return true;
            }
            timestamp = new Timestamp();
            return false;
        }

        public static bool TryParse(string s, IFormatProvider provider, DateTimeStyles style, out Timestamp timestamp)
        {
            DateTime val;
            if (DateTime.TryParse(s, provider, style, out  val))
            {
                timestamp = new Timestamp(val);
                return true;
            }
            timestamp = new Timestamp();
            return false;
        }

        #endregion

        public Timestamp Add(TimeSpan value)
        {
            return new Timestamp(_timestamp + value.Ticks / TicksM);
        }

    }
}
