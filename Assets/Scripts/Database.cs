using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;

[System.Serializable]
public class SpritesDict : SerializableDictionaryBase<AllSprites, Sprite> { }
public class Database : MonoBehaviour
{
    [SerializeField] private SpritesDict _allSpritesDict = new SpritesDict();
    public SpritesDict AllSpritesDict { get => _allSpritesDict; }
}
