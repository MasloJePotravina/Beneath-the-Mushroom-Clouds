using UnityEngine;

public class firearmFunctions : MonoBehaviour
{

    public GameObject bulletPrefab;
    //Applies cover to enemy crouching behind a wall
    //Enemies STANDING behind a wall have no bonus from this since they already get the implied protection from the bulet hitting the wall
    //This function is supposed to simulate bulltes flying over half walls and over the heads of the enemy or hitting them based on
    //the distances of the shooter and the wall and the distance of the enemy and the wall
    //This happens because of the fact that the gun is shot from a higher place than the halfwall and therefore can still hit enemies that
    //are too far from the wall to take cover. See ASCII illustration:
    //
    //   O
    //   |=====> ----------___________   
    //   || \\                        ------------___________
    //   |                                           ||      ----------__________
    //   /\                                          ||                          ----------___________
    //__/__\_________________________________________||___________________________________________________________
    //    <---------Shooter-Wall Distance------------><--Safe-><------------Cover loses effect-------------><---No Cover---->
    //
    //Formulas used to calculate the zones after the wall:
    // Safe: between 0 and 0.25 * Shooter-Wall Distance ==> The enemy has zero chance of being hit, full cover
    // Cover loses effect: between 0.25 * SWD and 2 * SWD ==> Linear loss of cover
    // No Cover = 2 * Shooter-Wall Distance and more ==> The enemy is too far behind cover and therefore he is fully visible, no cover
    public float ApplyCoverToEnemy(float halfWallDistance, RaycastHit2D hit)
    {
        float percentage;
        if (halfWallDistance <= 0)
        {
            percentage = 1.0f;
        }
        else
        {
            if (hit.distance - halfWallDistance <= 0.25 * halfWallDistance)
            {
                percentage = 0.0f;
            }
            else if (hit.distance - halfWallDistance >= 2 * halfWallDistance)
            {
                percentage = 1.0f;
            }
            else
            {
                percentage = ((hit.distance - halfWallDistance - 0.25f * halfWallDistance) / (1.75f * halfWallDistance));
            }
        }
        return percentage;
    }
    public bool NPCHit(float halfWallDistance, RaycastHit2D hit)
    {
        bool isCrouched = hit.transform.gameObject.GetComponent<NPCStatus>().isCrouched;
        if (isCrouched)
        {
            float hitChance = ApplyCoverToEnemy(halfWallDistance, hit);
            if (hit.distance <= 50.0f) //A crouched enemy will be hit 100% of the time from distance < 5m
            {
                Debug.Log(hitChance);
                if (Random.value < 1.0f * hitChance)
                    return true;
                else
                    return false;
            }
            else if (hit.distance >= 150.0f)
            {
                if (Random.value < 0.8 * hitChance) //A crouched enemy will be hit 80% of the time from distance > 15m 
                    return true;
                else
                    return false;
            }
            else //linear dropoff between 5 metres and 15 metres (from 100% to 80%)
            {
                Debug.Log(hitChance);
                if (Random.value < (1 - (hit.distance - 50.0f) * 0.002) * hitChance)
                    return true;
                else
                    return false;
            }
        }
        else
        {
            return true; //Standing enemy will get hit 100% of the time regardless of distance. TODO: Prone to future balancing
        }

    }

    //distance - distance between the shooter and the wall
    public bool HalfWallPassed(float distance)
    {
        if (distance < 50.0f)
        {
            return true; //bullet passes 100% of the time if shooter is less than 5 metres from HW
        }
        else if (distance > 150.0f)
        {
            return (Random.value > 0.5); //bullet passes 50% of the time if shooter is further than 15 metres from HW
        }
        else
        {
            return (Random.value > (distance - 50.0f) * 0.005);//linear dropoff between 5 metres and 15 metres (from 100% to 50%)
        }
    }
    //Applies the random error within the confines of the cone to the bullet
    //Degrees represent half the angle of the cone (15 degree -> 30degree cone)
    public Vector2 ApplyAimErrorToRaycast(Vector2 aimAngle, float degrees)
    {

        degrees = Random.Range(0, degrees);

        //The bullet either deviates to the right or the left
        if (Random.value <= 0.5f)
            return Quaternion.Euler(0, 0, -degrees) * aimAngle;
        else
            return Quaternion.Euler(0, 0, degrees) * aimAngle;

    }

    //Creates the bullet raycast
    public void BulletLine(RaycastHit2D hit, Vector3 muzzlePos)
    {
        GameObject bullet = Instantiate(bulletPrefab);
        LineRenderer lineRenderer = bullet.GetComponent<LineRenderer>();
        Vector3[] trajectory = { muzzlePos, hit.point };
        lineRenderer.SetPositions(trajectory);
    }
}
