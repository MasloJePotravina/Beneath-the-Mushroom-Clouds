using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements a light flickering effect for fires.
/// </summary>
public class LightFlickeringEffect : MonoBehaviour
{
    /// <summary>
    /// Reference to the light component of the object.
    /// </summary>
    private UnityEngine.Rendering.Universal.Light2D light;
    /// <summary>
    /// Speed of the flickering.
    /// </summary>
    public float flickerSpeed = 1f;

    /// <summary>
    /// Average intensity of the light
    /// </summary>
    public float averageIntensity = 2f;

    /// <summary>
    /// How intensively the light moves around
    /// </summary>
    public float moveIntensity = 0.2f;

    /// <summary>
    /// Time between the change of values.
    /// </summary>
    private float timeBetweenValues = 0.5f;

    /// <summary>
    /// Timer for the time between the change of values.
    /// </summary>
    private float timer = 0f;

    /// <summary>
    /// Current position of the light.
    /// </summary>
    private Vector2 lightPosition;

    /// <summary>
    /// Original position of the light.
    /// </summary>
    private Vector2 originalPosition;

    /// <summary>
    /// Current intensity of the light.
    /// </summary>
    private float intensity = 0f;
    
    /// <summary>
    /// Gets the reference to the light component and original position of the light on awake.
    /// </summary>
    void Awake()
    {
        originalPosition = transform.localPosition;
        light = GetComponent<UnityEngine.Rendering.Universal.Light2D>();
    }

    /// <summary>
    /// Each frame randomly alters the intensity and position of the light.
    /// </summary>
    void Update()
    {
        if(timer > timeBetweenValues)
        {
            timer = 0f;
            timeBetweenValues = Random.Range(0.1f, 0.3f);
            intensity = Random.Range(averageIntensity-0.5f, averageIntensity+0.5f);
            lightPosition = new Vector2(originalPosition.x + Random.Range(-moveIntensity, moveIntensity), originalPosition.y + Random.Range(-moveIntensity, moveIntensity));

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
