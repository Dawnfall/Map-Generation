using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using math;

//TL - 1
//TR - 2
//BR - 4
//BL - 8

namespace mapGen
{
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

        public static HashSet<Edge<MSNodeBase>>[] m_outlineEdgesSingleAndMultiple; //first are one edge only,second are more than one edge

        //**************
        // Marching Square data
        //**************
        static MarchingSquare[] m_marchSquareGrid;

        //**************
        // Results
        //**************

        public static Mesh RoofMesh { get; private set; }
        public static Mesh WallMesh { get; private set; }
        public static Mesh FloorMesh { get; private set; }
        public static List<List<Vector3>> EdgePaths { get; private set; }

        static void Restart()
        {
            RoofMesh = WallMesh = FloorMesh = null;
            EdgePaths = new List<List<Vector3>>();

            m_roofVertices = new List<Vector3>();
            m_roofIndices = new List<int>();

            m_wallVertices = new List<Vector3>();
            m_wallIndices = new List<int>();

            m_floorVertices = new List<Vector3>();
            m_floorIndices = new List<int>();

            m_outlineEdgesSingleAndMultiple = new HashSet<Edge<MSNodeBase>>[2];
            m_outlineEdgesSingleAndMultiple[0] = new HashSet<Edge<MSNodeBase>>();
            m_outlineEdgesSingleAndMultiple[1] = new HashSet<Edge<MSNodeBase>>();
        }

        /// <summary>
        /// Create marching squares
        /// </summary>
        /// <param name="map"></param>
        /// <param name="wallSize"></param>
        /// <param name="is3D"></param>
        static void CreateSquareGrid(IMarchingMap map, MapGenParams parameters)
        {
            if (map.MapData == null)
                return;

            MSNode[] allMeshNodes = new MSNode[map.MapData.NodeCount];
            for (int row = 0; row < map.MapData.RowCount; row++)
                for (int col = 0; col < map.MapData.ColCount; col++)
                {
                    allMeshNodes[row * map.MapData.ColCount + col] = new MSNode(
                        map.MapData.GetNodePos(row, col) - Vector3.forward * parameters.wallSize,
                        map.MapData.GetNode(row, col).m_type == MapNodeType.WALL,
                        map.MapData.NodeSize
                        );
                }

            m_marchSquareGrid = new MarchingSquare[(map.MapData.RowCount - 1) * (map.MapData.ColCount - 1)];
            for (int sq = 0; sq < (map.MapData.RowCount - 1) * (map.MapData.ColCount - 1); sq++)
            {
                int row = sq / (map.MapData.ColCount - 1);
                int col = sq % (map.MapData.ColCount - 1);

                m_marchSquareGrid[sq] = new MarchingSquare(
                    allMeshNodes[(row + 1) * map.MapData.ColCount + col],
                    allMeshNodes[(row + 1) * map.MapData.ColCount + col + 1],
                    allMeshNodes[row * map.MapData.ColCount + col],
                    allMeshNodes[row * map.MapData.ColCount + col + 1]

                    );
            }
        }

        /// <summary>
        /// Triangulates every marching square based on formation it is in
        /// </summary>
        static void MarchingSquaresToMeshes()
        {
            foreach (var square in m_marchSquareGrid)
            {
                switch (square.GetConfiguration())
                {
                    case 0:
                        break;
                    case 1:
                        MeshHelper.PolygonToMesh(new MSNodeBase[] { square.CL, square.TL, square.CT }, m_roofVertices, m_roofIndices, m_outlineEdgesSingleAndMultiple);
                        break;
                    case 2:
                        MeshHelper.PolygonToMesh(new MSNodeBase[] { square.CT, square.TR, square.CR }, m_roofVertices, m_roofIndices, m_outlineEdgesSingleAndMultiple);
                        break;
                    case 3:
                        MeshHelper.PolygonToMesh(new MSNodeBase[] { square.CL, square.TL, square.TR, square.CR }, m_roofVertices, m_roofIndices, m_outlineEdgesSingleAndMultiple);
                        break;
                    case 4:
                        MeshHelper.PolygonToMesh(new MSNodeBase[] { square.CR, square.BR, square.CB }, m_roofVertices, m_roofIndices, m_outlineEdgesSingleAndMultiple);
                        break;
                    case 5:
                        MeshHelper.PolygonToMesh(new MSNodeBase[] { square.TL, square.CT, square.CR, square.BR, square.CB, square.CL }, m_roofVertices, m_roofIndices, m_outlineEdgesSingleAndMultiple);
                        break;
                    case 6:
                        MeshHelper.PolygonToMesh(new MSNodeBase[] { square.CT, square.TR, square.BR, square.CB }, m_roofVertices, m_roofIndices, m_outlineEdgesSingleAndMultiple);
                        break;
                    case 7:
                        MeshHelper.PolygonToMesh(new MSNodeBase[] { square.TL, square.TR, square.BR, square.CB, square.CL }, m_roofVertices, m_roofIndices, m_outlineEdgesSingleAndMultiple);
                        break;
                    case 8:
                        MeshHelper.PolygonToMesh(new MSNodeBase[] { square.CB, square.BL, square.CL }, m_roofVertices, m_roofIndices, m_outlineEdgesSingleAndMultiple);
                        break;
                    case 9:
                        MeshHelper.PolygonToMesh(new MSNodeBase[] { square.CB, square.BL, square.TL, square.CT }, m_roofVertices, m_roofIndices, m_outlineEdgesSingleAndMultiple);
                        break;
                    case 10:
                        MeshHelper.PolygonToMesh(new MSNodeBase[] { square.TR, square.CR, square.CB, square.BL, square.CL, square.CT }, m_roofVertices, m_roofIndices, m_outlineEdgesSingleAndMultiple);
                        break;
                    case 11:
                        MeshHelper.PolygonToMesh(new MSNodeBase[] { square.TL, square.TR, square.CR, square.CB, square.BL }, m_roofVertices, m_roofIndices, m_outlineEdgesSingleAndMultiple);
                        break;
                    case 12:
                        MeshHelper.PolygonToMesh(new MSNodeBase[] { square.CR, square.BR, square.BL, square.CL }, m_roofVertices, m_roofIndices, m_outlineEdgesSingleAndMultiple);
                        break;
                    case 13:
                        MeshHelper.PolygonToMesh(new MSNodeBase[] { square.TL, square.CT, square.CR, square.BR, square.BL }, m_roofVertices, m_roofIndices, m_outlineEdgesSingleAndMultiple);
                        break;
                    case 14:
                        MeshHelper.PolygonToMesh(new MSNodeBase[] { square.CL, square.CT, square.TR, square.BR, square.BL }, m_roofVertices, m_roofIndices, m_outlineEdgesSingleAndMultiple);
                        break;
                    case 15:
                        MeshHelper.PolygonToMesh(new MSNodeBase[] { square.TL, square.TR, square.BR, square.BL }, m_roofVertices, m_roofIndices, m_outlineEdgesSingleAndMultiple);
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
        private static void CreateFloorMesh(MapData mapData)
        {
            Vector3 topLeft = mapData.GetNodePos(mapData.RowCount - 1, 0);
            Vector3 topRight = mapData.GetNodePos(mapData.RowCount - 1, mapData.ColCount - 1);
            Vector3 botLeft = mapData.GetNodePos(0, 0);
            Vector3 botRight = mapData.GetNodePos(0, mapData.ColCount - 1);

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
        static void CreateWallMesh(MapGenParams parameters)
        {
            foreach (List<Vector3> edgePath in EdgePaths)
            {               
                for (int i = 0; i < edgePath.Count - 1; i++)
                {
                    Vector3 topLeft = edgePath[i];
                    Vector3 topRight = edgePath[i + 1];
                    Vector3 botLeft = topLeft - Vector3.forward * parameters.wallSize;
                    Vector3 botRight = topRight - Vector3.forward * parameters.wallSize;

                    m_wallVertices.Add(topLeft);
                    m_wallVertices.Add(topRight);
                    m_wallVertices.Add(botLeft);
                    m_wallVertices.Add(botRight);

                    m_wallIndices.Add(i * 4);
                    m_wallIndices.Add(i * 4 + 2);
                    m_wallIndices.Add(i * 4 + 1);
                    m_wallIndices.Add(i * 4 + 2);
                    m_wallIndices.Add(i * 4 + 3);
                    m_wallIndices.Add(i * 4 + 1);
                }
            }

            WallMesh = new Mesh();
            WallMesh.vertices = m_wallVertices.ToArray();
            WallMesh.triangles = m_wallIndices.ToArray();
            WallMesh.RecalculateNormals();
        }

        private static void CreateEdgePaths()
        {
            Dictionary<MSNodeBase, MSNodeBase> edgesPerVertex = new Dictionary<MSNodeBase, MSNodeBase>();
            foreach (Edge<MSNodeBase> edge in m_outlineEdgesSingleAndMultiple[0])
            {
                if (!edgesPerVertex.ContainsKey(edge.A))
                    edgesPerVertex.Add(edge.A, edge.B);
                else
                    Debug.Log("Problem, Outer edge starts 2 times!");
            }

            HashSet<MSNodeBase> usedNodes = new HashSet<MSNodeBase>();
            foreach (Edge<MSNodeBase> edge in m_outlineEdgesSingleAndMultiple[0])
            {
                if (usedNodes.Contains(edge.A))
                {
                    continue;
                }
                MSNodeBase startNode = edge.A;
                MSNodeBase fromNode = startNode;

                List<Vector3> pathPoints = new List<Vector3>();
                do
                {
                    usedNodes.Add(fromNode);
                    pathPoints.Add(new Vector2(fromNode.m_position.x, fromNode.m_position.y));
                    fromNode = edgesPerVertex[fromNode];
                } while (fromNode != startNode);

                pathPoints.Add(startNode.m_position);
                EdgePaths.Add(pathPoints);
            }
        }

        public static void CreateMesh(IMarchingMap map, MapGenParams parameters)
        {
            if (map == null || map.MapData == null || parameters == null)
                return;

            Restart();

            CreateSquareGrid(map, parameters);
            MarchingSquaresToMeshes();

            CreateFloorMesh(map.MapData);
            CreateRoofMesh();

            map.RoofMesh.mesh = RoofMesh;
            map.FloorMesh.mesh = FloorMesh;


            CreateEdgePaths();
            if (parameters.Is3D)
            {
                CreateWallMesh(parameters);
                map.WallMesh.mesh = WallMesh;
            }
            else
            {
                map.PolygonCollider.pathCount = EdgePaths.Count;
                for (int i = 0; i < EdgePaths.Count; i++)
                {
                    List<Vector3> edges = EdgePaths[i];
                    Vector2[] vec2Edges = new Vector2[edges.Count];
                    for (int j = 0; j < edges.Count; j++)
                        vec2Edges[j] = edges[j];

                    map.PolygonCollider.SetPath(i, vec2Edges);
                }
            }
        }
    }

    public interface IMarchingMap
    {
        MapData MapData { get; set; }

        MeshFilter RoofMesh { get; }
        MeshFilter WallMesh { get; }
        MeshFilter FloorMesh { get; }

        PolygonCollider2D PolygonCollider { get; }
        MeshCollider WallCollider { get; }
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