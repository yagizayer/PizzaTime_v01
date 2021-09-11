using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

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
    public static string ToStringWithFormat(this int me, int numberOfDigits)
    {
        string prefix = "";
        int digitDiff = numberOfDigits - me.ToString().Length;
        for (int i = 0; i < digitDiff ; i++)
            prefix += "0";
        return prefix + me.ToString();
    }

    public static T Next<T>(this T src) where T : struct
    {
        if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));
        T[] Arr = (T[])Enum.GetValues(src.GetType());
        int j = Array.IndexOf<T>(Arr, src) + 1;
        return (Arr.Length == j) ? Arr[0] : Arr[j];
    }
    public static T Previous<T>(this T src) where T : struct
    {
        if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));
        T[] Arr = (T[])Enum.GetValues(src.GetType());
        int j = Array.IndexOf<T>(Arr, src) - 1;
        return (j <= 0) ? Arr[0] : Arr[j];
    }

}