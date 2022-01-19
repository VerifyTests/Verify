﻿using System.Linq.Expressions;

// ReSharper disable UseObjectOrCollectionInitializer

namespace VerifyTests;

public partial class SerializationSettings
{
    internal Dictionary<Type, List<string>> ignoredMembers = new();
    internal List<string> ignoredByNameMembers = new();
    internal Dictionary<Type, List<Func<object, bool>>> ignoredInstances = new();

    public void IgnoreMembers<T>(params Expression<Func<T, object?>>[] expressions)
    {
        foreach (var expression in expressions)
        {
            IgnoreMember(expression);
        }
    }

    public void IgnoreMember<T>(Expression<Func<T, object?>> expression)
    {
        var member = expression.FindMember();
        IgnoreMember(member.DeclaringType!, member.Name);
    }

    public void IgnoreMembers<T>(params string[] names)
    {
        Guard.AgainstNullOrEmpty(names, nameof(names));
        foreach (var name in names)
        {
            IgnoreMember(typeof(T), name);
        }
    }

    public void IgnoreMember<T>(string name)
    {
        Guard.AgainstNullOrEmpty(name, nameof(name));
        IgnoreMember(typeof(T), name);
    }

    public void IgnoreMembers(Type declaringType, params string[] names)
    {
        foreach (var name in names)
        {
            IgnoreMember(declaringType, name);
        }
    }

    public void IgnoreMember(Type declaringType, string name)
    {
        Guard.AgainstNullOrEmpty(name, nameof(name));
        if (!ignoredMembers.TryGetValue(declaringType, out var list))
        {
            ignoredMembers[declaringType] = list = new();
        }

        list.Add(name);
    }

    public void IgnoreMember(string name)
    {
        Guard.AgainstNullOrEmpty(name, nameof(name));
        ignoredByNameMembers.Add(name);
    }

    public void IgnoreMembers(params string[] names)
    {
        Guard.AgainstNullOrEmpty(names, nameof(names));
        foreach (var name in names)
        {
            IgnoreMember(name);
        }
    }

    public void IgnoreInstance<T>(Func<T, bool> shouldIgnore)
    {
        var type = typeof(T);
        IgnoreInstance(
            type,
            target =>
            {
                var arg = (T) target;
                return shouldIgnore(arg);
            });
    }

    public void IgnoreInstance(Type type, Func<object, bool> shouldIgnore)
    {
        if (!ignoredInstances.TryGetValue(type, out var list))
        {
            ignoredInstances[type] = list = new();
        }

        list.Add(shouldIgnore);
    }

    List<Type> ignoredTypes = new();

    public void IgnoreMembersWithType<T>()
    {
        ignoredTypes.Add(typeof(T));
    }

    internal List<Func<Exception, bool>> ignoreMembersThatThrow = new();

    public void IgnoreMembersThatThrow<T>()
        where T : Exception
    {
        ignoreMembersThatThrow.Add(x => x is T);
    }

    public void IgnoreMembersThatThrow(Func<Exception, bool> item)
    {
        IgnoreMembersThatThrow<Exception>(item);
    }

    public void IgnoreMembersThatThrow<T>(Func<T, bool> item)
        where T : Exception
    {
        ignoreMembersThatThrow.Add(
            x =>
            {
                if (x is T exception)
                {
                    return item(exception);
                }

                return false;
            });
    }

    bool ignoreEmptyCollections = true;

    public void DontIgnoreEmptyCollections()
    {
        ignoreEmptyCollections = false;
    }

    internal bool dontIgnoreFalse;

    public void DontIgnoreFalse()
    {
        dontIgnoreFalse = true;
    }

    internal bool ShouldIgnore(MemberInfo member)
    {
        if (!includeObsoletes)
        {
            if (member.GetCustomAttribute<ObsoleteAttribute>(true) is not null)
            {
                return true;
            }
        }

        var propertyType = member.MemberType();
        if (ignoredTypes.Any(x => x.IsAssignableFrom(propertyType)))
        {
            return true;
        }

        if (ignoredByNameMembers.Contains(member.Name))
        {
            return true;
        }

        foreach (var pair in ignoredMembers)
        {
            if (pair.Value.Contains(member.Name))
            {
                if (pair.Key.IsAssignableFrom(member.DeclaringType))
                {
                    return true;
                }
            }
        }

        return false;
    }

    internal bool TryGetShouldSerialize(Type propertyType, Func<object, object?> getValue, out Predicate<object>? shouldSerialize)
    {
        if (ignoredInstances.TryGetValue(propertyType, out var funcs))
        {
            shouldSerialize = declaringInstance =>
            {
                var instance = getValue(declaringInstance);

                if (instance is null)
                {
                    return false;
                }

                return funcs.All(func => !func(instance));
            };

            return true;
        }

        if (ignoreEmptyCollections &&
            propertyType.IsCollection() ||
            propertyType.IsDictionary())
        {
            shouldSerialize = declaringInstance =>
            {
                var instance = getValue(declaringInstance);

                if (instance is null)
                {
                    return false;
                }

                // since inside IsCollection, it is safe to use IEnumerable
                var collection = (IEnumerable) instance;

                return collection.HasMembers();
            };

            return true;
        }

        shouldSerialize = null;
        return false;
    }
}