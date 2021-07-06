using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonMover : MonoBehaviour, IPointerDownHandler
{
    public Vector3 CameraPos;
    public GameObject Open;
    
    MenuCamera menuCamera;

    public void OnPointerDown(PointerEventData eventData)
    {
        menuCamera.MoveCamera(CameraPos);
        if(Open != null)
        {
            transform.parent.gameObject.SetActive(false);
            Open.SetActive(true);
        }
        else
        {
            Debug.Log("no gameobject");
        }
    }

    void Start()
    {
        menuCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MenuCamera>();
    }
}
