using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewButtonController : MonoBehaviour
{
    private CameraController cameraController;
    [SerializeField]
    private bool isClockwiseSpinAbove;

    void Awake() {
       cameraController = FindObjectOfType<CameraController>();
       Button thisButton = GetComponent<Button>();
       thisButton.onClick.AddListener(() => {cameraController.RotateView(isClockwiseSpinAbove);});    
    }
}
