using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomersManager : MonoBehaviour
{
    [SerializeField] private float Cooldown = 3;
    [SerializeField, Range(.01f, 100f)] private float _percentage = 5;

    private List<Customer> _allCustomers = new List<Customer>();
    private GameManager _gameManager;
    private EventManager _eventManager;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _eventManager = _gameManager.GameEventManager;
        foreach (KeyValuePair<Transform, Customer> customer in _gameManager.AllCustomers)
            _allCustomers.Add(customer.Value);

        InitializeCustomers();

    }

    //---------------

    public void SpawnDeliveryOnChance()
    {
        foreach (Customer customer in _allCustomers)
        {
            if (customer.LastDeliveredTime + Cooldown > Time.time)
                continue;
            if (customer.CurrentlyWaitingForPizza)
                continue;

            if (UnityEngine.Random.Range(0, 100) < _percentage)
                SpawnDelivery(customer);
        }
    }
    public void ProceedCustomerTimers()
    {
        if (_gameManager.CurrentGameState != GameState.Started)
            return;
        foreach (Customer customer in _allCustomers)
        {
            if (customer.CurrentlyWaitingForPizza)
            {
                customer.ProceedTimer();
            }
        }
    }
    public void PizzaReceived(Customer customer)
    {
        customer.CurrentlyWaitingForPizza = false;
        customer.LastDeliveredTime = Time.time;
        customer.RemainingTime = 8;
        customer.HideTimer();
    }
    public void ClearAllDeliveries()
    {
        StartCoroutine(ClearingDeliveries());
    }
    private IEnumerator ClearingDeliveries()
    {
        yield return new WaitForSecondsRealtime(3);
        foreach (Customer customer in _allCustomers)
            customer.CancelDelivery();
    }


    //---------------

    private void InitializeCustomers()
    {
        foreach (Customer customer in _allCustomers)
            customer.LastDeliveredTime = Time.time;
    }
    private void SpawnDelivery(Customer customer)
    {
        customer.CurrentlyWaitingForPizza = true;
        customer.ShowTimer();
    }


}
