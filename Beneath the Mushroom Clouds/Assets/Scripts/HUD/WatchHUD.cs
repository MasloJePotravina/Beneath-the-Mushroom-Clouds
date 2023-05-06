using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Implements the behaviour of the Watch section of the HUD.
/// </summary>
public class WatchHUD : MonoBehaviour
{

    /// <summary>
    /// Reference to the text elment that shows the time.
    /// </summary>
    private TextMeshProUGUI timeText;

    /// <summary>
    /// Reference to the text element that shows the date.
    /// </summary>
    private TextMeshProUGUI dateText;

    /// <summary>
    /// Reference to the text element that shows the day (MON/TUE/...).
    /// </summary>
    private TextMeshProUGUI dayText;

    /// <summary>
    /// Reference to the text element that shows the temperature.
    /// </summary>
    private TextMeshProUGUI tempText;
    
    /// <summary>
    /// Reference to the world status.
    /// </summary>
    private WorldStatus worldStatus;

    /// <summary>
    /// The string that will be displayed on the watch as time.
    /// </summary>
    private string timeString;

    /// <summary>
    /// The string that will be displayed on the watch as date.
    /// </summary>
    private string dateString;

    /// <summary>
    /// The string that will be displayed on the watch as day.
    /// </summary>
    private string dayString;

    /// <summary>
    /// The string that will be displayed on the watch as temperature.
    /// </summary>
    private string tempString;

    /// <summary>
    /// List of string shortcuts for the days of the week.
    /// </summary>
    public List<string> dayShortcuts = new List<string>{"MON", "TUE", "WED", "THU", "FRI", "SAT", "SUN"};

    //Note: When I created the digital font for BTMC, I only added letters and numbers I needed for the watch
    // and forgot to add the minus sign. So I made a fake one using an image.
    /// <summary>
    /// Reference to a fake minus sign that will be displayed when the temperature is negative.
    /// </summary>
    private RectTransform fakeMinusRect;

    /// <summary>
    /// Get all the necessary references on awake.
    /// </summary>
    void Awake(){
        timeText = this.gameObject.transform.Find("WatchTime").GetComponent<TextMeshProUGUI>();
        dateText = this.gameObject.transform.Find("WatchDate").GetComponent<TextMeshProUGUI>();
        dayText = this.gameObject.transform.Find("WatchDay").GetComponent<TextMeshProUGUI>();
        tempText = this.gameObject.transform.Find("WatchTemp").GetComponent<TextMeshProUGUI>();

        fakeMinusRect = this.gameObject.transform.Find("FakeMinus").gameObject.GetComponent<RectTransform>();

        worldStatus = GameObject.Find("GameManager").GetComponent<WorldStatus>();
    }

    /// <summary>
    /// Update waych strings each frame.
    /// </summary>
    public void Update(){
        timeString = worldStatus.hour.ToString("00") + ":" + worldStatus.minute.ToString("00");

        dateString = worldStatus.day.ToString("00") + "/" + worldStatus.month.ToString("00") + "/" + worldStatus.year.ToString("00");
        dayString = dayShortcuts[worldStatus.dayOfWeek];

        //I know this is terrible but updating the font is a considerable amount of work at this point
        if(worldStatus.outsideTemp >= 0){
            fakeMinusRect.gameObject.SetActive(false);
        }else if(worldStatus.outsideTemp < 0 && worldStatus.outsideTemp > -10){
            fakeMinusRect.anchoredPosition = new Vector3(-55.4f, fakeMinusRect.anchoredPosition.y);
            fakeMinusRect.gameObject.SetActive(true);
        }else{
            fakeMinusRect.anchoredPosition = new Vector3(-72.3f, fakeMinusRect.anchoredPosition.y);
            fakeMinusRect.gameObject.SetActive(true);
        }


        int tempInt = Mathf.Abs((int)worldStatus.outsideTemp);
        tempString = tempInt.ToString() + "C";


        timeText.text = timeString;
        dateText.text = dateString;
        dayText.text = dayString;
        tempText.text = tempString;
    }

}




