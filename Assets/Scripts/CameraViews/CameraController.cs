using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Range(2, 8)]
    [SerializeField] private int distanceToCharacter = 5;
    [Range(0,90)]
    [SerializeField] private int angleOfInclineDegrees = 45;
    private DataClass.ViewDirection currentView = DataClass.ViewDirection.North;
    private CatMovement catMovement;
    private Vector3 deltaCatMovement;

    //private Vector3 newCameraPos;

    public void RotateView(bool isDirectionClockwiseAbove) {
        currentView += (isDirectionClockwiseAbove ? 1 : -1);
        currentView = (DataClass.ViewDirection)(((int)currentView + 4) % 4);
        SetViewDirectionTo(currentView);
    }

    void SetViewDirectionTo(DataClass.ViewDirection destinationView){
        StopAllCoroutines();
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
        //StartCoroutine(SetTransformQuaternion());
    }

    public DataClass.ViewDirection GetCameraView() {
        return currentView;
    }

    void AdjustPosition(Vector3 currentDeltaCatMovement) {
        transform.position += currentDeltaCatMovement;
        deltaCatMovement += currentDeltaCatMovement;
    }

    private void Awake()
    {
        catMovement = FindObjectOfType<CatMovement>();
    }

    void OnEnable() {
        catMovement.CatMoveAction += AdjustPosition;
    }
    void OnDisable() {
        catMovement.CatMoveAction -= AdjustPosition;
    }
    void Start() {
        deltaCatMovement += catMovement.transform.position;
        SetViewDirectionTo(currentView); 
    }

/*    IEnumerator SetTransformQuaternion()
    {
        float dist = 100f;
        while(dist < 0.5)
        {
            transform.position = Vector3.Lerp(transform.position, newCameraPos, 0.05f);
            dist = Vector3.Distance(transform.position, newCameraPos);
            yield return null;
        }
    }*/
}
