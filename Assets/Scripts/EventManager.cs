using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    [SerializeField] private UnityEvent<CellDirection> PlayerInputEvent = new UnityEvent<CellDirection>();


    public void InvokePlayerInputEvent(CellDirection direction)
    {
        PlayerInputEvent.Invoke(direction);
    }

    public void TestFunc(CellDirection message)
    {
        Debug.Log(message);
    }
}
