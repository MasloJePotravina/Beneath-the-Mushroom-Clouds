using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WorldStatus : MonoBehaviour
{
    public int year = 28; //2028
    public int month = 1;
    public int day = 1;
    public int hour = 6;
    public int minute = 0;
    public float second = 0;

    public float timeMultiplier = 20f;

    public float gameSecondsInDay = 0f;

    public float secondsFromStart = 0;

    [SerializeField] private GameObject eveningLight;
    [SerializeField] private UnityEngine.Rendering.Universal.Light2D eveningLightComponent;

    private Dictionary<int,int> daysInMonth = new Dictionary<int, int>(){
        {1, 31},
        {2, 28},
        {3, 31},
        {4, 30},
        {5, 31},
        {6, 30},
        {7, 31},
        {8, 31},
        {9, 30},
        {10, 31},
        {11, 30},
        {12, 31}
    };

    

    public int dayOfWeek = 0;//TODO set to starting day of week 

    public float globalLightIntensity = 0f;
    private Color globalLightColor = new Color(1f, 1f, 1f);

    private UnityEngine.Rendering.Universal.Light2D globalLight;

    //Temperature
    public float outsideTemp = 4f;
    private float maxTempDifferenceBetweenDays = 2.5f; //Max difference between average temperatures of two consecutive days halved
    private float maxTempDifferenceInDay = 5f; //Max difference between coldest and warmest temperature in a day halved


    private float minTempYesterday;
    private float maxTempYesterday;
    private float minTempToday;
    private float maxTempToday;
    private float minTempTomorrow;
    private float maxTempTomorrow;

    private float averageTempYesterday;
    private float averageTempToday;
    private float averageTempTomorrow;
    
    private bool tempBoostTowardsAverage = false;
    private bool tempReductionTowardsAverage = false;


    //Monthly average
    private Dictionary<int, float> monthlyAverageTemp = new Dictionary<int, float>(){
        {1, -15f},
        {2, -12f},
        {3, -8f},
        {4, -2f},
        {5, 0f},
        {6, 4f},
        {7, 6f},
        {8, 4f},
        {9, 0f},
        {10, -4f},
        {11, -8f},
        {12, -12f}
    };

    

    void Start()
    {
        globalLight = GameObject.Find("GlobalLight").GetComponent<UnityEngine.Rendering.Universal.Light2D>();
        Time.timeScale = 1f;
        InitializeTemp();
    }
    

    void Update()
    {
        GameTimeFlow();
        outsideTemp = CalculateOutsideTemp();
        globalLightIntensity = CalculateLightIntensity();
        globalLightColor = CalculateLightColor();
        globalLight.intensity = globalLightIntensity;
        globalLight.color = globalLightColor;

        
    }

    private float CalculateLightIntensity(){
        float lightIntensity = 1f;

        //Recreate above code using gameSecondsInDay (0-86400)
        if(gameSecondsInDay >= 36000f && gameSecondsInDay < 61200f){ //36000 = 10:00, 61200 = 17:00 //Fully bright
            lightIntensity = 1f;
        }else if(gameSecondsInDay >= 61200f && gameSecondsInDay < 79200f){//61200 = 17:00, 79200 = 22:00 //Fade to dark
            lightIntensity = 0.2f + (79200f - gameSecondsInDay) * 0.8f / 18000f;
        }else if(gameSecondsInDay >= 79200f || gameSecondsInDay < 18000f){//79200 = 22:00, 18000 = 5:00 //Fully dark
            lightIntensity = 0.2f;
        }else if(gameSecondsInDay >= 18000f && gameSecondsInDay < 36000f){//18000 = 5:00, 36000 = 10:00 //Fade to bright
            lightIntensity = 0.2f + (gameSecondsInDay - 18000f) * 0.8f / 18000f;
        }

        return lightIntensity;
}

    private Color CalculateLightColor(){
        Color lightColor = new Color(1f, 1f, 1f);
        float valueMultiplier = 0f;

        //Recreate above code using gameSecondsInDay (0-86400)
        if(gameSecondsInDay >= 36000f && gameSecondsInDay < 61200f){ //36000 = 10:00, 61200 = 17:00 //White light
            lightColor = new Color(1f, 1f, 1f);
        }else if(gameSecondsInDay >= 61200f && gameSecondsInDay < 77400f){//61200 = 17:00, 77400f = 21:30 //White to orange fade
            valueMultiplier = (gameSecondsInDay - 61200f) / 16200f;
            lightColor = new Color(1f, 1f, 1f) - (new Color(0f, 0.5f, 1f) * valueMultiplier);
            eveningLightComponent.intensity = 0.15f * valueMultiplier;
            eveningLight.transform.position = new Vector3(-300f + 540f*valueMultiplier, eveningLight.transform.position.y, eveningLight.transform.position.z);
        }else if(gameSecondsInDay >= 77400f && gameSecondsInDay < 79200f){//77400f = 21:30, 79200 = 22:00 //Orange to white fade
            valueMultiplier = (79200f - gameSecondsInDay) / 1800f;
            lightColor = new Color(1f, 1f, 1f) - (new Color(0f, 0.5f, 1f) * valueMultiplier);
            eveningLightComponent.intensity = 0.15f * valueMultiplier;
            eveningLight.transform.position = new Vector3(240f + 60f*(1-valueMultiplier),eveningLight.transform.position.y, eveningLight.transform.position.z);
        }else if(gameSecondsInDay >= 79200f || gameSecondsInDay < 18000f){//79200 = 22:00, 18000 = 5:00 //White light
            lightColor = new Color(1f, 1f, 1f);
        }else if(gameSecondsInDay >= 18000f && gameSecondsInDay < 19800f){//18000 = 5:00, 19800 = 5:30 //White to orange fade
            lightColor = new Color(1f, 1f, 1f) - new Color(0f, 0.5f, 1f) * (gameSecondsInDay - 18000f) / 1800f;
        }else if(gameSecondsInDay >= 19800f && gameSecondsInDay < 36000f){//19800 = 5:30, 36000 = 10:00 //Orange to white fade
            lightColor = new Color(1f, 1f, 1f) - new Color(0f, 0.5f, 1f) * (36000f - gameSecondsInDay) / 16200f;
        }

        return lightColor;
    }

    //Note for myself: //32400f = 9 hours 50400f = 14 hours
    private float CalculateOutsideTemp(){
        float temperature = 0f;

        //Recreate above code using gameSecondsInDay (0-86400)
        if(gameSecondsInDay >= 0f && gameSecondsInDay < 18000f){ //0 = 0:00, 18000 = 5:00 //DroppingTemp
            //Lerp from maxTempYesterday to minTempToday
            temperature = Mathf.Lerp(maxTempYesterday, minTempToday, ((32400f + gameSecondsInDay) / 50400f));
        }else if(gameSecondsInDay >= 18000f && gameSecondsInDay < 54000f){ //18000 = 5:00, 54000 = 15:00 //RisingTemp
            //Lerp from minTempToday to maxTempToday
            temperature = Mathf.Lerp(minTempToday, maxTempToday, ((gameSecondsInDay - 18000f) / 36000f));
        }else if(gameSecondsInDay >= 54000f && gameSecondsInDay < 86400f){ //54000 = 15:00, 86400 = 24:00 //DroppingTemp
            //Lerp from maxTempToday to minTempTomorrow
            temperature = Mathf.Lerp(maxTempToday, minTempTomorrow, ((gameSecondsInDay - 54000f) / 32400f));
        }


        return temperature;
    }

    private void GameTimeFlow(){

        second += Time.deltaTime * timeMultiplier;

        gameSecondsInDay += Time.deltaTime * timeMultiplier;
        secondsFromStart += Time.deltaTime * timeMultiplier;
        if(gameSecondsInDay >= 86400f){
            gameSecondsInDay = 0f;
            UpdateDate();
        }
       

        UpdateTime();  
    }  

    private void UpdateTime(){
        hour = (int)(gameSecondsInDay / 3600f);
        minute = (int)((gameSecondsInDay - (hour * 3600f)) / 60f);
    }

    private void UpdateDate(){
        day += 1;
        if(day > daysInMonth[month]){
            day = 1;
            month += 1;
            if(month > 12){
                month = 1;
                year += 1;
            }
        }
        GenerateNewTemperatureValues();
    }

    public void StopTime(){
        Time.timeScale = 0f;
    }

    public void StartTime(){
        Time.timeScale = 1f;
    }

    public void SetTimeMultiplier(float newTimeMultiplier){
        timeMultiplier = newTimeMultiplier;
    }

    private void GenerateNewTemperatureValues(){

        averageTempTomorrow = Random.Range(averageTempToday - maxTempDifferenceBetweenDays, averageTempToday + maxTempDifferenceBetweenDays);

        maxTempYesterday = maxTempToday;
        minTempYesterday = minTempToday;
        maxTempToday = maxTempTomorrow;
        minTempToday = minTempTomorrow;

        averageTempYesterday = (maxTempYesterday + minTempYesterday) / 2f;
        averageTempToday = (maxTempToday + minTempToday) / 2f;

        float tempFluctuationToday = Random.Range(-maxTempDifferenceInDay, maxTempDifferenceInDay);

        maxTempTomorrow = averageTempTomorrow + tempFluctuationToday;
        minTempTomorrow = averageTempTomorrow - tempFluctuationToday;

        if(averageTempTomorrow < monthlyAverageTemp[month]){
            tempBoostTowardsAverage = true;
        }else{
            tempReductionTowardsAverage = true;
        }



        if(tempBoostTowardsAverage){
            maxTempTomorrow += 1f;
            minTempTomorrow += 1f;
            tempBoostTowardsAverage = false;
        }

        if(tempReductionTowardsAverage){
            maxTempTomorrow -= 1f;
            minTempTomorrow -= 1f;
            tempReductionTowardsAverage = false;
        }



    }

    private void InitializeTemp(){
        averageTempToday = monthlyAverageTemp[month];
        averageTempTomorrow = monthlyAverageTemp[month];

        maxTempYesterday = averageTempToday + maxTempDifferenceInDay;
        minTempYesterday = averageTempToday - maxTempDifferenceInDay;
        maxTempToday = averageTempToday + maxTempDifferenceInDay;
        minTempToday = averageTempToday - maxTempDifferenceInDay;
        maxTempTomorrow = averageTempTomorrow + maxTempDifferenceInDay;
        minTempTomorrow = averageTempTomorrow - maxTempDifferenceInDay;
    }
}


