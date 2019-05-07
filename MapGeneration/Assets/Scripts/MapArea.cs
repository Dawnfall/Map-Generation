using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapArea
{
    public readonly MapData m_parentMap;

    private int m_colCount;
    private int m_rowCount;
    private int m_botLeftRow;
    private int m_botLeftCol;

    private int m_edgeSize;

    public List<Room> m_allRooms= new List<Room>();

    public MapArea(MapData parentMap,int botLeftRow, int botLeftCol, int rowCount, int colCount)
    {
        m_parentMap = parentMap;

        m_botLeftRow = botLeftRow;
        m_botLeftCol = botLeftCol;
        m_rowCount = rowCount;
        m_colCount = colCount;
        m_edgeSize = 0;
    }

    public int ColCount
    {
        get { return m_colCount; }
    }
    public int RowCount
    {
        get { return m_rowCount; }
    }
    public int RowStart
    {
        get { return m_botLeftRow; }
    }
    public int RowEnd
    {
        get { return m_botLeftRow + m_rowCount; }
    }
    public int ColStart
    {
        get { return m_botLeftCol; }
    }
    public int ColEnd
    {
        get { return m_botLeftCol + m_colCount; }
    }
    public int EdgeSize
    {
        get { return m_edgeSize; }
    }

    public void SetEdge(int edgeSize)
    {
        m_edgeSize = edgeSize;
        for (int row = RowStart; row < RowStart + m_rowCount; row++)
            for (int col = ColStart; col < ColStart + m_colCount; col++)
            {
                if (IsInAndInEdge(row, col))
                    m_parentMap.GetNode(row, col).m_type = MapNodeType.WALL;
            }
    }

    public bool IsInArea(int row, int col)
    {
        return
            row >= m_botLeftRow &&
            col >= m_botLeftCol &&
            row < RowEnd &&
            col < ColEnd;
    }
    public bool IsInAndNotInEdge(int row, int col)
    {
        return
            row >= m_botLeftRow + m_edgeSize &&
            col >= m_botLeftCol + m_edgeSize &&
            row < RowEnd - m_edgeSize &&
            col < ColEnd - m_edgeSize;
    }
    public bool IsInAndInEdge(int row, int col)
    {
        return IsInArea(row, col) && !IsInAndNotInEdge(row, col);
    }
}
