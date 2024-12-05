partial class SerializationSettings
{
    List<Func<MemberInfo, ScrubOrIgnore?>> ignoredMemberPredicatesByMember = [];
    List<Func<string, ScrubOrIgnore?>> ignoredMemberPredicatesByString = [];

    public void IgnoreMembers(Func<MemberInfo, bool> predicate)
    {
        Guard.NotNull(predicate);
        ignoredMemberPredicatesByMember.Add(_ => predicate(_) ? ScrubOrIgnore.Ignore : null);
    }

    public void IgnoreMembers(Func<string, bool> predicate)
    {
        Guard.NotNull(predicate);
        ignoredMemberPredicatesByString.Add(_ => predicate(_) ? ScrubOrIgnore.Ignore : null);
    }

    public void ScrubMembers(Func<MemberInfo, bool> predicate)
    {
        Guard.NotNull(predicate);
        ignoredMemberPredicatesByMember.Add(_ => predicate(_) ? ScrubOrIgnore.Scrub : null);
    }

    public void ScrubMembers(Func<string, bool> predicate)
    {
        Guard.NotNull(predicate);
        ignoredMemberPredicatesByString.Add(_ => predicate(_) ? ScrubOrIgnore.Scrub : null);
    }

    internal bool TryGetScrubOrIgnorePredicateByName(string name, MemberInfo? memberInfo, [NotNullWhen(true)] out ScrubOrIgnore? scrubOrIgnore)
    {
        foreach (var predicate in ignoredMemberPredicatesByString)
        {
            if (predicate(name) is { } result)
            {
                scrubOrIgnore = result;
                return true;
            }
        }

        if (memberInfo != null)
        {
            foreach (var predicate in ignoredMemberPredicatesByMember)
            {
                if(predicate(memberInfo) is { } result)
                {
                    scrubOrIgnore = result;
                    return true;
                }
            }
        }

        scrubOrIgnore = null;
        return false;
    }
}