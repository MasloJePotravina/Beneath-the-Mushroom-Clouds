using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVTrail : MonoBehaviour
{
    private Mesh mesh;
    // Start is called before the first frame update
    void Start()
    {
        
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateFOVShape(Vector3[] vertices, Vector2[] uv, int[] triangles)
    {
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

    }
}
