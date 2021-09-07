using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(Image))]
public class TrafficLightManager : MonoBehaviour
{
    public float Cooldown = 5;
    [SerializeField, Range(.01f, 100f)] private float _percentage = 20;
    [HideInInspector]
    public Basic CurrentState = Basic.Null;
    private GameManager _gameManager;
    private EventManager _eventManager;
    private Image _myImage;
    private CarsManager _carsManager;
    private float _lastChangedTime;
    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _eventManager = _gameManager.GameEventManager;
        _carsManager = _gameManager.GameCarsManager;
        _myImage = GetComponent<Image>();

        if (_gameManager.Mode == GameMode.A)
            _percentage = 0;
        _myImage.sprite = _gameManager.SpriteDatabase[AllSprites.TrafficLightGo];
    }

    public void ChangeDirection()
    {
        if (CurrentState == Basic.On)
        {
            CurrentState = Basic.Off;
            _myImage.sprite = _gameManager.SpriteDatabase[AllSprites.TrafficLightGo];
        }
        else
        {
            CurrentState = Basic.On;
            _myImage.sprite = _gameManager.SpriteDatabase[AllSprites.TrafficLightStop];
        }
        _carsManager.ChangeDirection();
        _lastChangedTime = Time.time;
    }
    public void ChangeDirectionOnChance()
    {
        // on cooldown
        if (_lastChangedTime + Cooldown > Time.time)
            return;
        System.Random r = new System.Random();
        if (r.Next(0, 100) < _percentage)
            ChangeDirection();
    }
}
