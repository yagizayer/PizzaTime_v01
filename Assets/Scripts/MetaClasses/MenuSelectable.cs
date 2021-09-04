using System.Collections;
using System.Collections.Generic;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine.UI;
using UnityEngine;

[System.Serializable]
public class SelectionMap : SerializableDictionaryBase<CellDirection, RectTransform> { }

[RequireComponent(typeof(Button))]
public class MenuSelectable : MonoBehaviour
{
    public bool SelectionDefault = false;

    public SelectionMap SelectionDirections = new SelectionMap();

    [HideInInspector]
    public float Width = -1;
    [HideInInspector]
    public float Height = -1;

    private void Awake()
    {
        Width = (transform as RectTransform).rect.width;
        Height = (transform as RectTransform).rect.height;
    }


}
