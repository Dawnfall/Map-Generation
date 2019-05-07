using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using helper;

public class CelAutomataMapGen
{
    private static CelAutomataMapGen m_instance;
    public static CelAutomataMapGen Instance
    {
        get
        {
            if (m_instance == null)
                m_instance = new CelAutomataMapGen();
            return m_instance;
        }
    }

    public MapData CreateMapData(int rowCount, int colCount, float nodeSize)
    {
        return new MapData(Vector3.zero, rowCount, colCount, nodeSize);
    }

    public void PartitionMap(MapData mapData, int minAreaSize)
    {
        List<MapArea> finishedAreas = new List<MapArea>();
        List<MapArea> unfinishedSet = new List<MapArea>();
        unfinishedSet = mapData.m_allAreas;

        while (unfinishedSet.Count > 0)
        {
            MapArea currArea = unfinishedSet[unfinishedSet.Count - 1];
            unfinishedSet.RemoveAt(unfinishedSet.Count - 1);

            bool areMoreCols = currArea.ColCount > currArea.RowCount;
            int bigger = (areMoreCols) ? currArea.ColCount : currArea.RowCount;
            int smaller = (areMoreCols) ? currArea.RowCount : currArea.ColCount;

            if (bigger < 2 * minAreaSize)
            {
                finishedAreas.Add(currArea);
            }
            else
            {
                int newBiggerCount1 = Random.Range(minAreaSize, bigger - minAreaSize);
                int newBiggerCount2 = bigger - newBiggerCount1;

                MapArea newRoom1;
                MapArea newRoom2;
                if (areMoreCols)
                {
                    newRoom1 = new MapArea(mapData, currArea.RowStart, currArea.ColStart, smaller, newBiggerCount1);
                    newRoom2 = new MapArea(mapData, currArea.RowStart, currArea.ColStart + newBiggerCount1, smaller, newBiggerCount2);
                }
                else
                {
                    newRoom1 = new MapArea(mapData, currArea.RowStart, currArea.ColStart, newBiggerCount1, smaller);
                    newRoom2 = new MapArea(mapData, currArea.RowStart + newBiggerCount1, currArea.ColStart, newBiggerCount2, smaller);
                }
                unfinishedSet.Add(newRoom1);
                unfinishedSet.Add(newRoom2);
            }
        }
        mapData.m_allAreas = finishedAreas;
    }

    public void AddAreaEdges(MapData mapData, int areaEdgeSize)
    {
        foreach (var area in mapData.m_allAreas)
        {
            area.SetEdge(areaEdgeSize);
        }
    }

    public void AddRandom(MapData mapData, float fillChance)
    {
        foreach (var area in mapData.m_allAreas)
        {
            //...Fill with random...and walls
            for (int row = 0; row < area.RowCount; row++)
                for (int col = 0; col < area.ColCount; col++)
                {
                    if (!area.IsInAndInEdge(area.RowStart + row, area.ColStart + col))
                        mapData.GetNode(area.RowStart + row, area.ColStart + col).m_type = (Random.Range(0, 100) < fillChance) ? MapNodeType.WALL : MapNodeType.EMPTY;
                }
        }
    }

    public void CelularAutomata(MapData mapData, int iterCount)
    {
        foreach (var area in mapData.m_allAreas)
        {
            int currCount = 0;
            while (currCount < iterCount)
            {
                for (int row = area.EdgeSize; (row + area.EdgeSize) < area.RowCount; row++)
                    for (int col = area.EdgeSize; (col + area.EdgeSize) < area.ColCount; col++)
                    {
                        int currRow = area.RowStart + row;
                        int currCol = area.ColStart + col;
                        MapNode currNode = mapData.GetNode(currRow, currCol);

                        int neigbourWallCount = 0;
                        int neighbourEmptyCount = 0;

                        for (int i = -1; i <= 1; i++)
                            for (int j = -1; j <= 1; j++)
                            {
                                int neRow = currRow + i;
                                int neCol = currCol + j;

                                if (neRow == neCol || !area.IsInAndNotInEdge(neRow, neCol))
                                    continue;

                                if (mapData.GetNode(neRow, neCol).m_type == MapNodeType.WALL)
                                    neigbourWallCount++;
                                else
                                    neighbourEmptyCount++;
                            }

                        if (neigbourWallCount > neighbourEmptyCount)
                            currNode.m_type = MapNodeType.WALL;
                        else if (neigbourWallCount < neighbourEmptyCount)
                            currNode.m_type = MapNodeType.EMPTY;
                    }
                currCount++;
            }
        }
    }

    public void DetectAndUpdateRooms(MapData mapData, int minRoomSize)
    {
        foreach (var area in mapData.m_allAreas)
        {
            bool doAgain = true;
            while (doAgain)
            {
                doAgain = false;
                DetectRoomsInArea(mapData, area);
                foreach (Room room in area.m_allRooms)
                    if (room.Size < minRoomSize)
                    {
                        doAgain = true;
                        foreach (MapNode node in room.m_roomNodes)
                            node.m_type = (node.m_type == MapNodeType.EMPTY) ? MapNodeType.WALL : MapNodeType.EMPTY;
                    }
            }

            List<Room> finalRooms = new List<Room>();
            foreach (Room room in area.m_allRooms)
            {
                if (room.m_roomType == MapNodeType.EMPTY)
                    finalRooms.Add(room);
            }
            area.m_allRooms = finalRooms;
        }
    }

    void DetectRoomsInArea(MapData map, MapArea area)
    {
        List<Room> allRooms = new List<Room>();
        HashSet<MapNode> checkedNodes = new HashSet<MapNode>();

        for (int row = area.RowStart; row < area.RowEnd; row++)
            for (int col = area.ColStart; col < area.ColEnd; col++)
            {
                MapNode currNode = map.GetNode(row, col);
                if (checkedNodes.Contains(currNode))
                    continue;

                HashSet<MapNode> newRoomNodes = new HashSet<MapNode>();
                Queue<MapNode> queuedNodes = new Queue<MapNode>();
                queuedNodes.Enqueue(currNode);

                while (queuedNodes.Count > 0)
                {
                    MapNode node = queuedNodes.Dequeue();
                    if (checkedNodes.Contains(node))
                        continue;
                    newRoomNodes.Add(node);
                    checkedNodes.Add(node);
                    foreach (var neNode in map.Get4Neighbours(node.m_row, node.m_col))
                        if (area.IsInArea(neNode.m_row, neNode.m_col) && !checkedNodes.Contains(neNode) && neNode.m_type == currNode.m_type)
                            queuedNodes.Enqueue(neNode);
                }
                allRooms.Add(new Room(newRoomNodes, currNode.m_type, area, Random.ColorHSV()));
            }
        area.m_allRooms = allRooms;

    }

    static void ConnectAreaRooms(MapData map, MapArea area)
    {
        //TODO
    }

}