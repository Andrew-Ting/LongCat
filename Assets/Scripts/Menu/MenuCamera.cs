using System.Collections;
using UnityEngine;
public class MenuCamera : MonoBehaviour
{
    [SerializeField] private float tolerance;
    [SerializeField] private float rigidity;
    [SerializeField] private float speed;
    private Vector3 finalPos;
    public void MoveCamera(Vector3 pos)
    {
        Camera cam = GetComponent<Camera>();
        float ratio = (float)cam.scaledPixelWidth / cam.scaledPixelHeight;
        pos.x = pos.x * (ratio/720.0f*1520.0f);
        finalPos = pos;
        StartCoroutine(move());
    }

    IEnumerator move()
    {
        while(Vector3.Distance(transform.position, finalPos) > tolerance)
        {
            transform.position = Vector3.Lerp(transform.position, finalPos, rigidity);
            yield return new WaitForSecondsRealtime(speed);
        }
        yield return null;
    }
}
