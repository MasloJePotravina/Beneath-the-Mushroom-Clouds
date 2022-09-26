using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{

    public LayerMask layerMask;
    public LayerMask layerMaskAfterNPC;
    public GameObject player;

    public GameObject FOVTrail;
    private FOVTrail fovTrailScript;

    private Mesh mesh;
    public float fov = 90f;
    public int fovRayCount = 50;
    public float angle = 0f;
    private float angleIncrease;
    public float fovDistance = 50f;

    private float fop;
    public int fopRayCount = 50;
    public float fopDistance = 10f;

    // Start is called before the first frame update
    void Start()
    {

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        fovTrailScript = FOVTrail.GetComponent<FOVTrail>();
        
        
    }

    private void Update()
    {
        Vector3 origin = player.transform.position;

        Vector2 aimAngle = player.transform.up;
        float tmpXCoord = aimAngle.x;
        aimAngle.x = -aimAngle.y;
        aimAngle.y = tmpXCoord;

        angle = Vector3.SignedAngle(Vector2.up, aimAngle, Vector3.forward) + fov/2;

        Vector3[] vertices = new Vector3[fovRayCount + 2 + fopRayCount + 1]; //FOV Rays + origin point + the "zero" ray (FOV) + FOP Rays + the "zero" ray(FOP)
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[(fovRayCount + fopRayCount) * 3];

        vertices[0] = origin; //The first vertex is on the player

        int vertexIndex = 1;
        int triangleIndex = 0;
        angleIncrease = fov / fovRayCount;
        CalculateMesh(true, ref origin, ref vertices, ref triangles, ref vertexIndex, ref triangleIndex);

        fop = 360.0f - fov;
        angleIncrease = fop / fopRayCount;
        CalculateMesh(false, ref origin, ref vertices, ref triangles, ref vertexIndex, ref triangleIndex);

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        fovTrailScript.updateFOVShape(vertices, uv, triangles);

    }

    //Calculate mesh for Field Of Vision and Field Of Perception
    //fov == 1 -> calculating FOV
    //fov == 0 -> calculating FOP
    private void CalculateMesh(bool fov, ref Vector3 origin, ref Vector3[] vertices, ref int[] triangles, ref int vertexIndex, ref int triangleIndex)
    {
        int rayCount;
        float distance;
        if (fov)
        {
            rayCount = fovRayCount;
            distance = fovDistance;
        }
        else
        {
            rayCount = fopRayCount;
            distance = fopDistance;
        }
            
        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 vertex;
            RaycastHit2D hit = Physics2D.Raycast(origin, new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle)), distance, layerMask);


            if (hit.collider == null)
            {
                vertex = origin + new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle)) * distance;
            }
            else
            {
                vertex = hit.point;
                if (hit.transform.tag == "NPC")
                {

                    hit.transform.GetComponent<NPCStatus>().HitByFov();
                    hit = Physics2D.Raycast(origin, new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle)), distance, layerMaskAfterNPC);
                    if (hit.collider == null)
                    {
                        vertex = origin + new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle)) * distance;
                    }
                    else
                    {
                        vertex = hit.point;
                    }
                }
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

            if (i != rayCount)
            {
                angle -= angleIncrease; //Move the angle by the width of one of the triangles forming the fov
            }

        }
    }

}
