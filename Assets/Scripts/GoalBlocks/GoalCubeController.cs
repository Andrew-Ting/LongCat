using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalCubeController : MonoBehaviour
{
    private GoalBlockController goalBlockController; 
    [SerializeField] private int blockLayer = 7;
    void Start()
    {
        goalBlockController = transform.parent.gameObject.GetComponent<GoalBlockController>();
    }

    void OnTriggerEnter(Collider collisionObject) {
        Debug.Log("Detected collision");
        if (collisionObject.gameObject.layer == blockLayer) {
            Debug.Log("Added counter");
            goalBlockController.AddSatisfiedCount();
        }
    }
    void OnTriggerExit(Collider collisionObject) {
        if (collisionObject.gameObject.layer == blockLayer) {
            goalBlockController.DeductSatisfiedCount();
        }
    }
}
