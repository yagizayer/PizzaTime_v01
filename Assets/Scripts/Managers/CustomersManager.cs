using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomersManager : MonoBehaviour
{
    [SerializeField] private float Cooldown = 5;
    [SerializeField, Range(.01f, 100f)] private float _percentage = 5;

    private List<Customer> _allCustomers = new List<Customer>();
    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        foreach (KeyValuePair<Transform, Customer> customer in _gameManager.AllCustomers)
            _allCustomers.Add(customer.Value);

        InitializeCustomers();

    }

    private void InitializeCustomers()
    {
        foreach (Customer customer in _allCustomers)
        {
            customer.LastDeliveredTime = Time.time;
        }
    }

    private void SpawnDelivery(Customer customer)
    {
        customer.CurrentlyWaitingForPizza = true;
        customer.RelatedTimer.enabled = true;
    }

    public void SpawnDeliveryOnChance() // TODO : burda kaldın : timer image göster
    {
        foreach (Customer customer in _allCustomers)
        {
            if (customer.LastDeliveredTime + Cooldown > Time.time)
                continue;
            if (customer.CurrentlyWaitingForPizza)
                continue;
            System.Random r = new System.Random();
            if (r.Next(0, 100) < _percentage)
                SpawnDelivery(customer);
        }
    }

    public void ProceedCustomerTimers()
    {
        foreach (Customer customer in _allCustomers)
        {
            if(customer.CurrentlyWaitingForPizza){
                customer.ProceedTimer();
            }
        }
    }

}
