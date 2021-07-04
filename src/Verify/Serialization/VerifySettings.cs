using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace VerifyTests
{
    public partial class VerifySettings
    {
        internal SerializationSettings serialization = VerifierSettings.serialization;
        bool isCloned;

        public IReadOnlyList<string> IgnoredByNameMembers
        {
            get
            {
                return serialization.ignoredByNameMembers;
            }
        }

        public IReadOnlyDictionary<Type, IReadOnlyList<string>> IgnoredMembers
        {
            get
            {
                return serialization.ignoredMembers
                    .ToDictionary(x => x.Key, x => (IReadOnlyList<string>)x.Value);
            }
        }

        public IReadOnlyDictionary<Type, IReadOnlyList<Func<object, bool>>> IgnoredInstances
        {
            get
            {
                return serialization.ignoredInstances
                    .ToDictionary(x => x.Key, x => (IReadOnlyList<Func<object, bool>>)x.Value);
            }
        }

        public void ModifySerialization(Action<SerializationSettings> action)
        {
            if (!isCloned)
            {
                serialization = serialization.Clone();
                isCloned = true;
            }

            action(serialization);
            serialization.RegenSettings();
        }

        public void AddExtraSettings(Action<JsonSerializerSettings> action)
        {
            if (!isCloned)
            {
                serialization = serialization.Clone();
                isCloned = true;
            }

            serialization.AddExtraSettings(action);
            serialization.RegenSettings();
        }
    }
}