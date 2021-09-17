/// <summary>
/// This file is used for Managing Customers
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomersManager : MonoBehaviour
{
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

    /// <summary>
    /// Spawn a Delivery on a random avaliable customer
    /// </summary>
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

    /// <summary>
    /// Spawns Delivery if there is no other delivery waiting
    /// </summary>
    public void SpawnDeliveryOnChance()
    {
        bool noOtherDeliveries = true;
        foreach (Customer customer in _allCustomers)
            if (customer.CurrentlyWaitingForPizza) noOtherDeliveries = false;

        if (noOtherDeliveries)
            SpawnDelivery();
    }

    /// <summary>
    /// Sets las delivered customer (to avoid spawning same customer over and over again)
    /// </summary>
    /// <param name="customer">last customer</param>
    public void SetLastDeliveredCustomer(Customer customer)
    {
        _lastDeliveredCustomer = customer;
    }

    /// <summary>
    /// Proceed Customers Delivery times
    /// </summary>
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

    /// <summary>
    /// Receive Delivery and close timer/clear variables
    /// </summary>
    /// <param name="customer"></param>
    public void PizzaReceived(Customer customer)
    {
        customer.CurrentlyWaitingForPizza = false;
        customer.LastDeliveredTime = Time.time;
        customer.RemainingTime = 8;
        customer.HideTimer();
    }

    /// <summary>
    /// Clears all active deliveries (after a miss event)
    /// </summary>
    public void ClearAllDeliveries()
    {
        StartCoroutine(ClearingDeliveries());
    }
    
    /// <summary>
    /// Clears all active deliveries (after a miss event)
    /// </summary>
    private IEnumerator ClearingDeliveries()
    {
        yield return new WaitForSecondsRealtime(_gameManager.PauseDuration / 2);
        foreach (Customer customer in _allCustomers)
            customer.CancelDelivery();
    }


    //---------------

    /// <summary>
    /// Initialize all customers for game start
    /// </summary>
    private void InitializeCustomers()
    {
        foreach (Customer customer in _allCustomers)
            customer.LastDeliveredTime = Time.time;
    }

    /// <summary>
    /// Spawn a specific customers delivery 
    /// </summary>
    /// <param name="customer"></param>
    private void SpawnDelivery(Customer customer)
    {
        customer.CurrentlyWaitingForPizza = true;
        customer.ShowTimer();
    }

    //---------------

    /// <summary>
    /// Increase Timer count related to score
    /// </summary>
    /// <returns>timer count (up to "customer count-1")</returns>
    private int DecideCurrentTimerCount()
    {
        int result = 1;
        if (_gameManager.TotalPoint % TimerCountThresholds[3] > TimerCountThresholds[1]) result = 2;
        if (_gameManager.TotalPoint % TimerCountThresholds[3] > TimerCountThresholds[2]) result = 3;
        return result;
    }
    
    /// <summary>
    /// Returns an non-recurring random list of integers
    /// </summary>
    /// <param name="objectPool">selectable integers</param>
    /// <param name="desiredListCount">desired return list size</param>
    /// <returns>Random non-recurring integer list</returns>
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
