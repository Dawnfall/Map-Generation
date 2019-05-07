using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace helper
{
    public static class HelperCommon
    {
        public static T[] reverseArray<T>(T[] arr)
        {
            T[] reversedArr = new T[arr.Length];
            for (int i = 0; i < arr.Length; i++)
                reversedArr[i] = arr[arr.Length - i - 1];
            return reversedArr;
        }
        public static T[] subArray<T>(T[] arr, int startIndex, int length)
        {
            T[] subArr = new T[length];
            for (int i = 0; i < length; i++)
                subArr[i] = arr[i + startIndex];
            return subArr;
        }
        public static void AddToDictionary<T>(Dictionary<T, int> dict, T key, int valueToAdd)
        {
            if (dict.ContainsKey(key))
                dict[key] += valueToAdd;
            else
                dict[key] = valueToAdd;
        }
        public static bool isInArray<T>(IEnumerable<T> arrStr, T item)
        {
            if (arrStr == null)
                return false;
            foreach (T str in arrStr)
                if (str.Equals(item))
                    return true;
            return false;
        }
        public static List<System.Type> selectAllOfGivenType(List<System.Type> allTypes, System.Type classType)
        {
            List<System.Type> selectedTypes = new List<System.Type>();
            foreach (var t in allTypes)
            {
                if (classType.IsAssignableFrom(t))
                    selectedTypes.Add(t);
            }
            return selectedTypes;
        }
        public static void printCollection<T>(IEnumerable<T> enumerable)
        {
            foreach (var a in enumerable)
            {
                Debug.Log(a.ToString());
            }
        }
    }
}





//public static bool AddIfType<T>(AActorData actorData, List<T> list) where T : AActorData
//{
//    T castActor = actorData as T;
//    if (castActor != null)
//    {
//        list.Add(castActor);
//        return true;
//    }
//    return false;
//}