using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

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
    [SerializeField, Range(.001f, 5)] private float TimeStep = 1;
    [Header("Map Variables")]
    public Map GameMap;
    [Header("Player Variables")]
    public PositionCell StartingPosition;
    public PlayerManager Player;
    [Header("Customer Variables")]
    [SerializeField] private CellToCustomer _allCustomers = new CellToCustomer();
    public CellToCustomer AllCustomers => _allCustomers;
    [Header("Car Variables")]
    public CarSpawnerDict CarSpawners = new CarSpawnerDict();



    [HideInInspector]
    public EventManager GameEventManager;
    [HideInInspector]
    internal SpritesDict SpriteDatabase;
    private void Start()
    {
        GameEventManager = FindObjectOfType<EventManager>();
        SpriteDatabase = FindObjectOfType<Database>().AllSpritesDict;
        StartCoroutine(Tick());
    }

    private IEnumerator Tick()
    {
        while (true)
        {
            GameEventManager.InvokeTickEvent();
            yield return new WaitForSecondsRealtime(TimeStep);
        }
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

}
