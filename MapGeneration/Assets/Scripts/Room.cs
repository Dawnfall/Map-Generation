using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Room
{
    public readonly MapArea m_parentArea;

    public HashSet<MapNode> m_roomNodes;
    public readonly MapNodeType m_roomType;

    public readonly Color m_testColor;

    public Room(HashSet<MapNode> roomNodes, MapNodeType roomType, MapArea parentArea, Color testColor)
    {
        m_parentArea = parentArea;
        m_roomNodes = roomNodes;
        m_roomType = roomType;

        m_testColor = testColor;
    }

    public int Size
    {
        get { return m_roomNodes.Count; }
    }

    public HashSet<MapNode> GetEdgeNodes()
    {
        HashSet<MapNode> edgeNodes = new HashSet<MapNode>();
        foreach (MapNode mapNode in m_roomNodes)
        {
            foreach (MapNode neigbour in m_parentArea.m_parentMap.Get4Neighbours(mapNode.m_row, mapNode.m_col))
                if (!m_roomNodes.Contains(neigbour))
                {
                    edgeNodes.Add(mapNode);
                    continue;
                }
        }
        return edgeNodes;
    }

    public static float GetRoomToRoomDistance(Room a, Room b)
    {
        HashSet<MapNode> EdgesOfA = a.GetEdgeNodes();
        HashSet<MapNode> EdgesOfB = b.GetEdgeNodes();

        float bestDistanceSquared = -1;
        foreach(var aNode in EdgesOfA)
            foreach(var bNode in EdgesOfB)
            {
                float currDistanceSquared = Mathf.Pow(aNode.m_row - bNode.m_row, 2) + Mathf.Pow(aNode.m_col - bNode.m_col,2);
                if (currDistanceSquared < bestDistanceSquared || bestDistanceSquared == -1)
                    bestDistanceSquared = currDistanceSquared;
            }

        return Mathf.Sqrt(bestDistanceSquared);
    }
}
