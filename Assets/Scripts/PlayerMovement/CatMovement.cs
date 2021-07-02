using UnityEngine;
using System;

public class CatMovement : MonoBehaviour
{
    public int catHeight;

    LayerMask ground = 6;
    public event Action<Vector3> CatMoveAction;
    private CameraController cameraController;

    public void MoveCat(DataClass.Directions dirIndex)
    {
        int p = ((int) dirIndex + (int) cameraController.GetCameraView()) % 4;

        int dir = p < 2 ? 1 : -1;
        Vector3 newDirection = p % 2 == 0 ? dir * Vector3.forward : dir * Vector3.right;
        Ray moveCheckerRay = new Ray(transform.position + catHeight * Vector3.up  + newDirection, -Vector3.up);
        RaycastHit hit;

        //move only if collider collides with something
        if (Physics.Raycast(moveCheckerRay, out hit, catHeight + 2, ~ground))
        {
            FaceDirection(newDirection);
            if (hit.transform.gameObject.layer == ground)
            {
                if (hit.transform.position.y + 1 < transform.position.y) transform.position -= Vector3.up;
                transform.position += newDirection;
                CatMoveAction?.Invoke(newDirection);
            }
            else //hits a platform
            {
                //same layer
                if (hit.transform.position.y + 1 == transform.position.y)
                {
                    transform.position += newDirection;
                }
                //exactly one block lower but still platform
                else if (hit.transform.position.y + 1 < transform.position.y)
                {
                    transform.position += newDirection - Vector3.up;
                }
                //if one block higher
                else if (catHeight + (int)transform.position.y - 1 > hit.transform.position.y)
                {
                    transform.position += newDirection * (catHeight + (int)transform.position.y - 1 - (int)hit.transform.position.y) - Vector3.up * ((int)transform.position.y - 1 - (int)hit.transform.position.y);
                }
            }
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

    void Awake() {
        cameraController = FindObjectOfType<CameraController>();   
    }
    void Start()
    {
        transform.position = Vector3.zero + Vector3.up;
    }
}
