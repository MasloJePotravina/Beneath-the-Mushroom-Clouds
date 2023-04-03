using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RestScreen : MonoBehaviour
{
    private HUDController hudController;
    private WorldStatus worldStatus;
    
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI dateText;
    [SerializeField] private TextMeshProUGUI restingText;

    public float hoursToRest = 0;
    private int month;
    private int day;
    private int hour;
    private int minute;
    private float second;


    private int targetSeconds;
    private int targetHour;
    private int targetMinute;
    private int targetDay;
    private int targetMonth;


    private float timeMultiplier = 0f;


    float secondsToWait = 5f;


    private bool activated = false;

    void Awake(){
        hudController = GameObject.Find("HUD").GetComponent<HUDController>();
        worldStatus = GameObject.Find("GameManager").GetComponent<WorldStatus>();
    }

    void Update()
    {
        //Wait for activation
        if(!activated){
            return;
        }
        SpedUpTimeFlow();
        UpdateTextElements();

        if(CapSleepToTargetValues()){
            UpdateTextElements();
            activated = false;
        }

    }

    public void Init (float hours)
    {
        hoursToRest = hours;
        
        month = worldStatus.month;
        day = worldStatus.day;
        hour = worldStatus.hour;
        minute = worldStatus.minute;
        
        CalculateMultiplier();
        CalculateTargetTime();
        UpdateTextElements();
    }

    public void Activate(){
        activated = true;
    }

    public void Deactivate(){
        activated = false;
    }

    public void CalculateMultiplier(){
        float realSeconds = (hoursToRest/20f) * 3600f;
        timeMultiplier = (realSeconds/secondsToWait)* 20f;
        timeMultiplier += (0.1f * timeMultiplier);
    }

    private void SpedUpTimeFlow(){

        second += Time.unscaledDeltaTime * timeMultiplier;
    
        SetTimeVariables();
    }

    private void SetTimeVariables(){
        if (second >= 60)
        {
            minute++;
            second = 0;
        }
        if (minute >= 60)
        {
            hour++;
            minute = 0;
        }
        if (hour >= 24)
        {
            day++;
            hour = 0;
        }
        if (day >= 30)
        {
            month++;
            day = 0;
        }
        if (month >= 12)
        {
            month = 0;
        }
    }

    private void UpdateTextElements(){
        timeText.text = hour.ToString("00") + ":" + minute.ToString("00");
        dateText.text = day.ToString("00") + "/" + month.ToString("00") + "/" + "28";
    }
       
    public void SetAlpha(float alpha)
    {
        background.color = new Color(0,0,0, alpha);
        timeText.color = new Color(1,1,1, alpha);
        dateText.color = new Color(1,1,1, alpha);
        restingText.color = new Color(1,1,1, alpha);

    }

    public void CalculateTargetTime(){
        int secondsToRest = (int)(hoursToRest * 3600f);
        targetSeconds = (int)worldStatus.second;
        targetMinute = worldStatus.minute;
        targetHour = worldStatus.hour;
        targetDay = worldStatus.day;
        targetMonth = worldStatus.month;
        for(int seconds = 0; seconds < secondsToRest; seconds++){
            targetSeconds++;
            if(targetSeconds >= 60){
                targetMinute++;
                targetSeconds = 0;
            }
            if(targetMinute >= 60){
                targetHour++;
                targetMinute = 0;
            }
            if(targetHour >= 24){
                targetDay++;
                targetHour = 0;
            }
            if(targetDay >= 30){
                targetMonth++;
                targetDay = 0;
            }
            if(targetMonth >= 12){
                targetMonth = 0;
            }

            
        }
    }

    public bool CapSleepToTargetValues(){
        if(minute > targetMinute){
            if(hour >= targetHour){
                if(day >= targetDay){
                    if(month >= targetMonth){
                        minute = targetMinute;
                        hour = targetHour;
                        day = targetDay;
                        month = targetMonth;
                        return true;
                    }
                }
            }
        }
        return false;
    }


}
