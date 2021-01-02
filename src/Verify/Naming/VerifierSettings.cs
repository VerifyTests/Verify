﻿using System;
using System.Collections.Generic;

namespace VerifyTests
{
    public static partial class VerifierSettings
    {
        internal static Namer SharedNamer = new();
        static Dictionary<Type, Func<object, string>> parameterToNameLookup = new();

        public static void NameForParameter<T>(ParameterToName<T> func)
        {
            Guard.AgainstNull(func, nameof(func));
            parameterToNameLookup.Add(typeof(T), o => func((T) o));
        }

        internal static string GetNameForParameter(object parameter)
        {
            foreach (var parameterToName in parameterToNameLookup)
            {
                if (parameterToName.Key.IsInstanceOfType(parameter))
                {
                    return parameterToName.Value(parameter);
                }
            }

            var nameForParameter = parameter.ToString();
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (nameForParameter != null)
            {
                return nameForParameter;
            }

            throw new($"{parameter.GetType().FullName} returned a null for `ToString()`.");
        }

        public static void UniqueForRuntime()
        {
            SharedNamer.UniqueForRuntime = true;
        }

        public static void UniqueForAssemblyConfiguration()
        {
            SharedNamer.UniqueForAssemblyConfiguration = true;
        }

        public static void UniqueForRuntimeAndVersion()
        {
            SharedNamer.UniqueForRuntimeAndVersion = true;
        }
    }
}