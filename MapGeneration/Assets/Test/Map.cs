using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mapGen;

public class Map : MonoBehaviour, IMarchingMap
{
    [SerializeField] MapGenParams m_mapGenParams;

    public MapData MapData { get; set; }

    [SerializeField] MeshFilter m_roofMesh;
    public MeshFilter RoofMesh  { get { return m_roofMesh; } }

    [SerializeField] MeshFilter m_wallMesh;
    public MeshFilter WallMesh { get { return m_wallMesh; } }

    [SerializeField] MeshFilter m_floorMesh;
    public MeshFilter FloorMesh { get { return m_floorMesh; } }

    [SerializeField] PolygonCollider2D m_polygonCollider;
    public PolygonCollider2D PolygonCollider { get { return m_polygonCollider; } }

    [SerializeField] MeshCollider m_wallCollider;
    public MeshCollider WallCollider { get { return m_wallCollider; } }

    public void Generate()
    {
        RoofMesh.mesh = null;
        WallMesh.mesh = null;
        FloorMesh.mesh = null;
        PolygonCollider.pathCount = 0;

        MapData = HelperMapGen.GenerateMapData(m_mapGenParams);
        MarchingSquareMeshGen.CreateMesh(this, m_mapGenParams);
    }

    #region gizmos
    //**************
    // GIZMOS
    //**************
    public bool m_doDrawNodes = false;

    private void OnDrawGizmos()
    {
        if (MapData == null)
            return;
        if (m_doDrawNodes)
        {
            for (int row = 0; row < MapData.RowCount; row++)
                for (int col = 0; col < MapData.ColCount; col++)
                {
                    MapNode currNode = MapData.GetNode(row, col);
                    Gizmos.color = (currNode.m_type == MapNodeType.WALL) ? Color.black : Color.white;
                    Gizmos.DrawCube(MapData.GetNodePos(row, col), new Vector3(MapData.NodeSize - 0.1f, MapData.NodeSize - 0.1f, 0.1f)); //TODO 3d,2d
                }
        }
    }
    #endregion
}
