using System.Linq.Expressions;

namespace VerifyTests;

public static partial class VerifierSettings
{
    internal static Dictionary<Type, Dictionary<string, ConvertMember>> membersConverters = new();

    internal static ConvertMember? GetMemberConverter(MemberInfo member)
    {
        ConvertMember? membersConverter = null;
        foreach (var pair in membersConverters)
        {
            if (pair.Key.IsAssignableFrom(member.DeclaringType))
            {
                pair.Value.TryGetValue(member.Name, out membersConverter);
                break;
            }
        }

        return membersConverter;
    }

    public static void MemberConverter<TTarget, TMember>(
        Expression<Func<TTarget, TMember?>> expression,
        ConvertMember<TTarget, TMember?> converter)
    {
        var member = expression.FindMember();
        MemberConverter(
            member.DeclaringType!,
            member.Name,
            (target, memberValue) => converter((TTarget) target!, (TMember) memberValue!));
    }

    public static void MemberConverter(Type declaringType, string name, ConvertMember converter)
    {
        Guard.AgainstNullOrEmpty(name, nameof(name));
        if (!membersConverters.TryGetValue(declaringType, out var list))
        {
            membersConverters[declaringType] = list = new();
        }

        list[name] = converter;
    }
}