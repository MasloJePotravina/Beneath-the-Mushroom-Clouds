using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCStatus : MonoBehaviour
{

    public bool isCrouched = false;
    private bool hitByFov = false;

    private SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (hitByFov)
        {
            spriteRenderer.enabled = true;
        }
        else
        {
            spriteRenderer.enabled = false;
        }
        hitByFov = false;
    }

    public void HitByFov()
    {
        hitByFov = true;
    }
    
}
