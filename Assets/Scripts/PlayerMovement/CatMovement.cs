using UnityEngine;
using System;

public class CatMovement : MonoBehaviour
{
    LayerMask ground = 6;
    public event Action<Vector3> CatMoveAction;
    private CameraController cameraController;
    public void MoveCat(DataClass.Directions dirIndex)
    {
        int p = ((int) dirIndex + (int) cameraController.GetCameraView()) % 4;

        int dir = p < 2 ? 1 : -1;
        Vector3 newDirection = p % 2 == 0 ? dir * Vector3.forward : dir * Vector3.right;
        
        //move only if collider collides with something
        if (Physics.Raycast(transform.position + newDirection, -Vector3.up, 2, ~ground))
        {
            FaceDirection(newDirection);
            transform.position += newDirection;
            CatMoveAction?.Invoke(newDirection);
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
