using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuController : MonoBehaviour
{

    

    public void OnDemoLevelButton()
    {
        StartCoroutine(LoadScene("TestScene"));
    }


    public void OnOptionsButton()
    {
        Debug.Log("Options button clicked");
    }

    public void OnExitButton()
    {
        Application.Quit();
    }

    IEnumerator LoadScene(string levelName)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(levelName);
        while (!async.isDone)
        {
            yield return null;
        }
    }
}
