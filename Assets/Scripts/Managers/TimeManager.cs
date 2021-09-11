using System.Collections;
using System.Collections.Generic;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine.UI;
using UnityEngine;
using System;

[System.Serializable]
public class TimeGameObjectsDict : SerializableDictionaryBase<TimeGameObjects, Text> { }

[System.Serializable]
public class TimeAnimatorsDict : SerializableDictionaryBase<TimeEditingMode, Animator> { }

public partial class TimeManager : MonoBehaviour
{
    [SerializeField] private TimeMode _timeMode = TimeMode.NotSet;
    private GameManager _gameManager;
    private TimeEditingMode _timeEditingMode = TimeEditingMode.Null;
    [SerializeField] private TimeGameObjectsDict _timeGameobjects = new TimeGameObjectsDict();
    [SerializeField] private TimeAnimatorsDict _timeAnimators = new TimeAnimatorsDict();


    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        if (TimeKeeper.SetTime == null) TimeKeeper.SetTime = new CurrentTime();
        else UpdateUI();
    }

    private void Update()
    {
        if (_timeMode == TimeMode.Editing)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                UpdateTime(isIncrease: true);
                UpdateUI();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                UpdateTime(isIncrease: false);
                UpdateUI();
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                if (_timeEditingMode == TimeEditingMode.AmPm) return;
                _timeEditingMode = _timeEditingMode.Previous();
                AnimateTime();
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                if (_timeEditingMode == TimeEditingMode.Minutes)
                {
                    TimeSet();
                    return;
                }
                _timeEditingMode = _timeEditingMode.Next();
                AnimateTime();
            }
        }
    }

    //------------------


    public void EditTime()
    {
        _timeMode = TimeMode.Editing;
        _timeEditingMode = TimeEditingMode.AmPm;
        _timeAnimators[_timeEditingMode].enabled = true;

    }
    public void TimeSet()
    {
        _timeAnimators[_timeEditingMode].enabled = false;
        _timeMode = TimeMode.Set;
        _timeEditingMode = TimeEditingMode.Null;
        if (_gameManager)
            _gameManager.StartGame();
        ResetTimeObjects();
    }
    public void UpdateTime(bool isIncrease)
    {
        if (isIncrease)
        {
            if (_timeEditingMode == TimeEditingMode.AmPm) TimeKeeper.SetTime.ChangeAmPm();
            if (_timeEditingMode == TimeEditingMode.Hours) TimeKeeper.SetTime.IncreaseHours();
            if (_timeEditingMode == TimeEditingMode.Minutes) TimeKeeper.SetTime.IncreaseMinutes();
        }
        else
        {
            if (_timeEditingMode == TimeEditingMode.AmPm) TimeKeeper.SetTime.ChangeAmPm();
            if (_timeEditingMode == TimeEditingMode.Hours) TimeKeeper.SetTime.DecreaseHours();
            if (_timeEditingMode == TimeEditingMode.Minutes) TimeKeeper.SetTime.DecreaseMinutes();
        }
    }
    private void UpdateUI()
    {
        ResetTimeObjects();
        _timeGameobjects[TimeGameObjects.HoursGO].GetComponent<Text>().text = TimeKeeper.SetTime.Hours.ToStringWithFormat(2);
        _timeGameobjects[TimeGameObjects.MinutesGO].GetComponent<Text>().text = TimeKeeper.SetTime.Minutes.ToStringWithFormat(2);
    }
    private void AnimateTime()
    {
        ResetTimeObjects();
        _timeAnimators[_timeEditingMode].enabled = true;
    }
    private void ResetTimeObjects()
    {
        foreach (KeyValuePair<TimeEditingMode, Animator> item in _timeAnimators)
            item.Value.enabled = false;

        foreach (KeyValuePair<TimeGameObjects, Text> item in _timeGameobjects)
            if (item.Key != TimeGameObjects.AmGO && item.Key != TimeGameObjects.PmGO)
                item.Value.enabled = true;

        if (TimeKeeper.SetTime.IsAM)
        {
            _timeGameobjects[TimeGameObjects.AmGO].enabled = true;
            _timeGameobjects[TimeGameObjects.PmGO].enabled = false;
        }
        else
        {
            _timeGameobjects[TimeGameObjects.AmGO].enabled = false;
            _timeGameobjects[TimeGameObjects.PmGO].enabled = true;
        }
    }



}
