using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

[System.Serializable]
public class CellToCustomer : SerializableDictionaryBase<Transform, Customer> { }
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

    [HideInInspector]
    public EventManager GameEventManager;
    [HideInInspector]
    internal SpritesDict SpriteDatabase;
    [SerializeField] private CellToCustomer _allCustomers = new CellToCustomer();
    public CellToCustomer AllCustomers => _allCustomers;

    private void Start()
    {
        GameEventManager = FindObjectOfType<EventManager>();
        SpriteDatabase = FindObjectOfType<Database>().AllSpritesDict;
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
