using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public MapData m_mapData = null;
    public MeshFilter RoofMesh;
    public MeshFilter WallMesh;
    public MeshFilter FloorMesh;
    public PolygonCollider2D Collider2D;


    public void Start()
    {
        m_mapData = MapGen.GenerateMapData(ScriptableObject.CreateInstance<MapGenData>());
        //create meshes and colliders
    }


    #region gizmos
    //**************
    // GIZMOS
    //**************
    public bool m_doDrawNodes = false;
    public bool m_doDrawRooms = false;

    private void OnDrawGizmos()
    {
        //if (m_mapData == null)
        //    return;
        //if (m_doDrawNodes)
        //{
        //    for (int row = 0; row < m_mapData.m_rowCount; row++)
        //        for (int col = 0; col < m_mapData.m_colCount; col++)
        //        {
        //            MapNode currNode = m_mapData.GetNode(row, col);
        //            Gizmos.color = (currNode.m_type == MapNodeType.WALL) ? Color.black : Color.white;
        //            Gizmos.DrawCube(m_mapData.GetNodePos(row, col, is3D), new Vector3(m_mapData.m_nodeSize - 0.1f, m_mapData.m_nodeSize - 0.1f, 0.1f)); //TODO 3d,2d
        //        }
        //}
        //if (m_doDrawRooms)
        //{
        //    foreach (var area in m_mapData.m_allAreas)
        //        foreach (Room room in area.m_allRooms)
        //        {
        //            Gizmos.color = room.m_testColor;
        //            foreach (MapNode node in room.m_roomNodes)
        //            {
        //                Gizmos.DrawCube(m_mapData.GetNodePos(node.m_row, node.m_col, is3D), new Vector3(m_mapData.m_nodeSize - 0.1f, m_mapData.m_nodeSize - 0.1f, 0.1f));
        //            }
        //        }
        //}
    }
    #endregion
}