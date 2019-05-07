using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace helper
{
    /// <summary>
    /// represent an edge of a shape; orientation doesnt matter for equality
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Edge<T>
    {
        public readonly T A;
        public readonly T B;

        public Edge(T _A, T _B)
        {
            A = _A;
            B = _B;
        }

        public static bool operator ==(Edge<T> a, Edge<T> b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(Edge<T> a, Edge<T> b)
        {
            return !(a.Equals(b));
        }

        public override int GetHashCode()
        {
            return A.GetHashCode() + B.GetHashCode();
        }
        public bool Equals(Edge<T> other)
        {
            if (object.ReferenceEquals(this, null))
                return object.ReferenceEquals(other, null);

            return (this.A.Equals(other.A) && this.B.Equals(other.B)) || (this.A.Equals(other.B) && this.B.Equals(other.A));
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Edge<T>);
        }
        public bool Contains(T node)
        {
            return node.Equals(A) || node.Equals(B);
        }
    }
}