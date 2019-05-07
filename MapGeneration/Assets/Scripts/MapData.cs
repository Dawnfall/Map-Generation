using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MapNodeType
{
    EMPTY,
    WALL
}

public class MapNode
{
    public readonly int m_row;
    public readonly int m_col;

    public MapNodeType m_type;
    public MapNode(int row, int col, MapNodeType type)
    {
        m_row = row;
        m_col = col;
        m_type = type;
    }
}

public class MapData
{
    public readonly Vector3 m_worldPos;

    public MapNode[] m_mapData;
    public readonly float m_nodeSize;
    public readonly int m_rowCount;
    public readonly int m_colCount;

    public List<MapArea> m_allAreas = new List<MapArea>();

    public MapData(Vector3 worldPos, int rowCount, int colCount, float nodeSize)
    {
        m_worldPos = worldPos;
        m_rowCount = rowCount;
        m_colCount = colCount;
        m_nodeSize = nodeSize;

        m_mapData = new MapNode[m_rowCount * m_colCount];
        for (int row = 0; row < m_rowCount; row++)
            for (int col = 0; col < m_colCount; col++)
                m_mapData[row * m_colCount + col] = new MapNode(row, col, MapNodeType.EMPTY);
    }

    public int NodeCount
    {
        get { return m_rowCount * m_colCount; }
    }
    public float Width
    {
        get { return (float)m_colCount * m_nodeSize; }
    }
    public float Height
    {
        get { return (float)m_rowCount * m_nodeSize; }
    }

    public MapNode GetNode(int row, int col)
    {
        if (row < 0 || row >= m_rowCount || col < 0 || col >= m_colCount)
            return null;

        return m_mapData[row * m_colCount + col];
    }

    public Vector3 GetNodePos(int row, int col, bool Is3D)
    {
        if (row < 0 || row >= m_rowCount || col < 0 || col >= m_colCount)
            throw new System.Exception("Room coodinates out of boundries!");

        return
            m_worldPos +
            Vector3.left * ((float)m_colCount / 2.0f - ((float)col + 0.5f)) * m_nodeSize +
            ((Is3D) ? Vector3.back : Vector3.down) * ((float)m_rowCount / 2.0f - ((float)row + 0.5f)) * m_nodeSize
            ;
    }
    public Vector3 GetNodePos(MapNode node, bool is3D)
    {
        return GetNodePos(node.m_row, node.m_col, is3D);
    }

    public List<MapNode> Get4Neighbours(int row, int col)
    {
        List<MapNode> neigbours = new List<MapNode>();
        for (int i = -1; i <= 1; i++)
            for (int j = -1; j <= 1; j++)
            {
                if (i == j)
                    continue;
                MapNode ne = GetNode(row + i, col + j);
                if (ne != null)
                    neigbours.Add(ne);
            }
        return neigbours;
    }
    public List<MapNode> Get8Neighbours(int row, int col)
    {
        List<MapNode> neigbours = new List<MapNode>();
        for (int i = -1; i <= 1; i++)
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;

                MapNode ne = GetNode(row + i, col + j);
                if (ne != null)
                    neigbours.Add(ne);
            }
        return neigbours;
    }


}
