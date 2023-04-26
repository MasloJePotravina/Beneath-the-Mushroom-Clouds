using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlickeringEffect : MonoBehaviour
{
    private UnityEngine.Rendering.Universal.Light2D light;
    public float flickerSpeed = 1f;
    public float averageIntensity = 2f;
    public float moveIntensity = 0.2f;
    private float timeBetweenValues = 0.5f;
    private float timer = 0f;
    private Vector2 lightPosition;
    private float intensity = 0f;
    // Start is called before the first frame update
    void Awake()
    {
        light = GetComponent<UnityEngine.Rendering.Universal.Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(timer > timeBetweenValues)
        {
            timer = 0f;
            timeBetweenValues = Random.Range(0.1f, 0.3f);
            intensity = Random.Range(averageIntensity-0.5f, averageIntensity+0.5f);
            lightPosition = new Vector2(Random.Range(-moveIntensity, moveIntensity), Random.Range(-moveIntensity, moveIntensity));

        }
        else
        {
            timer += Time.deltaTime;
        }

        flickerSpeed = 1/timeBetweenValues;

        light.intensity = Mathf.Lerp(light.intensity, intensity, Time.deltaTime * flickerSpeed);
        light.transform.localPosition = Vector2.Lerp(light.transform.localPosition, lightPosition, Time.deltaTime * flickerSpeed);


    }
}
