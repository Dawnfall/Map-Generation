using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGen : MonoBehaviour
{
    #region prefabs
    //****************
    // PREFABS
    //****************
    [SerializeField]
    GameObject m_mapPrefab;
    [SerializeField]
    GameObject m_roofPrefab;
    [SerializeField]
    GameObject m_wallPrefab2D;
    [SerializeField]
    GameObject m_wallPrefab3D;
    [SerializeField]
    GameObject m_floorPrefab;
    #endregion

    [Range(1, 200)]
    public int m_rowCount;
    [Range(1, 200)]
    public int m_colCount;
    public float m_nodeSize;

    public int m_minAreaSize;
    public int m_areaEdgeSize;

    [Range(0, 100)]
    public float m_fillChance;
    [Range(1, 10)]
    public int m_celAutomataIterCount;

    public int m_minRoomSize;

    public bool m_is3D;
    public float m_wallSize;

    private void Start()
    {
        MapData mapData = CreateMapData();
        CreateMapGOs(mapData);
    }

    public MapData CreateMapData(bool doPartition = true, bool doEdges = true, bool doCelAutomata = true, bool doRooms = true)
    {
        CelAutomataMapGen celMapGen = CelAutomataMapGen.Instance;
        MapData mapData = celMapGen.CreateMapData(m_rowCount, m_colCount, m_nodeSize);

        if (doPartition)
            celMapGen.PartitionMap(mapData, m_minAreaSize);
        if (doEdges)
            celMapGen.AddAreaEdges(mapData, m_areaEdgeSize);
        if (doCelAutomata)
        {
            print("AA");
            celMapGen.AddRandom(mapData, m_fillChance);
            celMapGen.CelularAutomata(mapData, m_celAutomataIterCount);
        }
        if (doRooms)
            celMapGen.DetectAndUpdateRooms(mapData, m_minRoomSize);

        return mapData;
    }

    public Map CreateMapGOs(MapData mapData)
    {
        MarchingSquareMeshGen meshGen = MarchingSquareMeshGen.Instance;
        if (m_is3D)
            meshGen.CreateMesh3D(mapData, m_wallSize);
        else
            meshGen.CreateMesh2D(mapData);

        GameObject mapGO = GameObject.Instantiate(m_mapPrefab);
        mapGO.name = "Map";

        if (meshGen.RoofMesh)
        {
            GameObject roofInstance = GameObject.Instantiate(m_roofPrefab);
            roofInstance.GetComponent<MeshFilter>().sharedMesh = meshGen.RoofMesh;
            roofInstance.transform.SetParent(mapGO.transform);
        }

        if (m_is3D && meshGen.EdgePaths3D != null)
        {
            GameObject wallInstance = GameObject.Instantiate(m_wallPrefab3D);
            wallInstance.GetComponent<MeshFilter>().sharedMesh = meshGen.WallMesh;
            wallInstance.transform.SetParent(mapGO.transform);
        }
        else if(!m_is3D && meshGen.EdgePaths2D!=null)
        {
            GameObject wallInstance = GameObject.Instantiate(m_wallPrefab2D);
            PolygonCollider2D polygon = wallInstance.GetComponent<PolygonCollider2D>();

            polygon.pathCount = meshGen.EdgePaths2D.Count;
            for (int i = 0; i < meshGen.EdgePaths2D.Count; i++)
            {
                polygon.SetPath(i, meshGen.EdgePaths2D[i].ToArray());
            }
            wallInstance.transform.SetParent(mapGO.transform);
        }

        if (meshGen.FloorMesh)
        {
            GameObject floorInstance = GameObject.Instantiate(m_floorPrefab);
            floorInstance.GetComponent<MeshFilter>().sharedMesh = meshGen.FloorMesh;
            floorInstance.transform.SetParent(mapGO.transform);
        }
        Map map = mapGO.GetComponent<Map>();
        map.m_mapData = mapData;
        map.is3D = m_is3D;
        return map;
    }

}
