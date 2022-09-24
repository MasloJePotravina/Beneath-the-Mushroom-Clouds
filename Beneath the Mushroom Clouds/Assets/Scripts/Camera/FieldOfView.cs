using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{

    public LayerMask layerMask;
    public GameObject player;

    private Mesh mesh;
    public float fov = 90f;
    public int rayCount = 50;
    public float angle = 0f;
    private float angleIncrease;
    public float viewDistance = 50f;

    // Start is called before the first frame update
    void Start()
    {

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        angleIncrease = fov / rayCount;
    }

    private void Update()
    {
        Vector3 origin = player.transform.position;

        Vector2 aimAngle = player.transform.up;
        float tmpXCoord = aimAngle.x;
        aimAngle.x = -aimAngle.y;
        aimAngle.y = tmpXCoord;

        angle = Vector3.SignedAngle(Vector2.up, aimAngle, Vector3.forward) + fov/2;

        Vector3[] vertices = new Vector3[rayCount + 2]; //Amount of rays + origin point + the "zero" ray
        Vector2[] uv = new Vector2[rayCount + 2];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = origin; //The first vertex is on the player

        int vertexIndex = 1;
        int triangleIndex = 0;
        //For each ray coming from the player wi calculate a new vertex
        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 vertex;
            RaycastHit2D hit = Physics2D.Raycast(origin, new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle)), viewDistance, layerMask);

            if (hit.collider == null)
            {
                vertex = origin + new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle)) * viewDistance;
            }
            else
            {
                vertex = hit.point;
            }


            vertices[vertexIndex] = vertex;

            //After the first ray we are just connecting new triangles to the previous ones
            if (i > 0)
            {
                triangles[triangleIndex] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }
            vertexIndex++;

            angle -= angleIncrease; //Move the angle by the width of one of the triangles forming the fov
        }

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }

}
