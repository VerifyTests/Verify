using System;
using Newtonsoft.Json;
using Verify;

namespace VerifyXunit
{
    public partial class VerifyBase
    {
        public void ModifySerialization(Action<SerializationSettings> action)
        {
            verifier.ModifySerialization(action);
        }

        public void AddExtraSettings(Action<JsonSerializerSettings> action)
        {
            verifier.AddExtraSettings(action);
        }
    }
}