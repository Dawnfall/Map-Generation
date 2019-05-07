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

