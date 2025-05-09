namespace VerifyTests;

public partial class SettingsTask
{
    /// <inheritdoc cref="VerifySettings.DontScrubGuids()"/>
    [Pure]
    public SettingsTask DontScrubGuids()
    {
        CurrentSettings.DontScrubGuids();
        return this;
    }

    /// <inheritdoc cref="VerifySettings.UseStrictJson()"/>
    [Pure]
    public SettingsTask UseStrictJson()
    {
        CurrentSettings.UseStrictJson();
        return this;
    }

    /// <inheritdoc cref="VerifySettings.DontScrubDateTimes()"/>
    [Pure]
    public SettingsTask DontScrubDateTimes()
    {
        CurrentSettings.DontScrubDateTimes();
        return this;
    }

    /// <inheritdoc cref="VerifySettings.DontSortDictionaries()"/>
    [Pure]
    public SettingsTask DontSortDictionaries()
    {
        CurrentSettings.DontSortDictionaries();
        return this;
    }

    /// <inheritdoc cref="VerifySettings.OrderEnumerableBy{T}"/>
    [Pure]
    public SettingsTask OrderEnumerableBy<T>(Func<T, object?> keySelector)
    {
        CurrentSettings.OrderEnumerableBy(keySelector);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.OrderEnumerableByDescending{T}"/>
    [Pure]
    public SettingsTask OrderEnumerableByDescending<T>(Func<T, object?> keySelector)
    {
        CurrentSettings.OrderEnumerableByDescending(keySelector);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.IncludeObsoletes"/>
    [Pure]
    public SettingsTask IncludeObsoletes()
    {
        CurrentSettings.IncludeObsoletes();
        return this;
    }

    /// <inheritdoc cref="VerifySettings.DontIgnoreEmptyCollections"/>
    [Pure]
    public SettingsTask DontIgnoreEmptyCollections()
    {
        CurrentSettings.DontIgnoreEmptyCollections();
        return this;
    }

    /// <inheritdoc cref="VerifySettings.IgnoreMembers{T}(Expression{System.Func{T,object?}}[])"/>
    [Pure]
    public SettingsTask IgnoreMembers<T>(params Expression<Func<T, object?>>[] expressions)
        where T : notnull
    {
        CurrentSettings.IgnoreMembers(expressions);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubMembers{T}(Expression{Func{T,object?}}[])"/>
    [Pure]
    public SettingsTask ScrubMembers<T>(params Expression<Func<T, object?>>[] expressions)
        where T : notnull
    {
        CurrentSettings.ScrubMembers(expressions);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.IgnoreMember{T}(Expression{Func{T,object?}})"/>
    [Pure]
    public SettingsTask IgnoreMember<T>(Expression<Func<T, object?>> expression)
        where T : notnull
    {
        CurrentSettings.IgnoreMembers(expression);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubMember{T}(Expression{Func{T,object?}})"/>
    [Pure]
    public SettingsTask ScrubMember<T>(Expression<Func<T, object?>> expression)
        where T : notnull
    {
        CurrentSettings.ScrubMembers(expression);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.IgnoreMembers{T}(string[])"/>
    [Pure]
    public SettingsTask IgnoreMembers<T>(params string[] names)
        where T : notnull
    {
        CurrentSettings.IgnoreMembers<T>(names);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubMembers{T}(string[])"/>
    [Pure]
    public SettingsTask ScrubMembers<T>(params string[] names)
        where T : notnull
    {
        CurrentSettings.ScrubMembers<T>(names);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.IgnoreMember{T}(string)"/>
    [Pure]
    public SettingsTask IgnoreMember<T>(string name)
        where T : notnull
    {
        CurrentSettings.IgnoreMember<T>(name);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubMember{T}(string)"/>
    [Pure]
    public SettingsTask ScrubMember<T>(string name)
        where T : notnull
    {
        CurrentSettings.ScrubMember<T>(name);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.IgnoreMembers(Type,string[])"/>
    [Pure]
    public SettingsTask IgnoreMembers(Type declaringType, params string[] names)
    {
        CurrentSettings.IgnoreMembers(declaringType, names);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubMembers(Type,string[])"/>
    [Pure]
    public SettingsTask ScrubMembers(Type declaringType, params string[] names)
    {
        CurrentSettings.ScrubMembers(declaringType, names);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.IgnoreMember(Type,string)"/>
    [Pure]
    public SettingsTask IgnoreMember(Type declaringType, string name)
    {
        CurrentSettings.IgnoreMember(declaringType, name);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubMember(Type,string)"/>
    [Pure]
    public SettingsTask ScrubMember(Type declaringType, string name)
    {
        CurrentSettings.ScrubMember(declaringType, name);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.IgnoreMember(string)"/>
    [Pure]
    public SettingsTask IgnoreMember(string name)
    {
        CurrentSettings.IgnoreMember(name);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubMember(string)"/>
    [Pure]
    public SettingsTask ScrubMember(string name)
    {
        CurrentSettings.ScrubMember(name);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.IgnoreMembers(string[])"/>
    [Pure]
    public SettingsTask IgnoreMembers(params string[] names)
    {
        CurrentSettings.IgnoreMembers(names);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubMembers(string[])"/>
    [Pure]
    public SettingsTask ScrubMembers(params string[] names)
    {
        CurrentSettings.ScrubMembers(names);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.IgnoreInstance{T}"/>
    [Pure]
    public SettingsTask IgnoreInstance<T>(Func<T, bool> shouldIgnore)
        where T : notnull
    {
        CurrentSettings.IgnoreInstance(shouldIgnore);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubInstance{T}"/>
    [Pure]
    public SettingsTask ScrubInstance<T>(Func<T, bool> shouldScrub)
        where T : notnull
    {
        CurrentSettings.ScrubInstance(shouldScrub);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.IgnoreInstance(Type,ShouldIgnore)"/>
    [Pure]
    public SettingsTask IgnoreInstance(Type type, ShouldIgnore shouldIgnore)
    {
        CurrentSettings.IgnoreInstance(type, shouldIgnore);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubInstance(Type,ShouldScrub)"/>
    [Pure]
    public SettingsTask ScrubInstance(Type type, ShouldScrub shouldScrub)
    {
        CurrentSettings.ScrubInstance(type, shouldScrub);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.IgnoreMembersWithType{T}"/>
    [Pure]
    public SettingsTask IgnoreMembersWithType<T>()
        where T : notnull
    {
        CurrentSettings.IgnoreMembersWithType<T>();
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubMembersWithType{T}"/>
    [Pure]
    public SettingsTask ScrubMembersWithType<T>()
        where T : notnull
    {
        CurrentSettings.ScrubMembersWithType<T>();
        return this;
    }

    /// <inheritdoc cref="VerifySettings.AlwaysIncludeMembersWithType{T}"/>
    [Pure]
    public SettingsTask AlwaysIncludeMembersWithType<T>()
        where T : notnull
    {
        CurrentSettings.AlwaysIncludeMembersWithType<T>();
        return this;
    }

    /// <inheritdoc cref="VerifySettings.IgnoreMembersWithType"/>
    [Pure]
    public SettingsTask IgnoreMembersWithType(Type type)
    {
        CurrentSettings.IgnoreMembersWithType(type);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubMembersWithType"/>
    [Pure]
    public SettingsTask ScrubMembersWithType(Type type)
    {
        CurrentSettings.ScrubMembersWithType(type);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.AlwaysIncludeMembersWithType"/>
    [Pure]
    public SettingsTask AlwaysIncludeMembersWithType(Type type)
    {
        CurrentSettings.AlwaysIncludeMembersWithType(type);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.IgnoreMembersThatThrow{T}()"/>
    [Pure]
    public SettingsTask IgnoreMembersThatThrow<T>()
        where T : Exception
    {
        CurrentSettings.IgnoreMembersThatThrow<T>();
        return this;
    }

    /// <inheritdoc cref="VerifySettings.IgnoreMembersThatThrow(Func{Exception,bool})"/>
    [Pure]
    public SettingsTask IgnoreMembersThatThrow(Func<Exception, bool> item)
    {
        CurrentSettings.IgnoreMembersThatThrow(item);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.IgnoreMembersThatThrow{T}(Func{T,bool})"/>
    [Pure]
    public SettingsTask IgnoreMembersThatThrow<T>(Func<T, bool> item)
        where T : Exception
    {
        CurrentSettings.IgnoreMembersThatThrow(item);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubMembers(Func{string,bool})"/>
    [Pure]
    public SettingsTask ScrubMembers(Func<string, bool> predicate)
    {
        CurrentSettings.ScrubMembers(predicate);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.ScrubMembers(Func{MemberInfo,bool})"/>
    [Pure]
    public SettingsTask ScrubMembers(Func<MemberInfo, bool> predicate)
    {
        CurrentSettings.ScrubMembers(predicate);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.IgnoreMembers(Func{string,bool})"/>
    [Pure]
    public SettingsTask IgnoreMembers(Func<string, bool> predicate)
    {
        CurrentSettings.IgnoreMembers(predicate);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.IgnoreMembers(Func{MemberInfo,bool})"/>
    [Pure]
    public SettingsTask IgnoreMembers(Func<MemberInfo, bool> predicate)
    {
        CurrentSettings.IgnoreMembers(predicate);
        return this;
    }
}