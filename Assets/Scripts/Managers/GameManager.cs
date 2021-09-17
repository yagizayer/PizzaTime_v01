using System.Collections;
using System.Collections.Generic;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

[System.Serializable]
public class CellToCustomer : SerializableDictionaryBase<Transform, Customer> { }
[System.Serializable]
public class CarSpawnerDict : SerializableDictionaryBase<Transform, CarMotions> { }
public class GameManager : MonoBehaviour
{
    [Header("Important Variables")]
    public GameMode Mode = GameMode.A;
    public GameState CurrentGameState = GameState.Started;
    [Tooltip("How many seconds should pass each game update")]
    [Range(.001f, 5)] public float TimeStepStartValue = 1;
    [Range(.01f, .1f)] public float DifficultyIncreasePerPoint = .05f;
    [Tooltip("Forgiveness for players late dodges (higher is easier to dodge)")]
    [Range(1, 100)] public int SpeedIncreaseThreshold = 10;
    [Range(1, 1000)] public int SpeedResetThreshold = 100;
    [Range(1, 1000)] public int HealthResetThreshold = 200;



    [Header("Map Variables")]
    public Map GameMap;



    [Header("Player Variables")]
    public PositionCell StartingPosition;



    [Header("Customer Variables")]
    [SerializeField] private CellToCustomer _allCustomers = new CellToCustomer();
    public CellToCustomer AllCustomers => _allCustomers;



    [Header("Car Variables")]
    public CarSpawnerDict CarSpawners = new CarSpawnerDict();
    [Range(1,100)] public int DoubleCarSpawnThreshold = 30;



    [Header("User Interface Variables")]
    public List<Image> HealthImages = new List<Image>();
    internal int StartingHealth = 3;
    [SerializeField] private Text _totalPointUI;
    public int TotalPoint = 0;



    [Header("Other Variables")]
    [SerializeField] private AnimationManager GameAnimationManager;
    [Range(.01f, 100f)] public float PauseDuration = 6;




    [HideInInspector]
    public PlayerManager GamePlayerManager;
    [HideInInspector]
    public EventManager GameEventManager;
    [HideInInspector]
    public CarsManager GameCarsManager;
    [HideInInspector]
    public TimeManager GameTimeManager;
    [HideInInspector]
    internal SpritesDict SpriteDatabase;



    internal float _timeStep;
    private int _difficultyMultiplier = 0;
    [HideInInspector] public int _currentHealth;




    private void Awake()
    {
        SpriteDatabase = FindObjectOfType<Database>().AllSpritesDict;
    }
    private void Start()
    {
        GameEventManager = FindObjectOfType<EventManager>();
        GameCarsManager = FindObjectOfType<CarsManager>();
        GamePlayerManager = FindObjectOfType<PlayerManager>();
        GameTimeManager = FindObjectOfType<TimeManager>();

        if (HealthImages.Count != StartingHealth)
        {
            Debug.LogError($"Health Images count({HealthImages.Count}) and Current Health value({StartingHealth}) is not matching. Using Health Images count for Current Health value({HealthImages.Count})");
            StartingHealth = HealthImages.Count;
        }
        if (CurrentGameState == GameState.Started)
            StartCoroutine(StartGameAfterDelay());

        StartCoroutine(CheckingForTimeUp());

        _timeStep = TimeStepStartValue;
        _currentHealth = StartingHealth;
    }
    private IEnumerator Tick()
    {
        while (CurrentGameState == GameState.Started)
        {
            yield return new WaitForSecondsRealtime(_timeStep);
            GameEventManager.InvokeTickEvent();
        }
    }
    public void EndGame()
    {
        CurrentGameState = GameState.Ended;
        StopCoroutine(Tick());
        GameCarsManager.StopCarSpawning();
        StartCoroutine(EndingGame());
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

    public IEnumerator CheckCollisionLater(bool lookForNextCell)
    {
        yield return new WaitForSecondsRealtime(0);
        if (lookForNextCell)
        {
            foreach (Car car in GameCarsManager.CurrentCars)
                if (car.CarPosition.GetNeighbor(car.CarMotion.toCellDirection()) == GamePlayerManager.PlayerCell)
                    GameEventManager.InvokeMissEvent(car.CarPosition.MyImage);
        }
        else
        {
            foreach (Car car in GameCarsManager.CurrentCars)
                if (car.CarPosition == GamePlayerManager.PlayerCell)
                    GameEventManager.InvokeMissEvent(car.CarPosition.MyImage);

        }
    }

    public void ReduceHealth()
    {
        if (_currentHealth == 0) return;
        --_currentHealth;
    }
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void IncreasePoint()
    {
        TotalPoint++;
        string point = "";
        if (TotalPoint < 9) point += "0";
        if (TotalPoint < 99) point += "0";
        point += TotalPoint.ToString();
        _totalPointUI.text = point;
    }
    public void IncreaseDifficulty()
    {
        if (TotalPoint % SpeedIncreaseThreshold == 0) _difficultyMultiplier++;
        if (TotalPoint % HealthResetThreshold == 0)
            GameAnimationManager.ResetHealth();
        if (TotalPoint % SpeedResetThreshold == 0)
            _difficultyMultiplier = 0;
        _timeStep = TimeStepStartValue - _difficultyMultiplier * DifficultyIncreasePerPoint;
    }
    public void PauseGame(float duration)
    {
        StartCoroutine(PausingGame(duration));
    }
    public void PauseGame()
    {
        if (_currentHealth == 0)
        {
            GameEventManager.InvokeGameEndedEvent();
        }
        else
        {
            StartCoroutine(PausingGame());
            GamePlayerManager.RespawnPlayer();
        }
    }
    private IEnumerator PausingGame(float duration)
    {
        StopGame();
        yield return new WaitForSecondsRealtime(duration);
        StartGame();
    }
    private IEnumerator PausingGame()
    {
        StopGame();
        yield return new WaitForSecondsRealtime(PauseDuration);
        StartGame();
    }
    private IEnumerator EndingGame()
    {
        yield return new WaitForSecondsRealtime(PauseDuration);
        ChangeScene("MainMenu");
    }
    private IEnumerator CheckingForTimeUp()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(1);
            float diff = GameTimeManager.CalculateAlarmDiff(TimeKeeper.SetTime, TimeKeeper.SetAlarm);
            if (diff == 0)
                GameEventManager.InvokeTimesUpEvent();
        }
    }
    public float TimestepPercentage() => (_timeStep / TimeStepStartValue);

}
