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
    private GameManager _gameManager;
    private TimeEditingMode _timeEditingMode = TimeEditingMode.Null;
    [SerializeField] private TimeGameObjectsDict _timeGameobjects = new TimeGameObjectsDict();
    [SerializeField] private TimeAnimatorsDict _timeAnimators = new TimeAnimatorsDict();
    private bool _editingTime = true;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        if (TimeKeeper.SetAlarm == null) TimeKeeper.SetAlarm = new CurrentTime();
        if (TimeKeeper.SetTime == null) TimeKeeper.SetTime = new CurrentTime();
        else UpdateUI();
        if (TimeKeeper.SettingMode == TimeMode.Set) StartCoroutine(ProceedActiveTime());
    }
    private void Update()
    {
        if (TimeKeeper.SettingMode == TimeMode.Editing)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                if (_editingTime)
                    UpdateTime(isIncrease: true);
                else
                    UpdateAlarm(isIncrease: true);

                UpdateUI();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                if (_editingTime)
                    UpdateTime(isIncrease: false);
                else
                    UpdateAlarm(isIncrease: false);

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
                if (_timeEditingMode == TimeEditingMode.Minutes && _editingTime)
                {
                    TimeSet();
                    return;
                }
                if (_timeEditingMode == TimeEditingMode.Minutes && !_editingTime)
                {
                    AlarmSet();
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
        _editingTime = true;
        UpdateUI();
        TimeKeeper.SettingMode = TimeMode.Editing;
        _timeEditingMode = TimeEditingMode.AmPm;
        _timeAnimators[_timeEditingMode].enabled = true;
        StopCoroutine(ProceedActiveTime());

    }
    public void EditAlarm()
    {
        _editingTime = false;
        UpdateUI();
        TimeKeeper.SettingMode = TimeMode.Editing;
        _timeEditingMode = TimeEditingMode.AmPm;
        _timeAnimators[_timeEditingMode].enabled = true;

    }
    public void TimeSet()
    {
        if (_timeEditingMode != TimeEditingMode.Null)
            _timeAnimators[_timeEditingMode].enabled = false;
        TimeKeeper.SettingMode = TimeMode.Set;
        _timeEditingMode = TimeEditingMode.Null;
        if (_gameManager)
            _gameManager.StartGame();
        ResetTimeObjects();
        UpdateUI();
        StartCoroutine(ProceedActiveTime());
    }
    public void AlarmSet()
    {
        _editingTime = true;
        UpdateUI();
        if (_timeEditingMode != TimeEditingMode.Null)
            _timeAnimators[_timeEditingMode].enabled = false;
        TimeKeeper.SettingMode = TimeMode.Set;
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
    public void UpdateAlarm(bool isIncrease)
    {
        if (isIncrease)
        {
            if (_timeEditingMode == TimeEditingMode.AmPm) TimeKeeper.SetAlarm.ChangeAmPm();
            if (_timeEditingMode == TimeEditingMode.Hours) TimeKeeper.SetAlarm.IncreaseHours();
            if (_timeEditingMode == TimeEditingMode.Minutes) TimeKeeper.SetAlarm.IncreaseMinutes();
        }
        else
        {
            if (_timeEditingMode == TimeEditingMode.AmPm) TimeKeeper.SetAlarm.ChangeAmPm();
            if (_timeEditingMode == TimeEditingMode.Hours) TimeKeeper.SetAlarm.DecreaseHours();
            if (_timeEditingMode == TimeEditingMode.Minutes) TimeKeeper.SetAlarm.DecreaseMinutes();
        }
    }
    private void UpdateUI()
    {
        ResetTimeObjects();
        if (_editingTime)
        {
            _timeGameobjects[TimeGameObjects.HoursGO].GetComponent<Text>().text = TimeKeeper.SetTime.Hours.ToStringWithFormat(2);
            _timeGameobjects[TimeGameObjects.MinutesGO].GetComponent<Text>().text = TimeKeeper.SetTime.Minutes.ToStringWithFormat(2);
        }
        else
        {
            _timeGameobjects[TimeGameObjects.HoursGO].GetComponent<Text>().text = TimeKeeper.SetAlarm.Hours.ToStringWithFormat(2);
            _timeGameobjects[TimeGameObjects.MinutesGO].GetComponent<Text>().text = TimeKeeper.SetAlarm.Minutes.ToStringWithFormat(2);
        }
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

        if (_editingTime)
        {
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
        else
        {
            if (TimeKeeper.SetAlarm.IsAM)
            {
                _timeGameobjects[TimeGameObjects.AmGO].enabled = false;
                _timeGameobjects[TimeGameObjects.PmGO].enabled = true;
            }
            else
            {
                _timeGameobjects[TimeGameObjects.AmGO].enabled = true;
                _timeGameobjects[TimeGameObjects.PmGO].enabled = false;
            }
        }
    }

    //------------------

    public float CalculateAlarmDiff(CurrentTime currentTime, CurrentTime currentAlarm)
    {
        float result = 0;

        int hoursDiff = (currentTime.Hours - currentAlarm.Hours) % 12;
        int minutesDiff = (currentTime.Minutes - currentAlarm.Minutes) % 60;
        int amPmDiff = 0;
        if (currentTime.IsAM && currentAlarm.IsAM) amPmDiff = 0;
        if (currentTime.IsAM && !currentAlarm.IsAM) amPmDiff = 12;
        if (!currentTime.IsAM && currentAlarm.IsAM) amPmDiff = -12;
        if (!currentTime.IsAM && !currentAlarm.IsAM) amPmDiff = 0;
        hoursDiff += amPmDiff;
        hoursDiff = Mathf.Abs(hoursDiff);
        minutesDiff = Mathf.Abs(minutesDiff);

        result = hoursDiff * 60 + minutesDiff;

        return result;
    }

    private IEnumerator ProceedActiveTime()
    {
        while (TimeKeeper.SettingMode == TimeMode.Set)
        {
            yield return new WaitForSecondsRealtime(5); // testing purposes
            // yield return new WaitForSecondsRealtime(60); // real code
            TimeKeeper.SetTime.IncreaseMinutes();
            UpdateUI();
        }
    }
}
