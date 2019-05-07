using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

namespace helper
{
    public static class ReflectionHelper
    {
        public static Assembly[] getAllAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }
        public static List<Type> getAllTypesInAssemblies(Assembly[] assemblies)
        {
            List<Type> types = new List<Type>();
            foreach (var assemblie in assemblies)
            {
                foreach (var type in assemblie.GetTypes())
                    types.Add(type);
            }

            return types;
        }
        public static List<Type> getAllSubTypesInAssemblies(Type baseType, bool doIncludeAbstract)
        {
            List<Type> allSubTypes = new List<Type>();
            foreach (Assembly a in getAllAssemblies())
            {
                foreach (Type t in a.GetTypes())
                    if ((doIncludeAbstract || (!doIncludeAbstract && !t.IsAbstract)) && baseType.IsAssignableFrom(t))
                        allSubTypes.Add(t);
            }
            return allSubTypes;

        }

        public static bool findResultsForFirstIncludedSubtype<T>(Dictionary<Type, T> dict, Type searchType, out T result)
        {
            result = default(T);

            while (searchType != null)
            {
                if (dict.TryGetValue(searchType, out result))
                    return true;

                searchType = searchType.BaseType;
            }
            return false;
        }
        public static bool isAssignableToGenericType(Type fromType, Type toType)
        {
            if (fromType == null)
                return false;
            if (fromType.IsGenericType && fromType.GetGenericTypeDefinition() == toType)
                return true;
            fromType = fromType.BaseType;
            return isAssignableToGenericType(fromType, toType);
        }
        public static Type[] genericSubclassArgumentTypes(Type genericSubclassDefinition, Type toCheck)
        {
            while (toCheck != null)
            {
                if (toCheck.IsGenericType && toCheck.GetGenericTypeDefinition() == genericSubclassDefinition)
                {
                    return toCheck.GetGenericArguments();
                }
                toCheck = toCheck.BaseType;
            }

            return null;
        }
    }
}