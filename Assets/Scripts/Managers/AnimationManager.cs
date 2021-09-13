using System.Collections;
using System.Collections.Generic;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine.UI;
using UnityEngine;


public class AnimationManager : MonoBehaviour
{
    private GameManager _gameManager;
    private EventManager _eventManager;
    [SerializeField] private Animator AngryBoss;
    [SerializeField] private Animator Watch;


    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _eventManager = _gameManager.GameEventManager;
    }

    public void AnimateHealthReduce()
    {
        Image imageToAnimate = _gameManager.HealthImages[_gameManager._currentHealth];
        imageToAnimate.GetComponent<Animator>().enabled = true;
        imageToAnimate.GetComponent<Animator>().Play("HealthReduce");
    }

    public void AnimateTimesUp()
    {
        Watch.SetTrigger("Animate");
    }
    public void ResetHealth()
    {
        _gameManager._currentHealth = _gameManager.StartingHealth;
        foreach (Image item in _gameManager.HealthImages)
        {
            item.enabled = false;
            item.GetComponent<Animator>().enabled = false;
        }
    }

    public void AnimateAngryManager()
    {
        StartCoroutine(AnimatingAngryManager());
    }
    private IEnumerator AnimatingAngryManager()
    {
        yield return new WaitForSecondsRealtime(_gameManager.PauseDuration / 2);
        AngryBoss.SetTrigger("Animate");
    }

    public void AnimateMissFlickerEffect(Image targetGraphic)
    {
        Animator targetAnimator = targetGraphic.GetComponent<Animator>();
        targetAnimator.enabled = true;
        targetGraphic.preserveAspect = true;
        targetAnimator.SetTrigger("Animate");
        StartCoroutine(CloseAnimator(targetAnimator));
    }
    private IEnumerator CloseAnimator(Animator targetAnimator)
    {
        yield return new WaitForSecondsRealtime(3);
        targetAnimator.enabled = false;
    }

    public void AnimateTakingPizza(Customer customer)
    {
        if (customer.CurrentlyOpenedClosing == false)
            StartCoroutine(AnimatingTakingPizza(customer));
    }
    private IEnumerator AnimatingTakingPizza(Customer customer)
    {
        customer.CurrentlyOpenedClosing = true;
        AllSprites customerSpriteKey_On = customer.MySprites[Basic.On];
        AllSprites customerSpriteKey_Off = customer.MySprites[Basic.Off];
        Image customerImage = customer.GetComponent<Image>();

        customerImage.sprite = _gameManager.SpriteDatabase[customerSpriteKey_On];


        yield return new WaitForSecondsRealtime(1);


        customerImage.sprite = _gameManager.SpriteDatabase[customerSpriteKey_Off];
        customer.CurrentlyOpenedClosing = false;
    }

}
