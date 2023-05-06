using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Controller of the main menu.
/// </summary>
public class MainMenuController : MonoBehaviour
{

    /// <summary>
    /// Reference to the main section of the menu screen.
    /// </summary>
    private GameObject mainMenuMainScreen;

    /// <summary>
    /// Reference to the Options section of the menu screen.
    /// </summary>
    private GameObject mainMenuOptions;

    /// <summary>
    /// Get relevant references on awake. Set the main section of the menu screen to be active.
    /// </summary>
    void Awake(){
        mainMenuMainScreen = transform.Find("MainMenuMainScreen").gameObject;
        mainMenuOptions = transform.Find("MainMenuOptions").gameObject;

        mainMenuMainScreen.SetActive(true);
        mainMenuOptions.SetActive(false);
    } 

    /// <summary>
    /// When a demo level button is pressed, load the demo level.
    /// </summary>
    public void OnDemoLevelButton()
    {
        StartCoroutine(LoadScene("TestScene"));
    }

    /// <summary>
    /// When the options button is pressed, set the main section of the menu screen to be inactive and set the options section to be active.
    /// </summary>
    public void OnOptionsButton()
    {
        mainMenuMainScreen.SetActive(false);
        mainMenuOptions.SetActive(true);
    }

    /// <summary>
    /// When the back button in options menu is pressed, set the main section of the menu screen to be active and set the options section to be inactive.
    /// </summary>
    public void OnOptionsBackButton()
    {
        mainMenuMainScreen.SetActive(true);
        mainMenuOptions.SetActive(false);
    }

    /// <summary>
    /// When the quit game button is pressed, quit the game.
    /// </summary>
    public void OnQuitGameButton()
    {
        Application.Quit();
    }

    /// <summary>
    /// When the load scene button is pressed, asynchronously load the scene.
    /// </summary>
    IEnumerator LoadScene(string levelName)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(levelName);
        while (!async.isDone)
        {
            yield return null;
        }
    }
}
