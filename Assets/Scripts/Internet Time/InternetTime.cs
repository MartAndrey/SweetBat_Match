using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;

public class InternetTime : MonoBehaviour
{
    public static InternetTime Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    // This coroutine retrieves the current date and time from an internet server using an HTTP GET request.
    // It parses the date string returned by the server and invokes an action with a DateTime object representing the internet time.
    public IEnumerator GetInternetTime(Action<DateTime> onSuccess/*, Action<string> onError*/)
    {
        UnityWebRequest www = UnityWebRequest.Get("https://www.google.com");
        yield return www.SendWebRequest();
        string currentDate = www.GetResponseHeader("date");

        // if (www.result != UnityWebRequest.Result.Success)
        // {
        //     onError?.Invoke(www.error);
        //     yield break;
        // }

        if (currentDate == null) Debug.LogWarning("Error Server");
        else
        {
            // The date string returned by the server is parsed into a DateTime object using the "ddd, dd MMM yyyy HH:mm:ss 'GMT'" format.
            // The resulting DateTime object is assumed to be in the UTC time zone.
            DateTime internetTime = DateTime.ParseExact(currentDate,
                                "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                                CultureInfo.InvariantCulture.DateTimeFormat,
                                DateTimeStyles.AssumeUniversal);

            onSuccess?.Invoke(internetTime);
        }
    }

    // This method starts a coroutine that retrieves the current internet time and updates the PreviouslyAllottedTime property of a Timer component.
    public void UpdateInternetTime(GameObject objectTimeUpdate)
    {
        StartCoroutine(GetInternetTime(currentTime =>
        {
            objectTimeUpdate.GetComponent<Timer>().PreviouslyAllottedTime = currentTime;
        }));
    }
}