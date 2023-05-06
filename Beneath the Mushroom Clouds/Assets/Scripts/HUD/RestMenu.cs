using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Defines the behaviour of the rest menu which appears after the player interacts with a bed.
/// </summary>
public class RestMenu : MonoBehaviour
{

    /// <summary>
    /// Reference to the player status.
    /// </summary>
    private PlayerStatus playerStatus;

    /// <summary>
    /// Reference to the world status.
    /// </summary>
    private WorldStatus worldStatus;

    /////////Left side (specific time) rest menu/////////
    /// <summary>
    /// Amount of hours to rest.
    /// </summary>
    private int hours = 1;

    /// <summary>
    /// Reference to the text which shows the amount of hours to rest.
    /// </summary>
    [SerializeField] private TextMeshProUGUI hoursText;

    /// <summary>
    /// The "For X hours" text. This is adjusted if there is only one hour to rest.
    /// </summary>
    [SerializeField] private TextMeshProUGUI forXHoursText;

    /// <summary>
    /// Current value of hunger. Specified time part of the rest menu.
    /// </summary>
    [SerializeField] private TextMeshProUGUI specifiedTimeHungerCurrentValue;
    /// <summary>
    /// Value of hunger after resting. Specified time part of the rest menu.
    /// </summary>
    [SerializeField] private TextMeshProUGUI specifiedTimeHungerAfterValue;
    /// <summary>
    /// Current value of thirst. Specified time part of the rest menu.
    /// </summary>
    [SerializeField] private TextMeshProUGUI specifiedTimeThirstCurrentValue;
    /// <summary>
    /// Value of thirst after resting. Specified time part of the rest menu.
    /// </summary>
    [SerializeField] private TextMeshProUGUI specifiedTimeThirstAfterValue;
    /// <summary>
    /// Current value of tiredness. Specified time part of the rest menu.
    /// </summary>
    [SerializeField] private TextMeshProUGUI specifiedTimeTirednessCurrentValue;
    /// <summary>
    /// Value of tiredness after resting. Specified time part of the rest menu.
    /// </summary>
    [SerializeField] private TextMeshProUGUI specifiedTimeTirednessAfterValue;


    /////////Right side (until rested) rest menu/////////
    /// <summary>
    /// The estimated time to rest text element.
    /// </summary>
    [SerializeField] private TextMeshProUGUI estimatedTimeText;
    /// <summary>
    /// Current value of hunger. Until rested part of the rest menu.
    /// </summary>
    [SerializeField] private TextMeshProUGUI untilRestedHungerCurrentValue;
    /// <summary>
    /// Value of hunger after resting. Until rested part of the rest menu.
    /// </summary>
    [SerializeField] private TextMeshProUGUI untilRestedHungerAfterValue;
    /// <summary>
    /// Current value of thirst. Until rested part of the rest menu.
    /// </summary>
    [SerializeField] private TextMeshProUGUI untilRestedThirstCurrentValue;
    /// <summary>
    /// Value of thirst after resting. Until rested part of the rest menu.
    /// </summary>
    [SerializeField] private TextMeshProUGUI untilRestedThirstAfterValue;
    /// <summary>
    /// Current value of tiredness. Until rested part of the rest menu.
    /// </summary>
    [SerializeField] private TextMeshProUGUI untilRestedTirednessCurrentValue;
    /// <summary>
    /// Value of tiredness after resting. Until rested part of the rest menu.
    /// </summary>
    [SerializeField] private TextMeshProUGUI untilRestedTirednessAfterValue;

    /// <summary>
    /// Calculated difference between the current and after values of hunger.
    /// </summary>
    private float hungerDifference;

    /// <summary>
    /// Calculated difference between the current and after values of thirst.
    /// </summary>
    private float thirstDifference;

    /// <summary>
    /// Calculated difference between the current and after values of tiredness.
    /// </summary>
    private float tirednessDifference;

    /// <summary>
    /// Gets all necessary references on awake, updates sleep effects.
    /// </summary>
    void Awake(){
        playerStatus = GameObject.Find("Player").GetComponent<PlayerStatus>();
        worldStatus = GameObject.Find("GameManager").GetComponent<WorldStatus>();
        UpdateRestEffects();
        UpdateForHoursText();
    }

    /// <summary>
    /// Periodically updates the sleep effects each frame as they change.
    /// </summary>
    void Update(){
        UpdateRestEffects();

    }

    /// <summary>
    /// Adds one hour to the specified time of rest.
    /// </summary>
    public void AddHour(){
        hours++;
        if(hours > 8)
            hours = 8;
        UpdateForHoursText();
    }

    /// <summary>
    /// Removes one hour from the specified time of rest.
    /// </summary>
    public void RemoveHour(){
        hours--;
        if(hours < 1)
            hours = 1;
        UpdateForHoursText();
    }

    /// <summary>
    /// Updates the effects of resting on hunger, thirst and tiredness.
    /// </summary>
    void UpdateRestEffects(){
        //Round values
        int currentHunger = (int)playerStatus.playerHunger;
        int currentThirst = (int)playerStatus.playerThirst;
        int currentTiredness = (int)playerStatus.playerTiredness;

        specifiedTimeHungerCurrentValue.text = currentHunger.ToString();
        specifiedTimeThirstCurrentValue.text = currentThirst.ToString();
        specifiedTimeTirednessCurrentValue.text = currentTiredness.ToString();

        untilRestedHungerCurrentValue.text = currentHunger.ToString();
        untilRestedThirstCurrentValue.text = currentThirst.ToString();
        untilRestedTirednessCurrentValue.text = currentTiredness.ToString();

        //Calculate after values
        int hungerAfter = (int)CalculateAfterValue(playerStatus.playerHunger, hours, playerStatus.hungerDrain);
        int thirstAfter = (int)CalculateAfterValue(playerStatus.playerThirst, hours, playerStatus.thirstDrain);
        int tirednessAfter = (int)CalculateAfterValue(playerStatus.playerTiredness, hours, -playerStatus.baseTirednessRegen);

        specifiedTimeHungerAfterValue.text = hungerAfter.ToString();
        specifiedTimeThirstAfterValue.text = thirstAfter.ToString();
        specifiedTimeTirednessAfterValue.text = tirednessAfter.ToString();

        float wellRestedTime = WellRestedTimeInGameSeconds(playerStatus.playerTiredness, playerStatus.baseTirednessRegen);

        float hoursUntilRested = wellRestedTime / 3600f;

        int untilRestedHungerAfter = (int)CalculateAfterValue(playerStatus.playerHunger, hoursUntilRested, playerStatus.hungerDrain);
        int untilRestedThirstAfter = (int)CalculateAfterValue(playerStatus.playerThirst, hoursUntilRested, playerStatus.thirstDrain);
        int untilRestedTirednessAfter = (int)CalculateAfterValue(playerStatus.playerTiredness, hoursUntilRested, -playerStatus.baseTirednessRegen);

        untilRestedHungerAfterValue.text = untilRestedHungerAfter.ToString();
        untilRestedThirstAfterValue.text = untilRestedThirstAfter.ToString();
        untilRestedTirednessAfterValue.text = "100";

        estimatedTimeText.text = "(~" + Mathf.RoundToInt(hoursUntilRested).ToString() + " hours)";

        hungerDifference = currentHunger - hungerAfter;
        thirstDifference = currentThirst - thirstAfter;
        tirednessDifference = tirednessAfter - currentTiredness;




    }


    //Note: Use negative drain for tiredness
    /// <summary>
    /// Calculates the agyer values of a status variable after a specified amount of time, depending on current drain (For tiredness the drain is negative).
    /// </summary>
    /// <param name="currentValue">Current value of the status variable.</param>
    /// <param name="hours">Amount of hours of rest.</param>
    /// <param name="drain">Drain of the status variable per in-game second.</param>
    /// <returns></returns>
    private float CalculateAfterValue(float currentValue, float hours, float drain){
        float afterValue = currentValue - (hours * 3600f * drain);
        if(afterValue < 0){
            afterValue = 0;
        }
        if(afterValue > 100){
            afterValue = 100;
        }
        return afterValue;
    }

    /// <summary>
    /// Calculates the time in seconds it takes to get from the current tiredness to 100 tiredness.
    /// </summary>
    /// <param name="currentTiredness">Current tiredness value.</param>
    /// <param name="regen">Regeneration rate of tiredness when resting per 1 in-game second.</param>
    /// <returns></returns>
    private float WellRestedTimeInGameSeconds(float currentTiredness, float regen){
        float wellRestedTime = 0;
        float pointsToRegen = 100f - currentTiredness;
        wellRestedTime = pointsToRegen / regen;
        return wellRestedTime;
    }
    
    /// <summary>
    /// Updates the "For X hours" text to say "For X hour" if there is only 1 hour.
    /// </summary>
    void UpdateForHoursText(){
        hoursText.text = hours.ToString();
        if(hours == 1){
            forXHoursText.text = "For   hour";
        } else {
            forXHoursText.text = "For   hours";
        }
    }

    /// <summary>
    /// Activates the rest for a specified amount of hours.
    /// </summary>
    public void RestForXHours(){
        playerStatus.Rest(hours);
    }

    /// <summary>
    /// Activates rest until the player is well rested.
    /// </summary>
    public void RestUntilRested(){
        float wellRestedTime = WellRestedTimeInGameSeconds(playerStatus.playerTiredness, playerStatus.baseTirednessRegen);
        float hoursUntilRested = wellRestedTime / 3600f;
        playerStatus.Rest(hoursUntilRested);
    }



    



}
