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
            MemberExpression member;
            if (exp.Body.NodeType == ExpressionType.Convert)
                member = (MemberExpression)((UnaryExpression)exp.Body).Operand;
            else if (exp.Body.NodeType == ExpressionType.MemberAccess)
                member = (MemberExpression)exp.Body;
            else
                throw new MapperException("Lambda表达示不符合映射的标准，标准示例：x=>x.ID。");
            //
            return member;
        }
    }

}
