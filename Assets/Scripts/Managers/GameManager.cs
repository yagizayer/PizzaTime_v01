using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class CellToCustomer : SerializableDictionaryBase<Transform, Customer> { }
[System.Serializable]
public class CarSpawnerDict : SerializableDictionaryBase<Transform, CarMotions> { }
public class GameManager : MonoBehaviour
{
    [Header("Important Variables")]
    [SerializeField] private Sprite CarSpriteFront;
    [SerializeField] private Sprite CarSpriteSide;
    [Tooltip("How many seconds should pass each game update")]
    [Range(.001f, 5)] public float TimeStep = 1;
    [Header("Map Variables")]
    public Map GameMap;
    [Header("Player Variables")]
    public PositionCell StartingPosition;
    public PlayerManager GamePlayerManager;
    [Header("Customer Variables")]
    [SerializeField] private CellToCustomer _allCustomers = new CellToCustomer();
    public CellToCustomer AllCustomers => _allCustomers;
    [Header("Car Variables")]
    public CarSpawnerDict CarSpawners = new CarSpawnerDict();

    [Header("User Interface Variables")]
    [SerializeField] private List<Image> _healthImages = new List<Image>();
    [SerializeField] private int _currentHealth = 3;


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



        StartCoroutine(Tick());
    }
    private IEnumerator Tick()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(TimeStep);
            GameEventManager.InvokeTickEvent();
        }
    }

    //-------------

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
    private void RespawnPlayer()
    {
        StartCoroutine(RespawningPlayer());
    }
    private IEnumerator RespawningPlayer()
    {
        GamePlayerManager.IsMoveable = false;
        yield return new WaitForSecondsRealtime(1.5f);
        GamePlayerManager.PlayerCell = StartingPosition;
        GamePlayerManager.ShowPlayer();
        GamePlayerManager.IsMoveable = true;
    }
    public void ReduceHealth()
    {
        --_currentHealth;

        GamePlayerManager.PlayerCell = StartingPosition;
        GamePlayerManager.IsMoveable = false;

        if (_currentHealth == 0)
        {
            GameEventManager.InvokeGameEndedEvent();
            return;
        }

        RespawnPlayer();
    }
    public void AnimateHealthReduce()
    {
        Image imageToAnimate = _healthImages[_currentHealth];
        imageToAnimate.GetComponent<Animator>().enabled = true;
    }
    public void ReturnMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
