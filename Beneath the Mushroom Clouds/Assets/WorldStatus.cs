using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WorldStatus : MonoBehaviour
{
    public int month = 1;
    public int day = 1;
    public int hour = 0;
    public int minute = 0;
    public float second = 0;

    public float irlDaySeconds = 0f;

    public float timeMultiplier = 20f;

    public float outsideTemp = 4f;

    public int dayOfWeek = 0;//TODO set to starting day of week 

    public float globalLightIntensity = 0f;

    private UnityEngine.Rendering.Universal.Light2D globalLight;

    void Start()
    {
        globalLight = GameObject.Find("GlobalLight").GetComponent<UnityEngine.Rendering.Universal.Light2D>();
        Time.timeScale = 1f;
    }
    

    void Update()
    {
        NaturalTimeFlow();
        globalLightIntensity = SetLightIntensity();
        globalLight.intensity = globalLightIntensity;

        
    }

    private float SetLightIntensity(){
        float lightIntensity = 1f;
        if(irlDaySeconds >= 1800f && irlDaySeconds < 3060f){ //1800 = 10:00, 3060 = 17:00
            lightIntensity = 1f;
        }else if(irlDaySeconds >= 3060f && irlDaySeconds < 3960f){//3060 = 17:00, 3960 = 22:00
            lightIntensity = 0.2f + (3960f - irlDaySeconds) * 0.8f / 900f; 
        }else if(irlDaySeconds >= 3960f || irlDaySeconds < 900f){//3960 = 22:00, 900 = 5:00
            lightIntensity = 0.2f;
        }else if(irlDaySeconds >= 900f && irlDaySeconds < 1800f){//900 = 5:00, 1800 = 10:00
            lightIntensity = 0.2f + (irlDaySeconds - 900f) * 0.8f / 900f;
        }
        return lightIntensity;
}

    private void NaturalTimeFlow(){

        irlDaySeconds += Time.deltaTime;

        if(irlDaySeconds >= 4320f){
            irlDaySeconds = 0f;
        }

        second += Time.deltaTime * timeMultiplier;

        
    
        SetTimeVariables();
        
    }  

    public void TimeSkip(float hours){
        for(long i = 0; i < (int)((hours/20) * 3600f); i++){
            irlDaySeconds += 1;
            if(irlDaySeconds >= 4320f){
                irlDaySeconds = 0f;
            }

            second += timeMultiplier;

            SetTimeVariables();
        }
        
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
            dayOfWeek++;
            if(dayOfWeek >= 7)
            {
                dayOfWeek = 0;
            }
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

    public void StopTime(){
        Time.timeScale = 0f;
    }

    public void StartTime(){
        Time.timeScale = 1f;
    }
}


