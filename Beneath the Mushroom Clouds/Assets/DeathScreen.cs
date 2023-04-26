using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeathScreen : MonoBehaviour
{
    private WorldStatus worldStatus;

    private GameObject background;
    private GameObject youDiedtext;
    private GameObject causeOfDeathText;
    private GameObject causeOfDeath;
    private GameObject youSurvivedForText;
    private GameObject timeSurvived;
    private GameObject restartButton;
    private GameObject quitGameButton;

    private float fadeTimer = 0f;
    private float fadeTime = 2f;

    private int currentlyFadingIndex = 0;

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

    public void ActivateDeathScreen(string causeOfDeathString){
        causeOfDeath.GetComponent<TextMeshProUGUI>().text = causeOfDeathString;
        int daysSurvived = Mathf.FloorToInt(worldStatus.secondsFromStart / 86400);
        int hoursSurvived = Mathf.FloorToInt((worldStatus.secondsFromStart - (daysSurvived * 86400)) / 3600);
        int minutesSurvived = Mathf.FloorToInt((worldStatus.secondsFromStart - (daysSurvived * 86400) - (hoursSurvived * 3600)) / 60);
        timeSurvived.GetComponent<TextMeshProUGUI>().text = daysSurvived + " days " + hoursSurvived + " hours " + minutesSurvived + " minutes";
        gameObject.SetActive(true);
    }

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

    public void RestartButton(){
        Application.LoadLevel(Application.loadedLevel);
    }

    public void QuitGame(){
        Application.Quit();
    }
}
