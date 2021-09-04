using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

[System.Serializable]
public class CellToCustomer : SerializableDictionaryBase<Transform, Customer> { }
[System.Serializable]
public class CarSpawnerDict : SerializableDictionaryBase<Transform, CarMotions> { }
public class GameManager : MonoBehaviour
{
    [Header("Important Variables")]
    public GameMode Mode = GameMode.A;
    [Tooltip("How many seconds should pass each game update")]
    [Range(.001f, 5)] public float TimeStep = 1;
    [Range(.001f, .01f)] public float DifficultyIncreasePerPoint = .05f;
    [Tooltip("Forgiveness for players late dodges (higher is easier to dodge)")]
    [SerializeField, Range(.01f, 5f)] private float _collisionDetectionDelay = .1f;
    public bool IsGameRunning = true;
    [Header("Map Variables")]
    public Map GameMap;
    [SerializeField] private Animator AngryBoss;
    [Header("Player Variables")]
    public PositionCell StartingPosition;
    [HideInInspector]
    public PlayerManager GamePlayerManager;
    [Header("Customer Variables")]
    [SerializeField] private CellToCustomer _allCustomers = new CellToCustomer();
    public CellToCustomer AllCustomers => _allCustomers;
    [Header("Car Variables")]
    public CarSpawnerDict CarSpawners = new CarSpawnerDict();

    [Header("User Interface Variables")]
    [SerializeField] private List<Image> _healthImages = new List<Image>();
    [SerializeField] private int _currentHealth = 3;
    [SerializeField] private Text _totalPointUI;
    [SerializeField] private int _totalPoint = 0;


    [HideInInspector]
    public EventManager GameEventManager;
    [HideInInspector]
    public CarsManager GameCarsManager;
    [HideInInspector]
    internal SpritesDict SpriteDatabase;

    private void Awake()
    {
        SpriteDatabase = FindObjectOfType<Database>().AllSpritesDict;
    }
    private void Start()
    {
        GameEventManager = FindObjectOfType<EventManager>();
        GameCarsManager = FindObjectOfType<CarsManager>();
        GamePlayerManager = FindObjectOfType<PlayerManager>();

        if (_healthImages.Count != _currentHealth)
        {
            Debug.LogError($"Health Images count({_healthImages.Count}) and Current Health value({_currentHealth}) is not matching. Using Health Images count for Current Health value({_healthImages.Count})");
            _currentHealth = _healthImages.Count;
        }
        if (IsGameRunning)
            StartGame();
    }
    private IEnumerator Tick()
    {
        while (IsGameRunning)
        {
            yield return new WaitForSecondsRealtime(TimeStep);
            GameEventManager.InvokeTickEvent();
        }
    }
    public void StopGame()
    {
        IsGameRunning = false;
        StopCoroutine(Tick());
    }
    public void StartGame()
    {
        IsGameRunning = true;
        StartCoroutine(Tick());
    }

    //-------------

    public IEnumerator CheckCollisionLater()
    {
        yield return new WaitForSecondsRealtime(_collisionDetectionDelay);
        foreach (Car car in GameCarsManager.CurrentCars)
            if (car.CarPosition == GamePlayerManager.PlayerCell)
                GameEventManager.InvokeMissEvent();
    }
    public void TakePizza(Customer customer)
    {
        if (customer.CurrentlyOpenedClosing == false) // not open
            StartCoroutine(TakingPizza(customer));
    }
    private IEnumerator TakingPizza(Customer customer)
    {
        customer.CurrentlyOpenedClosing = true;
        AllSprites customerSpriteKey_On = customer.MySprites[Basic.On];
        AllSprites customerSpriteKey_Off = customer.MySprites[Basic.Off];
        Image customerImage = customer.GetComponent<Image>();

        customerImage.sprite = SpriteDatabase[customerSpriteKey_On];


        yield return new WaitForSecondsRealtime(1);


        customerImage.sprite = SpriteDatabase[customerSpriteKey_Off];
        customer.CurrentlyOpenedClosing = false;
    }
    public void ReduceHealth()
    {

        if (_currentHealth == 0) return;

        --_currentHealth;


        if (_currentHealth == 0)
        {
            GameEventManager.InvokeGameEndedEvent();
            return;
        }
    }
    public void AnimateHealthReduce()
    {
        Image imageToAnimate = _healthImages[_currentHealth];
        imageToAnimate.GetComponent<Animator>().enabled = true;
    }
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void IncreasePoint()
    {
        _totalPoint++;
        string point = "";
        if (_totalPoint < 9) point += "0";
        if (_totalPoint < 99) point += "0";
        point += _totalPoint.ToString();
        _totalPointUI.text = point;
    }
    public void IncreaseDifficulty()
    {
        if (TimeStep >= .1f)
            TimeStep -= DifficultyIncreasePerPoint;
    }
    public void PauseGame()
    {
        StartCoroutine(PausingGame());
    }
    private IEnumerator PausingGame()
    {
        StopGame();
        yield return new WaitForSecondsRealtime(3);
        StartGame();
    }

    public void AnimateAngryManager()
    {
        StartCoroutine(AnimatingAngryManager());
    }

    private IEnumerator AnimatingAngryManager()
    {
        yield return null;
    }
}
