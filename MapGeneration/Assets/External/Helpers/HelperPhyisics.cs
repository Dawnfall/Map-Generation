using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace helper
{
    public static class HelperPhysics
    {
        public static RaycastHit2D RaycastMouse2D(Camera camera)
        {
            Vector3 camWorld = HelperUnity.getMouseWorldPos(camera);
            Vector2 position = new Vector2(camWorld.x, camWorld.y);

            return Physics2D.Raycast(position, Vector2.zero, 0f);
        }

        public static Vector3 GetDirFromTo(Transform from, Transform to, bool isNormalized)
        {
            return (isNormalized) ?
                (to.position - from.position).normalized :
                (to.position - from.position);
        }
        public static float GetDistanceFromTo(Transform from, Transform to, bool isSquared)
        {
            return (isSquared) ?
                (to.position - from.position).sqrMagnitude :
                (to.position - from.position).magnitude;
        }

        public static List<RaycastHit2D> Racast2DArk(Vector3 position, Vector3 forward, float angle, int rayCount, float distance, bool doAllPerRay)//TODO layermask
        {
            List<RaycastHit2D> hitList = new List<RaycastHit2D>();

            foreach (Vector2 dir in HelperMath.GetArkVectors2D(forward, angle, rayCount))
            {
                if (doAllPerRay)
                    hitList.AddRange(Physics2D.RaycastAll(position, dir, distance));
                else
                    hitList.Add(Physics2D.Raycast(position, dir, distance));
            }
            return hitList;
        }
    }
}