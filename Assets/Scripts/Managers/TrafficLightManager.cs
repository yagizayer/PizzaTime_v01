using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(Image))]
public class TrafficLightManager : MonoBehaviour
{
    public float Cooldown = 5;
    [HideInInspector]
    public Basic CurrentState = Basic.Null;
    private GameManager _gameManager;
    private EventManager _eventManager;
    private Image _myImage;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _eventManager = _gameManager.GameEventManager;
        _myImage = GetComponent<Image>();
    }

    public void ChangeDirection()
    {
        if (CurrentState == Basic.On)
        {
            CurrentState = Basic.Off;
            _myImage.sprite = _gameManager.SpriteDatabase[AllSprites.TrafficLightStop];
        }
        else
        {
            CurrentState = Basic.On;
            _myImage.sprite = _gameManager.SpriteDatabase[AllSprites.TrafficLightGo];
        }
    }
}
