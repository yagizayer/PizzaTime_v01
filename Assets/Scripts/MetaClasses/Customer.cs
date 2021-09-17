/// <summary>
/// This file is used for refering a customer.
/// </summary>
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(Image))]
public class Customer : MonoBehaviour
{
    public int CustomerNo = 0;
    [HideInInspector]
    public bool CurrentlyOpenedClosing = false;
    public PositionCell RelatedCell;
    public Image RelatedTimer;
    public SerializableDictionaryBase<Basic, AllSprites> MySprites = new SerializableDictionaryBase<Basic, AllSprites>();


    [HideInInspector]
    public float LastDeliveredTime;
    [HideInInspector]
    public bool CurrentlyWaitingForPizza = false;
    [HideInInspector]
    internal int RemainingTime = 8;


    private GameManager _gameManager;
    private EventManager _eventManager;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _eventManager = _gameManager.GameEventManager;
    }

    /// <summary>
    /// If customer is waiting for a deliver, this function proceeds its time each tick.
    /// </summary>
    public void ProceedTimer()
    {
        switch (--RemainingTime)
        {
            case 7:
                RelatedTimer.sprite = _gameManager.SpriteDatabase[AllSprites.Watch_1];
                break;
            case 6:
                RelatedTimer.sprite = _gameManager.SpriteDatabase[AllSprites.Watch_2];
                break;
            case 5:
                RelatedTimer.sprite = _gameManager.SpriteDatabase[AllSprites.Watch_3];
                break;
            case 4:
                RelatedTimer.sprite = _gameManager.SpriteDatabase[AllSprites.Watch_4];
                break;
            case 3:
                RelatedTimer.sprite = _gameManager.SpriteDatabase[AllSprites.Watch_5];
                break;
            case 2:
                RelatedTimer.sprite = _gameManager.SpriteDatabase[AllSprites.Watch_6];
                break;
            case 1:
                RelatedTimer.sprite = _gameManager.SpriteDatabase[AllSprites.Watch_7];
                break;
            case 0:
                RelatedTimer.sprite = _gameManager.SpriteDatabase[AllSprites.Watch_0];
                CurrentlyWaitingForPizza = false;
                RemainingTime = 8;
                _eventManager.InvokeMissEvent(RelatedTimer);
                break;
        }
    }

    /// <summary>
    /// Cancels current customers delivery and resets its timer.
    /// </summary>
    public void CancelDelivery()
    {
        CurrentlyWaitingForPizza = false;
        LastDeliveredTime = Time.time;
        RemainingTime = 8;
        HideTimer();
    }

    /// <summary>
    /// Make watch object visible on scene
    /// </summary>
    public void ShowTimer()
    {
        RelatedTimer.sprite = _gameManager.SpriteDatabase[AllSprites.Watch_1]; 
        RelatedTimer.enabled = true;
    }

    /// <summary>
    /// Make watch object invisible on scene
    /// </summary>
    public void HideTimer()
    {
        RelatedTimer.enabled = false;
    }
}
