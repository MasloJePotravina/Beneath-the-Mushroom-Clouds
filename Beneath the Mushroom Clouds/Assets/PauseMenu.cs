using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public bool isPaused = false;
    private string prevActionMap = "Player";

    public void TogglePauseMenu()
    {
        PlayerInput playerInput = GameObject.FindObjectOfType<PlayerInput>(true);
        gameObject.SetActive(!gameObject.activeSelf);
        if(isPaused){
            Time.timeScale = 1;
            isPaused = false;
            playerInput.SwitchCurrentActionMap(prevActionMap);
        }else{
            prevActionMap = playerInput.currentActionMap.name;
            Time.timeScale = 0;
            isPaused = true;
            playerInput.SwitchCurrentActionMap("UI");
        }
        
    }

    public void Resume()
    {
        TogglePauseMenu();
    }

    public void Options(){
        this.gameObject.transform.Find("PauseMenuMain").gameObject.SetActive(false);
        this.gameObject.transform.Find("PauseMenuOptions").gameObject.SetActive(true);
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Back(){
        this.gameObject.transform.Find("PauseMenuMain").gameObject.SetActive(true);
        this.gameObject.transform.Find("PauseMenuOptions").gameObject.SetActive(false);
    }
}
