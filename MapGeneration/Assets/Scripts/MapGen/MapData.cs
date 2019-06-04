using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mapGen
{
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

        public MapNode[] Nodes;
        public readonly float NodeSize;
        public readonly int RowCount;
        public readonly int ColCount;

        public int EdgeSize;

        public MapData(Vector3 worldPos, int rowCount, int colCount, float nodeSize)
        {
            m_worldPos = worldPos;
            RowCount = rowCount;
            ColCount = colCount;
            NodeSize = nodeSize;

            Nodes = new MapNode[RowCount * ColCount];
            for (int row = 0; row < RowCount; row++)
                for (int col = 0; col < ColCount; col++)
                    Nodes[row * ColCount + col] = new MapNode(row, col, MapNodeType.EMPTY);
        }

        public int NodeCount
        {
            get { return RowCount * ColCount; }
        }
        public float Width
        {
            get { return (float)ColCount * NodeSize; }
        }
        public float Height
        {
            get { return (float)RowCount * NodeSize; }
        }

        public MapNode GetNode(int row, int col)
        {
            if (row < 0 || row >= RowCount || col < 0 || col >= ColCount)
                return null;

            return Nodes[row * ColCount + col];
        }

        public Vector3 GetNodePos(int row, int col)
        {
            if (row < 0 || row >= RowCount || col < 0 || col >= ColCount)
                throw new System.Exception("Room coodinates out of boundries!");

            return
                m_worldPos +
                Vector3.left * ((float)ColCount / 2.0f - ((float)col + 0.5f)) * NodeSize +
                Vector3.down * ((float)RowCount / 2.0f - ((float)row + 0.5f)) * NodeSize
                ;
        }
        public Vector3 GetNodePos(MapNode node)
        {
            return GetNodePos(node.m_row, node.m_col);
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
}