using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomersManager : MonoBehaviour
{
    [SerializeField] private float Cooldown = 3;
    [SerializeField, Range(.01f, 100f)] private float _percentage = 5;
    [SerializeField] private int[] TimerCountThresholds = new int[4] { 0, 20, 50, 100 };
    private List<Customer> _allCustomers = new List<Customer>();
    private GameManager _gameManager;
    private EventManager _eventManager;
    private Customer _lastDeliveredCustomer;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _eventManager = _gameManager.GameEventManager;
        foreach (KeyValuePair<Transform, Customer> customer in _gameManager.AllCustomers)
            _allCustomers.Add(customer.Value);

        InitializeCustomers();
        _lastDeliveredCustomer = _allCustomers[0];
    }

    //---------------

    public void SpawnDelivery()
    {
        List<int> indexes = new List<int>();
        for (int i = 0; i < _allCustomers.Count; i++)
            indexes.Add(i);
        List<int> randomIndexes = GetUniqueRandomList(indexes, DecideCurrentTimerCount());
        for (int i = 0; i < randomIndexes.Count; i++)
        {
            Customer customer = _allCustomers[randomIndexes[i]];
            SpawnDelivery(customer);
        }
    }
    public void SpawnDeliveryOnChance()
    {
        bool noOtherDeliveries = true;
        foreach (Customer customer in _allCustomers)
            if (customer.CurrentlyWaitingForPizza) noOtherDeliveries = false;

        if (noOtherDeliveries)
            SpawnDelivery();
    }

    public void SetLastDeliveredCustomer(Customer customer)
    {
        _lastDeliveredCustomer = customer;
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
        yield return new WaitForSecondsRealtime(_gameManager.PauseDuration / 2);
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

    //---------------
    private int DecideCurrentTimerCount()
    {
        int result = 1;
        if (_gameManager.TotalPoint % TimerCountThresholds[3] > TimerCountThresholds[1]) result = 2;
        if (_gameManager.TotalPoint % TimerCountThresholds[3] > TimerCountThresholds[2]) result = 3;
        return result;
    }
    private List<int> GetUniqueRandomList(List<int> objectPool, int desiredListCount)
    {
        List<int> excludedIndexes = new List<int>();
        excludedIndexes.Add(_allCustomers.IndexOf(_lastDeliveredCustomer));
        List<int> result = new List<int>();
        System.Random r = new System.Random();
        int retryCount = 0;
        while (true)
        {
            if (retryCount < 500) retryCount++;
            else
            {
                Debug.LogWarning("Size error");
                return new List<int>();
            }
            if (result.Count == desiredListCount) return result;

            int randomIndex = r.Next(0, objectPool.Count);
            if (excludedIndexes.Contains(randomIndex)) continue;

            result.Add(objectPool[randomIndex]);
            excludedIndexes.Add(randomIndex);
        }
    }
}
