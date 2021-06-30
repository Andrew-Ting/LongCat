using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewButtonController : MonoBehaviour
{
    private CameraController cameraController;
    [SerializeField]
    private ViewDirection setCameraViewTo;
    // Start is called before the first frame update
    void Awake() {
       cameraController = FindObjectOfType<CameraController>();
       Button thisButton = GetComponent<Button>();
       thisButton.onClick.AddListener(() => {cameraController.SetCurrentView(setCameraViewTo);});    
    }
}
