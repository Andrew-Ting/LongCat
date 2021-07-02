using UnityEngine;

public class CatMovement : MonoBehaviour
{
    LayerMask ground = 6;

    void MoveCat(int p)
    {
        int dir = p % 2 == 0 ? 1 : -1;
        Vector3 newDirection = p < 2 ? dir * Vector3.forward : dir * Vector3.right;
        
        //move only if collider collides with something
        if (Physics.Raycast(transform.position + newDirection, -Vector3.up, 2, ~ground))
        {
            FaceDirection(newDirection);
            transform.position += newDirection;
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

    void Start()
    {
        transform.position = Vector3.zero + Vector3.up;
    }

    void Update()
    {
        if(Input.anyKeyDown)
        {
            //check if next area is a possible move
            if (Input.GetKey("up"))
            {
                MoveCat((int)DataClass.Directions.Forward);
            }
            else if(Input.GetKey("down"))
            {
                MoveCat((int)DataClass.Directions.Behind);
            }
            else if(Input.GetKey("right"))
            {
                MoveCat((int)DataClass.Directions.Right);
            }
            else if(Input.GetKey("left"))
            {
                MoveCat((int)DataClass.Directions.Left);
            }
        }
    }
}
