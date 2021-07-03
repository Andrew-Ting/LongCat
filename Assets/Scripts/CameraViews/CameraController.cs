using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private int distanceToCharacter = 5;
    [SerializeField] private int angleOfInclineDegrees = 45;
    private DataClass.ViewDirection currentView = DataClass.ViewDirection.North;
    private CatMovement catMovement;
    private Vector3 deltaCatMovement;
    public void RotateView(bool isDirectionClockwiseAbove) {
        currentView += (isDirectionClockwiseAbove ? 1 : -1);
        currentView = (DataClass.ViewDirection)(((int)currentView + 4) % 4);
        SetViewDirectionTo(currentView);
    }

    void SetViewDirectionTo(DataClass.ViewDirection destinationView){
        float cameraHeight = distanceToCharacter * Mathf.Sin(Mathf.PI / 180 * angleOfInclineDegrees);
        float cameraHorizontalProjection = distanceToCharacter * Mathf.Cos(Mathf.PI / 180 * angleOfInclineDegrees) / Mathf.Sqrt(2);
        switch (destinationView) {
            case DataClass.ViewDirection.North:
                transform.position = new Vector3(-cameraHorizontalProjection, cameraHeight, -cameraHorizontalProjection) + deltaCatMovement;
                transform.rotation = Quaternion.Euler(angleOfInclineDegrees, 45, 0);
                break;
            case DataClass.ViewDirection.East:
                transform.position = new Vector3(-cameraHorizontalProjection, cameraHeight, cameraHorizontalProjection) + deltaCatMovement;
                transform.rotation = Quaternion.Euler(angleOfInclineDegrees, 135, 0);
                break;
            case DataClass.ViewDirection.South:
                transform.position = new Vector3(cameraHorizontalProjection, cameraHeight, cameraHorizontalProjection) + deltaCatMovement;
                transform.rotation = Quaternion.Euler(angleOfInclineDegrees, 225, 0);
                break;
            case DataClass.ViewDirection.West:
                transform.position = new Vector3(cameraHorizontalProjection, cameraHeight, -cameraHorizontalProjection) + deltaCatMovement;
                transform.rotation = Quaternion.Euler(angleOfInclineDegrees, 315, 0);
                break;
        }
    }

    public DataClass.ViewDirection GetCameraView() {
        return currentView;
    }

    void AdjustPosition(Vector3 currentDeltaCatMovement) {
        transform.position += currentDeltaCatMovement;
        deltaCatMovement += currentDeltaCatMovement;
    }

    void Awake() {
        catMovement = FindObjectOfType<CatMovement>();
    }
    void OnEnable() {
        catMovement.CatMoveAction += AdjustPosition;
    }
    void OnDisable() {
        catMovement.CatMoveAction -= AdjustPosition;
    }
    void Start() {
       SetViewDirectionTo(currentView); 
    }
}
