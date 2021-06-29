using UnityEngine;

public class CatMovement : MonoBehaviour
{
    LayerMask ground = 6;

    //for directions, +X = right, +Z = up, relative to cat position rn
    //0 - forward -> 1
    //1 - back -> -1
    //2 - right -> 1
    //3 - left -> -1

    void MoveCat(int p)
    {
        int dir = p % 2 == 0 ? 1 : -1;
        Vector3 newDirection = p < 2 ? dir * Vector3.forward : dir * Vector3.right;
        //move only if collider collides with something
        if (Physics.Raycast(transform.position + newDirection, -Vector3.up, 2, ~ground))
        {
            //Debug.Log("ground hit");

            FaceDirection(newDirection);
            transform.position += newDirection;
        }
        else
        {
            Debug.Log("Can't move there :(");
        }
    }

/*
    void MoveCatX(int p)
    {
        Vector3 newDirection = p * Vector3.right;
        FaceDirection(newDirection);
        transform.position += newDirection;
    }

    void MoveCatZ(int p)
    {
        Vector3 newDirection = p * Vector3.forward;
        FaceDirection(newDirection);
        transform.position += newDirection;
    }
*/

    void FaceDirection(Vector3 direction)
    {
        float angle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
        //Debug.Log(angle);
        transform.Rotate(Vector3.up, angle);
    }

    void Start()
    {
        transform.position = Vector3.zero + Vector3.up;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.anyKeyDown)
        {
            //check if next area is a possible move
            if (Input.GetKey("up"))
            {
                //MoveCatZ(1);
                MoveCat(0);
            }
            else if(Input.GetKey("down"))
            {
                //MoveCatZ(-1);
                MoveCat(1);
            }
            else if(Input.GetKey("right"))
            {
                //MoveCatX(1);
                MoveCat(2);
            }
            else if(Input.GetKey("left"))
            {
                //MoveCatX(-1);
                MoveCat(3);
            }
        }
    }
}
