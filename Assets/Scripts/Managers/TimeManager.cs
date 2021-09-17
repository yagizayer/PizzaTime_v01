/// <summary>
/// This file used for setting and editing Time and Alarm
/// </summary>
using System.Collections;
using System.Collections.Generic;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine.UI;
using UnityEngine;
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

    /// <summary>
    /// When called Game enters edit mode and you can use wasd keys for editing time
    /// </summary>
    public void EditTime()
    {
        _editingTime = true;
        UpdateUI();
        TimeKeeper.SettingMode = TimeMode.Editing;
        _timeEditingMode = TimeEditingMode.AmPm;
        _timeAnimators[_timeEditingMode].enabled = true;
        StopCoroutine(ProceedActiveTime());

    }

    /// <summary>
    /// When called Game enters edit mode and you can use wasd keys for editing alarm
    /// </summary>
    public void EditAlarm()
    {
        _editingTime = false;
        UpdateUI();
        TimeKeeper.SettingMode = TimeMode.Editing;
        _timeEditingMode = TimeEditingMode.AmPm;
        _timeAnimators[_timeEditingMode].enabled = true;

    }
    
    /// <summary>
    /// When called Game Exits edit mode and sets edited time as current time to TimeKeeper.cs
    /// </summary>
    public void TimeSet()
    {
        if (TimeKeeper.SetTime.Equals(TimeKeeper.SetAlarm))
            TimeKeeper.SetAlarm.IncreaseMinutes();
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
    
    /// <summary>
    /// When called Game Exits edit mode and sets edited alarm as current alarm to TimeKeeper.cs
    /// </summary>
    public void AlarmSet()
    {
        if (TimeKeeper.SetTime.Equals(TimeKeeper.SetAlarm))
            TimeKeeper.SetAlarm.IncreaseMinutes();
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
    
    /// <summary>
    /// When player editing time used w or s key. As result time is changed.
    /// </summary>
    /// <param name="isIncrease">If player used w key this is true</param>
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
    
    /// <summary>
    /// When player editing Alarm used w or s key. As result Alarm is changed.
    /// </summary>
    /// <param name="isIncrease">If player used w key this is true</param>
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
    
    /// <summary>
    /// Updates Scene objects according to current time.
    /// </summary>
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
    
    /// <summary>
    /// Blinking effect for indicating which part of time is Editing 
    /// </summary>
    private void AnimateTime()
    {
        ResetTimeObjects();
        _timeAnimators[_timeEditingMode].enabled = true;
    }
    
    /// <summary>
    /// Resets Time objects to prevent staying them close during navigating between them.
    /// (ex. 
    ///     during blinking animation if we move from hour part to minutes part, 
    ///     Hour part may be stay invisible at scene. 
    ///     This function prevents this.
    /// )
    /// </summary>
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

    /// <summary>
    /// Returns Differance between Current time and setted alarm
    /// </summary>
    /// <param name="currentTime">current time</param>
    /// <param name="currentAlarm">current alarm</param>
    /// <returns>differance between them as minutes</returns>
    public float CalculateAlarmDiff(CurrentTime currentTime, CurrentTime currentAlarm)
    {
        float result = 0;

        if (TimeKeeper.SettingMode != TimeMode.Set) { Debug.Log(TimeKeeper.SettingMode); return 24 * 60; }


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

    /// <summary>
    /// proceeds current time as expected
    /// </summary>
    /// <returns></returns>
    private IEnumerator ProceedActiveTime()
    {
        while (TimeKeeper.SettingMode == TimeMode.Set)
        {
            // yield return new WaitForSecondsRealtime(5); // testing purposes
            yield return new WaitForSecondsRealtime(60); // real code
            TimeKeeper.SetTime.IncreaseMinutes();
            UpdateUI();
        }
    }

    /// <summary>
    /// used for resetting Alarm
    /// </summary>
    public void ResetAlarm()
    {
        TimeKeeper.SetAlarm = new CurrentTime();
    }
}