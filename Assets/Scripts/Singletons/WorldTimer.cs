using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text.RegularExpressions;

public class WorldTimer: MonoBehaviour
{
    const string API_URL = "https://worldtimeapi.org/api/timezone/Asia/Hong_Kong";
    [HideInInspector] bool isTimeLoaded = false;

    private Dictionary<string, DateTime> dateTimeList;

    private DateTime _currentTime = DateTime.Now;

    // Start is called before the first frame update
    void Start()
    {
        ConnectToInternet();
    }

    void ConnectToInternet()
    {
        StartCoroutine(GetRealTimeFromAPI());
    }

    //get last time difference of key value.
    //prime usage is for ads
    public int GetTimeDifferenceNow(string name)
    {
        if (dateTimeList.ContainsKey(name))
        {
            int difference = (int)(GetCurrentDateTime() - dateTimeList[name]).TotalSeconds;
            return difference;
        }
        //DNE
        return -1;
    }

    public void SetTime(string name)
    {
        if (!isTimeLoaded) ConnectToInternet();
        if (dateTimeList == null) dateTimeList = new Dictionary<string, DateTime>();
        if (!dateTimeList.ContainsKey(name))
        {
            dateTimeList.Add(name, GetCurrentDateTime());
        }
    }

    DateTime GetCurrentDateTime()
    {
        return _currentTime.AddSeconds(Time.realtimeSinceStartup);
    }

    IEnumerator GetRealTimeFromAPI()
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(API_URL);

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("error " + webRequest.error);
            StartCoroutine(ReconnectLater());
        }
        else
        {
            Debug.Log("Time Load Success! Connected to the internet time");
            TimeData timeData = JsonUtility.FromJson<TimeData>(webRequest.downloadHandler.text);

            _currentTime = ParseDateTime(timeData.datetime);
            isTimeLoaded = true;
        }
    }

    IEnumerator ReconnectLater()
    {
        yield return new WaitForSecondsRealtime(300);
        ConnectToInternet();
    }

    struct TimeData
    {
        public string datetime;
    }

    DateTime ParseDateTime(string datetime)
    {
        string date = Regex.Match(datetime, @"^\d{4}-\d{2}-\d{2}").Value;
        string time = Regex.Match(datetime, @"\d{2}:\d{2}:\d{2}").Value;

        return DateTime.Parse(string.Format("{0} {1}", date, time));
    }
}
