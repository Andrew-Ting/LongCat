using System.Collections;
using UnityEngine;
using System;

public class CameraController : MonoBehaviour
{
    [Range(2, 8)]
    [SerializeField] private int distanceToCharacter = 5;
    [Range(0,90)]
    [SerializeField] private int angleOfInclineDegrees = 45;
    [SerializeField] private float switchViewSpeed = 0.01f; // normalized value; 1 means instant switch of view
    [SerializeField] private float moveViewSpeed = 0.05f;
    private DataClass.ViewDirection currentView = DataClass.ViewDirection.North;
    private CatMovement catMovement;
    private Vector3 deltaCatMovement; 
    private PlayRecord playRecord;

    public void RotateView(bool isDirectionClockwiseAbove) {
        currentView += (isDirectionClockwiseAbove ? 1 : -1);
        currentView = (DataClass.ViewDirection)(((int)currentView + 4) % 4);
        SetViewDirectionTo(currentView);
    }

    void SetViewDirectionTo(DataClass.ViewDirection destinationView, bool hasTransition = true){
        StopAllCoroutines();
        float cameraHeight = distanceToCharacter * Mathf.Sin(Mathf.PI / 180 * angleOfInclineDegrees);
        float cameraHorizontalProjection = distanceToCharacter * Mathf.Cos(Mathf.PI / 180 * angleOfInclineDegrees) / Mathf.Sqrt(2);
        Vector3 newPosition = Vector3.zero;
        Quaternion newRotation = Quaternion.identity;
        switch (destinationView) {
            case DataClass.ViewDirection.North:
                newPosition = new Vector3(-cameraHorizontalProjection, cameraHeight, -cameraHorizontalProjection) + deltaCatMovement;
                newRotation = Quaternion.Euler(angleOfInclineDegrees, 45, 0);
                break;
            case DataClass.ViewDirection.East:
                newPosition = new Vector3(-cameraHorizontalProjection, cameraHeight, cameraHorizontalProjection) + deltaCatMovement;
                newRotation = Quaternion.Euler(angleOfInclineDegrees, 135, 0);
                break;
            case DataClass.ViewDirection.South:
                newPosition = new Vector3(cameraHorizontalProjection, cameraHeight, cameraHorizontalProjection) + deltaCatMovement;
                newRotation = Quaternion.Euler(angleOfInclineDegrees, 225, 0);
                break;
            case DataClass.ViewDirection.West:
                newPosition = new Vector3(cameraHorizontalProjection, cameraHeight, -cameraHorizontalProjection) + deltaCatMovement;
                newRotation = Quaternion.Euler(angleOfInclineDegrees, 315, 0);
                break;
        }
       
       StartCoroutine(SetTransformQuaternion(newPosition, newRotation, hasTransition));
        
    }

    public DataClass.ViewDirection GetCameraView() {
        return currentView;
    }

    void AdjustPosition(Vector3 currentDeltaCatMovement) {
        StartCoroutine(SetTransform(transform.position + currentDeltaCatMovement));
        deltaCatMovement += currentDeltaCatMovement;
    }

    private void Awake()
    {
        catMovement = FindObjectOfType<CatMovement>();
        playRecord = FindObjectOfType<PlayRecord>();
        playRecord.UndoEvent += SetTransformRelativeToCatInstantly;
    }

    void OnEnable() {
        catMovement.CatMoveAction += AdjustPosition;
        catMovement.CatLoaded += CatLoad;
    }
    void OnDisable() {
        catMovement.CatMoveAction -= AdjustPosition;
        catMovement.CatLoaded -= CatLoad;
    }

    void CatLoad(bool isLoaded)
    {
        if(isLoaded)
        {
            deltaCatMovement += catMovement.transform.position;
            SetViewDirectionTo(currentView);
        }
    }

    IEnumerator SetTransformQuaternion(Vector3 newCameraPos, Quaternion newCameraRot, bool hasTransition)
    {
        float cameraHeight = distanceToCharacter * Mathf.Sin(Mathf.PI / 180 * angleOfInclineDegrees);
        float progress = 0;
        Vector3 startPos = transform.position;
        Vector3 pivotOfRotation = new Vector3(catMovement.transform.position.x, cameraHeight + deltaCatMovement.y, catMovement.transform.position.z);
        startPos -= pivotOfRotation;
        newCameraPos -= pivotOfRotation;
        Quaternion startAngle = transform.rotation;
        if (hasTransition)
        {
            while (progress <= 1)
            {
                transform.position = Vector3.Slerp(startPos, newCameraPos, progress) + Vector3.up * cameraHeight + deltaCatMovement;
                transform.rotation = Quaternion.Slerp(startAngle, newCameraRot, progress);
                progress += switchViewSpeed;
                yield return null;
            }
        }
        else
        {
            transform.position = newCameraPos + Vector3.up * cameraHeight + deltaCatMovement;
            transform.rotation = newCameraRot;
        }
    }
    private void SetTransformRelativeToCatInstantly(PlayRecord.MoveState moveState)
    {
        deltaCatMovement = moveState.catPosition;
        SetViewDirectionTo(currentView, false);
    }

    IEnumerator SetTransform(Vector3 newCameraPos)
    {
        float progress = 0;
        Vector3 startPos = transform.position;
        while (progress <= 1)
        {
            transform.position = Vector3.Lerp(startPos, newCameraPos, progress);
            progress += moveViewSpeed;
            yield return null;
        }
    }
}
