/// <summary>
/// This file used for determining player behaviour.
/// </summary>
using System.Collections;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [HideInInspector]
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
        if (_gameManager.CurrentGameState == GameState.Started)
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

    /// <summary>
    /// Moves player from current cell to a neighbor cell
    /// </summary>
    /// <param name="targetDirection"></param>
    public void Move(CellDirection targetDirection)
    {
        PositionCell targetCell = PlayerCell.GetNeighbor(targetDirection);

        // edge of the map
        if (targetCell == null)
            return;

        //in case of forbidded direction
        if (PlayerCell.ForbidPlayerOnInput == targetDirection)
            return;

        // in case of skipping a cell
        if (targetCell.TeleportPlayerOnInput.Contains(targetDirection) || PlayerCell.TeleportPlayerOnInput.Contains(targetDirection))
            targetCell = targetCell.GetNeighbor(targetDirection);

        // everytihng is fine
        if (targetCell.PlaceFor.Contains(AcceptedEntities.Player))
            StartCoroutine(ShowHidePlayer(targetCell));

        // collision detection
        foreach (Car car in _carsManager.CurrentCars)
            StartCoroutine(_gameManager.CheckCollisionLater(false));

    }
    
    /// <summary>
    /// Used for triggering Pizza delivery event
    /// </summary>
    /// <param name="customer"></param>
    public void GivePizza(Customer customer)
    {
        if (customer.CurrentlyWaitingForPizza)
            _eventManager.InvokeDeliverEvent(customer);
    }

    //-----------------

    /// <summary>
    /// Flicker effect on move
    /// player appears on desired cell and after a certain time disappears from old position.
    /// </summary>
    /// <param name="targetCell">Desired cell</param>
    private IEnumerator ShowHidePlayer(PositionCell targetCell)
    {
        PositionCell tempCell = PlayerCell;
        PlayerCell = targetCell;
        ShowPlayer();
        yield return new WaitForSecondsRealtime(_fickerEffectDuration);
        HidePlayer(tempCell);
        foreach (Car car in _gameManager.GameCarsManager.CurrentCars)
            if (car.CarPosition == tempCell)
                _gameManager.GameCarsManager.ShowCar(car.CarPosition, _gameManager.GameCarsManager.CarSprites[car.CarMotion]);
    }

    /// <summary>
    /// Makes Player visible on scene
    /// </summary>
    public void ShowPlayer()
    {
        PlayerCell.ShowMainSprite();
        PlayerCell.MyImage.enabled = true;
        PlayerCell.MyImage.preserveAspect = true;
    }
    
    /// <summary>
    /// Makes Player invisible on scene
    /// </summary>
    public void HidePlayer(PositionCell targetCell)
    {
        targetCell.MyImage.enabled = false;
        targetCell.MyImage.preserveAspect = false;
    }

    /// <summary>
    /// Makes Player invisible on scene
    /// </summary>
    public void HidePlayer()
    {
        PlayerCell.MyImage.enabled = false;
        PlayerCell.MyImage.preserveAspect = false;
    }

    /// <summary>
    /// Respawns Player after a certain time
    /// </summary>
    public void RespawnPlayer()
    {
        StartCoroutine(RespawningPlayer());
    }

    /// <summary>
    /// Respawns Player after a certain time
    /// </summary>
    private IEnumerator RespawningPlayer()
    {
        IsMoveable = false;
        PlayerCell = _gameManager.StartingPosition;
        yield return new WaitForSecondsRealtime(_gameManager.PauseDuration);
        ShowPlayer();
        IsMoveable = true;
    }

}
