using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace FarmTown.Tests.EditMode
{
    internal static class RuntimeTypes
    {
        public static Type Get(string typeName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(typeName, false);
                if (type != null)
                    return type;

                var separator = typeName.LastIndexOf('.');
                if (separator > 0)
                {
                    type = assembly.GetType(typeName.Substring(0, separator) + "+" + typeName.Substring(separator + 1), false);
                    if (type != null)
                        return type;
                }
            }

            throw new InvalidOperationException($"Runtime type not found: {typeName}");
        }

        public static object GetField(object instance, string fieldName)
        {
            return instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(instance);
        }

        public static Array GetArrayField(object instance, string fieldName)
        {
            return (Array)GetField(instance, fieldName);
        }

        public static Component GetComponent(GameObject gameObject, string typeName)
        {
            return gameObject.GetComponent(Get(typeName));
        }

        public static Component[] GetComponentsInChildren(GameObject gameObject, string typeName)
        {
            return gameObject.GetComponentsInChildren(Get(typeName), true).Cast<Component>().ToArray();
        }
    }
}
