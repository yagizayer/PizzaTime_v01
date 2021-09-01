using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerManager : MonoBehaviour
{
    public PositionCell PlayerCell;
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
            _eventManager.InvokePlayerMovementEvent(direction: CellDirection.Next);
        if (Input.GetKeyDown(KeyCode.S))
            _eventManager.InvokePlayerMovementEvent(direction: CellDirection.Previous);
        if (Input.GetKeyDown(KeyCode.A))
            _eventManager.InvokePlayerMovementEvent(direction: CellDirection.Left);
        if (Input.GetKeyDown(KeyCode.D))
            _eventManager.InvokePlayerMovementEvent(direction: CellDirection.Right);
        if (Input.GetKeyDown(KeyCode.E))
            if (_gameManager.AllCustomers.Keys.Contains(PlayerCell.transform))
                _eventManager.InvokePlayerKnockEvent(customer: _gameManager.AllCustomers[PlayerCell.transform]);

    }
    public void Move(CellDirection targetDirection)
    {
        PositionCell targetCell = _map.GetNeighbor(PlayerCell.Column, PlayerCell.Row, targetDirection);
        if (targetCell && targetCell.PlaceFor.Contains(AcceptedEntities.Player))
        {
            HidePlayer();
            PlayerCell = targetCell;
            ShowPlayer(targetDirection, targetCell);
        }
    }


    //-----------------


    // can be called only at the start
    private void ShowPlayer()
    {
        Sprite targetSprite = _gameManager.SpriteDatabase[AllSprites.PlayerTalkingBack];
        PlayerCell.MyImage.sprite = targetSprite;
        PlayerCell.MyImage.enabled = true;
        PlayerCell.MyImage.preserveAspect = true;
    }
    public void ShowPlayer(CellDirection targetDirection, PositionCell targetCell)
    {
        Sprite targetSprite = _gameManager.SpriteDatabase[AllSprites.Null];
        if (targetDirection == CellDirection.Next)
        {
            // select "player walking back" sprite 
            targetSprite = _gameManager.SpriteDatabase[AllSprites.PlayerWalkingBack];
        }
        if (targetDirection == CellDirection.Previous)
        {
            // select "player walking front" sprite 
            targetSprite = _gameManager.SpriteDatabase[AllSprites.PlayerWalkingFront];
        }
        if (targetDirection == CellDirection.Left)
        {
            if (targetCell.Column == 2 || targetCell.Column == 3)
            {
                // select "player crossing" sprite
                targetSprite = _gameManager.SpriteDatabase[AllSprites.PlayerCrossingLeft];
            }
            else
            {
                // select "player walking Left" sprite
                targetSprite = _gameManager.SpriteDatabase[AllSprites.PlayerStandingLeft_2];
            }
        }
        if (targetDirection == CellDirection.Right)
        {
            if (targetCell.Column == 2 || targetCell.Column == 3)
            {
                // select "player crossing" sprite
                targetSprite = _gameManager.SpriteDatabase[AllSprites.PlayerCrossingRight];
            }
            else
            {
                // select "player walking Right" sprite
                targetSprite = _gameManager.SpriteDatabase[AllSprites.PlayerStandingRight_2];
            }
        }
        PlayerCell.MyImage.sprite = targetSprite;
        PlayerCell.MyImage.enabled = true;
        PlayerCell.MyImage.preserveAspect = true;
    }
    public void HidePlayer()
    {
        PlayerCell.MyImage.sprite = null;
        PlayerCell.MyImage.enabled = false;
        PlayerCell.MyImage.preserveAspect = false;
    }
    public void RotatePlayerToCustomer(Customer customer)
    {
        if (customer.CustomerNo == 1 || customer.CustomerNo == 2)
        {
            PlayerCell.MyImage.sprite = _gameManager.SpriteDatabase[AllSprites.PlayerStandingRight_2];
            PlayerCell.MyImage.enabled = true;
            PlayerCell.MyImage.preserveAspect = true;
        }
        if (customer.CustomerNo == 3 || customer.CustomerNo == 4)
        {
            PlayerCell.MyImage.sprite = _gameManager.SpriteDatabase[AllSprites.PlayerStandingLeft_2];
            PlayerCell.MyImage.enabled = true;
            PlayerCell.MyImage.preserveAspect = true;
        }
    }


}
