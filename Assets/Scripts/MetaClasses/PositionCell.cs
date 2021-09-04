using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PositionCell : MonoBehaviour
{

    public List<PositionCell> Neighbors = new List<PositionCell>();
    public List<AcceptedEntities> PlaceFor = new List<AcceptedEntities>(){
        AcceptedEntities.Player
    };
    public AllSprites MyMainSprite;
    public List<CellDirection> TeleportPlayerOnInput = new List<CellDirection>() {
        CellDirection.Null
    };
    public CellDirection ForbidPlayerOnInput = CellDirection.Null;


    public int Row = -1;
    public int Column = -1;
    [HideInInspector]
    public Image MyImage;
    private GameManager _gameManager;

    private void Start()
    {
        MyImage = GetComponent<Image>();
        _gameManager = FindObjectOfType<GameManager>();
        if (MyMainSprite != AllSprites.Null)
            MyImage.sprite = _gameManager.SpriteDatabase[MyMainSprite];
    }
    public PositionCell GetNeighbor(CellDirection targetCellDirection)
    {
        Debug.Log(transform.name + " : " + targetCellDirection);
        switch (targetCellDirection)
        {
            case CellDirection.Next:
                if (Row - 1 < 0) return null;
                return _gameManager.GameMap.MapLayout.Item2[Column][Row - 1];
            case CellDirection.Previous:
                if (Row + 1 > _gameManager.GameMap.MapSize.Item1) return null;
                return _gameManager.GameMap.MapLayout.Item2[Column][Row + 1];
            case CellDirection.Left:
                if (Column - 1 < 0) return null;
                return _gameManager.GameMap.MapLayout.Item2[Column - 1][Row];
            case CellDirection.Right:
                if (Column + 1 > _gameManager.GameMap.MapSize.Item2) return null;
                return _gameManager.GameMap.MapLayout.Item2[Column + 1][Row];
            default:
                return null;
        }
    }
    public void ShowMainSprite()
    {
        if (MyMainSprite != AllSprites.Null)
            MyImage.sprite = _gameManager.SpriteDatabase[MyMainSprite];
    }
}
