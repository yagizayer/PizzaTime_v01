using UnityEngine;
using System.Collections.Generic;

namespace Helper
{
    public enum Vector3Values
    {
        X, Y, Z, XY, XZ, YZ, XYZ
    }

    public enum ColorValues
    {
        R, G, B, A
    }
    public static class Extesions
    {
        public static Vector3 Modify(this Vector3 oldValues, Vector3Values axis, float newValue)
        {
            switch (axis)
            {
                case Vector3Values.X:
                    return new Vector3(newValue, oldValues.y, oldValues.z);
                case Vector3Values.Y:
                    return new Vector3(oldValues.x, newValue, oldValues.z);
                case Vector3Values.Z:
                    return new Vector3(oldValues.x, oldValues.y, newValue);
            }
            return oldValues;
        }
        public static Vector3 Modify(this Vector3 oldValues, Vector3Values axis, Vector3 newValues)
        {
            switch (axis)
            {
                case Vector3Values.X:
                    return new Vector3(newValues.x, oldValues.y, oldValues.z);
                case Vector3Values.Y:
                    return new Vector3(oldValues.x, newValues.y, oldValues.z);
                case Vector3Values.Z:
                    return new Vector3(oldValues.x, oldValues.y, newValues.z);
                case Vector3Values.XY:
                    return new Vector3(newValues.x, newValues.y, oldValues.z);
                case Vector3Values.XZ:
                    return new Vector3(newValues.x, oldValues.y, newValues.z);
                case Vector3Values.YZ:
                    return new Vector3(oldValues.x, newValues.y, newValues.z);
                case Vector3Values.XYZ:
                    return newValues;
            }
            return oldValues;
        }
        public static Vector3 toVector3(this Vector2 vector2)
        {
            return new Vector3(vector2.x, vector2.y, 0);
        }
        public static Transform Clear(this Transform transform)
        {
            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            return transform;
        }
        public static bool HasChild(this Transform transform)
        {
            foreach (Transform child in transform)
                return true;
            return false;
        }
        public static void Reset(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }
        public static Transform ResetPosition(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            return transform;
        }
        public static RectTransform Reset(this RectTransform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            transform.anchorMin = Vector2.zero;
            transform.anchorMax = Vector2.one;
            transform.sizeDelta = Vector2.zero;
            transform.rect.Set(0, 0, 100, 100);
            return transform;
        }
        public static Dictionary<T1, T2> AddRange<T1, T2>(this Dictionary<T1, T2> me, Dictionary<T1, T2> other)
        {
            foreach (KeyValuePair<T1, T2> item in other)
                me.Add(item.Key, item.Value);
            return me;
        }
        public static Transform LookTarget(this Transform me, Transform target)
        {
            me.rotation = Quaternion.LookRotation(me.position - target.position, Vector3.up);
            return me;
        }
        public static Transform GetFirstChild(this Transform me)
        {
            foreach (Transform item in me)
            {
                return item;
            }
            return null;
        }
        public static Vector3 RelativeToCamera(this Vector3 me, Transform currentCamera)
        {
            Vector3 rawMoveDir = me;

            Vector3 cameraForwardNormalized = Vector3.ProjectOnPlane(currentCamera.forward, Vector3.up);
            Quaternion rotationToCamNormal = Quaternion.LookRotation(cameraForwardNormalized, Vector3.up);

            Vector3 finalMoveDir = rotationToCamNormal * rawMoveDir;
            return finalMoveDir;
        }
        public static float Remap(this float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }
        /// <summary>
        /// Returns any array's slice
        /// </summary>
        /// <param name="me">Specified array</param>
        /// <param name="startIndex">Start index(included)</param>
        /// <param name="endIndex">End index(excluded)</param>
        /// <returns>Array of given type</returns>
        public static object[] GetRange(this object[] me, int startIndex, int endIndex)
        {
            int resultLength = endIndex - startIndex;
            object[] result = new object[resultLength];
            for (int i = 0; i < resultLength; i++)
                result[i] = me[i + startIndex];
            return result;
        }
        /// <summary>
        /// Returns any array's slice from start
        /// </summary>
        /// <param name="me">Specified array</param>
        /// <param name="rangeSize">Size of array to be return</param>
        /// <returns>Array of given type</returns>
        public static object[] GetRange(this object[] me, int rangeSize)
        {
            return me.GetRange(0, rangeSize);
        }
        /// <summary>
        /// Modify color inline
        /// </summary>
        /// <param name="oldValues">Caller object</param>
        /// <param name="variableToModify">Specifying variable for which variable to modify in color</param>
        /// <param name="newValue">New value of specified variable</param>
        /// <returns>Color</returns>
        public static Color Modify(this Color oldValues, ColorValues variableToModify, float newValue)
        {
            switch (variableToModify)
            {
                case ColorValues.R:
                    oldValues = new Color(newValue, oldValues.g, oldValues.b, oldValues.a);
                    break;
                case ColorValues.G:
                    oldValues = new Color(oldValues.r, newValue, oldValues.b, oldValues.a);
                    break;
                case ColorValues.B:
                    oldValues = new Color(oldValues.r, oldValues.g, newValue, oldValues.a);
                    break;
                case ColorValues.A:
                    oldValues = new Color(oldValues.r, oldValues.g, oldValues.b, newValue);
                    break;
                default:
                    break;
            }
            return oldValues;
        }
        /// <summary>
        /// Finds all children with given tag
        /// </summary>
        /// <param name="me">Caller object</param>
        /// <param name="tagName">Desired tag</param>
        /// <returns>List of Transforms</returns>
        public static List<Transform> GetChildrenWithTag(this Transform me, string tagName)
        {
            List<Transform> result = new List<Transform>();
            foreach (Transform child in me)
                if (child.CompareTag(tagName)) result.Add(child);

            return result;
        }

        /// <summary>
        /// Returns random element of given list 
        /// </summary>
        /// <param name="me">Caller object</param>
        /// <typeparam name="T">Any generic type which can be a List</typeparam>
        /// <returns>Given generic type</returns>
        public static T GetRandom<T>(this List<T> me)
        {
            System.Random r = new System.Random();
            return me[r.Next(0, me.Count)];
        }

        public static List<T> ToList<T>(this T[] me)
        {
            List<T> result = new List<T>();
            foreach (T item in me)
                result.Add(item);
            return result;
        }

        public static int GetChildIndex(this Transform me, Transform targetChild)
        {
            int childIndex = 0;
            foreach (Transform child in me)
                if (child.GetHashCode() == targetChild.GetHashCode()) return childIndex;
            return -1;
        }

        public static List<T> ReduceDimension<T>(this List<List<T>> me)
        {
            List<T> result = new List<T>();
            foreach (List<T> item in me)
                result.AddRange(item);
            return result;
        }

    }

}