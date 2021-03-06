/// <summary>
/// This file determines only one cell
/// </summary>
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
    public bool TeleportCar = false;

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
    /// <summary>
    /// Gives a Neighbor of this cell
    /// </summary>
    /// <param name="targetCellDirection">Desired Cells direction</param>
    /// <returns>Neighbor PositionCell of this one</returns>
    public PositionCell GetNeighbor(CellDirection targetCellDirection)
    {
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
    /// <summary>
    /// Shows the MainSprite of this cell. 
    /// </summary>
    public void ShowMainSprite()
    {
        if (MyMainSprite != AllSprites.Null)
            MyImage.sprite = _gameManager.SpriteDatabase[MyMainSprite];
    }
}
