using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Extensions
{
    public static CellDirection toCellDirection(this CarMotions me)
    {
        CellDirection result = CellDirection.Null;
        if (me == CarMotions.LeftToRight) result = CellDirection.Right;
        if (me == CarMotions.RightToLeft) result = CellDirection.Left;
        if (me == CarMotions.FrontToBack) result = CellDirection.Next;
        if (me == CarMotions.BackToFront) result = CellDirection.Previous;
        return result;
    }
}