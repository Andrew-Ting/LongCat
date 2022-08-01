using UnityEngine;

public class StartingBlock : MonoBehaviour
{
    public void Start()
    {
        transform.GetComponent<MeshRenderer>().enabled = false;
    }

    public Vector3 GetPos()
    {
        return transform.position;
    }
}
