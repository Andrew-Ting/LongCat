using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private int distanceToCharacter = 5;
    [SerializeField] private int angleOfInclineDegrees = 45;
    private ViewDirection currentView = ViewDirection.North;

    public void SetCurrentView(ViewDirection newView) {
        currentView = newView;
        float cameraHeight = distanceToCharacter * Mathf.Sin(Mathf.PI / 180 * angleOfInclineDegrees);
        float cameraHorizontalProjection = distanceToCharacter * Mathf.Cos(Mathf.PI / 180 * angleOfInclineDegrees) / Mathf.Sqrt(2);
        switch (newView) {
            case ViewDirection.North:
                transform.position = new Vector3(-cameraHorizontalProjection, cameraHeight, -cameraHorizontalProjection);
                transform.rotation = Quaternion.Euler(angleOfInclineDegrees, 45, 0);
                break;
            case ViewDirection.East:
                transform.position = new Vector3(-cameraHorizontalProjection, cameraHeight, cameraHorizontalProjection);
                transform.rotation = Quaternion.Euler(angleOfInclineDegrees, 135, 0);
                break;
            case ViewDirection.South:
                transform.position = new Vector3(cameraHorizontalProjection, cameraHeight, cameraHorizontalProjection);
                transform.rotation = Quaternion.Euler(angleOfInclineDegrees, 225, 0);
                break;
            case ViewDirection.West:
                transform.position = new Vector3(cameraHorizontalProjection, cameraHeight, -cameraHorizontalProjection);
                transform.rotation = Quaternion.Euler(angleOfInclineDegrees, 315, 0);
                break;
        }
    }
    void Start() {
       SetCurrentView(currentView); 
    }
}
