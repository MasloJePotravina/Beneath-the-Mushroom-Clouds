using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LowerStatusHUD : MonoBehaviour
{
    private Image hungerBarFillImage;
    private Image thirstBarFillImage;
    private Image tirednessBarFillImage;


    private TextMeshProUGUI hungerBarText;
    private TextMeshProUGUI thirstBarText;
    private TextMeshProUGUI tirednessBarText;

    private RawImage stanceImage;

    /// <summary>
    /// Texture for standing stance
    /// </summary>
    [SerializeField] private Texture stanceStandingTexture;

    /// <summary>
    /// Texture for crouching stance
    /// </summary>
    [SerializeField] private Texture stanceCrouchingTexture;

    private PlayerStatus status;



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


    void Update()
    {


        hungerBarFillImage.fillAmount = status.playerHunger / 100;
        thirstBarFillImage.fillAmount = status.playerThirst / 100;
        tirednessBarFillImage.fillAmount = status.playerTiredness / 100;

        if(status.playerHunger <= 20){
            hungerBarFillImage.color = new Color32(255, 0, 0, 255);
        }else{
            //Brown
            hungerBarFillImage.color = new Color32(168, 106, 0, 255);
        }

        if(status.playerThirst <= 20){
            thirstBarFillImage.color = new Color32(255, 0, 0, 255);
        }else{
            //Blue
            thirstBarFillImage.color = new Color32(112, 255, 240, 255);
        }

        if(status.playerTiredness <= 20){
            tirednessBarFillImage.color = new Color32(255, 0, 0, 255);
        }else{
            //Purple
            tirednessBarFillImage.color = new Color32(137, 0, 63, 255);
        }


        hungerBarText.text = status.playerHunger.ToString("F0");
        thirstBarText.text = status.playerThirst.ToString("F0");
        tirednessBarText.text = status.playerTiredness.ToString("F0");
    }

    public void StanceCrouch(){
        stanceImage.texture = stanceCrouchingTexture;
    }

    public void StanceStand(){
        stanceImage.texture = stanceStandingTexture;
    }


    
}
