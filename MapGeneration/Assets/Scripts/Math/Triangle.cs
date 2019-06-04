using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace math
{
    public class Triangle<T>
    {
        public readonly T a, b, c;
        public Triangle(T _a, T _b, T _c)
        {
            a = _a;
            b = _b;
            c = _c;
        }

        public Edge<T>[] GetAllEdges()
        {
            return new Edge<T>[] { AB, BC, CA };
        }

        public Edge<T> AB { get { return new Edge<T>(a, b); } }
        public Edge<T> BC { get { return new Edge<T>(b, c); } }
        public Edge<T> CA { get { return new Edge<T>(c, a); } }
    }
}
