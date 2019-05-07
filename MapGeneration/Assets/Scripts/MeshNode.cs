using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshNodeBase
{
    public int m_index = -1;
    public Vector3 m_position;

    public MeshNodeBase(Vector3 pos)
    {
        m_position = pos;
    }
}

public class MeshNode : MeshNodeBase
{
    public readonly MeshNodeBase right, top;
    public readonly bool m_isWall;

    public MeshNode(Vector3 pos, bool isWall, float nodeSize) : base(pos)
    {
        m_isWall = isWall;
        right = new MeshNodeBase(m_position + Vector3.right * nodeSize / 2.0f);
        top = new MeshNodeBase(m_position + Vector3.forward * nodeSize / 2.0f);
    }
}

public class MeshSquare
{
    public readonly MeshNode TL, TR, BL, BR;
    public readonly MeshNodeBase CT, CB, CL, CR;

    public MeshSquare(MeshNode tl, MeshNode tr, MeshNode bl, MeshNode br)
    {
        TL = tl;
        TR = tr;
        BL = bl;
        BR = br;
        CT = tl.right;
        CB = bl.right;
        CL = bl.top;
        CR = br.top;
    }

    public int GetConfiguration()
    {
        return
            ((TL.m_isWall) ? 1 : 0) * 1 +
            ((TR.m_isWall) ? 1 : 0) * 2 +
            ((BR.m_isWall) ? 1 : 0) * 4 +
            ((BL.m_isWall) ? 1 : 0) * 8
            ;
    }
}

public struct MeshTriangle
{
    public readonly int indexA;
    public readonly int indexB;
    public readonly int indexC;

    public MeshTriangle(int _indexA, int _indexB, int _indexC)
    {
        indexA = _indexA;
        indexB = _indexB;
        indexC = _indexC;
    }
}

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
