using UnityEngine;
using System;

public class CatMovement : MonoBehaviour
{
    public int catHeight;

    LayerMask objects = 6 | 7;
    public event Action<Vector3> CatMoveAction;
    private CameraController cameraController;

    public void MoveCat(DataClass.Directions dirIndex)
    {
        int p = ((int)dirIndex + (int)cameraController.GetCameraView()) % 4;
        int dir = p < 2 ? 1 : -1;
        Vector3 newDirection = p % 2 == 0 ? dir * Vector3.forward : dir * Vector3.right;
        FaceDirection(newDirection);
        Vector3 newMoveDirection = newDirection; // this is relative

        if (!Physics.Raycast(transform.position + Vector3.up * catHeight + transform.forward, -Vector3.up, catHeight, ~objects))
        {
            Ray ray = new Ray(transform.position + transform.forward, -Vector3.up);
            RaycastHit hit;
            //nothing is in front of it
            if (Physics.Raycast(ray, out hit, 2, ~objects))
            {
                //hits something
                if(hit.transform.position.y + 1 != transform.position.y)
                {
                    newMoveDirection -= Vector3.up;
                }
                //else make newMoveDirection forward(but since it's already set as that anyway)
            }
            else
            {
                //can't move
                newMoveDirection = Vector3.zero;
            }
        }
        else
        {
            //hits something / something is blocking the way
            bool canClimb = false;
            Vector3 blockClimb = Vector3.zero;
            for(int i = 0; i < catHeight; i++)
            {
                Vector3 blockCheck = transform.position + transform.forward + Vector3.up * i;
                if (!Physics.Raycast(blockCheck, Vector3.up, 1, ~objects) && Physics.Raycast(blockCheck + Vector3.up, -Vector3.up, 1, ~objects))
                {
                    canClimb = true;
                    blockClimb = blockCheck;
                    break;
                }
            }
            if(canClimb)
            {
                Vector3 finalPos = newDirection * (catHeight + (int)transform.position.y - 1 - (int)blockClimb.y) - Vector3.up * ((int)transform.position.y - 1 - (int)blockClimb.y);
                Vector3 catHeightClimb = new Vector3(transform.position.x, finalPos.y + transform.position.y, transform.position.z);
                float dist = Vector3.Distance(catHeightClimb, blockClimb);
                if (!Physics.Raycast(catHeightClimb, newDirection, dist, ~objects)) //check if space is not occupied
                {
                    if (Physics.Raycast(transform.position + finalPos, -Vector3.up, 1, ~objects)) // check if theres ground at final space
                    {
                        if(!Physics.Raycast(transform.position + finalPos, Vector3.up, catHeight -1, ~objects)) // height at final pos is enough
                        {
                            newMoveDirection = finalPos;
                        }
                        else
                        {
                            //add animation for no height room?
                            newMoveDirection = Vector3.zero;
                            Debug.Log("final space not enough height");
                        }
                    }
                    else
                    {
                        //add animation for no ground at final space?
                        newMoveDirection = Vector3.zero;
                        Debug.Log("no block below to land on");
                    }
                }
                else
                {
                    //add animation for block along the way?
                    newMoveDirection = Vector3.zero;
                    Debug.Log("Something is blocking on space to land on");
                }
            }
            else
            {
                newMoveDirection = Vector3.zero;
            }
        }
        transform.position += newMoveDirection;
        CatMoveAction?.Invoke(newMoveDirection);
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
        transform.position = Vector3.forward + Vector3.up;
    }
}
