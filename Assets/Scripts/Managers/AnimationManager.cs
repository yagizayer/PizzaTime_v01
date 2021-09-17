/// <summary>
/// This file is used for most of the animations
/// </summary>
using System.Collections;
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

    /// <summary>
    /// Animate Health Images for reducing health
    /// </summary>
    public void AnimateHealthReduce()
    {
        Image imageToAnimate = _gameManager.HealthImages[_gameManager._currentHealth];
        imageToAnimate.GetComponent<Animator>().enabled = true;
        imageToAnimate.GetComponent<Animator>().Play("HealthReduce");
    }

    /// <summary>
    /// Animate Time Images for Time's up
    /// </summary>
    public void AnimateTimesUp()
    {
        Watch.SetTrigger("Animate");
    }
    
    /// <summary>
    /// Resetting all health images (on 200 score)
    /// </summary>
    public void ResetHealth()
    {
        _gameManager._currentHealth = _gameManager.StartingHealth;
        foreach (Image item in _gameManager.HealthImages)
        {
            item.enabled = false;
            item.GetComponent<Animator>().enabled = false;
        }
    }

    /// <summary>
    /// Animating angry manager on each miss event
    /// </summary>
    public void AnimateAngryManager()
    {
        StartCoroutine(AnimatingAngryManager());
    }
    
    /// <summary>
    /// Animating angry manager on each miss event
    /// </summary>
    private IEnumerator AnimatingAngryManager()
    {
        yield return new WaitForSecondsRealtime(_gameManager.PauseDuration / 2);
        AngryBoss.SetTrigger("Animate");
    }

    /// <summary>
    /// Animating Flicker effect on each miss event (timer or car)
    /// </summary>
    /// <param name="targetGraphic"></param>
    public void AnimateMissFlickerEffect(Image targetGraphic)
    {
        Animator targetAnimator = targetGraphic.GetComponent<Animator>();
        targetAnimator.enabled = true;
        targetGraphic.preserveAspect = true;
        targetAnimator.SetTrigger("Animate");
        StartCoroutine(CloseAnimator(targetAnimator));
    }
    
    /// <summary>
    /// Closes given animator after 3 seconds
    /// </summary>
    /// <param name="targetAnimator">Target animator</param>
    private IEnumerator CloseAnimator(Animator targetAnimator)
    {
        yield return new WaitForSecondsRealtime(3);
        targetAnimator.enabled = false;
    }

    /// <summary>
    /// Animates Customer to take pizza
    /// </summary>
    /// <param name="customer">Target Customer</param>
    public void AnimateTakingPizza(Customer customer)
    {
        if (customer.CurrentlyOpenedClosing == false)
            StartCoroutine(AnimatingTakingPizza(customer));
    }
    
    /// <summary>
    /// Animates Customer to take pizza
    /// </summary>
    /// <param name="customer">Target Customer</param>
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
