using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonMover : MonoBehaviour, IPointerDownHandler
{
    public DataClass.MenuCameraViews cameraView;
    public GameObject Open;
    
    MenuCamera menuCamera;
    MenuManager menuManager;

    public void OnPointerDown(PointerEventData eventData)
    {
        menuCamera.MoveCamera(menuManager.cameraViews[(int)cameraView]);
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
        menuCamera = FindObjectOfType<MenuCamera>();
        menuManager = FindObjectOfType<MenuManager>();
    }
}
