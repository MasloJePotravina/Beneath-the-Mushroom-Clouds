using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class WatchHUD : MonoBehaviour
{
    private TextMeshProUGUI timeText;
    private TextMeshProUGUI dateText;
    private TextMeshProUGUI dayText;
    private TextMeshProUGUI tempText;
    

    private WorldStatus worldStatus;

    private string timeString;
    private string dateString;
    private string dayString;
    private string tempString;

    public List<string> dayShortcuts = new List<string>{"MON", "TUE", "WED", "THU", "FRI", "SAT", "SUN"};

    //Note: When I created the digital font for BTMC, I only added letters and numbers I needed for the watch
    // and forgot to add the minus sign. So I made a fake one using an image.
    private RectTransform fakeMinusRect;


    void Awake(){
        timeText = this.gameObject.transform.Find("WatchTime").GetComponent<TextMeshProUGUI>();
        dateText = this.gameObject.transform.Find("WatchDate").GetComponent<TextMeshProUGUI>();
        dayText = this.gameObject.transform.Find("WatchDay").GetComponent<TextMeshProUGUI>();
        tempText = this.gameObject.transform.Find("WatchTemp").GetComponent<TextMeshProUGUI>();

        fakeMinusRect = this.gameObject.transform.Find("FakeMinus").gameObject.GetComponent<RectTransform>();

        worldStatus = GameObject.Find("GameManager").GetComponent<WorldStatus>();
    }

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




