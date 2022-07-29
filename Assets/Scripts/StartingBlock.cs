using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingBlock : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3 GetPos()
    {
        transform.GetComponent<MeshRenderer>().enabled = false;
        return transform.position;
    }
}
