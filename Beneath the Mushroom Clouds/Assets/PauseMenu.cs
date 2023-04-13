using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{


    public void TogglePauseMenu()
    {
        gameObject.SetActive(!gameObject.activeSelf);
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
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
