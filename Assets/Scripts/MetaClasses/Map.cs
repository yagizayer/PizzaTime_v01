/// <summary>
/// This file is used for creating a dynamic map on scene
/// </summary>


using System.Collections.Generic;
using UnityEngine;


public class Map : MonoBehaviour
{
    [SerializeField] private List<Transform> AllColumns;
    [HideInInspector]
    public List<List<PositionCell>> Cells = new List<List<PositionCell>>();


    // MapLayout is how you Want to be the map on scene(hand written),
    // each column has 0s and 1s 0 means "no cell here" and 1 means "there is a cell"
    // first row(most top) has 2 cells and only cars spawns in these cells
    internal (List<List<int>>, List<List<PositionCell>>) MapLayout = (new List<List<int>>(){
        new List<int>(){0,1,1,1,1},// first column
        new List<int>(){1,1,1,1,1},
        new List<int>(){1,1,1,1,1},
        new List<int>(){0,1,1,1,1},// last column
    }, new List<List<PositionCell>>());
    public readonly (int, int) MapSize = (4, 3); // row,column
    private void Start()
    {
        Cells = FillCells(AllColumns);
        FillNeighbors();
    }

    /// <summary>
    /// Creates a Map for determine playable area
    /// </summary>
    /// <param name="columns">Real Scene objects referances</param>
    /// <returns>A map filled with usable PositionCell components</returns>
    private List<List<PositionCell>> FillCells(List<Transform> columns)
    {
        // Fill Cells
        List<List<PositionCell>> result = new List<List<PositionCell>>();
        foreach (Transform column in columns)
        {
            List<PositionCell> tempList = new List<PositionCell>();
            foreach (PositionCell item in column.GetComponentsInChildren<PositionCell>())
                tempList.Add(item);
            result.Add(tempList);
        }

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
    /// <summary>
    /// Fills created Cells object's neighbors
    /// </summary>
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


}
