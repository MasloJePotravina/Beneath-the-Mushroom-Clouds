//Based on: https://www.youtube.com/watch?v=CSeUMTaNFYk
using UnityEngine;
/// <summary>
/// Implements the behaviour of the field of view.
/// </summary>
public class FieldOfView : MonoBehaviour
{
    //Which layers do raycasts collide with
    /// <summary>
    /// Layer mask containing the layers that the field of view raycasts collide with when the player is standing.
    /// </summary>
    [SerializeField] private LayerMask layerMaskStanding;

    /// <summary>
    /// Layer mask containing the layers that the field of view raycasts collide with when the player is crouched.
    /// </summary>
    [SerializeField] private LayerMask layerMaskCrouched;

    /// <summary>
    /// Current layer mask containing the layers that the field of view raycasts collide with.
    /// </summary>
    private LayerMask layerMask;

    /// <summary>
    /// Reference to the player game object.
    /// </summary>
    public GameObject player;

    /// <summary>
    /// Reference to the Player Status script.
    /// </summary>
    private PlayerStatus playerStatus;

    /// <summary>
    /// Mesh of the field of view. This is used to draw the field of view based on raycast hits.
    /// </summary>
    private Mesh mesh;

    /// <summary>
    /// Standard angle of the FOV (not aiming).
    /// </summary>
    public float fovAngleStandard;

    /// <summary>
    /// Aiming angle of the FOV when the player is aiming.
    /// </summary>
    public float fovAngleAiming;

    /// <summary>
    /// Current angle of the FOV.
    /// </summary>
    private float fovAngle;

    /// <summary>
    /// Standard distance of the FOV (not aiming).
    /// </summary>
    public float fovDistanceStandard;

    /// <summary>
    /// Distance of the FOV when the player is aiming.
    /// </summary>
    public float fovDistanceAiming;

    /// <summary>
    /// Current distance of the FOV.
    /// </summary>
    private float fovDistance;

    /// <summary>
    /// Amount of rays used to draw the FOV (angle which the player character sees).
    /// </summary>
    public int fovRayCount = 50;

    /// <summary>
    /// Angle of the currently drawn raycast.
    /// </summary>
    private float angle = 0f;

    /// <summary>
    /// By how much does the angle increase with each raycast (Depends on the amount of rays).
    /// </summary>
    private float angleIncrease;
    
    /// <summary>
    /// Angle of the field of perception (angle behind the player, which they technically do not see but are aware of).
    /// </summary>
    private float fop;

    /// <summary>
    /// Amount of rays used to draw the FOP (angle which the player character is aware of).
    /// </summary>
    public int fopRayCount = 50;

    /// <summary>
    /// Distance of the field of perception (distance behind the player, which they technically do not see but are aware of).
    /// </summary>
    public float fopDistance = 10f;


    /// <summary>
    /// New FOV raycount set in the options menu. The ray count will be changed at the start of the next frame.
    /// </summary>
    private int newFovRayCount = 0;



    /// <summary>
    /// Initializes the FOV references and values at start.
    /// </summary>
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        //FOV mask is used to hide footprints and bloodtrails of enemies that are not in the FOV, it uses the same mesh as dov but divverent material
        GameObject.Find("FOVMask").GetComponent<MeshFilter>().mesh = mesh;
        playerStatus = player.GetComponent<PlayerStatus>();
        fovAngle = fovAngleStandard;
        fovDistance = fovDistanceStandard;
        layerMask = layerMaskStanding;

    }

    /// <summary>
    /// Each frame, calculates and draws the FOV mesh.
    /// </summary>
    private void Update()
    {   
        //When the FOV ray count is changed, it cannot be changed immediately as it would break the mesh calculation
        //Therefore it is changed at the start of a new frame
        if(newFovRayCount > 0)
        {
            fovRayCount = newFovRayCount;
            newFovRayCount = 0;
        }
        //Determine the angle and distance of the fov raycasts based on whether the player is aiming or not
        if (playerStatus.isAiming)
        {
            fovAngle = fovAngleAiming;
            fovDistance = fovDistanceAiming;
        }
        else
        {
            fovAngle = fovAngleStandard;
            fovDistance = fovDistanceStandard;
        }

        //Determine the layer mask based on whether the player is crouched or not
        if (playerStatus.isCrouched)
            layerMask = layerMaskCrouched;
        else
            layerMask = layerMaskStanding;


        //Set the origin of raycasts and the angle of aiming
        Vector3 origin = player.transform.position;
        Vector2 aimAngle = player.transform.up;

        //Swap the x and y coordinates of the aiming angle (as player.transform.up is not the real aiming angle, just a reference point)
        float tmpXCoord = aimAngle.x;
        aimAngle.x = -aimAngle.y;
        aimAngle.y = tmpXCoord;

        //Calculate the angle of the first raycast (first raycast is the leftmost raycast of FOV angle)
        angle = Vector3.SignedAngle(Vector2.up, aimAngle, Vector3.forward) + fovAngle / 2;

        //Vaiables for the mesh
        Vector3[] vertices = new Vector3[fovRayCount + 2 + fopRayCount + 1]; //FOV Rays + origin point + the "zero" ray (FOV) + FOP Rays + the "zero" ray(FOP)
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[(fovRayCount + fopRayCount) * 3];

         //The first vertex is on the player's position
        vertices[0] = origin;

        //Calculate mesh for FOV
        int vertexIndex = 1;
        int triangleIndex = 0;
        angleIncrease = fovAngle / fovRayCount;
        CalculateMesh(true, ref origin, ref vertices, ref triangles, ref vertexIndex, ref triangleIndex);

        //Calculate mesh for FOP
        fop = 360.0f - fovAngle;
        angleIncrease = fop / fopRayCount;
        CalculateMesh(false, ref origin, ref vertices, ref triangles, ref vertexIndex, ref triangleIndex);

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        mesh.Clear();
        mesh.SetVertices(vertices);
        mesh.SetUVs(0, uv);
        mesh.SetTriangles(triangles, 0);

    }

    //Calculate mesh for Field Of Vision and Field Of Perception
    //fov == 1 -> calculating FOV
    //fov == 0 -> calculating FOP
    /// <summary>
    /// Calculates the mesh for the FOV or FOP.
    /// </summary>
    /// <param name="fov">True if calculating FOV, false if calculating FOP.</param>
    /// <param name="origin">Origin of the raycasts (player's position).</param>
    /// <param name="vertices">Array of vertices of the calculated mesh (set in this function by reference).</param>
    /// <param name="triangles">Array of triangles of the calculated mesh (set in this function by reference).</param>
    /// <param name="vertexIndex">Vertex index of the last calculated vertex (set in this function by reference). Used so that the FOP calculation starts off where the FOV calculation ended.</param>
    /// <param name="triangleIndex">Triangle index of the last calculated triangle (set in this function by reference). Used so that the FOP calculation starts off where the FOV calculation ended.</param>
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
                        hit.transform.root.GetComponent<NPCStatus>().HitByFov();
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


    /// <summary>
    /// Sets the number of the FOV raycasts. Will be set at the beginning of the next frame.
    /// </summary>
    /// <param name="count">New amount of fov raycasts.</param>
    public void SetFOVRayCount(int count)
    {
        newFovRayCount = count;
    }

}
