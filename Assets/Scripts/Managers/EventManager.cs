/// <summary>
/// This files is used for Events Firing base
/// </summary>
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    [SerializeField] private UnityEvent TickEvent = new UnityEvent();
    [SerializeField] private UnityEvent<CellDirection> PlayerMovementEvent = new UnityEvent<CellDirection>();
    [SerializeField] private UnityEvent<Customer> PlayerKnockEvent = new UnityEvent<Customer>();
    [SerializeField] private UnityEvent<Customer> DeliverEvent = new UnityEvent<Customer>();
    [SerializeField] private UnityEvent<Image> MissEvent = new UnityEvent<Image>();
    [SerializeField] private UnityEvent TimesUpEvent = new UnityEvent();
    [SerializeField] private UnityEvent GameEndedEvent = new UnityEvent();
    [SerializeField] private bool _showEventFiredMessages = false;

    [SerializeField] private GameManager _gameManager;

    private void Start() {
        _gameManager = FindObjectOfType<GameManager>();
    }

    /// <summary>
    /// Invokes Added Events When player Wants to move
    /// </summary>
    /// <param name="direction">Desired Movement direction</param>
    public void InvokePlayerMovementEvent(CellDirection direction)
    {
        if (_showEventFiredMessages)
            Debug.Log("PlayerMovementEvent fired");
        PlayerMovementEvent.Invoke(direction);
    }
    
    /// <summary>
    /// Invokes Added Events When player Wants to deliver pizza
    /// </summary>
    /// <param name="customer">Delivering target</param>
    public void InvokePlayerKnockEvent(Customer customer)
    {
        if (_showEventFiredMessages)
            Debug.Log("PlayerKnockEvent fired");
        PlayerKnockEvent.Invoke(customer);
    }

    /// <summary>
    /// Invokes Added Events for Every tick 
    /// </summary>
    public void InvokeTickEvent()
    {
        if (_showEventFiredMessages)
            Debug.Log("TickEvent fired");
        TickEvent.Invoke();
    }
    /// <summary>
    /// Invokes Added Events for Successfully delivering a pizza
    /// </summary>
    /// <param name="customer">Delivery target</param>
    public void InvokeDeliverEvent(Customer customer)
    {
        if (_showEventFiredMessages)
            Debug.Log("DeliverEvent fired");
        DeliverEvent.Invoke(customer);
    }

    /// <summary>
    /// Invokes Added Events for Player Missing a deliver or colliding with a car
    /// </summary>
    /// <param name="imageToFlicker">Car or Timer of Delivery</param>
    public void InvokeMissEvent(Image imageToFlicker)
    {
        if (_showEventFiredMessages)
            Debug.Log("MissEvent fired");
        MissEvent.Invoke(imageToFlicker);
    }

    /// <summary>
    /// Invokes Added Events for Time's up event
    /// </summary>
    public void InvokeTimesUpEvent()
    {
        if (_showEventFiredMessages)
            Debug.Log("TimesUpEvent fired");
        TimesUpEvent.Invoke();
    }

    /// <summary>
    /// Invokes Added Events for Game Ended event
    /// </summary>
    public void InvokeGameEndedEvent()
    {
        if (_showEventFiredMessages)
            Debug.Log("GameEndedEvent fired");
        GameEndedEvent.Invoke();
    }

    //-------------------

    /// <summary>
    /// Invokes Added Events for Player Movement event(for Nintendo UI Controls)
    /// </summary>
    /// <param name="wasd">Selected Key(one of WASD)</param>
    public void InvokePlayerMovementEvent(string wasd)
    {
        CellDirection direction = CellDirection.Null;
        if (wasd == "W") direction = CellDirection.Next;
        if (wasd == "S") direction = CellDirection.Previous;
        if (wasd == "A") direction = CellDirection.Left;
        if (wasd == "D") direction = CellDirection.Right;

        if (_showEventFiredMessages)
            Debug.Log("PlayerMovementEvent fired");
        PlayerMovementEvent.Invoke(direction);
    }

    /// <summary>
    /// Invokes Added Events for Knock event(for Nintendo UI Controls)
    /// </summary>
    /// <param name="knock"></param>
    public void InvokePlayerKnockEvent(bool knock)
    {
        Customer customer = null;
        if (!_gameManager.AllCustomers.Keys.Contains(_gameManager.GamePlayerManager.PlayerCell.transform))
            return;// there is no customer near player
        if (_gameManager.AllCustomers[_gameManager.GamePlayerManager.PlayerCell.transform].CurrentlyOpenedClosing)
            return;// customer is not avaliable for interaction

        customer = _gameManager.AllCustomers[_gameManager.GamePlayerManager.PlayerCell.transform];

        if (_showEventFiredMessages)
            Debug.Log("PlayerKnockEvent fired");
        PlayerKnockEvent.Invoke(customer);
    }











    public void TestFunc(string message)
    {
        Debug.Log(message);
    }
}
