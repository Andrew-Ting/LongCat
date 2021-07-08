using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Tooltip("0-homeview \n1-levelview \n2-no view1 \n3-no view2")]
    public Vector3[] cameraViews;
    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform child in transform)
        {
            child.transform.gameObject.SetActive(false);
        }
        transform.GetChild(0).gameObject.SetActive(true);
        FindObjectOfType<MenuCamera>().MoveCamera(cameraViews[0]);
    }
}
