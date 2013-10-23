using Sparrow.CommonLibrary.Data.Mapper;
using Sparrow.CommonLibrary.Data.Mapper.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Sparrow.CommonLibrary.Data.Query
{
    /// <summary>
    /// 条件表达式
    /// </summary>
    public class ConditionExpressions : IEnumerable<ConditionExpression>
    {
        private readonly IList<ConditionExpression> _expression;

        /// <summary>
        /// 条件的个数
        /// </summary>
        public int Count
        {
            get { return _expression.Count; }
        }

        /// <summary>
        /// 条件表达式初始化
        /// </summary>
        public ConditionExpressions()
        {
            _expression = new List<ConditionExpression>();
        }

        /// <summary>
        /// 条件表达式初始化
        /// </summary>
        /// <param name="capacity">默认集合分配的空间</param>
        public ConditionExpressions(int capacity)
        {
            _expression = new List<ConditionExpression>(capacity);
        }

        /// <summary>
        /// 增加一个条件表达式(默认是等于)
        /// </summary>
        /// <param name="field">成员字段</param>
        /// <param name="value">成员值</param>
        /// <returns>返回对象本身。</returns>
        public ConditionExpressions Append(string field, object value)
        {
            if (string.IsNullOrEmpty(field)) throw new ArgumentNullException("field");

            if (value == null)
            {
                _expression.Add(new ConditionExpression(new FieldExpression(field), new ConstantExpression(value), "IS"));
            }
            else
            {
                _expression.Add(new ConditionExpression(new FieldExpression(field), new ConstantExpression(value), OperaterChar(Operator.Equal)));
            }
            //
            return this;
        }

        /// <summary>
        /// 增加一个条件表达式
        /// </summary>
        /// <param name="field">成员字段</param>
        /// <param name="opt">运算符</param>
        /// <param name="value">成员值</param>
        /// <returns>返回对象本身。</returns>
        public ConditionExpressions Append(string field, Operator opt, object value)
        {
            if (string.IsNullOrEmpty(field)) throw new ArgumentNullException("field");
            
            if (Operator.In == opt)
            {
                if (value is object[]) // 数据需要转换成集合使用
                    _expression.Add(new ConditionInExpression(new FieldExpression(field), (object[])value));
                else if (value is ICollection)
                    _expression.Add(new ConditionInExpression(new FieldExpression(field), (ICollection)value));
                else // 不是数组,只有一个值的直接用等于
                    Append(field, value);
            }
            else if (Operator.Between == opt)
            {
                object value1;
                object value2;
                var arrayValue = value as object[];
                if (arrayValue != null && arrayValue.Length == 2)
                {
                    value1 = arrayValue[0];
                    value2 = arrayValue[1];
                }
                else
                {
                    var collectionValue = value as ICollection;
                    if (collectionValue != null && collectionValue.Count == 2)
                    {
                        arrayValue = new object[2];
                        collectionValue.CopyTo(arrayValue, 0);
                        value1 = arrayValue[0];
                        value2 = arrayValue[1];
                    }
                    else
                    { throw new ArgumentException("Between运算符必须要有两个参数。"); }
                }
                _expression.Add(new ConditionBetweenExpression(new FieldExpression(field), value1, value2));
            }
            else if (Operator.All == opt)
            {
                _expression.Add(new ConditionLikeExpression(new FieldExpression(field), value));
            }
            else if (Operator.StartWith == opt)
            {
                _expression.Add(new ConditionLikeExpression(new FieldExpression(field), value, true, false));
            }
            else if (Operator.EndWith == opt)
            {
                _expression.Add(new ConditionLikeExpression(new FieldExpression(field), value, false, true));
            }
            else
            {
                Append(field, value);
            }
            //
            return this;
        }

        #region implement IEnumerable<Expression>

        public IEnumerator<ConditionExpression> GetEnumerator()
        {
            return _expression.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _expression.GetEnumerator();
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString()
        {
            var content = new StringBuilder("Expressions:");
            content.AppendLine();
            foreach (var item in _expression)
            {
                content.Append("[").Append(item.ToString()).AppendLine("]");
            }
            content.Append(';');
            return content.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        public readonly static string[] Operaters = { "=", "<", "<=", ">", ">=", "IN", "LIKE", "LIKE", "LIKE", "<>", "BETWEEN" };
        /// <summary>
        /// 获取运算符
        /// </summary>
        /// <param name="opt">运算符枚举</param>
        /// <returns>转换成数据库的运算符</returns>
        public static string OperaterChar(Operator opt)
        {
            if ((int)opt < 0 || (int)opt >= Operaters.Length)
                throw new ArgumentException("指定的参数opt是一个可接受的枚举值。");

            return Operaters[(int)opt];
        }
    }

    /// <summary>
    /// 条件表达式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConditionExpression<T> : ConditionExpressions
    {
        private readonly IMetaFieldInfo[] _metaField;
        /// <summary>
        /// 
        /// </summary>
        public ConditionExpression()
        {
            var mapper = MapperManager.GetIMapper<T>();
            _metaField = mapper.MetaInfo.GetFields();
        }

        /// <summary>
        /// 增加一个条件表达式(默认是等于)
        /// </summary>
        /// <param name="field">成员字段</param>
        /// <param name="value">成员值</param>
        /// <returns>返回对象本身，它的目的是让编码更顺畅更轻松（Fluent）。</returns>
        public ConditionExpressions Append(Expression<Func<T, object>> field, object value)
        {
            if (field == null) throw new ArgumentNullException("field");
            if (value == null) throw new ArgumentNullException("value");

            base.Append(FieldName(field), value);
            //
            return this;
        }

        /// <summary>
        /// 增加一个条件表达式
        /// </summary>
        /// <param name="field">成员字段</param>
        /// <param name="opt">运算符</param>
        /// <param name="value">成员值</param>
        /// <returns>返回对象本身，它的目的是让编码更顺畅更轻松（Fluent）。</returns>
        public ConditionExpressions Append(Expression<Func<T, object>> field, Operator opt, object value)
        {
            if (field == null) throw new ArgumentNullException("field");
            if (value == null) throw new ArgumentNullException("value");

            base.Append(FieldName(field), opt, value);
            //
            return this;
        }

        private string FieldName(Expression<Func<T, object>> field)
        {
            var propertyInfo = (PropertyInfo)PropertyExpression.ExtractMemberExpression(field).Member;
            foreach (var fieldMap in _metaField)
            {
                if (fieldMap.PropertyInfo == propertyInfo)
                    return fieldMap.FieldName;
            }
            throw new ArgumentException("参数不支持作为查询条件，因为无法获取该属性所映射的成员字段。");
        }
    }
}
