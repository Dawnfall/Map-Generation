using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using helper;

//TL - 1
//TR - 2
//BR - 4
//BL - 8

public static class MarchingSquareMeshGen
{
    //**************
    // Roof mesh
    //**************
    public static List<Vector3> m_roofVertices;
    static List<int> m_roofIndices;

    //*************
    // WallMesh
    //*************
    static public List<Vector3> m_wallVertices;
    static List<int> m_wallIndices;

    //*************
    // Floor mesh
    //*************
    public static List<Vector3> m_floorVertices;
    static List<int> m_floorIndices;

    //**************
    // Outlines
    //**************

    public static HashSet<Edge<MeshNodeBase>>[] m_outlineEdgesSingleAndMultiple; //first are one edge only,second are more than one edge
    
    //**************
    // Marching Square data
    //**************
    static MeshSquare[] m_meshSquareGrid;

    //**************
    // Results
    //**************

    public static Mesh RoofMesh { get; private set; }
    public static Mesh WallMesh { get; private set; }
    public static Mesh FloorMesh { get; private set; }
    public static List<List<Vector3>> EdgePaths3D { get; private set; }
    public static List<List<Vector2>> EdgePaths2D { get; private set; }

    static void Restart()
    {
        RoofMesh = WallMesh = FloorMesh = null;
        EdgePaths2D = null;
        EdgePaths3D = null;

        m_roofVertices = new List<Vector3>();
        m_roofIndices = new List<int>();

        m_wallVertices = new List<Vector3>();
        m_wallIndices = new List<int>();

        m_floorVertices = new List<Vector3>();
        m_floorIndices = new List<int>();

        m_outlineEdgesSingleAndMultiple = new HashSet<Edge<MeshNodeBase>>[2];
        m_outlineEdgesSingleAndMultiple[0] = new HashSet<Edge<MeshNodeBase>>();
        m_outlineEdgesSingleAndMultiple[1] = new HashSet<Edge<MeshNodeBase>>();
    }

    /// <summary>
    /// Create marching squares
    /// </summary>
    /// <param name="map"></param>
    /// <param name="wallSize"></param>
    /// <param name="is3D"></param>
    static void CreateSquareGrid(MapData map, float wallSize, bool is3D)
    {
        MeshNode[] allMeshNodes = new MeshNode[map.NodeCount];
        for (int row = 0; row < map.m_rowCount; row++)
            for (int col = 0; col < map.m_colCount; col++)
            {
                allMeshNodes[row * map.m_colCount + col] = new MeshNode(
                    map.GetNodePos(row, col, is3D) + ((is3D) ? Vector3.up * wallSize : Vector3.zero),
                    map.GetNode(row, col).m_type == MapNodeType.WALL,
                    map.m_nodeSize
                    );
            }

        m_meshSquareGrid = new MeshSquare[(map.m_rowCount - 1) * (map.m_colCount - 1)];
        for (int sq = 0; sq < (map.m_rowCount - 1) * (map.m_colCount - 1); sq++)
        {
            int row = sq / (map.m_colCount - 1);
            int col = sq % (map.m_colCount - 1);

            m_meshSquareGrid[sq] = new MeshSquare(
                allMeshNodes[(row + 1) * map.m_colCount + col],
                allMeshNodes[(row + 1) * map.m_colCount + col + 1],
                allMeshNodes[row * map.m_colCount + col],
                allMeshNodes[row * map.m_colCount + col + 1]
                );
        }
    }
    /// <summary>
    /// Triangulates every marching square based on formation it is in
    /// </summary>
    static void MarchingSquaresToMeshes()
    {
        foreach (var square in m_meshSquareGrid)
        {
            switch (square.GetConfiguration())
            {
                case 0:
                    break;
                case 1:
                    MeshHelper.PolygonToMesh(new MeshNodeBase[] { square.CL, square.TL, square.CT }, m_roofVertices, m_roofIndices, m_outlineEdgesSingleAndMultiple);
                    break;
                case 2:
                    MeshHelper.PolygonToMesh(new MeshNodeBase[] { square.CT, square.TR, square.CR }, m_roofVertices, m_roofIndices, m_outlineEdgesSingleAndMultiple);
                    break;
                case 3:
                    MeshHelper.PolygonToMesh(new MeshNodeBase[] { square.CL, square.TL, square.TR, square.CR }, m_roofVertices, m_roofIndices, m_outlineEdgesSingleAndMultiple);
                    break;
                case 4:
                    MeshHelper.PolygonToMesh(new MeshNodeBase[] { square.CR, square.BR, square.CB }, m_roofVertices, m_roofIndices, m_outlineEdgesSingleAndMultiple);
                    break;
                case 5:
                    MeshHelper.PolygonToMesh(new MeshNodeBase[] { square.TL, square.CT, square.CR, square.BR, square.CB, square.CL }, m_roofVertices, m_roofIndices, m_outlineEdgesSingleAndMultiple);
                    break;
                case 6:
                    MeshHelper.PolygonToMesh(new MeshNodeBase[] { square.CT, square.TR, square.BR, square.CB }, m_roofVertices, m_roofIndices, m_outlineEdgesSingleAndMultiple);
                    break;
                case 7:
                    MeshHelper.PolygonToMesh(new MeshNodeBase[] { square.TL, square.TR, square.BR, square.CB, square.CL }, m_roofVertices, m_roofIndices, m_outlineEdgesSingleAndMultiple);
                    break;
                case 8:
                    MeshHelper.PolygonToMesh(new MeshNodeBase[] { square.CB, square.BL, square.CL }, m_roofVertices, m_roofIndices, m_outlineEdgesSingleAndMultiple);
                    break;
                case 9:
                    MeshHelper.PolygonToMesh(new MeshNodeBase[] { square.CB, square.BL, square.TL, square.CT }, m_roofVertices, m_roofIndices, m_outlineEdgesSingleAndMultiple);
                    break;
                case 10:
                    MeshHelper.PolygonToMesh(new MeshNodeBase[] { square.TR, square.CR, square.CB, square.BL, square.CL, square.CT }, m_roofVertices, m_roofIndices, m_outlineEdgesSingleAndMultiple);
                    break;
                case 11:
                    MeshHelper.PolygonToMesh(new MeshNodeBase[] { square.TL, square.TR, square.CR, square.CB, square.BL }, m_roofVertices, m_roofIndices, m_outlineEdgesSingleAndMultiple);
                    break;
                case 12:
                    MeshHelper.PolygonToMesh(new MeshNodeBase[] { square.CR, square.BR, square.BL, square.CL }, m_roofVertices, m_roofIndices, m_outlineEdgesSingleAndMultiple);
                    break;
                case 13:
                    MeshHelper.PolygonToMesh(new MeshNodeBase[] { square.TL, square.CT, square.CR, square.BR, square.BL }, m_roofVertices, m_roofIndices, m_outlineEdgesSingleAndMultiple);
                    break;
                case 14:
                    MeshHelper.PolygonToMesh(new MeshNodeBase[] { square.CL, square.CT, square.TR, square.BR, square.BL }, m_roofVertices, m_roofIndices, m_outlineEdgesSingleAndMultiple);
                    break;
                case 15:
                    MeshHelper.PolygonToMesh(new MeshNodeBase[] { square.TL, square.TR, square.BR, square.BL }, m_roofVertices, m_roofIndices, m_outlineEdgesSingleAndMultiple);
                    break;
            }
        }
    }

    private static void CreateRoofMesh()
    {
        RoofMesh = new Mesh();
        RoofMesh.vertices = m_roofVertices.ToArray();
        RoofMesh.triangles = m_roofIndices.ToArray();
        RoofMesh.RecalculateNormals();
    }
    private static void CreateFloorMesh(MapData map, bool is3D)
    {
        Vector3 topLeft = map.GetNodePos(map.m_rowCount - 1, 0, true);
        Vector3 topRight = map.GetNodePos(map.m_rowCount - 1, map.m_colCount - 1, true);
        Vector3 botLeft = map.GetNodePos(0, 0, true);
        Vector3 botRight = map.GetNodePos(0, map.m_colCount - 1, true);

        m_floorVertices.Add(topLeft);
        m_floorVertices.Add(topRight);
        m_floorVertices.Add(botLeft);
        m_floorVertices.Add(botRight);

        m_floorIndices.Add(0);
        m_floorIndices.Add(1);
        m_floorIndices.Add(2);

        m_floorIndices.Add(2);
        m_floorIndices.Add(1);
        m_floorIndices.Add(3);

        FloorMesh = new Mesh();
        FloorMesh.vertices = m_floorVertices.ToArray();
        FloorMesh.triangles = m_floorIndices.ToArray();
        FloorMesh.RecalculateNormals();
    }
    static void CreateWallMesh(float wallHeight)
    {
        foreach (List<Vector3> edgePath in EdgePaths3D)
        {
            for (int i = 0; i < edgePath.Count - 1; i++)
            {
                Vector3 topLeft = edgePath[i];
                Vector3 topRight = edgePath[i + 1];
                Vector3 botLeft = topLeft + Vector3.down * wallHeight;
                Vector3 botRight = topRight + Vector3.down * wallHeight;

                m_wallVertices.Add(topLeft);
                m_wallVertices.Add(topRight);
                m_wallVertices.Add(botLeft);
                m_wallVertices.Add(botRight);

                m_wallIndices.Add(i * 4);
                m_wallIndices.Add(i * 4 + 1);
                m_wallIndices.Add(i * 4 + 2);
                m_wallIndices.Add(i * 4 + 2);
                m_wallIndices.Add(i * 4 + 1);
                m_wallIndices.Add(i * 4 + 3);
            }
        }

        WallMesh = new Mesh();
        WallMesh.vertices = m_wallVertices.ToArray();
        WallMesh.triangles = m_wallIndices.ToArray();
        WallMesh.RecalculateNormals();
    }

    private static void CreateEdgePaths2D()
    {
        Dictionary<MeshNodeBase, MeshNodeBase> edgesPerVertex = new Dictionary<MeshNodeBase, MeshNodeBase>();
        foreach (Edge<MeshNodeBase> edge in m_outlineEdgesSingleAndMultiple[0])
        {
            if (!edgesPerVertex.ContainsKey(edge.A))
                edgesPerVertex.Add(edge.A, edge.B);
            else
                Debug.Log("Problem, Outer edge starts 2 times!");
        }

        HashSet<MeshNodeBase> usedNodes = new HashSet<MeshNodeBase>();
        foreach (Edge<MeshNodeBase> edge in m_outlineEdgesSingleAndMultiple[0])
        {
            if (usedNodes.Contains(edge.A))
            {
                continue;
            }
            MeshNodeBase startNode = edge.A;
            MeshNodeBase fromNode = startNode;

            List<Vector2> pathPoints = new List<Vector2>();
            do
            {
                usedNodes.Add(fromNode);
                pathPoints.Add(new Vector2(fromNode.m_position.x, fromNode.m_position.y));
                fromNode = edgesPerVertex[fromNode];
            } while (fromNode != startNode);

            pathPoints.Add(startNode.m_position);
            EdgePaths2D.Add(pathPoints);
        }
    }
    private static void CreateEdgePaths3D()
    {
        Dictionary<MeshNodeBase, MeshNodeBase> edgesPerVertex = new Dictionary<MeshNodeBase, MeshNodeBase>();
        foreach (Edge<MeshNodeBase> edge in m_outlineEdgesSingleAndMultiple[0])
        {
            if (!edgesPerVertex.ContainsKey(edge.A))
                edgesPerVertex.Add(edge.A, edge.B);
            else
                Debug.Log("Problem, Outer edge starts 2 times!");
        }

        HashSet<MeshNodeBase> usedNodes = new HashSet<MeshNodeBase>();
        foreach (Edge<MeshNodeBase> edge in m_outlineEdgesSingleAndMultiple[0])
        {
            if (usedNodes.Contains(edge.A))
            {
                continue;
            }
            MeshNodeBase startNode = edge.A;
            MeshNodeBase fromNode = startNode;

            List<Vector3> pathPoints = new List<Vector3>();
            do
            {
                usedNodes.Add(fromNode);
                pathPoints.Add(new Vector2(fromNode.m_position.x, fromNode.m_position.y));
                fromNode = edgesPerVertex[fromNode];
            } while (fromNode != startNode);

            pathPoints.Add(startNode.m_position);
            EdgePaths3D.Add(pathPoints);
        }
    }



    public static void CreateMesh2D(MapData mapData, MapGenData mapGenParams)
    {
        Restart();
        CreateSquareGrid(mapData, 0, false);
        MarchingSquaresToMeshes();

        CreateRoofMesh();
        CreateFloorMesh(mapData, false);
        CreateEdgePaths2D();
    }
    public static void CreateMesh3D(MapData mapData, MapGenData mapGenParams)
    {
        Restart();
        CreateSquareGrid(mapData, mapGenParams.wallSize, true);
        MarchingSquaresToMeshes();

        CreateRoofMesh();
        CreateEdgePaths3D();
        CreateWallMesh(mapGenParams.wallSize);
        CreateFloorMesh(mapData, true);
    }
}



////restart indexes
//foreach (Edge<MeshNodeBase> edge in m_outlineEdgesSingleAndMultiple[0])
//{
//    edge.A.m_index = -1;
//    edge.B.m_index = -1;
//}
//foreach (Edge<MeshNodeBase> edge in m_outlineEdgesSingleAndMultiple[0])
//{
//    if (edge.A.m_index == -1)
//    {
//        edge.A.m_index = m_wallVertices.Count;
//        Vector3 topLeft = edge.A.m_position;
//        Vector3 botLeft = topLeft - Vector3.up * 2.0f;

//        m_wallVertices.Add(topLeft);
//        m_wallVertices.Add(botLeft);
//    }

//    if (edge.B.m_index == -1)
//    {
//        edge.B.m_index = m_wallVertices.Count;
//        Vector3 topRight = edge.B.m_position;
//        Vector3 botRight = topRight - Vector3.up * 2.0f;

//        m_wallVertices.Add(topRight);
//        m_wallVertices.Add(botRight);
//    }

//    m_wallIndices.Add(edge.A.m_index);
//    m_wallIndices.Add(edge.A.m_index + 1);
//    m_wallIndices.Add(edge.B.m_index);

//    m_wallIndices.Add(edge.B.m_index);
//    m_wallIndices.Add(edge.A.m_index + 1);
//    m_wallIndices.Add(edge.B.m_index + 1);
//}

//m_wallMesh = new Mesh();
//m_wallMesh.vertices = m_wallVertices.ToArray();
//m_wallMesh.triangles = m_wallIndices.ToArray();
//m_wallMesh.RecalculateNormals();