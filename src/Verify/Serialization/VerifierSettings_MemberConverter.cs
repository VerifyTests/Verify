namespace VerifyTests;

public static partial class VerifierSettings
{
    static Dictionary<Type, Dictionary<string, ConvertTargetMember>> membersConverters = new();

    internal static ConvertTargetMember? GetMemberConverter(MemberInfo member)
    {
        return GetMemberConverter(member.DeclaringType, member.Name);
    }

    internal static ConvertTargetMember? GetMemberConverter<T>(string name)
    {
        return GetMemberConverter(typeof(T), name);
    }

    static ConvertTargetMember? GetMemberConverter(Type? declaringType, string name)
    {
        foreach (var pair in membersConverters)
        {
            if (pair.Key.IsAssignableFrom(declaringType))
            {
                pair.Value.TryGetValue(name, out var membersConverter);
                return membersConverter;
            }
        }

        return null;
    }

    public static void MemberConverter<TTarget, TMember>(
        Expression<Func<TTarget, TMember?>> expression,
        ConvertTargetMember<TTarget, TMember?> converter)
    {
        var member = expression.FindMember();
        MemberConverter(
            member.DeclaringType!,
            member.Name,
            (target, memberValue) => converter((TTarget) target, (TMember) memberValue!));
    }

    public static void MemberConverter<TTarget, TMember>(
        Expression<Func<TTarget, TMember?>> expression,
        ConvertMember<TMember?> converter)
    {
        var member = expression.FindMember();
        MemberConverter(
            member.DeclaringType!,
            member.Name,
            memberValue => converter((TMember) memberValue!));
    }

    public static void MemberConverter(Type declaringType, string name, ConvertTargetMember converter)
    {
        Guard.AgainstNullOrEmpty(name, nameof(name));
        if (!membersConverters.TryGetValue(declaringType, out var list))
        {
            membersConverters[declaringType] = list = new();
        }

        list[name] = converter;
    }

    public static void MemberConverter(Type declaringType, string name, ConvertMember converter)
    {
        Guard.AgainstNullOrEmpty(name, nameof(name));
        if (!membersConverters.TryGetValue(declaringType, out var list))
        {
            membersConverters[declaringType] = list = new();
        }

        list[name] = (_, value) => converter(value);
    }
}