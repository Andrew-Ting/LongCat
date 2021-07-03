using UnityEngine;
using System;

public class CatMovement : MonoBehaviour
{
    public int catHeight;

    LayerMask ground = 6;
    LayerMask platforms = 7;
    public event Action<Vector3> CatMoveAction;
    private CameraController cameraController;

    public void MoveCat(DataClass.Directions dirIndex)
    {
        int p = ((int)dirIndex + (int)cameraController.GetCameraView()) % 4;

        int dir = p < 2 ? 1 : -1;
        Vector3 newDirection = p % 2 == 0 ? dir * Vector3.forward : dir * Vector3.right;
        Ray moveCheckerRay = new Ray(transform.position + catHeight * Vector3.up + newDirection, -Vector3.up);
        RaycastHit hit;

        //move only if collider collides with something
        if (Physics.Raycast(moveCheckerRay, out hit, catHeight + 2, ~ground))
        {
            FaceDirection(newDirection);
            Vector3 newMoveDirection = newDirection;
            if (hit.transform.gameObject.layer == ground) // hits ground, meaning either lowest point or 1 layer 
            {
                if (hit.transform.position.y + 1 < transform.position.y) newMoveDirection -= Vector3.up;
            }
            else //hits a platform
            {
                //same layer
                if (hit.transform.position.y + 1 == transform.position.y)
                {
                    newMoveDirection = newDirection;
                }
                //exactly one block lower but still platform
                else if (hit.transform.position.y + 1 < transform.position.y)
                {
                    newMoveDirection -= Vector3.up;
                }
                //if one block or higher, since no other cases do this (this is for climbing)
                else
                {
                    RaycastHit[] checkPlatforms = Physics.RaycastAll(moveCheckerRay, catHeight, ~platforms);
                    Vector3 climbBlockPosition = hit.transform.position;
                    if (checkPlatforms.Length >= 2) // gap is possible
                    {
                        if (catHeight > checkPlatforms[1].transform.position.y - transform.position.y + 1) // 
                        {
                            climbBlockPosition = checkPlatforms[1].transform.position;
                        }
                    }
                    if(climbBlockPosition.y != transform.position.y + catHeight - 1)
                    {
                        //now check if final pos is a movable pos
                        Vector3 finalPos = newDirection * (catHeight + (int)transform.position.y - 1 - (int)climbBlockPosition.y) - Vector3.up * ((int)transform.position.y - 1 - (int)climbBlockPosition.y);
                        Vector3 catHeightClimb = new Vector3(transform.position.x, finalPos.y + transform.position.y, transform.position.z);
                        float dist = Vector3.Distance(catHeightClimb, climbBlockPosition) + 1;

                        if(!Physics.Raycast(catHeightClimb, newDirection, dist, ~(platforms | ground))) //check if space is not occupied
                        {
                            if(Physics.Raycast(transform.position + finalPos, -Vector3.up, 1, ~(platforms | ground))) // check if theres ground at final space
                            {
                                newMoveDirection = finalPos;
                            }
                            else
                            {
                                //add animation?
                                newMoveDirection = Vector3.zero;
                                Debug.Log("no block below to land on");
                            }
                        }
                        else
                        {
                            //add animation?
                            newMoveDirection = Vector3.zero;
                            Debug.Log("Something is blocking on space to land on");
                        }
                    }
                }
            }
            transform.position += newMoveDirection;
            CatMoveAction?.Invoke(newMoveDirection);
        }
        else
        {
            Debug.Log("Can't move there :(");
        }
    }

    void FaceDirection(Vector3 direction)
    {
        float angle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
        transform.Rotate(Vector3.up, angle);
    }

    void Awake()
    {
        cameraController = FindObjectOfType<CameraController>();
    }

    void Start()
    {
        transform.position = Vector3.zero + Vector3.up;
    }
}
