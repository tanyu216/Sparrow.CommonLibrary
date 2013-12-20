using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Sparrow.CommonLibrary.Database;
using Sparrow.CommonLibrary.Mapper;
using Sparrow.CommonLibrary.Database.SqlBuilder;

namespace Sparrow.CommonLibrary.Query
{
    public class FieldExpression : SqlExpression
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string FieldName { get; protected set; }

        /// <summary>
        /// 字段别名
        /// </summary>
        public string AliasName { get; set; }

        /// <summary>
        /// 表名称或别名
        /// </summary>
        public string TableName { get; set; }

        protected FieldExpression(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException("fieldName");

            FieldName = fieldName;
        }

        public override ExpressionType NodeType
        {
            get { return ExpressionType.TableField; }
        }

        public override string OutputSqlString(ISqlBuilder builder, ParameterCollection output)
        {
            if (string.IsNullOrEmpty(AliasName))
                return builder.BuildField(TableName != null ? string.Concat(TableName, ".", FieldName) : FieldName);
            else
                return builder.BuildField(TableName != null ? string.Concat(TableName, ".", FieldName) : FieldName, AliasName);
        }

        internal static FieldExpression Expression(string fieldName)
        {
            return new FieldExpression(fieldName);
        }

        internal static FieldExpression Expression(string fieldName, string alias)
        {
            return new FieldExpression(fieldName) { AliasName = alias };
        }

        internal static FieldExpression Expression(string fieldName, string alias, string tableName)
        {
            return new FieldExpression(fieldName) { AliasName = alias, TableName = tableName };
        }
    }

    public class FieldExpression<T> : FieldExpression
    {
        protected FieldExpression(System.Linq.Expressions.Expression<Func<T, object>> field)
            : base(GetFieldName(field))
        {
        }

        internal static FieldExpression<T> Expression(System.Linq.Expressions.Expression<Func<T, object>> field)
        {
            return new FieldExpression<T>(field);
        }

        internal static FieldExpression<T> Expression(System.Linq.Expressions.Expression<Func<T, object>> field, string alias)
        {
            return new FieldExpression<T>(field) { AliasName = alias };
        }

        internal static FieldExpression<T> Expression(System.Linq.Expressions.Expression<Func<T, object>> field, string alias, string tableName)
        {
            return new FieldExpression<T>(field) { AliasName = alias, TableName = tableName };
        }

        private static string GetFieldName(System.Linq.Expressions.Expression<Func<T, object>> field)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            var propertyInfo = (PropertyInfo)PropertyExpression.ExtractMemberExpression(field).Member;
            var fieldInfo = Map.GetCheckedAccessor<T>().MetaInfo[propertyInfo];
            if (fieldInfo != null)
                return fieldInfo.PropertyName;
            throw new ArgumentException("参数不支持作为查询条件，因为无法获取该属性所映射的成员字段。");
        }
    }
}
