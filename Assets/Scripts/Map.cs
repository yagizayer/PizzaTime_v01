using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Helper;


public class Map : MonoBehaviour
{
    [SerializeField] private List<Transform> AllColumns;
    [HideInInspector]
    public List<List<PositionCell>> Cells = new List<List<PositionCell>>();

    private (List<List<int>>, List<List<PositionCell>>) _mapLayout = (new List<List<int>>(){
        new List<int>(){0,0,0,0,1,0,0},// first column
        new List<int>(){0,1,1,1,1,1,0},
        new List<int>(){1,1,1,1,1,1,1},
        new List<int>(){1,1,1,1,1,1,1},
        new List<int>(){0,1,1,1,1,1,0},
        new List<int>(){0,0,0,0,1,0,0} // last column
    }, new List<List<PositionCell>>());
    private (int, int) mapSize = (6, 5); // row,column
    private void Start()
    {

        Cells = FillCells(AllColumns);

        Cells = FillNeighbors(Cells);
    }

    private List<List<PositionCell>> FillCells(List<Transform> columns)
    {
        // Fill Cells
        List<List<PositionCell>> result = new List<List<PositionCell>>();
        foreach (Transform column in columns)
            result.Add(column.GetComponentsInChildren<PositionCell>().ToList());

        // Fill MapLayout.Item2
        for (int columnNo = 0; columnNo < _mapLayout.Item1.Count; columnNo++)
        {
            int cellCount = 0;
            List<PositionCell> tempList = new List<PositionCell>();
            for (int rowNo = 0; rowNo < _mapLayout.Item1[columnNo].Count; rowNo++)
            {
                if (_mapLayout.Item1[columnNo][rowNo] == 1)
                {
                    tempList.Add(result[columnNo][cellCount]);
                    result[columnNo][cellCount].Column = columnNo;
                    result[columnNo][cellCount].Row = rowNo;
                    cellCount++;
                }
                else
                    tempList.Add(null);
            }
            _mapLayout.Item2.Add(tempList);
        }

        return result;
    }

    private List<List<PositionCell>> FillNeighbors(List<List<PositionCell>> cells)
    {
        for (int columnNo = 0; columnNo < _mapLayout.Item2.Count; columnNo++)
        {
            for (int rowNo = 0; rowNo < _mapLayout.Item2[columnNo].Count; rowNo++)
            {
                PositionCell me = _mapLayout.Item2[columnNo][rowNo];
                if (me)
                {
                    PositionCell left = GetNeighbor(columnNo, rowNo, CellDirection.Left);
                    PositionCell right = GetNeighbor(columnNo, rowNo, CellDirection.Right);
                    PositionCell next = GetNeighbor(columnNo, rowNo, CellDirection.Next);
                    PositionCell previous = GetNeighbor(columnNo, rowNo, CellDirection.Previous);
                    if (left)
                        if (!me.Neighbors.Contains(left))
                            me.Neighbors.Add(left);
                    if (right)
                        if (!me.Neighbors.Contains(right))
                            me.Neighbors.Add(right);
                    if (next)
                        if (!me.Neighbors.Contains(next))
                            me.Neighbors.Add(next);
                    if (previous)
                        if (!me.Neighbors.Contains(previous))
                            me.Neighbors.Add(previous);
                }
            }
        }

        return cells;
    }

    public PositionCell GetNeighbor(int currentColumn, int currentRow, CellDirection targetCellDirection)
    {
        switch (targetCellDirection)
        {
            case CellDirection.Next:
                if (currentRow + 1 > mapSize.Item1) return null;
                return _mapLayout.Item2[currentColumn][currentRow + 1];
            case CellDirection.Previous:
                if (currentRow - 1 < 0) return null;
                return _mapLayout.Item2[currentColumn][currentRow - 1];
            case CellDirection.Left:
                if (currentColumn - 1 < 0) return null;
                return _mapLayout.Item2[currentColumn - 1][currentRow];
            case CellDirection.Right:
                if (currentColumn + 1 > mapSize.Item2) return null;
                return _mapLayout.Item2[currentColumn + 1][currentRow];
            default:
                return null;
        }
    }
}
