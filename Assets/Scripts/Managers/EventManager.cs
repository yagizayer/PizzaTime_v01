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



    public void TestFunc(string message)
    {
        Debug.Log(message);
    }
}
