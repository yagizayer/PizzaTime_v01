using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Important Variables")]
    public Sprite PlayerSprite;
    [SerializeField] private Sprite CarSpriteFront;
    [SerializeField] private Sprite CarSpriteSide;
    [Tooltip("How many seconds should pass each game update")]
    [SerializeField, Range(.001f, 5)] private float TimeStep = 1;
    [Header("Map Variables")]
    public Map GameMap;
    [Header("Player Variables")]
    public PositionCell StartingPosition;
    public PlayerManager Player;

    [HideInInspector]
    public EventManager GameEventManager;

    private void Start()
    {
        GameEventManager = FindObjectOfType<EventManager>();
    }
}
