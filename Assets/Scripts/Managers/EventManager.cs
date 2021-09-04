using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    [SerializeField] private UnityEvent<CellDirection> MenuMovementEvent = new UnityEvent<CellDirection>();
    [SerializeField] private UnityEvent MenuSelectEvent = new UnityEvent();
    [SerializeField] private UnityEvent TickEvent = new UnityEvent();
    [SerializeField] private UnityEvent<CellDirection> PlayerMovementEvent = new UnityEvent<CellDirection>();
    [SerializeField] private UnityEvent<Customer> PlayerKnockEvent = new UnityEvent<Customer>();
    [SerializeField] private UnityEvent<Customer> DeliverEvent = new UnityEvent<Customer>();
    [SerializeField] private UnityEvent MissEvent = new UnityEvent();
    [SerializeField] private UnityEvent GameEndedEvent = new UnityEvent();


    public void InvokeMenuMovementEvent(CellDirection direction)
    {
        Debug.Log("MenuMovementEvent fired");
        MenuMovementEvent.Invoke(direction);
    }
    public void InvokeMenuSelectEvent()
    {
        Debug.Log("MenuSelectEvent fired");
        MenuSelectEvent.Invoke();
    }
    public void InvokePlayerMovementEvent(CellDirection direction)
    {
        Debug.Log("PlayerMovementEvent fired");
        PlayerMovementEvent.Invoke(direction);
    }
    public void InvokePlayerKnockEvent(Customer customer)
    {
        Debug.Log("PlayerKnockEvent fired");
        PlayerKnockEvent.Invoke(customer);
    }
    public void InvokeTickEvent()
    {
        Debug.Log("TickEvent fired");
        TickEvent.Invoke();
    }
    public void InvokeDeliverEvent(Customer customer)
    {
        Debug.Log("DeliverEvent fired");
        DeliverEvent.Invoke(customer);
    }
    public void InvokeMissEvent()
    {
        Debug.Log("MissEvent fired");
        MissEvent.Invoke();
    }
    public void InvokeGameEndedEvent()
    {
        Debug.Log("GameEndedEvent fired");
        GameEndedEvent.Invoke();
    }



    public void TestFunc(string message)
    {
        Debug.Log(message);
    }
}
