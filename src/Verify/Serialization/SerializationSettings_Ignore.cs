using System;
using System.Collections.Generic;
using System.Linq.Expressions;

// ReSharper disable UseObjectOrCollectionInitializer

namespace VerifyTests
{
    public partial class SerializationSettings
    {
        internal Dictionary<Type, List<string>> ignoredMembers = new();
        internal List<string> ignoredByNameMembers = new();
        internal Dictionary<Type, List<Func<object, bool>>> ignoredInstances = new();

        public void IgnoreMembers<T>(params Expression<Func<T, object?>>[] expressions)
        {
            Guard.AgainstNull(expressions, nameof(expressions));
            foreach (var expression in expressions)
            {
                IgnoreMember(expression);
            }
        }

        public void IgnoreMember<T>(Expression<Func<T, object?>> expression)
        {
            Guard.AgainstNull(expression, nameof(expression));
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
            Guard.AgainstNull(names, nameof(names));
            foreach (var name in names)
            {
                IgnoreMember(declaringType, name);
            }
        }

        public void IgnoreMember(Type declaringType, string name)
        {
            Guard.AgainstNull(declaringType, nameof(declaringType));
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
            Guard.AgainstNull(shouldIgnore, nameof(shouldIgnore));
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
            Guard.AgainstNull(type, nameof(type));
            Guard.AgainstNull(shouldIgnore, nameof(shouldIgnore));

            if (!ignoredInstances.TryGetValue(type, out var list))
            {
                ignoredInstances[type] = list = new();
            }

            list.Add(shouldIgnore);
        }

        List<Type> ignoreMembersWithType = new();

        public void IgnoreMembersWithType<T>()
        {
            ignoreMembersWithType.Add(typeof(T));
        }

        List<Func<Exception, bool>> ignoreMembersThatThrow = new();

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
            Guard.AgainstNull(item, nameof(item));
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

        bool ignoreFalse = true;

        public void DontIgnoreFalse()
        {
            ignoreFalse = false;
        }
    }
}
