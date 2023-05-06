using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Manages the death screen when the player dies.
/// </summary>
public class DeathScreen : MonoBehaviour
{

    /// <summary>
    /// Reference to world status.
    /// </summary>
    private WorldStatus worldStatus;

    /// <summary>
    /// Reference to the background of the screen.
    /// </summary>
    private GameObject background;

    /// <summary>
    /// Reference to the "YOU DIED" text shown on death screen.
    /// </summary>
    private GameObject youDiedtext;

    /// <summary>
    /// Reference to the "Cause of death:" text shown on death screen.
    /// </summary>
    private GameObject causeOfDeathText;

    /// <summary>
    /// Reference to the text which shows the cause of death.
    /// </summary>
    private GameObject causeOfDeath;

    /// <summary>
    /// Reference to the "You survived for:" text shown on death screen.
    /// </summary>
    private GameObject youSurvivedForText;

    /// <summary>
    /// Reference to the text which shows the time survived in days, hours and minutes.
    /// </summary>
    private GameObject timeSurvived;

    /// <summary>
    /// Reference to the restart button.
    /// </summary>
    private GameObject restartButton;

    /// <summary>
    /// Reference to the quit game button.
    /// </summary>
    private GameObject quitGameButton;

    /// <summary>
    /// Fade timer for each element.
    /// </summary>
    private float fadeTimer = 0f;

    /// <summary>
    /// Fade time for each element.
    /// </summary>
    private float fadeTime = 2f;

    /// <summary>
    /// Which element is currently fading in. The order is as follows:
    /// 0 - Background, 1 - "YOU DIED" text, 2 - "Cause of death:" text and cause of death, 3 - "You survived for:" text and time survived, 4 - Restart and quit game buttons.
    /// </summary>
    private int currentlyFadingIndex = 0;

    /// <summary>
    /// Get all of the references to the world status and necessary child objects.
    /// </summary>
    void Awake(){
        background = transform.Find("Background").gameObject;
        youDiedtext = transform.Find("YouDied").gameObject;
        causeOfDeathText = transform.Find("CauseOfDeathText").gameObject;
        causeOfDeath = transform.Find("CauseOfDeath").gameObject;
        youSurvivedForText = transform.Find("YouSurvivedForText").gameObject;
        timeSurvived = transform.Find("TimeSurvived").gameObject;
        restartButton = transform.Find("RestartButton").gameObject;
        quitGameButton = transform.Find("QuitGameButton").gameObject;

        

        

        worldStatus = GameObject.FindObjectOfType<WorldStatus>(true);

        DisableElements();

        gameObject.SetActive(false);
        
    }

    /// <summary>
    /// Once activated, starts fading in the elements one by one using the fade timer.
    /// </summary>
    void Update(){
        if(currentlyFadingIndex >= 5){
            return;
        }
        if(fadeTimer < fadeTime){
            fadeTimer += Time.deltaTime;
            FadeElelentIn();
        }else{
            fadeTimer = 0f;
            currentlyFadingIndex++;
        }
    }

    /// <summary>
    /// Initialized values of the death screen and activates it. Called when the player dies.
    /// </summary>
    /// <param name="causeOfDeathString">Cause of the player's death.</param>
    public void ActivateDeathScreen(string causeOfDeathString){
        causeOfDeath.GetComponent<TextMeshProUGUI>().text = causeOfDeathString;
        int daysSurvived = Mathf.FloorToInt(worldStatus.secondsFromStart / 86400);
        int hoursSurvived = Mathf.FloorToInt((worldStatus.secondsFromStart - (daysSurvived * 86400)) / 3600);
        int minutesSurvived = Mathf.FloorToInt((worldStatus.secondsFromStart - (daysSurvived * 86400) - (hoursSurvived * 3600)) / 60);
        timeSurvived.GetComponent<TextMeshProUGUI>().text = daysSurvived + " days " + hoursSurvived + " hours " + minutesSurvived + " minutes";
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Disables all of the elements of the death screen.
    /// </summary>
    private void DisableElements(){
        background.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
        youDiedtext.GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f, 0f);
        causeOfDeathText.GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f, 0f);
        causeOfDeath.GetComponent<TextMeshProUGUI>().color = new Color(0.8f, 0f, 0f, 0f);
        youSurvivedForText.GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f, 0f);
        timeSurvived.GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f, 0f);
        restartButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        restartButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f, 0f);
        restartButton.SetActive(false);
        quitGameButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        quitGameButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f, 0f);
        quitGameButton.SetActive(false);
    }

    /// <summary>
    /// Fades the element in based on the currently fading index.
    /// </summary>
    private void FadeElelentIn(){
        if(currentlyFadingIndex == 0){
            background.GetComponent<Image>().color = new Color(0f, 0f, 0f, fadeTimer/fadeTime);
        }else if(currentlyFadingIndex == 1){
            youDiedtext.GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f, fadeTimer/fadeTime);
        }else if(currentlyFadingIndex == 2){
            causeOfDeathText.GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f, fadeTimer/fadeTime);
            causeOfDeath.GetComponent<TextMeshProUGUI>().color = new Color(0.8f, 0f, 0f, fadeTimer/fadeTime);
        }else if(currentlyFadingIndex == 3){
            youSurvivedForText.GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f, fadeTimer/fadeTime);
            timeSurvived.GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f, fadeTimer/fadeTime);
        }else if(currentlyFadingIndex == 4){
            restartButton.SetActive(true);
            quitGameButton.SetActive(true);
            restartButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, fadeTimer/fadeTime);
            restartButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f, fadeTimer/fadeTime);
            quitGameButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, fadeTimer/fadeTime);
            quitGameButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f, fadeTimer/fadeTime);
        }
    }

    /// <summary>
    /// Called by clicking the restart button of the death screen. Restarts the demo level.
    /// </summary>
    public void RestartButton(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Called by clicking the quit game button of the death screen. Quits the game.
    /// </summary>
    public void QuitGame(){
        Application.Quit();
    }
}
