using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Implements the behaviour of the rest screen that appears when the player rests.
/// </summary>
public class RestScreen : MonoBehaviour
{   
    /// <summary>
    /// Reference to the HUD controller.
    /// </summary>
    private HUDController hudController;

    /// <summary>
    /// Reference to the world status.
    /// </summary>
    private WorldStatus worldStatus;
    
    /// <summary>
    /// Reference ot the background of the rest screen.
    /// </summary>
    /// <returns></returns>
    [SerializeField] private Image background;

    /// <summary>
    /// Reference to the text that shows the time when sleeping.
    /// </summary>
    [SerializeField] private TextMeshProUGUI timeText;

    /// <summary>
    /// Reference to the text that shows the date when sleeping.
    /// </summary>
    [SerializeField] private TextMeshProUGUI dateText;

    /// <summary>
    /// Text displaying "Resting...".
    /// </summary>
    [SerializeField] private TextMeshProUGUI restingText;


    /// <summary>
    /// Gets the necessary references.
    /// </summary>
    void Awake(){
        hudController = GameObject.Find("HUD").GetComponent<HUDController>();
        worldStatus = GameObject.Find("GameManager").GetComponent<WorldStatus>();
    }

    /// <summary>
    /// Updates the text elements each frame.
    /// </summary>
    void Update()
    {
        UpdateTextElements();
    }

    /// <summary>
    /// Updates the text elements of rest screen (time and date).
    /// </summary>
    private void UpdateTextElements(){
        timeText.text = worldStatus.hour.ToString("00") + ":" + worldStatus.minute.ToString("00");
        dateText.text = worldStatus.day.ToString("00") + "/" + worldStatus.month.ToString("00") + "/" + "28";
    }
    
    /// <summary>
    /// Sets the alpha of elements of the rest screen.
    /// </summary>
    /// <param name="alpha">Alpha to set the individual elements to.</param>
    public void SetAlpha(float alpha)
    {
        background.color = new Color(0,0,0, alpha);
        timeText.color = new Color(1,1,1, alpha);
        dateText.color = new Color(1,1,1, alpha);
        restingText.color = new Color(1,1,1, alpha);

    }


}
