using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TL - 1
//TR - 2
//BR - 4
//BL - 8

public class MarchingSquareMeshGen
{
    private static MarchingSquareMeshGen m_instance;
    public static MarchingSquareMeshGen Instance
    {
        get
        {
            if (m_instance == null)
                m_instance = new MarchingSquareMeshGen();
            return m_instance;
        }
    }

    //**************
    // Roof mesh
    //**************
    public List<Vector3> m_roofVertices;
    List<int> m_roofIndices;
    Mesh m_roofMesh = null;

    //*************
    // WallMesh
    //*************
    public List<Vector3> m_wallVertices;
    List<int> m_wallIndices;
    Mesh m_wallMesh = null;

    //*************
    // Floor mesh
    //*************
    public List<Vector3> m_floorVertices;
    List<int> m_floorIndices;
    Mesh m_floorMesh;

    //**************
    // Edge Paths
    //**************
    List<List<Vector2>> m_edgePaths2D;
    List<List<Vector3>> m_edgePaths3D;

    //**************
    // Outlines
    //**************

    public HashSet<Edge<MeshNodeBase>>[] m_outlineEdgesSingleAndMultiple; //first are one edge only,second are more than one edge
    MeshSquare[] m_meshSquareGrid;


    public Mesh RoofMesh
    {
        get { return m_roofMesh; }
    }
    public Mesh WallMesh
    {
        get { return m_wallMesh; }
    }
    public Mesh FloorMesh
    {
        get { return m_floorMesh; }
    }
    public List<List<Vector3>> EdgePaths3D
    {
        get { return m_edgePaths3D; }
    }
    public List<List<Vector2>> EdgePaths2D
    {
        get { return m_edgePaths2D; }
    }

    public void CreateMesh2D(MapData mapData)
    {
        Restart();
        CreateSquareGrid(mapData, 0, false);
        SquareGridToMesh();

        CreateRoofMesh();
        CreateFloorMesh(mapData, false);
        CreateEdgePaths2D();
    }

    public void CreateMesh3D(MapData mapData, float wallHeight)
    {
        Restart();
        CreateSquareGrid(mapData, wallHeight, true);
        SquareGridToMesh();

        CreateRoofMesh();
        CreateEdgePaths3D();
        CreateWallMesh(wallHeight);
        CreateFloorMesh(mapData, true);
    }

    void Restart()
    {
        m_roofVertices = new List<Vector3>();
        m_roofIndices = new List<int>();
        m_roofMesh = null;

        m_wallVertices = new List<Vector3>();
        m_wallIndices = new List<int>();
        m_wallMesh = null;

        m_floorVertices = new List<Vector3>();
        m_floorIndices = new List<int>();
        m_floorMesh = null;

        m_edgePaths2D = new List<List<Vector2>>();
        m_edgePaths3D = new List<List<Vector3>>();

        m_outlineEdgesSingleAndMultiple = new HashSet<Edge<MeshNodeBase>>[2];
        m_outlineEdgesSingleAndMultiple[0] = new HashSet<Edge<MeshNodeBase>>();
        m_outlineEdgesSingleAndMultiple[1] = new HashSet<Edge<MeshNodeBase>>();
    }

    private void CreateRoofMesh()
    {
        m_roofMesh = new Mesh();
        m_roofMesh.vertices = m_roofVertices.ToArray();
        m_roofMesh.triangles = m_roofIndices.ToArray();
        m_roofMesh.RecalculateNormals();
    }

    private void CreateFloorMesh(MapData map, bool is3D)
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

        m_floorMesh = new Mesh();
        m_floorMesh.vertices = m_floorVertices.ToArray();
        m_floorMesh.triangles = m_floorIndices.ToArray();
        m_floorMesh.RecalculateNormals();
    }

    void CreateSquareGrid(MapData map, float wallSize, bool is3D)
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

    void SquareGridToMesh()
    {
        foreach (var square in m_meshSquareGrid)
        {
            TriangulateSquare(square);
        }
    }

    void TriangulateSquare(MeshSquare square)
    {
        switch (square.GetConfiguration())
        {
            case 0:
                break;
            case 1:
                RoofMeshFromPoints(square.CL, square.TL, square.CT);
                break;
            case 2:
                RoofMeshFromPoints(square.CT, square.TR, square.CR);
                break;
            case 3:
                RoofMeshFromPoints(square.CL, square.TL, square.TR, square.CR);
                break;
            case 4:
                RoofMeshFromPoints(square.CR, square.BR, square.CB);
                break;
            case 5:
                RoofMeshFromPoints(square.TL, square.CT, square.CR, square.BR, square.CB, square.CL);
                break;
            case 6:
                RoofMeshFromPoints(square.CT, square.TR, square.BR, square.CB);
                break;
            case 7:
                RoofMeshFromPoints(square.TL, square.TR, square.BR, square.CB, square.CL);
                break;
            case 8:
                RoofMeshFromPoints(square.CB, square.BL, square.CL);
                break;
            case 9:
                RoofMeshFromPoints(square.CB, square.BL, square.TL, square.CT);
                break;
            case 10:
                RoofMeshFromPoints(square.TR, square.CR, square.CB, square.BL, square.CL, square.CT);
                break;
            case 11:
                RoofMeshFromPoints(square.TL, square.TR, square.CR, square.CB, square.BL);
                break;
            case 12:
                RoofMeshFromPoints(square.CR, square.BR, square.BL, square.CL);
                break;
            case 13:
                RoofMeshFromPoints(square.TL, square.CT, square.CR, square.BR, square.BL);
                break;
            case 14:
                RoofMeshFromPoints(square.CL, square.CT, square.TR, square.BR, square.BL);
                break;
            case 15:
                RoofMeshFromPoints(square.TL, square.TR, square.BR, square.BL);
                break;
        }
    }

    void RoofMeshFromPoints(params MeshNodeBase[] meshNodes)
    {
        AssignRoofVertices(meshNodes);

        if (meshNodes.Length >= 3)
            AssignRoofIndices(meshNodes[0], meshNodes[1], meshNodes[2]);
        if (meshNodes.Length >= 4)
            AssignRoofIndices(meshNodes[0], meshNodes[2], meshNodes[3]);
        if (meshNodes.Length >= 5)
            AssignRoofIndices(meshNodes[0], meshNodes[3], meshNodes[4]);
        if (meshNodes.Length >= 6)
            AssignRoofIndices(meshNodes[0], meshNodes[4], meshNodes[5]);
    }

    void AssignRoofVertices(MeshNodeBase[] meshNodes)
    {
        foreach (var node in meshNodes)
        {
            if (node.m_index == -1)
            {
                node.m_index = m_roofVertices.Count;
                m_roofVertices.Add(node.m_position);
            }
        }

    }

    void AssignRoofIndices(MeshNodeBase a, MeshNodeBase b, MeshNodeBase c)
    {
        m_roofIndices.Add(a.m_index);
        m_roofIndices.Add(b.m_index);
        m_roofIndices.Add(c.m_index);

        Edge<MeshNodeBase> AB = new Edge<MeshNodeBase>(a, b);
        Edge<MeshNodeBase> BC = new Edge<MeshNodeBase>(b, c);
        Edge<MeshNodeBase> CA = new Edge<MeshNodeBase>(c, a);

        CheckForOutlineEdge(AB);
        CheckForOutlineEdge(BC);
        CheckForOutlineEdge(CA);
    }

    void CheckForOutlineEdge(Edge<MeshNodeBase> edge)
    {
        if (m_outlineEdgesSingleAndMultiple[0].Contains(edge))
        {
            m_outlineEdgesSingleAndMultiple[0].Remove(edge);
            m_outlineEdgesSingleAndMultiple[1].Add(edge);
        }
        else if (!m_outlineEdgesSingleAndMultiple[1].Contains(edge))
            m_outlineEdgesSingleAndMultiple[0].Add(edge);
    }

    private void CreateEdgePaths2D()
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
            m_edgePaths2D.Add(pathPoints);
        }
    }

    private void CreateEdgePaths3D()
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
            m_edgePaths3D.Add(pathPoints);
        }
    }

    void CreateWallMesh(float wallHeight)
    {
        foreach (List<Vector3> edgePath in m_edgePaths3D)
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
        m_wallMesh = new Mesh();
        m_wallMesh.vertices = m_wallVertices.ToArray();
        m_wallMesh.triangles = m_wallIndices.ToArray();
        m_wallMesh.RecalculateNormals();
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