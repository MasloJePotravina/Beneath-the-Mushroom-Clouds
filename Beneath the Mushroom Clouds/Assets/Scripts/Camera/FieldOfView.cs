//Based on: https://www.youtube.com/watch?v=CSeUMTaNFYk
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    //Which layers do raycasts collide with
    public LayerMask layerMaskStandard;
    public LayerMask layerMaskCrouched;
    private LayerMask layerMask;
    //Which layers do raycasts collide with after colliding with an NPC
    //This solves a problem in which the player could see thgrough walls if they looked "through" an NPC
    public GameObject player;
    private PlayerStatus playerStatus;

    //This is an identical game object of a different color used in creating the "discovered" texture in Fog of War
    public GameObject fovTrail;
    //The script of the FOV Trail
    private FOVTrail fovTrailScript;

    //Mesh of the FOV object
    private Mesh mesh;

    public float fovAngleStandard;
    public float fovAngleAiming;
    private float fovAngle;

    public float fovDistanceStandard;
    public float fovDistanceAiming;
    private float fovDistance;


    public int fovRayCount = 50;
    private float angle = 0f;
    private float angleIncrease;
    

    private float fop;
    public int fopRayCount = 50;
    public float fopDistance = 10f;



    // Start is called before the first frame update
    void Start()
    {

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        fovTrailScript = fovTrail.GetComponent<FOVTrail>();
        playerStatus = player.GetComponent<PlayerStatus>();
        fovAngle = fovAngleStandard;
        fovDistance = fovDistanceStandard;
        layerMask = layerMaskStandard;

    }

    private void Update()
    {
        if (playerStatus.playerAiming)
        {
            fovAngle = fovAngleAiming;
            fovDistance = fovDistanceAiming;
        }
        else
        {
            fovAngle = fovAngleStandard;
            fovDistance = fovDistanceStandard;
        }

        if (playerStatus.playerCrouched)
            layerMask = layerMaskCrouched;
        else
            layerMask = layerMaskStandard;


        Vector3 origin = player.transform.position;

        Vector2 aimAngle = player.transform.up;
        float tmpXCoord = aimAngle.x;
        aimAngle.x = -aimAngle.y;
        aimAngle.y = tmpXCoord;

        angle = Vector3.SignedAngle(Vector2.up, aimAngle, Vector3.forward) + fovAngle / 2;

        Vector3[] vertices = new Vector3[fovRayCount + 2 + fopRayCount + 1]; //FOV Rays + origin point + the "zero" ray (FOV) + FOP Rays + the "zero" ray(FOP)
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[(fovRayCount + fopRayCount) * 3];

        vertices[0] = origin; //The first vertex is on the player

        int vertexIndex = 1;
        int triangleIndex = 0;
        angleIncrease = fovAngle / fovRayCount;
        CalculateMesh(true, ref origin, ref vertices, ref triangles, ref vertexIndex, ref triangleIndex);

        fop = 360.0f - fovAngle;
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
            Vector3 vertex = origin;
            RaycastHit2D[] hits = Physics2D.RaycastAll(origin, new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle)), distance, layerMask);

            if (hits.Length == 0)
            {
                vertex = origin + new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle)) * distance;
            }
            else
            {
                
                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.transform.CompareTag("NPC"))
                    {
                        hit.transform.GetComponent<NPCStatus>().HitByFov();
                        continue;
                    }
                    else
                    {
                        vertex = hit.point;
                        break;
                    }     
                }
                //If ray passed an NPC but did not hit anything else, we need to set it just as if it didn't hit anything
                if (vertex == origin)
                {
                    vertex = origin + new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle)) * distance;
                }
            }

            if(vertex == new Vector3(5,5,5))
                Debug.Log(hits.Length);

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
