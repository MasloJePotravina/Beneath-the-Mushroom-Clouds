using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBarrel : MonoBehaviour
{
    private GameObject fire;
    private UnityEngine.Rendering.Universal.Light2D fireLight;
    public float flickerSpeed = 1f;
    public float intensity = 2f;
    private float timeBetweenValues = 0.5f;
    private float timer = 0f;
    private Vector2 lightPosition;
    // Start is called before the first frame update
    void Awake()
    {
        fire = this.transform.GetChild(0).gameObject;
        fireLight = fire.GetComponent<UnityEngine.Rendering.Universal.Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(timer > timeBetweenValues)
        {
            timer = 0f;
            timeBetweenValues = Random.Range(0.1f, 0.3f);
            intensity = Random.Range(1.5f, 2.5f);
            lightPosition = new Vector2(Random.Range(-0.2f, 0.2f), Random.Range(-0.1f, 0.1f));

        }
        else
        {
            timer += Time.deltaTime;
        }

        flickerSpeed = 1/timeBetweenValues;

        fireLight.intensity = Mathf.Lerp(fireLight.intensity, intensity, Time.deltaTime * flickerSpeed);
        fireLight.transform.localPosition = Vector2.Lerp(fireLight.transform.localPosition, lightPosition, Time.deltaTime * flickerSpeed);


    }
}
