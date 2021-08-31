using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerManager : MonoBehaviour
{
    public PositionCell PlayerCell = new PositionCell();
    private EventManager _eventManager;
    private GameManager _gameManager;
    private Map _map;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _eventManager = _gameManager.GameEventManager;
        _map = _gameManager.GameMap;

        PlayerCell = _gameManager.StartingPosition;
        ShowPlayer();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
            _eventManager.InvokePlayerInputEvent(direction: CellDirection.Next);
        if (Input.GetKeyDown(KeyCode.S))
            _eventManager.InvokePlayerInputEvent(direction: CellDirection.Previous);
        if (Input.GetKeyDown(KeyCode.A))
            _eventManager.InvokePlayerInputEvent(direction: CellDirection.Left);
        if (Input.GetKeyDown(KeyCode.D))
            _eventManager.InvokePlayerInputEvent(direction: CellDirection.Right);

    }

    public void Move(CellDirection targetDirection)
    {
        PositionCell targetCell = _map.GetNeighbor(PlayerCell.Column, PlayerCell.Row, targetDirection);
        if (targetCell)
        {
            HidePlayer();
            PlayerCell = targetCell;
            ShowPlayer();
        }
    }

    public void ShowPlayer()
    {
        PlayerCell.MyImage.sprite = _gameManager.PlayerSprite;
        PlayerCell.MyImage.enabled = true;
        PlayerCell.MyImage.preserveAspect = true;
    }
    public void HidePlayer()
    {
        PlayerCell.MyImage.sprite = null;
        PlayerCell.MyImage.enabled = false;
        PlayerCell.MyImage.preserveAspect = false;
    }
}
