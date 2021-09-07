using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    [SerializeField] private UnityEvent<CellDirection> MenuMovementEvent = new UnityEvent<CellDirection>();
    [SerializeField] private UnityEvent MenuSelectEvent = new UnityEvent();
    [SerializeField] private UnityEvent TickEvent = new UnityEvent();
    [SerializeField] private UnityEvent<CellDirection> PlayerMovementEvent = new UnityEvent<CellDirection>();
    [SerializeField] private UnityEvent<Customer> PlayerKnockEvent = new UnityEvent<Customer>();
    [SerializeField] private UnityEvent<Customer> DeliverEvent = new UnityEvent<Customer>();
    [SerializeField] private UnityEvent<Image> MissEvent = new UnityEvent<Image>();
    [SerializeField] private UnityEvent GameEndedEvent = new UnityEvent();
    [SerializeField] private bool _showEventFiredMessages = false;

    [SerializeField] private GameManager _gameManager;

    private void Start() {
        _gameManager = FindObjectOfType<GameManager>();
    }


    public void InvokeMenuMovementEvent(CellDirection direction)
    {
        if (_showEventFiredMessages)
            Debug.Log("MenuMovementEvent fired");
        MenuMovementEvent.Invoke(direction);
    }
    public void InvokeMenuSelectEvent()
    {
        if (_showEventFiredMessages)
            Debug.Log("MenuSelectEvent fired");
        MenuSelectEvent.Invoke();
    }
    public void InvokePlayerMovementEvent(CellDirection direction)
    {
        if (_showEventFiredMessages)
            Debug.Log("PlayerMovementEvent fired");
        PlayerMovementEvent.Invoke(direction);
    }
    public void InvokePlayerKnockEvent(Customer customer)
    {
        if (_showEventFiredMessages)
            Debug.Log("PlayerKnockEvent fired");
        PlayerKnockEvent.Invoke(customer);
    }
    public void InvokeTickEvent()
    {
        if (_showEventFiredMessages)
            Debug.Log("TickEvent fired");
        TickEvent.Invoke();
    }
    public void InvokeDeliverEvent(Customer customer)
    {
        if (_showEventFiredMessages)
            Debug.Log("DeliverEvent fired");
        DeliverEvent.Invoke(customer);
    }
    public void InvokeMissEvent(Image imageToFlicker)
    {
        if (_showEventFiredMessages)
            Debug.Log("MissEvent fired");
        MissEvent.Invoke(imageToFlicker);
    }
    public void InvokeGameEndedEvent()
    {
        if (_showEventFiredMessages)
            Debug.Log("GameEndedEvent fired");
        GameEndedEvent.Invoke();
    }

    //-------------------

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
