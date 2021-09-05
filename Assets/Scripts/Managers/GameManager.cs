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
    public GameState CurrentGameState = GameState.Started;
    [Header("Map Variables")]
    public Map GameMap;
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
    public List<Image> HealthImages = new List<Image>();
    internal int CurrentHealth = 3;
    [SerializeField] private Text _totalPointUI;
    [SerializeField] private int _totalPoint = 0;

    [Header("Other Variables")]
    [SerializeField] private AnimationManager GameAnimationManager;

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

        if (HealthImages.Count != CurrentHealth)
        {
            Debug.LogError($"Health Images count({HealthImages.Count}) and Current Health value({CurrentHealth}) is not matching. Using Health Images count for Current Health value({HealthImages.Count})");
            CurrentHealth = HealthImages.Count;
        }
        if (CurrentGameState == GameState.Started)
            StartCoroutine(StartGameAfterDelay());
    }
    private IEnumerator Tick()
    {
        while (CurrentGameState == GameState.Started)
        {
            yield return new WaitForSecondsRealtime(TimeStep);
            GameEventManager.InvokeTickEvent();
        }
    }
    public void EndGame()
    {
        CurrentGameState = GameState.Ended;
        StopCoroutine(Tick());
        GameCarsManager.StopCarSpawning();
    }
    public void StopGame()
    {
        CurrentGameState = GameState.Paused;
        StopCoroutine(Tick());
        GameCarsManager.StopCarSpawning();
    }
    public void StartGame()
    {
        CurrentGameState = GameState.Started;
        StartCoroutine(Tick());
        GameCarsManager.StartCarSpawning();
    }

    private IEnumerator StartGameAfterDelay()
    {
        yield return new WaitForSecondsRealtime(.2f);
        StartGame();
    }
    //-------------

    public IEnumerator CheckCollisionLater()
    {
        yield return new WaitForSecondsRealtime(_collisionDetectionDelay);
        foreach (Car car in GameCarsManager.CurrentCars)
            if (car.CarPosition == GamePlayerManager.PlayerCell)
                GameEventManager.InvokeMissEvent(car.CarPosition.MyImage);
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

        if (CurrentHealth == 0) return;

        --CurrentHealth;
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
    public void PauseGameForThreeSeconds()
    {
        if (CurrentHealth == 0)
        {
            GameEventManager.InvokeGameEndedEvent();
            return;
        }
        else
        {
            StartCoroutine(PausingGame());
        }
    }
    private IEnumerator PausingGame()
    {
        StopGame();
        yield return new WaitForSecondsRealtime(3);
        StartGame();
    }

}
