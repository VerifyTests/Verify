namespace VerifyTests;

public static partial class VerifierSettings
{
    static Dictionary<Type, Dictionary<string, ConvertTargetMember>> membersConverters = [];

    internal static ConvertTargetMember? GetMemberConverter(MemberInfo member) =>
        GetMemberConverter(member.DeclaringType, member.Name);

    internal static ConvertTargetMember? GetMemberConverter<T>(string name) =>
        GetMemberConverter(typeof(T), name);

    internal static ConvertTargetMember? GetMemberConverter(Type? declaringType, string name)
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
        InnerVerifier.ThrowIfVerifyHasBeenRun();
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
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        var member = expression.FindMember();
        MemberConverter(
            member.DeclaringType!,
            member.Name,
            memberValue => converter((TMember) memberValue!));
    }

    public static void MemberConverter(Type declaringType, string name, ConvertTargetMember converter)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        Guard.AgainstNullOrEmpty(name);
        if (!membersConverters.TryGetValue(declaringType, out var list))
        {
            membersConverters[declaringType] = list = [];
        }

        list[name] = converter;
    }

    public static void MemberConverter(Type declaringType, string name, ConvertMember converter)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        Guard.AgainstNullOrEmpty(name);
        if (!membersConverters.TryGetValue(declaringType, out var list))
        {
            membersConverters[declaringType] = list = [];
        }

        list[name] = (_, value) => converter(value);
    }
}