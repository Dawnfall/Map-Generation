using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MapGen/Map Gen Data")]
public class MapGenData : ScriptableObject
{
    [Range(1, 200)] public int m_rowCount=100;
    [Range(1, 200)] public int m_colCount=100;
    public float m_nodeSize;

    public bool DoPartion = true;
    [Range(1, 1000)] public int minPartitionSize=25;

    public bool DoEdges = true;
    [Range(0, 30)] public int edgeSize=2;

    public bool DoAddRandom = true;
    [Range(0, 100)] public float fillChance=0.65f;

    public bool DoCellularAutomata = true;
    [Range(1, 20)] public int cellAutomataIterCount = 5;





    public bool Is3D = false;
    public float wallSize;
}
