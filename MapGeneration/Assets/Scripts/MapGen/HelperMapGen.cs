using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mapGen
{
    public static class HelperMapGen
    {
        public static MapData GenerateMapData(MapGenParams mapGenParams)
        {
            MapData newMapData = new MapData(Vector3.zero, mapGenParams.m_rowCount, mapGenParams.m_colCount, mapGenParams.m_nodeSize);

            if (mapGenParams.DoEdges)
                AddEdge(newMapData, mapGenParams.edgeSize);
            if (mapGenParams.DoAddRandom)
                AddRandom(newMapData, mapGenParams.fillChance);
            if (mapGenParams.DoCellularAutomata)
                CelularAutomata(newMapData, mapGenParams.cellAutomataIterCount);

            return newMapData;
        }

        private static void AddEdge(MapData mapData, int areaEdgeSize)
        {
            mapData.EdgeSize = areaEdgeSize;

            for (int edge = 0; edge < areaEdgeSize; edge++)
            {
                for (int row = 0; row < mapData.RowCount; row++)
                {
                    mapData.GetNode(row, edge).m_type = MapNodeType.WALL;
                    mapData.GetNode(row, mapData.ColCount - edge-1).m_type = MapNodeType.WALL;
                }

                for (int col = 0; col < mapData.ColCount; col++)
                {
                    mapData.GetNode(edge, col).m_type = MapNodeType.WALL;
                    mapData.GetNode(mapData.RowCount - edge-1, col).m_type = MapNodeType.WALL;
                }
            }
        }
        private static void AddRandom(MapData mapData, float fillChance)
        {
            //...Fill with random...and walls
            for (int row = 0; row < mapData.RowCount; row++)
                for (int col = 0; col < mapData.ColCount; col++)
                {
                    MapNode node = mapData.GetNode(row, col);
                    if (node.m_type == MapNodeType.EMPTY)
                        mapData.GetNode(row, col).m_type = (Random.Range(0, 100) < fillChance) ? MapNodeType.WALL : MapNodeType.EMPTY;
                }

        }
        private static void CelularAutomata(MapData mapData, int iterCount)
        {
            int currCount = 0;
            while (currCount < iterCount)
            {
                for (int row = mapData.EdgeSize; (row + mapData.EdgeSize) < mapData.RowCount; row++)
                    for (int col = mapData.EdgeSize; (col + mapData.EdgeSize) < mapData.ColCount; col++)
                    {
                        int currRow = row;
                        int currCol = col;
                        MapNode currNode = mapData.GetNode(currRow, currCol);

                        int neigbourWallCount = 0;
                        int neighbourEmptyCount = 0;

                        for (int i = -1; i <= 1; i++)
                            for (int j = -1; j <= 1; j++)
                            {
                                int neRow = currRow + i;
                                int neCol = currCol + j;

                                if (neRow == neCol)
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
}


//TODO......

//static void DetectAndUpdateRooms(MapData mapData, int minRoomSize)
//{
//    //bool doAgain = true;
//    //while (doAgain)
//    //{
//    //    doAgain = false;
//    //    DetectRoomsInArea(mapData, area);
//    //    foreach (Room room in area.m_allRooms)
//    //        if (room.Size < minRoomSize)
//    //        {
//    //            doAgain = true;
//    //            foreach (MapNode node in room.m_roomNodes)
//    //                node.m_type = (node.m_type == MapNodeType.EMPTY) ? MapNodeType.WALL : MapNodeType.EMPTY;
//    //        }
//    //}

//    //List<Room> finalRooms = new List<Room>();
//    //foreach (Room room in area.m_allRooms)
//    //{
//    //    if (room.m_roomType == MapNodeType.EMPTY)
//    //        finalRooms.Add(room);
//    //}
//    //area.m_allRooms = finalRooms;

//}
//static void DetectRoomsInArea(MapData map, MapArea area)
//{
//    //List<Room> allRooms = new List<Room>();
//    //HashSet<MapNode> checkedNodes = new HashSet<MapNode>();

//    //for (int row = area.RowStart; row < area.RowEnd; row++)
//    //    for (int col = area.ColStart; col < area.ColEnd; col++)
//    //    {
//    //        MapNode currNode = map.GetNode(row, col);
//    //        if (checkedNodes.Contains(currNode))
//    //            continue;

//    //        HashSet<MapNode> newRoomNodes = new HashSet<MapNode>();
//    //        Queue<MapNode> queuedNodes = new Queue<MapNode>();
//    //        queuedNodes.Enqueue(currNode);

//    //        while (queuedNodes.Count > 0)
//    //        {
//    //            MapNode node = queuedNodes.Dequeue();
//    //            if (checkedNodes.Contains(node))
//    //                continue;
//    //            newRoomNodes.Add(node);
//    //            checkedNodes.Add(node);
//    //            foreach (var neNode in map.Get4Neighbours(node.m_row, node.m_col))
//    //                if (area.IsInArea(neNode.m_row, neNode.m_col) && !checkedNodes.Contains(neNode) && neNode.m_type == currNode.m_type)
//    //                    queuedNodes.Enqueue(neNode);
//    //        }
//    //        allRooms.Add(new Room(newRoomNodes, currNode.m_type, area, Random.ColorHSV()));
//    //    }
//    //area.m_allRooms = allRooms;

//}
//static void ConnectAreaRooms(MapData map, MapArea area)
//{
//    //TODO
//}

//public static void CreateMapGOs(Map map, MapData mapData)
//{
//    map.m_mapData = mapData;

//    Mesh roofMesh = null;
//    Mesh wallMesh = null;
//    Mesh floorMesh = null;
//    List<List<Vector2>> edgePaths2D = null;

//    map.RoofMesh.sharedMesh = roofMesh;
//    map.WallMesh.sharedMesh = wallMesh;
//    map.FloorMesh.sharedMesh = floorMesh;

//    map.Collider2D.pathCount = edgePaths2D.Count;
//    for (int i = 0; i < edgePaths2D.Count; i++)
//        map.Collider2D.SetPath(i, edgePaths2D[i].ToArray());
//}


//public static void PartitionMap(MapData mapData, int minAreaSize)
//{
//    List<MapArea> finishedAreas = new List<MapArea>();
//    List<MapArea> unfinishedSet = new List<MapArea>();

//    while (unfinishedSet.Count > 0)
//    {
//        MapArea currArea = unfinishedSet[unfinishedSet.Count - 1];
//        unfinishedSet.RemoveAt(unfinishedSet.Count - 1);

//        bool areMoreCols = currArea.ColCount > currArea.RowCount;
//        int bigger = (areMoreCols) ? currArea.ColCount : currArea.RowCount;
//        int smaller = (areMoreCols) ? currArea.RowCount : currArea.ColCount;

//        if (bigger < 2 * minAreaSize)
//        {
//            finishedAreas.Add(currArea);
//        }
//        else
//        {
//            int newBiggerCount1 = Random.Range(minAreaSize, bigger - minAreaSize);
//            int newBiggerCount2 = bigger - newBiggerCount1;

//            MapArea newRoom1;
//            MapArea newRoom2;
//            if (areMoreCols)
//            {
//                newRoom1 = new MapArea(mapData, currArea.RowStart, currArea.ColStart, smaller, newBiggerCount1);
//                newRoom2 = new MapArea(mapData, currArea.RowStart, currArea.ColStart + newBiggerCount1, smaller, newBiggerCount2);
//            }
//            else
//            {
//                newRoom1 = new MapArea(mapData, currArea.RowStart, currArea.ColStart, newBiggerCount1, smaller);
//                newRoom2 = new MapArea(mapData, currArea.RowStart + newBiggerCount1, currArea.ColStart, newBiggerCount2, smaller);
//            }
//            unfinishedSet.Add(newRoom1);
//            unfinishedSet.Add(newRoom2);
//        }
//    }
//}

