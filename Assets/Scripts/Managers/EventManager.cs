using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    [SerializeField] private UnityEvent TickEvent = new UnityEvent();
    [SerializeField] private UnityEvent<CellDirection> PlayerMovementEvent = new UnityEvent<CellDirection>();
    [SerializeField] private UnityEvent<Customer> PlayerKnockEvent = new UnityEvent<Customer>();
    [SerializeField] private UnityEvent MissEvent = new UnityEvent();


    public void InvokePlayerMovementEvent(CellDirection direction)
    {
        PlayerMovementEvent.Invoke(direction);
    }
    public void InvokePlayerKnockEvent(Customer customer)
    {
        PlayerKnockEvent.Invoke(customer);
    }
    public void InvokeTickEvent()
    {
        TickEvent.Invoke();
    }
    public void InvokeMissEvent()
    {
        MissEvent.Invoke();
    }



    public void TestFunc(string message)
    {
        Debug.Log(message);
    }
}
