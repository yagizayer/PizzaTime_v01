using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerManager : MonoBehaviour
{
    public PositionCell PlayerCell;
    [SerializeField, Range(.01f, 1)] private float _fickerEffectDuration = .1f;
    private EventManager _eventManager;
    private GameManager _gameManager;
    private CarsManager _carsManager;
    private Map _map;
    [HideInInspector]
    public bool IsMoveable = true;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _eventManager = _gameManager.GameEventManager;
        _carsManager = _gameManager.GameCarsManager;
        _map = _gameManager.GameMap;

        PlayerCell = _gameManager.StartingPosition;
        ShowPlayer();
    }
    private void Update()
    {
        if (_gameManager.IsGameRunning)
        {
            if (IsMoveable == false)
                return;

            // Gameplay Movements
            if (Input.GetKeyDown(KeyCode.W))
                _eventManager.InvokePlayerMovementEvent(direction: CellDirection.Next);
            if (Input.GetKeyDown(KeyCode.S))
                _eventManager.InvokePlayerMovementEvent(direction: CellDirection.Previous);
            if (Input.GetKeyDown(KeyCode.A))
                _eventManager.InvokePlayerMovementEvent(direction: CellDirection.Left);
            if (Input.GetKeyDown(KeyCode.D))
                _eventManager.InvokePlayerMovementEvent(direction: CellDirection.Right);
            if (Input.GetKeyDown(KeyCode.E))
                if (_gameManager.AllCustomers.Keys.Contains(PlayerCell.transform)) // there is a customer near player
                    if (_gameManager.AllCustomers[PlayerCell.transform].CurrentlyOpenedClosing == false) // customer is avaliable for interaction
                        _eventManager.InvokePlayerKnockEvent(customer: _gameManager.AllCustomers[PlayerCell.transform]);
        }

    }

    //----------------

    public void Move(CellDirection targetDirection)
    {
        PositionCell targetCell = PlayerCell.GetNeighbor(targetDirection);

        if (targetCell == null)
        {
            return;
        }

        //in case of forbidded direction desired
        if (PlayerCell.ForbidPlayerOnInput == targetDirection)
            return;

        // in case of skipping a cell
        if (targetCell.TeleportPlayerOnInput.Contains(targetDirection) || PlayerCell.TeleportPlayerOnInput.Contains(targetDirection))
            targetCell = targetCell.GetNeighbor(targetDirection);

        if (targetCell.PlaceFor.Contains(AcceptedEntities.Player))
        {
            StartCoroutine(ShowHidePlayer(targetCell));
            // ShowPlayer(targetDirection, targetCell); // old code (Dynamic Sprites for each Image)
        }

        // collision detection
        foreach (Car car in _carsManager.CurrentCars)
            StartCoroutine(_gameManager.CheckCollisionLater());

    }
    public void GivePizza(Customer customer)
    {
        if (customer.CurrentlyWaitingForPizza)
            _eventManager.InvokeDeliverEvent(customer);
    }

    //-----------------

    private IEnumerator ShowHidePlayer(PositionCell targetCell)
    {
        PositionCell tempCell = PlayerCell;
        PlayerCell = targetCell;
        ShowPlayer();
        yield return new WaitForSecondsRealtime(_fickerEffectDuration);
        HidePlayer(tempCell);
    }

    public void ShowPlayer()
    {
        #region OldCode (Dynamic Sprites for each Image)

        // Sprite targetSprite = _gameManager.SpriteDatabase[AllSprites.PlayerTalkingBack];
        // PlayerCell.MyImage.sprite = targetSprite;

        #endregion

        PlayerCell.ShowMainSprite();
        PlayerCell.MyImage.enabled = true;
        PlayerCell.MyImage.preserveAspect = true;
    }

    #region OldCode (Dynamic Sprites for each Image)
    // public void ShowPlayer(CellDirection targetDirection, PositionCell targetCell)
    // {
    //     Sprite targetSprite = _gameManager.SpriteDatabase[AllSprites.Null];
    //     if (targetDirection == CellDirection.Next)
    //     {
    //         // select "player walking back" sprite 
    //         targetSprite = _gameManager.SpriteDatabase[AllSprites.PlayerWalkingBack];
    //     }
    //     if (targetDirection == CellDirection.Previous)
    //     {
    //         // select "player walking front" sprite 
    //         targetSprite = _gameManager.SpriteDatabase[AllSprites.PlayerWalkingFront];
    //     }
    //     if (targetDirection == CellDirection.Left)
    //     {
    //         if (targetCell.Column == 2 || targetCell.Column == 3)
    //         {
    //             // select "player crossing" sprite
    //             targetSprite = _gameManager.SpriteDatabase[AllSprites.PlayerCrossingLeft];
    //         }
    //         else
    //         {
    //             // select "player walking Left" sprite
    //             targetSprite = _gameManager.SpriteDatabase[AllSprites.PlayerStandingLeft_2];
    //         }
    //     }
    //     if (targetDirection == CellDirection.Right)
    //     {
    //         if (targetCell.Column == 2 || targetCell.Column == 3)
    //         {
    //             // select "player crossing" sprite
    //             targetSprite = _gameManager.SpriteDatabase[AllSprites.PlayerCrossingRight];
    //         }
    //         else
    //         {
    //             // select "player walking Right" sprite
    //             targetSprite = _gameManager.SpriteDatabase[AllSprites.PlayerStandingRight_2];
    //         }
    //     }
    //     PlayerCell.MyImage.sprite = targetSprite;

    //     PlayerCell.MyImage.enabled = true;
    //     PlayerCell.MyImage.preserveAspect = true;
    // }
    #endregion

    public void HidePlayer(PositionCell targetCell)
    {
        targetCell.MyImage.enabled = false;
        targetCell.MyImage.preserveAspect = false;
    }

    public void HidePlayer()
    {
        // PlayerCell.MyImage.sprite = null; // old code (Dynamic Sprites for each Image)
        PlayerCell.MyImage.enabled = false;
        PlayerCell.MyImage.preserveAspect = false;
    }

    public void RespawnPlayer()
    {
        StartCoroutine(RespawningPlayer());
    }
    private IEnumerator RespawningPlayer()
    {
        IsMoveable = false;
        PlayerCell = _gameManager.StartingPosition;
        yield return new WaitForSecondsRealtime(1.5f);
        ShowPlayer();
        IsMoveable = true;
    }



}
