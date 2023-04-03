using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RestMenu : MonoBehaviour
{

    private PlayerStatus playerStatus;

    private WorldStatus worldStatus;
    private HUDController hudController;

    //Left side (specific time) rest menu
    private int hours = 1;
    [SerializeField] private TextMeshProUGUI hoursText;
    [SerializeField] private TextMeshProUGUI forXHoursText;

    [SerializeField] private TextMeshProUGUI hungerCurrentValue;
    [SerializeField] private TextMeshProUGUI hungerAfterValue;
    [SerializeField] private TextMeshProUGUI thirstCurrentValue;
    [SerializeField] private TextMeshProUGUI thirstAfterValue;
    [SerializeField] private TextMeshProUGUI tirednessCurrentValue;
    [SerializeField] private TextMeshProUGUI tirednessAfterValue;

    //Right side (until rested) rest menu
    [SerializeField] private TextMeshProUGUI estimatedTimeText;

    [SerializeField] private TextMeshProUGUI untilRestedHungerCurrentValue;
    [SerializeField] private TextMeshProUGUI untilRestedHungerAfterValue;
    [SerializeField] private TextMeshProUGUI untilRestedThirstCurrentValue;
    [SerializeField] private TextMeshProUGUI untilRestedThirstAfterValue;
    [SerializeField] private TextMeshProUGUI untilRestedTirednessCurrentValue;
    [SerializeField] private TextMeshProUGUI untilRestedTirednessAfterValue;

    private float hungerDifference;
    private float thirstDifference;
    private float tirednessDifference;

    void Awake(){
        playerStatus = GameObject.Find("Player").GetComponent<PlayerStatus>();
        worldStatus = GameObject.Find("GameManager").GetComponent<WorldStatus>();
        hudController = GameObject.Find("HUD").GetComponent<HUDController>();
        UpdateSleepEffects();
        UpdateForHoursText();
    }

    void Update(){
        UpdateSleepEffects();
    }

    
    public void AddHour(){
        hours++;
        if(hours > 8)
            hours = 8;
        UpdateForHoursText();
    }

    public void RemoveHour(){
        hours--;
        if(hours < 1)
            hours = 1;
        UpdateForHoursText();
    }

    void UpdateSleepEffects(){
        //Round values
        int currentHunger = (int)playerStatus.playerHunger;
        int currentThirst = (int)playerStatus.playerThirst;
        int currentTiredness = (int)playerStatus.playerTiredness;

        hungerCurrentValue.text = currentHunger.ToString();
        thirstCurrentValue.text = currentThirst.ToString();
        tirednessCurrentValue.text = currentTiredness.ToString();

        untilRestedHungerCurrentValue.text = currentHunger.ToString();
        untilRestedThirstCurrentValue.text = currentThirst.ToString();
        untilRestedTirednessCurrentValue.text = currentTiredness.ToString();

        //Calculate after values
        int hungerAfter = (int)CalculateAfterValue(playerStatus.playerHunger, hours, playerStatus.hungerDrain);
        int thirstAfter = (int)CalculateAfterValue(playerStatus.playerThirst, hours, playerStatus.thirstDrain);
        int tirednessAfter = (int)CalculateAfterValue(playerStatus.playerTiredness, hours, -playerStatus.baseTirednessRegen);

        hungerAfterValue.text = hungerAfter.ToString();
        thirstAfterValue.text = thirstAfter.ToString();
        tirednessAfterValue.text = tirednessAfter.ToString();

        float wellRestedTime = WellRestedTimeInRealSeconds(playerStatus.playerTiredness, playerStatus.baseTirednessRegen);

        float hoursUntilRested = Mathf.RoundToInt(RealSecondsToGameHours(wellRestedTime));

        int untilRestedHungerAfter = (int)CalculateAfterValue(playerStatus.playerHunger, hoursUntilRested, playerStatus.hungerDrain);
        int untilRestedThirstAfter = (int)CalculateAfterValue(playerStatus.playerThirst, hoursUntilRested, playerStatus.thirstDrain);
        int untilRestedTirednessAfter = (int)CalculateAfterValue(playerStatus.playerTiredness, hoursUntilRested, -playerStatus.baseTirednessRegen);

        untilRestedHungerAfterValue.text = untilRestedHungerAfter.ToString();
        untilRestedThirstAfterValue.text = untilRestedThirstAfter.ToString();
        untilRestedTirednessAfterValue.text = "100";

        estimatedTimeText.text = "(~" + hoursUntilRested.ToString() + " hours)";

        hungerDifference = currentHunger - hungerAfter;
        thirstDifference = currentThirst - thirstAfter;
        tirednessDifference = tirednessAfter - currentTiredness;




    }

    private float GameHoursToRealSeconds(float hours){
        return (hours/20) * 60 * 60;
    }

    private float RealSecondsToGameHours(float seconds){
        return (seconds / 60 / 60) * 20;
    }

    //Use negative drain for tiredness
    private float CalculateAfterValue(float currentValue, float hours, float drain){
        float afterValue = currentValue - (GameHoursToRealSeconds(hours) * drain);
        if(afterValue < 0){
            afterValue = 0;
        }
        if(afterValue > 100){
            afterValue = 100;
        }
        return afterValue;
    }

    private float WellRestedTimeInRealSeconds(float currentTiredness, float regen){
        float wellRestedTime = 0;
        float pointsToRegen = 100f - currentTiredness;
        wellRestedTime = pointsToRegen / regen;
        return wellRestedTime;
    }
    
    void UpdateForHoursText(){
        hoursText.text = hours.ToString();
        if(hours == 1){
            forXHoursText.text = "For   hour";
        } else {
            forXHoursText.text = "For   hours";
        }
    }

    public void RestForXHours(){
        hudController.Rest(hours);
    }

    public void RestUntilRested(){
        float wellRestedTime = WellRestedTimeInRealSeconds(playerStatus.playerTiredness, playerStatus.baseTirednessRegen);
        float hoursUntilRested = RealSecondsToGameHours(wellRestedTime);
        hudController.Rest(hours);
    }

    public void ApplySleepEffects(){
        playerStatus.IncreaseHunger(-hungerDifference);
        playerStatus.IncreaseThirst(-thirstDifference);
        playerStatus.IncreaseTiredness(tirednessDifference);
    }

    



}
