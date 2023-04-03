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

        int hungerInt = (int) status.playerHunger;
        int thirstInt = (int) status.playerThirst;
        int tirednessInt = (int) status.playerTiredness;

        hungerBarFillImage.fillAmount = status.playerHunger / 100;
        thirstBarFillImage.fillAmount = status.playerThirst / 100;
        tirednessBarFillImage.fillAmount = status.playerTiredness / 100;

        if(hungerInt <= 20){
            hungerBarFillImage.color = new Color32(255, 0, 0, 255);
        }else{
            //Brown
            hungerBarFillImage.color = new Color32(168, 106, 0, 255);
        }

        if(thirstInt <= 20){
            thirstBarFillImage.color = new Color32(255, 0, 0, 255);
        }else{
            //Blue
            thirstBarFillImage.color = new Color32(112, 255, 240, 255);
        }

        if(tirednessInt <= 20){
            tirednessBarFillImage.color = new Color32(255, 0, 0, 255);
        }else{
            //Purple
            tirednessBarFillImage.color = new Color32(137, 0, 63, 255);
        }


        hungerBarText.text = hungerInt.ToString();
        thirstBarText.text = thirstInt.ToString();
        tirednessBarText.text = tirednessInt.ToString();
    }

    public void StanceCrouch(){
        stanceImage.texture = stanceCrouchingTexture;
    }

    public void StanceStand(){
        stanceImage.texture = stanceStandingTexture;
    }


    
}
