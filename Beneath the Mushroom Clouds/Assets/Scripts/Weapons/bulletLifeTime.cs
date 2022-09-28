using UnityEngine;

public class BulletLifeTime : MonoBehaviour

{

    private float TTL = 0.05f;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, TTL);
    }
}
