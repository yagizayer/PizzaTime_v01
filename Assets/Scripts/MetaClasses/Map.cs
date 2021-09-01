using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Helper;


public class Map : MonoBehaviour
{
    [SerializeField] private List<Transform> AllColumns;
    [SerializeField] private float PerspectiveRatio = .1f;
    [HideInInspector]
    public List<List<PositionCell>> Cells = new List<List<PositionCell>>();

    internal (List<List<int>>, List<List<PositionCell>>) MapLayout = (new List<List<int>>(){
        new List<int>(){0,0,0,0,1,0,0},// first column
        new List<int>(){0,1,1,1,1,1,0},
        new List<int>(){1,1,1,1,1,1,1},
        new List<int>(){1,1,1,1,1,1,1},
        new List<int>(){0,1,1,1,1,1,0},
        new List<int>(){0,0,0,0,1,0,0} // last column
    }, new List<List<PositionCell>>());
    public readonly (int, int) MapSize = (6, 5); // row,column
    private void Start()
    {
        Cells = FillCells(AllColumns);
        FillNeighbors();
        MakePerspective(PerspectiveRatio);
    }

    private List<List<PositionCell>> FillCells(List<Transform> columns)
    {
        // Fill Cells
        List<List<PositionCell>> result = new List<List<PositionCell>>();
        foreach (Transform column in columns)
            result.Add(column.GetComponentsInChildren<PositionCell>().ToList());

        // Fill MapLayout.Item2
        for (int columnNo = 0; columnNo < MapLayout.Item1.Count; columnNo++)
        {
            int cellCount = 0;
            List<PositionCell> tempList = new List<PositionCell>();
            for (int rowNo = 0; rowNo < MapLayout.Item1[columnNo].Count; rowNo++)
            {
                if (MapLayout.Item1[columnNo][rowNo] == 1)
                {
                    tempList.Add(result[columnNo][cellCount]);
                    result[columnNo][cellCount].Column = columnNo;
                    result[columnNo][cellCount].Row = rowNo;
                    cellCount++;
                }
                else
                    tempList.Add(null);
            }
            MapLayout.Item2.Add(tempList);
        }

        return result;
    }
    private void FillNeighbors()
    {
        for (int columnNo = 0; columnNo < MapLayout.Item2.Count; columnNo++)
        {
            for (int rowNo = 0; rowNo < MapLayout.Item2[columnNo].Count; rowNo++)
            {
                PositionCell me = MapLayout.Item2[columnNo][rowNo];
                if (me)
                {
                    PositionCell left = me.GetNeighbor(CellDirection.Left);
                    PositionCell right = me.GetNeighbor(CellDirection.Right);
                    PositionCell next = me.GetNeighbor(CellDirection.Next);
                    PositionCell previous = me.GetNeighbor(CellDirection.Previous);
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
    }

    private void MakePerspective(float perspectiveRatio)
    {
        for (int columnNo = 0; columnNo < MapLayout.Item2.Count; columnNo++)
            for (int rowNo = 0; rowNo < MapLayout.Item2[columnNo].Count; rowNo++)
            {
                // if there is a cell at present
                PositionCell me = MapLayout.Item2[columnNo][rowNo];
                if (me)
                    me.GetComponent<RectTransform>().localScale -= Vector3.one * rowNo * perspectiveRatio;
            }
    }



}
