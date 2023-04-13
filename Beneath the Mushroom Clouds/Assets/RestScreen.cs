using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RestScreen : MonoBehaviour
{
    private HUDController hudController;
    private WorldStatus worldStatus;
    
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI dateText;
    [SerializeField] private TextMeshProUGUI restingText;



    void Awake(){
        hudController = GameObject.Find("HUD").GetComponent<HUDController>();
        worldStatus = GameObject.Find("GameManager").GetComponent<WorldStatus>();
    }

    void Update()
    {
        UpdateTextElements();
    }


    private void UpdateTextElements(){
        timeText.text = worldStatus.hour.ToString("00") + ":" + worldStatus.minute.ToString("00");
        dateText.text = worldStatus.day.ToString("00") + "/" + worldStatus.month.ToString("00") + "/" + "28";
    }
       
    public void SetAlpha(float alpha)
    {
        background.color = new Color(0,0,0, alpha);
        timeText.color = new Color(1,1,1, alpha);
        dateText.color = new Color(1,1,1, alpha);
        restingText.color = new Color(1,1,1, alpha);

    }


}
