using System.Collections;
using System.Collections.Generic;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(Image))]
public class Customer : MonoBehaviour
{
    public int CustomerNo = 0;
    [HideInInspector]
    public bool CurrentlyOpenedClosing = false;
    public PositionCell RelatedCell;
    public Image RelatedTimer;
    public SerializableDictionaryBase<Basic, AllSprites> MySprites = new SerializableDictionaryBase<Basic, AllSprites>();
}
