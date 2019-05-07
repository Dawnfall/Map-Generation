using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace helper
{
    public static class HelperMath
    {
        public static Vector2 Vector2FromAngle(float angleInDeg)
        {
            angleInDeg *= Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(angleInDeg), Mathf.Sign(angleInDeg));
        }
        public static Vector2 RotateVector2AroundZ(Vector2 vec, float angleInDeg)
        {
            angleInDeg *= Mathf.Deg2Rad;

            float sin = Mathf.Sin(angleInDeg);
            float cos = Mathf.Cos(angleInDeg);

            return new Vector2(vec.x * cos - vec.y * sin, vec.x * sin + vec.y * cos);
        }

        public static Vector2[] GetArkVectors2D(Vector2 forward, float angle, int rayCount)
        {
            Vector2[] allDir = new Vector2[rayCount];

            float deltaAngle = angle / (rayCount - 1);
            for (int i = 0; i < rayCount; i++)
            {
                float currAngle = -angle / 2f + deltaAngle * i;
                allDir[i] = RotateVector2AroundZ(forward, currAngle);
            }

            return allDir;
        }
    }
}