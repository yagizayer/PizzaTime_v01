using System.Collections;
using System.Collections.Generic;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine.UI;
using UnityEngine;
using System;

public class AnimationManager : MonoBehaviour
{
    private GameManager _gameManager;
    private EventManager _eventManager;
    [SerializeField] private Animator AngryBoss;
    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _eventManager = _gameManager.GameEventManager;
    }

    public void AnimateHealthReduce()
    {
        Image imageToAnimate = _gameManager.HealthImages[_gameManager.CurrentHealth];
        imageToAnimate.GetComponent<Animator>().enabled = true;
    }

    public void AnimateAngryManager()
    {
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

}
