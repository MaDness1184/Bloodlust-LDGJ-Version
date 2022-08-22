using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager main;

    [Header("Settings")]

    [SerializeField] private bool freezeTime = false;
    [SerializeField] private float _timeScale = 0.25f;
    [SerializeField] private Text timeText;
    [SerializeField] private Light2D globalLight;
    [SerializeField] private Transform player;
    [SerializeField] private Color sixAm;
    [SerializeField] private Color eightAm;
    [SerializeField] private Color tenAm;
    [SerializeField] private Color twoPm;
    [SerializeField] private Color fourPm;
    [SerializeField] private Color sixPm;
    [SerializeField] private Color eightPm;

    [Header("Debugs")]
    [SerializeField] private float _runtime;
    [SerializeField] private float _currentMinuteRaw;
    [SerializeField] private float _currentMinute;
    [SerializeField] private float _currentHour;
    [SerializeField] private float _currentDay;

    [Header("Debugs 2")]
    [SerializeField] private int suspisionCount = 0;

    public float timeScale { get { return _timeScale; } }
    public float currentMinuteRaw { get { return _currentMinuteRaw; } }
    public float currentHour { get { return _currentHour; } }
    public float currentDay { get { return _currentDay; } }

    public static float dayScore;
    public static int killScore;

    private float startTime;
    private Color colorA;
    private Color colorB;
    private float lerpDuration;

    private float cachedHour;

    private void Awake()
    {
        if (main == null)
            main = this;
        else
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        GetColor();
        cachedHour = _currentHour;
        lerpDuration = 2 * 60 / _timeScale;
        t = 0;
    }

    // Update is called once per frame
    void Update()
    {
       if(suspisionCount > 3)
        {
            //Big sus
        }
    }

    private void FixedUpdate()
    {
        if (freezeTime) return;

        UpdateIngameTime();
    }

    [SerializeField] float t;
    [SerializeField] bool tickedHour = false;
    [SerializeField] bool tickedDay = false;

    private void UpdateIngameTime()
    {
        _runtime = Mathf.FloorToInt((Time.time - startTime));
        _currentMinuteRaw =  (Time.time - startTime) * _timeScale;
        _currentMinute = Mathf.RoundToInt(_currentMinuteRaw) % 60;

        if (_currentMinute == 59)
            tickedHour = false;

        if (!tickedHour && _currentMinute == 0)
        {
            tickedHour = true;
            _currentHour++;
            if (_currentHour > 23)
                _currentHour = 0;

            if (_currentHour != cachedHour)
            {
                if (currentHour == 6 || currentHour == 8
                 || currentHour == 10|| currentHour == 16 
                 || currentHour == 18 || currentHour == 20)
                {
                    GetColor();
                    cachedHour = _currentHour;
                    lerpDuration = 2 * 60 / _timeScale;
                    t = 0;
                }
            }
        }

        if (_currentHour == 23)
            tickedDay = false;

        if(!tickedDay && Mathf.RoundToInt(_currentMinuteRaw) % 1440 == 0)
        {
            tickedDay = true;
            _currentDay++;
            dayScore = currentDay;
        }

        timeText.text = $"Day {_currentDay.ToString("00")} - {_currentHour.ToString("00")}:{_currentMinute.ToString("00")}";

        globalLight.color = Color.Lerp(colorA, colorB, t);
        if (t < 1)
            t += Time.deltaTime / lerpDuration;

    }

    public delegate void DelAlarmCallback();

    public IEnumerator SetAlarm(float duration, DelAlarmCallback AlarmCallback)
    {
        Debug.Log("Alarm started");

        var endTime = _currentMinuteRaw + duration;

        while(_currentMinuteRaw != endTime)
        {
            yield return new WaitForSeconds(1/_timeScale);
        }

        AlarmCallback();

        Debug.Log("Alarm stopped");
    }

    private void GetColor()
    {
        if (_currentHour >= 6 && _currentHour < 8)
        {
            colorA = eightPm;
            colorB = sixAm;
        }
        else if (_currentHour >= 8 && _currentHour < 10)
        {
            colorA = sixAm;
            colorB = eightAm;
        }
        else if(_currentHour >= 10 && _currentHour < 16)
        {
            colorA = eightAm;
            colorB = tenAm;
        }
        else if (_currentHour >= 16 && _currentHour < 18)
        {
            colorA = tenAm;
            colorB = fourPm;
        }
        else if (_currentHour >= 18 && _currentHour < 20)
        {
            colorA = fourPm;
            colorB = sixPm;
        }
        else if (_currentHour >= 20 && _currentHour < 22)
        {
            colorA = sixPm;
            colorB = eightPm;
        }
        else
        {
            colorA = eightPm;
            colorB = eightPm;
        }
    }

    public void NotifySuspicion()
    {
        suspisionCount++;
    }
    public void DeNotifySuspicion()
    {
        suspisionCount--;
    }

    public Transform GetPlayer()
    {
        return player;
    }

    public static void NotifyKill()
    {
        killScore++;
    }
}
