using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the behaviour and current status of "world" variables such as time, temperature, etc.
/// </summary>
public class WorldStatus : MonoBehaviour
{   
    /// <summary>
    /// Current year. Just last two digits "20XX" is implied. 
    /// </summary>
    public int year = 28; //2028

    /// <summary>
    /// Current month. 1-12.
    /// </summary>
    public int month = 1;

    /// <summary>
    /// Current day of month. 1-31.
    /// </summary>
    public int day = 1;

    /// <summary>
    /// Current hour. 0-23.
    /// </summary>
    public int hour = 6;

    /// <summary>
    /// Current minute. 0-59.
    /// </summary>
    public int minute = 0;

    /// <summary>
    /// Current second. 0-59.
    /// </summary>
    public float second = 0;

    /// <summary>
    /// How much faster does the in-game time flow compared to real time. By default, 20x faster.
    /// </summary>
    public float timeMultiplier = 20f;

    /// <summary>
    /// Counter for seconds in a day. Resets to 0 after 86400 seconds (24 hours).
    /// </summary>
    public float gameSecondsInDay = 0f;

    /// <summary>
    /// How many total seconds have passed since the start of the game.
    /// </summary>
    public float secondsFromStart = 0;

    //Unused currently, ignore
    //[SerializeField] private GameObject eveningLight;
    //[SerializeField] private UnityEngine.Rendering.Universal.Light2D eveningLightComponent;

    /// <summary>
    /// Array of all enemy spawners in the scene.
    /// </summary>
    private EnemySpawner[] enemySpawners;

    /// <summary>
    /// How many days each month has.
    /// </summary>
    /// <typeparam name="int">Month</typeparam>
    /// <typeparam name="int">Amount of days</typeparam>
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

    
    /// <summary>
    /// What day of the week is it. Game starts at 15/5/2028 which is a monday. 
    /// </summary>
    public int dayOfWeek = 0;

    /// <summary>
    /// Intensity of the global light illuminating the entire scene.
    /// </summary>
    public float globalLightIntensity = 0f;

    /// <summary>
    /// Color of the global light illuminating the entire scene.
    /// </summary>
    private Color globalLightColor = new Color(1f, 1f, 1f);

    /// <summary>
    /// Reference to the global light component.
    /// </summary>
    private UnityEngine.Rendering.Universal.Light2D globalLight;

    //Temperature
    /// <summary>
    /// Current outside temperature.
    /// </summary>
    public float outsideTemp = 4f;
    /// <summary>
    /// Maximum difference between average temperatures of two consecutive days, halved.
    /// </summary>
    private float maxTempDifferenceBetweenDays = 2.5f; //Max difference between average temperatures of two consecutive days halved

    /// <summary>
    /// Maximum difference between coldest and warmest temperature in a day, halved.
    /// </summary>
    private float maxTempDifferenceInDay = 5f; //Max difference between coldest and warmest temperature in a day halved

    /// <summary>
    /// Minimum temperature in the previous day.
    /// </summary>
    private float minTempYesterday;

    /// <summary>
    /// Maximum temperature in the previous day.
    /// </summary>
    private float maxTempYesterday;

    /// <summary>
    /// Minimum temperature in the current day.
    /// </summary>
    private float minTempToday;

    /// <summary>
    /// Maximum temperature in the current day.
    /// </summary>
    private float maxTempToday;

    /// <summary>
    /// Minimum temperature in the next day.
    /// </summary>
    private float minTempTomorrow;

    /// <summary>
    /// Maximum temperature in the next day.
    /// </summary>
    private float maxTempTomorrow;

    /// <summary>
    /// Average temperature in the previous day.
    /// </summary>
    private float averageTempYesterday;

    /// <summary>
    /// Average temperature in the current day.
    /// </summary>
    private float averageTempToday;

    /// <summary>
    /// Average temperature in the next day.
    /// </summary>
    private float averageTempTomorrow;
    
    /// <summary>
    /// Whether the game should boost a temperure by a bit if if feviates too far from the expected monthly average.
    /// </summary>
    private bool tempBoostTowardsAverage = false;

    /// <summary>
    /// Whether the game should reduce a temperure by a bit if if feviates too far from the expected monthly average.
    /// </summary>
    private bool tempReductionTowardsAverage = false;


    


    /// <summary>
    /// Dictionary of average temperatures for each month.
    /// </summary>
    /// <typeparam name="int">Month</typeparam>
    /// <typeparam name="float">Average temperature</typeparam>
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

    /// <summary>
    /// Respawn timer for enemies in game seconds.
    /// </summary>
    private float enemyRespawnTimerInGameSeconds = 18000f;

    
    /// <summary>
    /// Get necessary references, initialize temperature and respawn enemies at the start.
    /// </summary>
    void Start()
    {
        globalLight = GameObject.Find("GlobalLight").GetComponent<UnityEngine.Rendering.Universal.Light2D>();
        Time.timeScale = 1f;
        InitializeTemp();
        enemySpawners = FindObjectsOfType<EnemySpawner>();

        RespawnEnemies(true);
    }
    
    /// <summary>
    /// Each frame update time, outside temperature, intensity and color of the global light and respawn enemies if the time has come.
    /// </summary>
    void Update()
    {
        GameTimeFlow();
        outsideTemp = CalculateOutsideTemp();
        globalLight.intensity = CalculateLightIntensity();
        globalLight.color = CalculateLightColor();

        if(enemyRespawnTimerInGameSeconds > 18000f){
            RespawnEnemies(false);
            enemyRespawnTimerInGameSeconds = 0f;
        }
        enemyRespawnTimerInGameSeconds += Time.deltaTime * timeMultiplier;
        
    }

    /// <summary>
    /// Calculates the light intensity of the global light (sun) based on the time of day.
    /// </summary>
    /// <returns>Intensity of the global light at this time of day (0-1).</returns>
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

    /// <summary>
    /// Calculates the light color of the global light (sun) based on the time of day. Responsible for orange shade of evening and morning.
    /// </summary>
    /// <returns></returns>
    private Color CalculateLightColor(){
        Color lightColor = new Color(1f, 1f, 1f);
        float valueMultiplier = 0f;

        //Recreate above code using gameSecondsInDay (0-86400)
        if(gameSecondsInDay >= 36000f && gameSecondsInDay < 61200f){ //36000 = 10:00, 61200 = 17:00 //White light
            lightColor = new Color(1f, 1f, 1f);
        }else if(gameSecondsInDay >= 61200f && gameSecondsInDay < 77400f){//61200 = 17:00, 77400f = 21:30 //White to orange fade
            valueMultiplier = (gameSecondsInDay - 61200f) / 16200f;
            lightColor = new Color(1f, 1f, 1f) - (new Color(0f, 0.5f, 1f) * valueMultiplier);
        }else if(gameSecondsInDay >= 77400f && gameSecondsInDay < 79200f){//77400f = 21:30, 79200 = 22:00 //Orange to white fade
            valueMultiplier = (79200f - gameSecondsInDay) / 1800f;
            lightColor = new Color(1f, 1f, 1f) - (new Color(0f, 0.5f, 1f) * valueMultiplier);
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
    /// <summary>
    /// Calculates the current outside temperature.
    /// </summary>
    /// <returns>Current outside temperature in celsius.</returns>
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

    /// <summary>
    /// Manages the flow of in-game time.
    /// </summary>
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

    /// <summary>
    /// Updates hours and minutes of the day.
    /// </summary>
    private void UpdateTime(){
        hour = (int)(gameSecondsInDay / 3600f);
        minute = (int)((gameSecondsInDay - (hour * 3600f)) / 60f);
    }

    /// <summary>
    /// Updates the date.
    /// </summary>
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

    /// <summary>
    /// Stops the flow of time.
    /// </summary>
    public void StopTime(){
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Starts the flow of time.
    /// </summary>
    public void StartTime(){
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Sets the time multiplier to a new value.
    /// </summary>
    /// <param name="newTimeMultiplier">New time multiplier value.</param>
    public void SetTimeMultiplier(float newTimeMultiplier){
        timeMultiplier = newTimeMultiplier;
    }

    /// <summary>
    /// Generates new temperature values for the next day based on previous temperatures, month average and so on.
    /// </summary>
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

    /// <summary>
    /// Initializes the temperature values for the first day.
    /// </summary>
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

    /// <summary>
    /// Calls all enemy spawners to respawn enemies if possible.
    /// </summary>
    /// <param name="initialSpawn"></param>
    private void RespawnEnemies(bool initialSpawn){
        

        foreach(EnemySpawner enemySpawner in enemySpawners){
            enemySpawner.SpawnEnemy(initialSpawn);
        }

       
    }

}


