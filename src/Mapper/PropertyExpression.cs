using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sparrow.CommonLibrary.Mapper
{
    /// <summary>
    /// 实体属性（lambda）表达式。
    /// </summary>
    public static class PropertyExpression
    {
        /// <summary>
        /// 获取表达未成员类型信息
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static MemberExpression ExtractMemberExpression<T>(Expression<Func<T, object>> exp)
        {
            MemberExpression member = null;
            if (exp.Body.NodeType == ExpressionType.Convert && ((UnaryExpression)exp.Body).Operand.NodeType == ExpressionType.MemberAccess)
            {
                member = ExtractMemberExpression(((UnaryExpression)exp.Body).Operand);
            }
            else
            {
                member = ExtractMemberExpression(exp.Body);
            }

            if (member.Expression.Type == typeof(T))
                return member;

            throw new MapperException("Lambda表达示不符合映射的标准，标准示例：x=>x.ID。");
        }

        /// <summary>
        /// 获取表达未成员类型信息
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static MemberExpression ExtractMemberExpression(Expression exp)
        {
            if (exp.NodeType == ExpressionType.MemberAccess)
            {
                if (((MemberExpression)exp).Expression.NodeType == ExpressionType.Parameter)
                    return (MemberExpression)exp;
            }

            throw new MapperException("Lambda表达示不符合映射的标准，标准示例：x=>x.ID。");
        }
    }

}
