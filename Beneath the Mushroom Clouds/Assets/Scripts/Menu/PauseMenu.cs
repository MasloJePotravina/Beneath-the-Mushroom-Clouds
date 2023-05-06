using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Implements the behaviour of a pause menu.
/// </summary>
public class PauseMenu : MonoBehaviour
{   
    /// <summary>
    /// Whether the game is paused.
    /// </summary>
    public bool isPaused = false;

    /// <summary>
    /// Previous action map, before the game was paused.
    /// </summary>
    private string prevActionMap = "Player";

    /// <summary>
    /// Reference to the cursor controller.
    /// </summary>
    private CursorController cursorController;

    /// <summary>
    /// Gets the reference to the cursor controller on awake.
    /// </summary>
    void Awake(){
        cursorController = GameObject.FindObjectOfType<CursorController>(true);
    }

    /// <summary>
    /// Toggles the pause menu.
    /// </summary>
    public void TogglePauseMenu()
    {
        PlayerInput playerInput = GameObject.FindObjectOfType<PlayerInput>(true);
        gameObject.SetActive(!gameObject.activeSelf);
        if(isPaused){
            Time.timeScale = 1;
            isPaused = false;
            playerInput.SwitchCurrentActionMap(prevActionMap);
            //Confine the cursor to the screen and switch to the correct cursor based on the previous action map
            Cursor.lockState = CursorLockMode.Confined;
            if(prevActionMap == "Player"){
                cursorController.SwitchToCrosshairCursor();
            }else{
                cursorController.SwitchToDefaultCursor();
            }
        }else{
            prevActionMap = playerInput.currentActionMap.name;
            Time.timeScale = 0;
            isPaused = true;
            playerInput.SwitchCurrentActionMap("UI");
            //Unlock the cursor, so that it can leave the game window and switch to the default cursor
            Cursor.lockState = CursorLockMode.None;
            cursorController.SwitchToDefaultCursor();
        }
        
    }

    /// <summary>
    /// Resume the game.
    /// </summary>
    public void Resume()
    {
        TogglePauseMenu();
    }

    /// <summary>
    /// Open the options menu in pause menu.
    /// </summary>
    public void Options(){
        this.gameObject.transform.Find("PauseMenuMain").gameObject.SetActive(false);
        this.gameObject.transform.Find("PauseMenuOptions").gameObject.SetActive(true);
    }

    /// <summary>
    /// Return to the main menu.
    /// </summary>
    public void MainMenu()
    {
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Quit the game.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Go back from the options menu to the pause menu.
    /// </summary>
    public void Back(){
        this.gameObject.transform.Find("PauseMenuMain").gameObject.SetActive(true);
        this.gameObject.transform.Find("PauseMenuOptions").gameObject.SetActive(false);
    }
}
