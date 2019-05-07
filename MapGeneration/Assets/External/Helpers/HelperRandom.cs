using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace helper
{
    public static class HelperRandom
    {
        public static Vector2Int GetRandomDiscreteCoordInSquare(Vector2Int lowerLeft, Vector2Int topRight)
        {
            return new Vector2Int(Random.Range(lowerLeft.x, topRight.x + 1), Random.Range(lowerLeft.y, topRight.y + 1));
        }

        public static Vector2 GetRandomPositionInSquare(Vector2 lowerLeft, Vector2 topRight)
        {
            return new Vector2(Random.Range(lowerLeft.x, topRight.x), Random.Range(lowerLeft.y, topRight.y));
        }

        //this changes the ingoing list
        public static List<T> getRandomFromList<T>(IList<T> list, int amount, bool doAllowMultiple)
        {
            List<T> randomList = new List<T>();

            if (doAllowMultiple)
                for (int i = 0; i < amount; i++)
                {
                    randomList.Add(list[Random.Range(0, list.Count)]);
                }
            else
                for (int i = 0; i < amount; i++)
                {
                    int index = Random.Range(0, list.Count - i);
                    randomList.Add(list[index]);
                    list[index] = list[list.Count - i - 1];
                }

            return randomList;
        }

        public static T getRandomFromList<T>(IList<T> list)
        {
            return list[Random.Range(0, list.Count)];
        }
    }
}
