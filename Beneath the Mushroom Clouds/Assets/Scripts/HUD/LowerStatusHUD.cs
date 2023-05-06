using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controls the behviour of the lower status HUD (Hunger, thirst, tiredness and stance)
/// </summary>
public class LowerStatusHUD : MonoBehaviour
{
    /// <summary>
    /// Reference to the fill image of the hunger bar.
    /// </summary>
    private Image hungerBarFillImage;

    /// <summary>
    /// Reference to the fill image of the thirst bar.
    /// </summary>
    private Image thirstBarFillImage;

    /// <summary>
    /// Reference to the fill image of the tiredness bar.
    /// </summary>
    private Image tirednessBarFillImage;

    /// <summary>
    /// Reference to the text (current value) of the hunger bar.
    /// </summary>
    private TextMeshProUGUI hungerBarText;

    /// <summary>
    /// Reference to the text (current value) of the thirst bar.
    /// </summary>
    private TextMeshProUGUI thirstBarText;

    /// <summary>
    /// Reference to the text (current value) of the tiredness bar.
    /// </summary>
    private TextMeshProUGUI tirednessBarText;

    /// <summary>
    /// Reference to the stance image.
    /// </summary>
    private RawImage stanceImage;

    /// <summary>
    /// Texture for standing stance.
    /// </summary>
    [SerializeField] private Texture stanceStandingTexture;

    /// <summary>
    /// Texture for crouching stance.
    /// </summary>
    [SerializeField] private Texture stanceCrouchingTexture;


    /// <summary>
    /// Reference to the player status.
    /// </summary>
    private PlayerStatus status;

    /// <summary>
    /// Dictionary which maps the status names to their respective colors.
    /// </summary>
    /// <typeparam name="string">Status value name.</typeparam>
    /// <typeparam name="Color32">Color of the status.</typeparam>
    private Dictionary<string, Color32> statusColors = new Dictionary<string, Color32>(){
        {"hunger", new Color32(168, 106, 0, 255)},
        {"thirst", new Color32(112, 255, 240, 255)},
        {"tiredness", new Color32(137, 0, 63, 255)}
    };


    /// <summary>
    /// Gets all of the needed references.
    /// </summary>
    void Awake()
    {
        hungerBarFillImage = GameObject.Find("HungerBarFill").GetComponent<Image>();
        thirstBarFillImage = GameObject.Find("ThirstBarFill").GetComponent<Image>();
        tirednessBarFillImage = GameObject.Find("TirednessBarFill").GetComponent<Image>();

        hungerBarText = GameObject.Find("HungerBarText").GetComponent<TextMeshProUGUI>();
        thirstBarText = GameObject.Find("ThirstBarText").GetComponent<TextMeshProUGUI>();
        tirednessBarText = GameObject.Find("TirednessBarText").GetComponent<TextMeshProUGUI>();

        stanceImage = GameObject.Find("Stance").GetComponent<RawImage>();

        status = GameObject.Find("Player").GetComponent<PlayerStatus>();

    }

    /// <summary>
    /// Updates the values of the status bars and stance based on the player's status. Adjusts the color of the bars and text based on the value.
    /// </summary>
    void Update()
    {


        hungerBarFillImage.fillAmount = status.playerHunger / 100;
        thirstBarFillImage.fillAmount = status.playerThirst / 100;
        tirednessBarFillImage.fillAmount = status.playerTiredness / 100;

        if(status.playerHunger <= 20){
            hungerBarFillImage.color = Color.red;
            hungerBarText.color = Color.red;
        }else{
            //Brown
            hungerBarFillImage.color = statusColors["hunger"];
            hungerBarText.color = Color.white;
        }

        if(status.playerThirst <= 20){
            thirstBarFillImage.color = Color.red;
            thirstBarText.color = Color.red;
        }else{
            //Blue
            thirstBarFillImage.color = statusColors["thirst"];
            thirstBarText.color = Color.white;
        }

        if(status.playerTiredness <= 20){
            tirednessBarFillImage.color = Color.red;
            tirednessBarText.color = Color.red;
        }else{
            //Purple
            tirednessBarFillImage.color = statusColors["tiredness"];
            tirednessBarText.color = Color.white;
        }


        hungerBarText.text = status.playerHunger.ToString("F0");
        thirstBarText.text = status.playerThirst.ToString("F0");
        tirednessBarText.text = status.playerTiredness.ToString("F0");
    }

    /// <summary>
    /// Sets the stance texture to crouching.
    /// </summary>
    public void StanceCrouch(){
        stanceImage.texture = stanceCrouchingTexture;
    }

    /// <summary>
    /// Sets the stance texture to standing.
    /// </summary>
    public void StanceStand(){
        stanceImage.texture = stanceStandingTexture;
    }


    
}
