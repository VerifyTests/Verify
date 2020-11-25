using System;
using System.Linq.Expressions;
using System.Reflection;

static class ExpressionExtensions
{
    public static MemberInfo FindMember<T, TMember>(this Expression<Func<T, TMember>> expression)
    {
        if (expression.Body is UnaryExpression unary)
        {
            if (unary.Operand is MemberExpression unaryMember)
            {
                return unaryMember.Member;
            }
        }

        if (expression.Body is MemberExpression member)
        {
            return member.Member;
        }

        throw new ArgumentException("expression");
    }
}