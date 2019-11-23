using System.Collections;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ObjectApproval
{
    public static class ContractResolutionHelpers
    {
        public static void ConfigureIfBool(this JsonProperty property, MemberInfo member, bool ignoreFalse)
        {
            Guard.AgainstNull(property, nameof(property));
            Guard.AgainstNull(member, nameof(member));

            if (ignoreFalse)
            {
                return;
            }

            if (property.PropertyType == typeof(bool))
            {
                property.DefaultValueHandling = DefaultValueHandling.Include;
                return;
            }

            if (property.PropertyType == typeof(bool?))
            {
                property.DefaultValueHandling = DefaultValueHandling.Include;
                property.ShouldSerialize = instance =>
                {
                    var value = member.GetValue<bool?>(instance);
                    return value.GetValueOrDefault(false);
                };
            }
        }

        public static void SkipEmptyCollections(this JsonProperty property, MemberInfo member)
        {
            Guard.AgainstNull(property, nameof(property));
            Guard.AgainstNull(member, nameof(member));
            var type = property.PropertyType;
            if (type == null)
            {
                return;
            }
            if (type == typeof(string))
            {
                return;
            }
            if (type.IsCollection())
            {
                property.ShouldSerialize = instance =>
                {
                    var collection = member.GetValue<IEnumerable>(instance);

                    if (collection == null)
                    {
                        // if the list is null, we defer the decision to NullValueHandling
                        return true;
                    }

                    // check to see if there is at least one item in the Enumerable
                    return collection.GetEnumerator().MoveNext();
                };
            }
        }
    }
}